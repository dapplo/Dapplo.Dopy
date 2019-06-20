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

#region using

using System;
using Autofac.Features.OwnedInstances;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Translations;
using Dapplo.Dopy.UseCases.Configuration.ViewModels;
using MahApps.Metro.IconPacks;

#endregion

namespace Dapplo.Dopy.UseCases.ContextMenu
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
            Func<Owned<ConfigViewModel>> configViewModelFactory
        )
        {
            Id = "D_Configure";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Settings
            };
            ClickAction = clickedItem =>
            {
                IsEnabled = false;
                try
                {
                    using (var configViewModel = configViewModelFactory())
                    {
                        windowsManager.ShowDialog(configViewModel.Value);
                    }
                }
                finally
                {
                    // TODO: Check why this might cause a NullReferenceException in the PresentationFramework at System.Windows.DeferredAppResourceReference.GetValue(BaseValueSourceInternal valueSource)
                    IsEnabled = true;
                }
            };
            coreTranslations.CreateDisplayNameBinding(this, nameof(ICoreTranslations.Configure));
        }
    }
}