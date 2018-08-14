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
using System.ComponentModel;
using System.Windows;
using Caliburn.Micro;
using Dapplo.Dopy.Shared;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Shared.Extensions;

namespace Dapplo.Dopy.UseCases.History.ViewModels
{
    /// <summary>
    /// Viewmodel for the clipboard "clip" entry
    /// </summary>
    public class ClipViewModel : Screen, IDisposable
    {
        private Clip _item;

        /// <summary>
        /// The actual clip to display
        /// </summary>
        public Clip Item
        {
            get => _item;
            set
            {
                _item = value;
                if (_item != null)
                {
                    ImageContainer = _item.GetImage();
                }
            }
        }

        /// <summary>
        /// Tells the view if the content has text
        /// </summary>
        public bool IsText => Item.HasText();

        /// <summary>
        /// Tells the view if the content has text
        /// </summary>
        public bool IsImage => ImageContainer != null;

        /// <summary>
        /// The image container
        /// </summary>
        public ImageContainer ImageContainer { get; private set; }

#if DEBUG
        /// <summary>
        /// Designtime constructor, not compiled in release
        /// </summary>
        public ClipViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Item = new Clip
                {
                    SequenceNumber = 10,
                    OriginalWindowHandle = new IntPtr(100000),
                    Formats = new List<string> { "CF_TEXT", "PNG" },
                    OriginalFormats = new List<string> { "CF_TEXT", "PNG", "Something unneeded" },
                    WindowTitle = "Not existing",
                    ProcessName = "bollocks.exe",
                    ProductName = "Not the Dapplo"
                };
            }
        }
#endif
        /// <inheritdoc />
        public void Dispose()
        {
            Item?.Dispose();
        }
    }
}
