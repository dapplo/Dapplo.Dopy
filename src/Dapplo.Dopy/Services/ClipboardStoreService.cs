//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using Autofac.Features.AttributeFilters;
using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.Dopy.Configuration;
using Dapplo.Dopy.Shared;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Shared.Extensions;
using Dapplo.Dopy.Shared.Repositories;
using Dapplo.Log;
using Dapplo.Windows.Advapi32;
using Dapplo.Windows.Clipboard;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.Messages;
using Dapplo.Windows.Icons;
using Dapplo.Windows.Kernel32;

namespace Dapplo.Dopy.Services
{
    /// <summary>
    /// This service takes care of automatically storing every clipboard change to the IClipboardRepository
    /// </summary>
    [Service(nameof(ClipboardStoreService), nameof(CaliburnServices.CaliburnMicroBootstrapper))]
    public class ClipboardStoreService : IStartup, IShutdown
    {
        private static readonly LogSource Log = new LogSource();
        private IDisposable _clipboardMonitor;
        private readonly SynchronizationContext _uiSynchronizationContext;
        private readonly IClipRepository _clipRepository;
        private readonly IDopyConfiguration _dopyConfiguration;
        private readonly Session _currentSession;

        /// <summary>
        /// Initializes the needed depedencies
        /// </summary>
        /// <param name="clipRepository">IClipRepository</param>
        /// <param name="sessionRepository">ISessionRepository</param>
        /// <param name="dopyConfiguration">Configuration</param>
        /// <param name="uiSynchronizationContext">SynchronizationContext to register the Clipboard Monitor with</param>
        public ClipboardStoreService(
            IClipRepository clipRepository,
            ISessionRepository sessionRepository,
            IDopyConfiguration dopyConfiguration,
            [KeyFilter("ui")]SynchronizationContext uiSynchronizationContext)
        {
            _clipRepository = clipRepository;
            _uiSynchronizationContext = uiSynchronizationContext ?? throw new ArgumentNullException(nameof(uiSynchronizationContext));
            _dopyConfiguration = dopyConfiguration;

            var currentSession = CreateSession();
            _currentSession = sessionRepository.Find(session => session.SessionSid == currentSession.SessionSid).FirstOrDefault();
            if (_currentSession != null)
            {
                return;
            }
            sessionRepository.Create(currentSession);
            _currentSession = currentSession;
        }

        /// <summary>
        /// Start will register all needed services
        /// </summary>
        public void Startup()
        {
            bool firstClip = true;
            _clipboardMonitor = ClipboardNative
                .OnUpdate
                .SubscribeOn(_uiSynchronizationContext)
                .Where(contents => contents.OwnerHandle != WinProcHandler.Instance.Handle)
                .Synchronize().Subscribe(clipboardContents =>
            {
                // Ignore our own modifications
                if (clipboardContents.IsModifiedByDopy())
                {
                    Log.Info().WriteLine("Skipping clipboard id {0} as it was pasted by 'us'", clipboardContents.Id);
                    return;
                }

                // Ignore empty clips (e.g. if Dopy is started directly at windows startup)
                if (!clipboardContents.Formats.Any() && firstClip)
                {
                    firstClip = false;
                    Log.Info().WriteLine("Empty clipboard at startup, restoring latest from DB");
                    // Empty clipboard, Place the last stored on the clipboard
                    _clipRepository.Find().OrderByDescending(databaseClip => databaseClip.Timestamp).LastOrDefault()?.PlaceOnClipboard();
                    return;
                }
                firstClip = false;

                // Double detection, if there is already a clip with the same sequence and session, we already got it
                if (_clipRepository.Find(clip => clip.SequenceNumber == clipboardContents.Id && clip.SessionId == _currentSession.Id).Any())
                {
                    Log.Info().WriteLine("Clipboard with id {0} already stored.", clipboardContents.Id);
                    return;
                }

                // The clipboard contents are unique, so store them
                Log.Info().WriteLine("Processing clipboard id {0}.", clipboardContents.Id);
                var newClip = CreateClip(clipboardContents);

                // Store it in the repository
                _clipRepository.Create(newClip);
            });
        }

        /// <summary>
        /// Create the session object instance, this is used to reference from every clip
        /// </summary>
        /// <returns>Session</returns>
        private Session CreateSession()
        {
            return new Session
            {
                SessionSid = Advapi32Api.CurrentSessionId,
                Domain = Environment.UserDomainName,
                Username = Environment.UserName,
                WindowsStartup = Kernel32Api.SystemStartup,
                Timestamp = DateTimeOffset.Now
            };
        }

        /// <summary>
        /// Create the full-blown Clip object
        /// </summary>
        /// <param name="clipboardUpdateInformation">ClipboardContents</param>
        /// <returns>Clip</returns>
        private Clip CreateClip(ClipboardUpdateInformation clipboardUpdateInformation)
        {
            if (clipboardUpdateInformation.OwnerHandle == IntPtr.Zero)
            {
                // TODO: Handle "0" here!!!
                Log.Warn().WriteLine("Clipboard content is owned by process 0");
            }
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

                // Make sure we got something, when the caption is emtry up to now
                if (string.IsNullOrEmpty(caption))
                {
                    caption = process.MainWindowTitle;
                }

                // Try to create the product name
                string productName = process.ProcessName;

                try
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(process.MainModule.FileName);
                    productName = versionInfo.ProductName;
                }
                catch (Win32Exception ex)
                {
                    // This happens with elevated processes
                    Log.Warn().WriteLine(ex, "Problem retrieving process information for a process with ID {0} and name {1}", process.Id, process.ProcessName);
                }

                clip = new Clip
                {
                    WindowTitle = caption,
                    OwnerIcon = interopWindow.GetIcon<BitmapSource>(true) ?? toplevelWindow.GetIcon<BitmapSource>(true),
                    SessionId = _currentSession.Id,
                    ProcessName = process.ProcessName,
                    ProductName = productName,
                    OriginalWindowHandle = clipboardUpdateInformation.OwnerHandle,
                    SequenceNumber = clipboardUpdateInformation.Id,
                    OriginalFormats = clipboardUpdateInformation.Formats.ToList()
                };
            }

            using (var clipboardAccessToken = ClipboardNative.Access())
            {
                clip.Filenames = clipboardAccessToken.GetFilenames().ToList();
                if (clip.OriginalFormats.Contains("CF_UNICODETEXT"))
                {
                    clip.ClipboardText = clipboardAccessToken.GetAsUnicodeString();
                }

                foreach (var format in clipboardUpdateInformation.Formats)
                {
                    if (!_dopyConfiguration.IncludeFormats.Contains(format))
                    {
                        continue;
                    }
                    clip.Formats.Add(format);
                    using (var clipboardStream = clipboardAccessToken.GetAsStream(format))
                    {
                        var memoryStream = new MemoryStream((int)clipboardStream.Length);
                        clipboardStream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        clip.Contents[format] = memoryStream;
                    }
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
