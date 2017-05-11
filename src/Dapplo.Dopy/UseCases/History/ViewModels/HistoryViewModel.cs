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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Dopy.Entities;
using Dapplo.Dopy.Repositories;
using Dapplo.Dopy.Translations;
using Dapplo.Log;
using Dapplo.Windows.Clipboard;
using Dapplo.Windows.Desktop;

namespace Dapplo.Dopy.UseCases.History.ViewModels
{
    /// <summary>
    /// Viewmodel for the history
    /// </summary>
    [Export]
    public class HistoryViewModel : Screen, IHandle<ClipAddedMessage>
    {
        private static readonly LogSource Log = new LogSource();
        private readonly IClipRepository _clipRepository;
        private readonly IEventAggregator _eventAggregator;
        private bool _autoScroll;

        /// <summary>
        /// Changing this makes the history autoscroll
        /// </summary>
        public bool AutoScroll
        {
            get { return _autoScroll; }
            set
            {
                _autoScroll = value;
                NotifyOfPropertyChange();
            }
        }
#if DEBUG
        /// <summary>
        /// Designtime constructor, not compiled in release
        /// </summary>
        public HistoryViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                throw new InvalidOperationException("Should only be used in design mode.");
            }
            Clips.Add(new Clip
            {
                SequenceNumber = 10,
                OriginalWindowHandle = new IntPtr(100000),
                Formats = new List<string> {"CF_TEXT", "PNG" },
                OriginalFormats = new List<string> { "CF_TEXT", "PNG", "Something unneeded" },
                WindowTitle = "Not existing",
                ProcessName = "bollocks.exe",
                ProductName = "Not the Dapplo"
            });
            Clips.Add(new Clip
            {
                SequenceNumber = 12,
                OriginalWindowHandle = new IntPtr(100001),
                Formats = new List<string> { "CF_TEXT", "CF_UNICODETEXT" },
                OriginalFormats = new List<string> { "CF_TEXT", "PNG", "Something unneeded" },
                WindowTitle = "Not existing",
                ProcessName = "bollocks.exe",
                ProductName = "Not the Dapplo"
            });
        }
#endif

        /// <summary>
        /// Constructor for runtime
        /// </summary>
        /// <param name="clipRepository">IClipRepository</param>
        /// <param name="dopyTranslations">IDopyTranslations</param>
        /// <param name="eventAggregator">IEventAggregator to publish new clips</param>
        [ImportingConstructor]
        public HistoryViewModel(
            IClipRepository clipRepository,
            IDopyTranslations dopyTranslations,
            IEventAggregator eventAggregator
            )
        {
            _clipRepository = clipRepository;
            dopyTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.History));
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Public access for the View
        /// </summary>
        public IObservableCollection<Clip> Clips { get; } = new BindableCollection<Clip>();

        /// <inheritdoc />
        protected override void OnActivate()
        {
            Load();
            base.OnActivate();
            _eventAggregator.Subscribe(this);

        }

        /// <inheritdoc />
        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        /// <summary>
        /// Load the clips from the repository
        /// </summary>
        public void Load()
        {
            Clips.Clear();
            Clips.AddRange(_clipRepository.List());
        }

        /// <inheritdoc />
        public void Handle(ClipAddedMessage message)
        {
            Clips.Add(message.NewClip);
            AutoScroll = true;
        }

        /// <summary>
        /// Delete the specified clip
        /// </summary>
        /// <param name="clip">Clip</param>
        public void Delete(Clip clip)
        {
            Log.Debug().WriteLine("Clip {0} is going to be deleted.", clip.Id);
            _clipRepository.Delete(clip);
            Clips.Remove(clip);
        }

        /// <summary>
        /// Place the specified clip back on the clipboard
        /// </summary>
        /// <param name="clip">Clip</param>
        public void Restore(Clip clip)
        {
            var handle = IntPtr.Zero;
            
            if (InteropWindowFactory.CreateFor(clip.OriginalWindowHandle).Exits())
            {
                handle = clip.OriginalWindowHandle;
            }
            using (ClipboardNative.Lock(handle))
            {
                ClipboardNative.Clear();
                foreach (var key in clip.Contents.Keys)
                {
                    ClipboardNative.SetAsMemoryStream(key, clip.Contents[key]);
                }
            }
        }
    }
}
