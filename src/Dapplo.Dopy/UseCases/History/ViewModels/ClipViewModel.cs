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
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Translations;

namespace Dapplo.Dopy.UseCases.History.ViewModels
{
    /// <summary>
    /// Viewmodel for the clipboard "clip" entry
    /// </summary>
    [Export]
    public class ClipViewModel : Screen
    {
        /// <summary>
        /// The actual clip to display
        /// </summary>
        public Clip Item { get; set; }

#if DEBUG
        /// <summary>
        /// Designtime constructor, not compiled in release
        /// </summary>
        public ClipViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                throw new InvalidOperationException("Should only be used in design mode.");
            }
            Item = new Clip
            {
                SequenceNumber = 10,
                OriginalWindowHandle = new IntPtr(100000),
                Formats = new List<string> {"CF_TEXT", "PNG" },
                OriginalFormats = new List<string> { "CF_TEXT", "PNG", "Something unneeded" },
                WindowTitle = "Not existing",
                ProcessName = "bollocks.exe",
                ProductName = "Not the Dapplo"
            };
        }
#endif

        /// <summary>
        /// Constructor for runtime
        /// </summary>
        /// <param name="dopyTranslations">IDopyTranslations</param>
        [ImportingConstructor]
        public ClipViewModel(IDopyTranslations dopyTranslations
            )
        {
            dopyTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.History));
        }
    }
}
