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

#region using

using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Container.Translations;
using Dapplo.Dopy.Container.UseCases.Configuration.ViewModels;
using MahApps.Metro.IconPacks;

#endregion

namespace Dapplo.Dopy.Container.UseCases.ContextMenu
{
    /// <summary>
    ///     This will add an extry for the configuration to the context menu
    /// </summary>
    [Menu("contextmenu")]
    public sealed class ConfigMenuItem : ClickableMenuItem
    {
        public ConfigMenuItem(
            ICoreTranslations coreTranslations,
            IWindowManager windowsManager,
            ConfigViewModel configViewModel
        )
        {
            Id = "D_Configure";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Settings
            };
            ClickAction = clickedItem =>
            {
                // Prevent should it multiple times
                if (!configViewModel.IsActive)
                {
                    windowsManager.ShowDialog(configViewModel);
                }
            };
            coreTranslations.CreateDisplayNameBinding(this, nameof(ICoreTranslations.Configure));
        }
    }
}