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
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Entities;
using Dapplo.Dopy.Translations;
using Dapplo.Windows.Clipboard;
using Dapplo.Windows.Desktop;
using MahApps.Metro.IconPacks;

namespace Dapplo.Dopy.UseCases.History
{
    /// <summary>
    /// This makes a delete of a clip possible
    /// </summary>
    [Export("historyMenu", typeof(IMenuItem))]
    public sealed class RestoreMenuItem : ClickableMenuItem<Clip>
    {
        /// <summary>
        /// The constructor for the history MenuItem
        /// </summary>
        /// <param name="dopyContextMenuTranslations"></param>
        [ImportingConstructor]
        public RestoreMenuItem(IDopyTranslations dopyContextMenuTranslations)
        {
            // automatically update the DisplayName
            dopyContextMenuTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.Restore));
            Id = "A_Restore";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Restore,
            };
        }

        /// <inheritdoc />
        public override void Click(Clip clip)
        {
            var handle = IntPtr.Zero;

            if (InteropWindowFactory.CreateFor(clip.OriginalWindowHandle).Exists())
            {
                handle = clip.OriginalWindowHandle;
            }
            // TODO: Prevent detecting the restore, especially if Dopy doesn't "paste" with it's Window handle
            using (ClipboardNative.Lock(handle))
            {
                ClipboardNative.Clear();
                foreach (var key in clip.Contents.Keys)
                {
                    ClipboardNative.SetAsStream(key, clip.Contents[key]);
                }
            }
        }
    }
}
