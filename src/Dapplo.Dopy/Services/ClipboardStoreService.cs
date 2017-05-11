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
using Caliburn.Micro;
using Dapplo.Addons;
using Dapplo.Dopy.Configuration;
using Dapplo.Dopy.Entities;
using Dapplo.Dopy.Repositories;
using Dapplo.Log;
using Dapplo.Windows.Clipboard;
using Dapplo.Windows.Desktop;

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
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Initializes the needed depedencies
        /// </summary>
        /// <param name="clipRepository">IClipRepository</param>
        /// <param name="dopyConfiguration">Configuration</param>
        /// <param name="uiSynchronizationContext">SynchronizationContext to register the Clipboard Monitor with</param>
        /// <param name="eventAggregator">Used to publish changes</param>
        [ImportingConstructor]
        public ClipboardStoreService(
            IClipRepository clipRepository,
            IDopyConfiguration dopyConfiguration,
            [Import("ui", typeof(SynchronizationContext))]SynchronizationContext uiSynchronizationContext,
            IEventAggregator eventAggregator)
        {
            _clipRepository = clipRepository;
            _uiSynchronizationContext = uiSynchronizationContext;
            _dopyConfiguration = dopyConfiguration;
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Start will register all needed services
        /// </summary>
        public void Start()
        {
            _clipboardMonitor = ClipboardMonitor.OnUpdate.SubscribeOn(_uiSynchronizationContext).Synchronize().Subscribe(clipboardContents =>
            {

                Log.Info().WriteLine("Processing clipboard id {0}", clipboardContents.Id);
                var interopWindow = InteropWindowFactory.CreateFor(clipboardContents.OwnerHandle);

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

                using (var process = Process.GetProcessById(interopWindow.GetProcessId()))
                {
                    string productName = process.ProcessName;
                    try
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(process.MainModule.FileName);
                        productName = versionInfo.ProductName;
                    }
                    catch (Exception ex)
                    {
                        Log.Error().WriteLine(ex, "Problem retrieving process information for a process with ID {0} and name {1}", process.Id, process.ProcessName);
                    }
                    var clip = new Clip
                    {
                        WindowTitle = toplevelWindow.GetCaption(),
                        ProcessName = process.ProcessName,
                        ProductName = productName,
                        OriginalWindowHandle = clipboardContents.OwnerHandle,
                        SequenceNumber = clipboardContents.Id,
                        OriginalFormats = clipboardContents.Formats.ToList()
                    };
                    using (ClipboardNative.Lock())
                    {
                        foreach (var format in clipboardContents.Formats)
                        {
                            if (!_dopyConfiguration.CopyAlways.Contains(format))
                            {
                                continue;
                            }
                            clip.Formats.Add(format);
                            clip.Contents[format] = ClipboardNative.GetAsStream(format);
                        }
                    }
                    _clipRepository.Insert(clip);
                    _eventAggregator.PublishOnUIThread(new ClipAddedMessage());
                }
            });
        }

        public void Shutdown()
        {
            _clipboardMonitor?.Dispose();
        }
    }
}
