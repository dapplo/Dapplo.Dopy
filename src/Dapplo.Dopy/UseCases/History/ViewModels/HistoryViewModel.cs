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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Autofac.Features.AttributeFilters;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Shared.Repositories;
using Dapplo.Dopy.Translations;

namespace Dapplo.Dopy.UseCases.History.ViewModels
{
    /// <summary>
    /// Viewmodel for the history
    /// </summary>
    public class HistoryViewModel : Conductor<ClipViewModel>.Collection.OneActive
    {
        private readonly IClipRepository _clipRepository;
        private IDisposable _updateSubscription;
        private bool _autoScroll;
        private readonly IEnumerable<IMenuItem> _historyMenuItems;

        /// <summary>
        /// Changing this makes the history autoscroll
        /// </summary>
        public bool AutoScroll
        {
            get => _autoScroll;
            set
            {
                _autoScroll = value;
                NotifyOfPropertyChange();
            }
        }


        /// <summary>
        /// The items for the context menu of a Clip entry in the history
        /// </summary>
        public ObservableCollection<ITreeNode<IMenuItem>> MenuItems { get; } = new ObservableCollection<ITreeNode<IMenuItem>>();

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
            Items.Add(new ClipViewModel {
                Item = new Clip {
                    SequenceNumber = 10,
                    OriginalWindowHandle = new IntPtr(100000),
                    Formats = new List<string> {"CF_TEXT", "PNG" },
                    OriginalFormats = new List<string> { "CF_TEXT", "PNG", "Something unneeded" },
                    WindowTitle = "Not existing",
                    ProcessName = "bollocks.exe",
                    ProductName = "Not the Dapplo"
                }
            });
            Items.Add(new ClipViewModel
            {
                Item = new Clip
                {
                    SequenceNumber = 12,
                    OriginalWindowHandle = new IntPtr(100001),
                    Formats = new List<string> { "CF_TEXT", "CF_UNICODETEXT" },
                    OriginalFormats = new List<string> { "CF_TEXT", "PNG", "Something unneeded" },
                    WindowTitle = "Not existing",
                    ProcessName = "bollocks.exe",
                    ProductName = "Not the Dapplo"
                }
            });
        }
#endif

        /// <summary>
        /// Constructor for runtime
        /// </summary>
        /// <param name="clipRepository">IClipRepository</param>
        /// <param name="dopyTranslations">IDopyTranslations</param>
        /// <param name="historyMenuItems">IMenuItems for the history menu</param>
        public HistoryViewModel(
            IClipRepository clipRepository,
            IDopyTranslations dopyTranslations,
            [MetadataFilter("Menu", "menu")]IEnumerable<Lazy<IMenuItem>> historyMenuItems)
        {
            _clipRepository = clipRepository;
            dopyTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.History));

            _historyMenuItems = historyMenuItems.Select(lazy => lazy.Value).ToList();

            // Make sure the $clip is supported
            MessageBinder.SpecialValues.Add("$clip", context => context?.EventArgs == null ? null : ActiveItem.Item);
        }

        /// <inheritdoc />
        protected override void OnActivate()
        {
            Load();
            
            // Make sure all items are initialized
            foreach (var menuItem in _historyMenuItems)
            {
                menuItem.Initialize();
            }

            foreach (var historyMenuItem in _historyMenuItems.CreateTree())
            {
                MenuItems.Add(historyMenuItem);
            }
            base.OnActivate();

        }

        /// <inheritdoc />
        protected override void OnDeactivate(bool close)
        {
            MenuItems.Clear();
            _updateSubscription?.Dispose();
            base.OnDeactivate(close);
        }

        /// <summary>
        /// Load the clips from the repository
        /// </summary>
        public void Load()
        {
            Items.Clear();
            Items.AddRange(_clipRepository.Find().Select(clip => new ClipViewModel{ Item = clip}));

            _updateSubscription = _clipRepository.Updates.Subscribe(args =>
            {
                switch (args.CrudAction)
                {
                    case CrudActions.Create:
                        Items.Add(new ClipViewModel{ Item = args.Entity});
                        AutoScroll = true;
                        break;
                    case CrudActions.Delete:
                        Items.Remove(Items.First(model => model.Item.Id == args.Entity.Id));
                        break;
                }
            });
        }
    }
}
