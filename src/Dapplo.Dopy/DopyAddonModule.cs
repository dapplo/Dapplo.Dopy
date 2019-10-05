﻿//  Dapplo - building blocks for desktop applications
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

using Autofac;
using Autofac.Features.AttributeFilters;
using Dapplo.Addons;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.CaliburnMicro.Metro;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.Config.Ini;
using Dapplo.Config.Language;
using Dapplo.Dopy.Configuration;
using Dapplo.Dopy.Services;
using Dapplo.Dopy.Translations;
using Dapplo.Dopy.UseCases.Configuration.ViewModels;
using Dapplo.Dopy.UseCases.ContextMenu.ViewModels;
using Dapplo.Dopy.UseCases.History.ViewModels;

namespace Dapplo.Dopy
{
    /// <inheritdoc />
    public class DopyAddonModule : AddonModule
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c =>
                {
                    var metroConfiguration = IniSection<IDopyUiConfiguration>.Create();

                    // add specific code
                    var metroThemeManager = c.Resolve<MetroThemeManager>();

                    metroConfiguration.RegisterAfterLoad(iniSection =>
                    {
                        metroThemeManager.ChangeTheme(metroConfiguration.Theme, metroConfiguration.ThemeColor);
                    });
                    return metroConfiguration;
                })
                .As<IDopyUiConfiguration>()
                .As<IIniSection>()
                .As<IMetroUiConfiguration>()
                .As<IUiConfiguration>()
                .SingleInstance();

            builder
                .Register(c =>IniSection<IDopyConfiguration>.Create())
                .As<IDopyConfiguration>()
                .As<IIniSection>()
                .SingleInstance();

            builder
                .Register(c => Language<IConfigTranslations>.Create())
                .As<IConfigTranslations>()
                .As<ILanguage>()
                .SingleInstance();

            builder
                .Register(c => Language<ICoreTranslations>.Create())
                .As<ICoreTranslations>()
                .As<ILanguage>()
                .SingleInstance();

            builder
                .Register(c => Language<IMainContextMenuTranslations>.Create())
                .As<IMainContextMenuTranslations>()
                .As<ILanguage>()
                .SingleInstance();

            builder
                .RegisterType<ConfigViewModel>()
                .AsSelf();

            // All config screens
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IConfigScreen>()
                .As<IConfigScreen>()
                .SingleInstance();

            builder
                .RegisterType<DapploTrayIconViewModel>()
                .As<ITrayIconViewModel>()
                .WithAttributeFiltering()
                .SingleInstance();

            // All IMenuItem with the context they belong to
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IMenuItem>()
                .As<IMenuItem>()
                .WithAttributeFiltering()
                .SingleInstance();

            builder
                .Register(c => Language<IDopyTranslations>.Create())
                .As<IDopyTranslations>()
                .As<ILanguage>()
                .SingleInstance();

            builder
                .RegisterType<ClipboardStoreService>()
                .As<IService>()
                .WithAttributeFiltering()
                .SingleInstance();

            builder
                .RegisterType<HistoryViewModel>()
                .AsSelf()
                .WithAttributeFiltering();

            base.Load(builder);
        }
    }
}
