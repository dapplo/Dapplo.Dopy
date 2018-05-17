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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Metro;
using Dapplo.Dopy.Container.Configuration;
using Dapplo.Dopy.Container.Translations;
using Dapplo.Dopy.Shared;
using Dapplo.Utils.Extensions;

namespace Dapplo.Dopy.Container.UseCases.Configuration.ViewModels
{
    [SuppressMessage("Sonar Code Smell", "S110:Inheritance tree of classes should not be too deep", Justification = "MVVM Framework brings huge interitance tree.")]
    public sealed class ThemeConfigViewModel : ConfigScreen, IDisposable
    {
        /// <summary>
        ///     Here all disposables are registered, so we can clean the up
        /// </summary>
        private CompositeDisposable _disposables;

        /// <summary>
        ///     The avaible theme accents
        /// </summary>
        public ObservableCollection<Tuple<ThemeAccents, string>> AvailableThemeAccents { get; set; } = new ObservableCollection<Tuple<ThemeAccents, string>>();

        /// <summary>
        ///     The avaible themes
        /// </summary>
        public ObservableCollection<Tuple<Themes, string>> AvailableThemes { get; set; } = new ObservableCollection<Tuple<Themes, string>>();

        private readonly MetroWindowManager _metroWindowManager;

        public IDopyUiConfiguration UiConfiguration { get; }

        public IConfigTranslations UiTranslations { get; }

        /// <summary>
        ///     Used to show a "normal" dialog
        /// </summary>
        private readonly IWindowManager _windowsManager;

        public ThemeConfigViewModel(
                IDopyUiConfiguration uiConfiguration,
                IConfigTranslations uiTranslations,
                MetroWindowManager metroWindowManager,
                IWindowManager windowsManager
            )
        {
            UiConfiguration = uiConfiguration;
            UiTranslations = uiTranslations;
            _metroWindowManager = metroWindowManager;
            _windowsManager = windowsManager;
        }

        /// <inheritdoc />
        public override void Rollback()
        {
            // Nothing to do
        }

        /// <inheritdoc />
        public override void Terminate()
        {
            // Nothing to do
        }

        /// <inheritdoc />
        public override void Commit()
        {
            // Manually commit
            UiConfiguration.CommitTransaction();
            _metroWindowManager.ChangeTheme(UiConfiguration.Theme);
            _metroWindowManager.ChangeThemeAccent(UiConfiguration.ThemeAccent);
        }

        public override void Initialize(IConfig config)
        {
            // Prepare disposables
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();

            AvailableThemeAccents.Clear();
            foreach (var themeAccent in Enum.GetValues(typeof(ThemeAccents)).Cast<ThemeAccents>())
            {
                var translation = themeAccent.EnumValueOf();
                AvailableThemeAccents.Add(new Tuple<ThemeAccents, string>(themeAccent, translation));
            }

            AvailableThemes.Clear();
            foreach (var theme in Enum.GetValues(typeof(Themes)).Cast<Themes>())
            {
                var translation = theme.EnumValueOf();
                AvailableThemes.Add(new Tuple<Themes, string>(theme, translation));
            }

            // Place this under the Ui parent
            ParentId = nameof(ConfigIds.Ui);

            // Make sure Commit/Rollback is called on the UiConfiguration
            config.Register(UiConfiguration);

            // automatically update the DisplayName
            _disposables.Add(UiTranslations.CreateDisplayNameBinding(this, nameof(IConfigTranslations.Theme)));

            base.Initialize(config);
        }

        protected override void OnDeactivate(bool close)
        {
            _disposables.Dispose();
            base.OnDeactivate(close);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
