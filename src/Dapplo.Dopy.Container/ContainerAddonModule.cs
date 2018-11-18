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

using Autofac;
using Autofac.Features.AttributeFilters;
using Dapplo.Addons;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.Config.Ini;
using Dapplo.Config.Language;
using Dapplo.Dopy.Container.Configuration;
using Dapplo.Dopy.Container.Configuration.Impl;
using Dapplo.Dopy.Container.Translations;
using Dapplo.Dopy.Container.Translations.Impl;
using Dapplo.Dopy.Container.UseCases.Configuration.ViewModels;
using Dapplo.Dopy.Container.UseCases.ContextMenu.ViewModels;

namespace Dapplo.Dopy.Container
{
    /// <inheritdoc />
    public class ContainerAddonModule : AddonModule
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DopyUiConfigurationImpl>()
                .As<IDopyUiConfiguration>()
                .As<IIniSection>()
                .As<IMetroUiConfiguration>()
                .As<IUiConfiguration>()      
                .SingleInstance();

            builder.RegisterType<ConfigTranslationsImpl>()
                .As<IConfigTranslations>()
                .As<ILanguage>()
                .SingleInstance();

            builder.RegisterType<CoreTranslationsImpl>()
                .As<ICoreTranslations>()
                .As<ILanguage>()
                .SingleInstance();

            builder.RegisterType<MainContextMenuTranslationsImpl>()
                .As<IMainContextMenuTranslations>()
                .As<ILanguage>()
                .SingleInstance();

            builder.RegisterType<ConfigViewModel>()
                .AsSelf();

            // All config screens
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IConfigScreen>()
                .As<IConfigScreen>()
                .SingleInstance();

            builder.RegisterType<DapploTrayIconViewModel>()
                .As<ITrayIconViewModel>()
                .WithAttributeFiltering()
                .SingleInstance();

            // All IMenuItem with the context they belong to
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IMenuItem>()
                .As<IMenuItem>()
                .WithAttributeFiltering()
                .SingleInstance();

            base.Load(builder);
        }
    }
}
