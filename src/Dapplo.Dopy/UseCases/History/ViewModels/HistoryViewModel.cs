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
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Dopy.Entities;
using Dapplo.Dopy.Repositories;
using Dapplo.Dopy.Translations;

namespace Dapplo.Dopy.UseCases.History.ViewModels
{
    [Export]
    public class HistoryViewModel : Screen
    {
        private readonly IClipRepository _clipRepository;

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
                WindowTitle = "Not existing",
                ProcessName = "bollocks.exe",
                ProductName = "Not the Dapplo"
            });
            Clips.Add(new Clip
            {
                SequenceNumber = 12,
                OriginalWindowHandle = new IntPtr(100001),
                Formats = new List<string> { "CF_TEXT", "CF_UNICODETEXT" },
                WindowTitle = "Not existing",
                ProcessName = "bollocks.exe",
                ProductName = "Not the Dapplo"
            });
        }

        [ImportingConstructor]
        public HistoryViewModel(
            IClipRepository clipRepository,
            IDopyTranslations dopyTranslations
            )
        {
            _clipRepository = clipRepository;
            dopyTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.History));
        }

        /// <summary>
        /// Public access for the View
        /// </summary>
        public IObservableCollection<Clip> Clips { get; } = new BindableCollection<Clip>();

        protected override void OnActivate()
        {
            Load();
            base.OnActivate();
        }

        /// <summary>
        /// Load the clips from the repository
        /// </summary>
        public void Load()
        {
            Clips.Clear();
            Clips.AddRange(_clipRepository.List().Take(10));
        }
    }
}
