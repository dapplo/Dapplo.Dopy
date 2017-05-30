//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2017 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.Dopy
// 
//  Dapplo.Dopy is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.Dopy is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.Dopy. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Dapplo.Addons;
using Dapplo.Dopy.Configuration;
using Dapplo.Dopy.Shared;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Shared.Repositories;
using Dapplo.Log;
using Dapplo.Windows.Clipboard;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.Messages;

namespace Dapplo.Dopy.Services
{
    /// <summary>
    /// This service takes care of automatically storing every clipboard change to the IClipboardRepository
    /// </summary>
    [StartupAction, ShutdownAction]
    public class ClipboardStoreService : IStartupAction, IShutdownAction
    {
        private static readonly LogSource Log = new LogSource();
        private IDisposable _clipboardMonitor;
        private readonly SynchronizationContext _uiSynchronizationContext;
        private readonly IClipRepository _clipRepository;
        private readonly IDopyConfiguration _dopyConfiguration;

        /// <summary>
        /// Initializes the needed depedencies
        /// </summary>
        /// <param name="clipRepository">IClipRepository</param>
        /// <param name="dopyConfiguration">Configuration</param>
        /// <param name="uiSynchronizationContext">SynchronizationContext to register the Clipboard Monitor with</param>
        [ImportingConstructor]
        public ClipboardStoreService(
            IClipRepository clipRepository,
            IDopyConfiguration dopyConfiguration,
            [Import("ui", typeof(SynchronizationContext))]SynchronizationContext uiSynchronizationContext)
        {
            _clipRepository = clipRepository;
            _uiSynchronizationContext = uiSynchronizationContext;
            _dopyConfiguration = dopyConfiguration;
        }

        /// <summary>
        /// Start will register all needed services
        /// </summary>
        public void Start()
        {
            ClipboardFormats.RegisterDapploDopy();
            _clipboardMonitor = ClipboardMonitor
                .OnUpdate
                .SubscribeOn(_uiSynchronizationContext)
                .Where(contents => contents.OwnerHandle != WinProcHandler.Instance.Handle)
                .Synchronize().Subscribe(clipboardContents =>
            {
                if (clipboardContents.IsModifiedByDopy())
                {
                    Log.Info().WriteLine("Skipping clipboard id {0} as it was pasted by 'us'", clipboardContents.Id);
                    return;
                }
                Log.Info().WriteLine("Processing clipboard id {0}", clipboardContents.Id);
                var clip = CreateClip(clipboardContents);

                // Store it in the repository
                _clipRepository.Create(clip);
            });
        }

        /// <summary>
        /// Create the full-blown Clip object
        /// </summary>
        /// <param name="clipboardUpdateInformation">ClipboardContents</param>
        /// <returns>Clip</returns>
        private Clip CreateClip(ClipboardUpdateInformation clipboardUpdateInformation)
        {
            var interopWindow = InteropWindowFactory.CreateFor(clipboardUpdateInformation.OwnerHandle);

            // Make sure we use the parent window (top level) for the title.
            IInteropWindow toplevelWindow = interopWindow;
            var parent = toplevelWindow.GetParent();
            while (true)
            {
                if (parent == IntPtr.Zero)
                {
                    break;
                }
                toplevelWindow = toplevelWindow?.GetParentWindow();
                parent = toplevelWindow.GetParent();
            }
            var caption = toplevelWindow.GetCaption();
            Clip clip;
            using (var process = Process.GetProcessById(interopWindow.GetProcessId()))
            {
                string productName = process.ProcessName;

                // Make sure we got something, when the caption is emtry up to now
                if (string.IsNullOrEmpty(caption))
                {
                    caption = process.MainWindowTitle;
                }
                try
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(process.MainModule.FileName);
                    productName = versionInfo.ProductName;
                }
                catch (Exception ex)
                {
                    Log.Error().WriteLine(ex, "Problem retrieving process information for a process with ID {0} and name {1}", process.Id, process.ProcessName);
                }

                clip = new Clip
                {
                    WindowTitle = caption,
                    ProcessName = process.ProcessName,
                    ProductName = productName,
                    OriginalWindowHandle = clipboardUpdateInformation.OwnerHandle,
                    SequenceNumber = clipboardUpdateInformation.Id,
                    OriginalFormats = clipboardUpdateInformation.Formats.ToList()
                };
            }
            using (ClipboardNative.Lock())
            {
                if (clip.OriginalFormats.Contains("CF_UNICODETEXT"))
                {
                    clip.ClipboardText = ClipboardNative.GetAsUnicodeString();
                }

                foreach (var format in clipboardUpdateInformation.Formats)
                {
                    if (!_dopyConfiguration.CopyAlways.Contains(format))
                    {
                        continue;
                    }
                    clip.Formats.Add(format);
                    clip.Contents[format] = ClipboardNative.GetAsStream(format);
                }
            }
            return clip;
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _clipboardMonitor?.Dispose();
        }
    }
}
