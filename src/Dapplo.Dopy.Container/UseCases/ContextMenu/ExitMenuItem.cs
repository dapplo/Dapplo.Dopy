﻿//  Dapplo - building blocks for desktop applications
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

#region using

using System.Windows;
using System.Windows.Media;
using Dapplo.CaliburnMicro.Dapp;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Container.Translations;
using MahApps.Metro.IconPacks;

#endregion

namespace Dapplo.Dopy.Container.UseCases.ContextMenu
{
    /// <summary>
    ///     This will add an extry for the exit to the context menu
    /// </summary>
    [Menu("contextmenu")]
    public sealed class ExitMenuItem : ClickableMenuItem
    {
        public ExitMenuItem(
            IMainContextMenuTranslations contextMenuTranslations
        )
        {
            // automatically update the DisplayName
            contextMenuTranslations.CreateDisplayNameBinding(this, nameof(IMainContextMenuTranslations.Exit));
            Id = "Z_Exit";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Close,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch
            };
            ClickAction = clickedItem =>
            {
                Dapplication.Current.Shutdown();
            };
            this.ApplyIconForegroundColor(Brushes.DarkRed);
        }
    }
}