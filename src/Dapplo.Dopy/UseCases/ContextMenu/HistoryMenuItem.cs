//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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
using Autofac.Features.OwnedInstances;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Translations;
using Dapplo.Dopy.UseCases.History.ViewModels;
using Dapplo.Windows.Input.Enums;
using Dapplo.Windows.Input.Keyboard;
using MahApps.Metro.IconPacks;

namespace Dapplo.Dopy.UseCases.ContextMenu
{
    /// <summary>
    ///     This will add an extry for the exit to the context menu
    /// </summary>
    [Menu("contextmenu")]
    public sealed class HistoryMenuItem : ClickableMenuItem
    {
        /// <summary>
        /// The constructor for the history MenuItem
        /// </summary>
        /// <param name="dopyContextMenuTranslations"></param>
        /// <param name="windowManager"></param>
        /// <param name="historyViewModelFactory"></param>
        public HistoryMenuItem(
            IDopyTranslations dopyContextMenuTranslations,
            IWindowManager windowManager,
            Func<Owned<HistoryViewModel>> historyViewModelFactory
        )
        {
            // automatically update the DisplayName
            dopyContextMenuTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.History));
            Id = "Y_History";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.History
            };

            // Key to react to
            var controlShiftPasteKey = new KeyCombinationHandler(VirtualKeyCode.Control, VirtualKeyCode.Shift, VirtualKeyCode.KeyV);

            KeyboardHook.KeyboardEvents
                // The hotkey to listen do
                .Where(controlShiftPasteKey)
#if !NETCOREAPP3_0
// TODO: What about dotnet core 3.0?
                // Make sure it's on the dispatcher
                .SubscribeOnDispatcher()
#endif
                // What to do
                .Subscribe(args =>
                {
                    if (IsEnabled)
                    {
                        Click(this);
                    }
                    args.Handled = true;
                });

            HotKeyHint = "Ctrl + Shift + V";
            ClickAction = clickedItem =>
            {
                IsEnabled = false;

                try
                {
                    using (var historyViewModel = historyViewModelFactory())
                    {
                        windowManager.ShowDialog(historyViewModel.Value);
                    }
                }
                finally
                {
                    IsEnabled = true;
                }
            };
        }
    }
}