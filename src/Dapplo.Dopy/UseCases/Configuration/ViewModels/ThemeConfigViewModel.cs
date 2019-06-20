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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Metro;
using Dapplo.Config.Intercepting;
using Dapplo.Dopy.Configuration;
using Dapplo.Dopy.Core;
using Dapplo.Dopy.Translations;
using Dapplo.Utils.Extensions;

namespace Dapplo.Dopy.UseCases.Configuration.ViewModels
{
    [SuppressMessage("Sonar Code Smell", "S110:Inheritance tree of classes should not be too deep", Justification = "MVVM Framework brings huge interitance tree.")]
    public sealed class ThemeConfigViewModel : ConfigScreen, IDisposable
    {
        /// <summary>
        ///     Here all disposables are registered, so we can clean the up
        /// </summary>
        private CompositeDisposable _disposables;

        /// <summary>
        ///     The available themes
        /// </summary>
        public ObservableCollection<string> AvailableThemes { get; set; } = new ObservableCollection<string>();

        /// <summary>
        ///     The available theme colors
        /// </summary>
        public ObservableCollection<string> AvailableThemeColors { get; set; } = new ObservableCollection<string>();

        private readonly MetroThemeManager _metroThemeManager;

        public IDopyUiConfiguration UiConfiguration { get; }

        public IConfigTranslations UiTranslations { get; }

        public ThemeConfigViewModel(
                IDopyUiConfiguration uiConfiguration,
                IConfigTranslations uiTranslations,
                MetroThemeManager metroThemeManager
 )
        {
            UiConfiguration = uiConfiguration;
            UiTranslations = uiTranslations;
            _metroThemeManager = metroThemeManager;
            _metroThemeManager.ChangeTheme(UiConfiguration.Theme, UiConfiguration.ThemeColor);
        }

        /// <inheritdoc />
        public override void Rollback()
        {
            UiConfiguration.RollbackTransaction();
            _metroThemeManager.ChangeTheme(UiConfiguration.Theme, UiConfiguration.ThemeColor);
        }

        /// <inheritdoc />
        public override void Terminate()
        {
            UiConfiguration.RollbackTransaction();
            _metroThemeManager.ChangeTheme(UiConfiguration.Theme, UiConfiguration.ThemeColor);
        }

        /// <inheritdoc />
        public override void Commit()
        {
            // Manually commit
            UiConfiguration.CommitTransaction();
            // Manually commit
            _metroThemeManager.ChangeTheme(UiConfiguration.Theme, UiConfiguration.ThemeColor);
        }

        public override void Initialize(IConfig config)
        {
            // Prepare disposables
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();

            AvailableThemes.Clear();
            MetroThemeManager.AvailableThemes.ForEach(themeBaseColor => AvailableThemes.Add(themeBaseColor));
            MetroThemeManager.AvailableThemeColors.ForEach(colorScheme => AvailableThemeColors.Add(colorScheme));


            // Place this under the Ui parent
            ParentId = nameof(ConfigIds.Ui);

            // Make sure Commit/Rollback is called on the UiConfiguration
            config.Register(UiConfiguration);

            // automatically update the DisplayName
            _disposables.Add(UiTranslations.CreateDisplayNameBinding(this, nameof(IConfigTranslations.Theme)));

            // Automatically show theme changes
            _disposables.Add(
                UiConfiguration.OnPropertyChanging(nameof(UiConfiguration.Theme)).Subscribe(args =>
                {
                    if (args is PropertyChangingEventArgsEx propertyChangingEventArgsEx)
                    {
                        _metroThemeManager.ChangeTheme(propertyChangingEventArgsEx.NewValue as string, null);
                    }
                })
            );
            _disposables.Add(
                UiConfiguration.OnPropertyChanging(nameof(UiConfiguration.ThemeColor)).Subscribe(args =>
                {
                    if (args is PropertyChangingEventArgsEx propertyChangingEventArgsEx)
                    {
                        _metroThemeManager.ChangeTheme(null, propertyChangingEventArgsEx.NewValue as string);
                    }
                })
            );
            base.Initialize(config);
        }

        protected override void OnDeactivate(bool close)
        {
            _disposables?.Dispose();
            base.OnDeactivate(close);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
