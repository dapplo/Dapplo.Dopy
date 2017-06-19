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

#region using

using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Windows.Threading;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Translations;
using Dapplo.Dopy.UseCases.History.ViewModels;
using Dapplo.Windows.Input;
using Dapplo.Windows.Input.Enums;
using MahApps.Metro.IconPacks;

#endregion

namespace Dapplo.Dopy.UseCases.ContextMenu
{
    /// <summary>
    ///     This will add an extry for the exit to the context menu
    /// </summary>
    [Export("contextmenu", typeof(IMenuItem))]
    public sealed class HistoryMenuItem : ClickableMenuItem
    {
        /// <summary>
        /// The constructor for the history MenuItem
        /// </summary>
        /// <param name="dopyContextMenuTranslations"></param>
        /// <param name="windowManager"></param>
        /// <param name="historyViewModel"></param>
        [ImportingConstructor]
        public HistoryMenuItem(
            IDopyTranslations dopyContextMenuTranslations,
            IWindowManager windowManager,
            HistoryViewModel historyViewModel
        )
        {
            // automatically update the DisplayName
            dopyContextMenuTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.History));
            Id = "Y_History";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.History,
            };

            KeyboardHook.KeyboardEvents
                // The hotkey to listen do
                .Where(args => args.IsControl && args.IsShift && args.Key == VirtualKeyCodes.KEY_V)
                // What to do
                .Subscribe(args =>
                {
                    Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                    {
                        // Prevent should it multiple times
                        if (!historyViewModel.IsActive)
                        {
                            windowManager.ShowWindow(historyViewModel);
                        }
                    });
                    args.Handled = true;
                });

            HotKeyHint = "Ctrl + Shift + V";
            ClickAction = clickedItem =>
            {
                // Prevent should it multiple times
                if (!historyViewModel.IsActive)
                {
                    windowManager.ShowWindow(historyViewModel);
                }
            };
        }
    }
}