using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
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
using Dapplo.Log;
using Dapplo.Utils.Extensions;

namespace Dapplo.Dopy.Container.UseCases.Configuration.ViewModels
{
    [Export(typeof(IConfigScreen))]
    [SuppressMessage("Sonar Code Smell", "S110:Inheritance tree of classes should not be too deep", Justification = "MVVM Framework brings huge interitance tree.")]
    public sealed class ThemeConfigViewModel : ConfigScreen, IDisposable
    {
        private static readonly LogSource Log = new LogSource();

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

        [Import(typeof(IWindowManager))]
        private MetroWindowManager MetroWindowManager { get; set; }

        [Import]
        public IUiConfiguration UiConfiguration { get; set; }

        [Import]
        public IConfigTranslations UiTranslations { get; set; }

        /// <summary>
        ///     Used to show a "normal" dialog
        /// </summary>
        [Import]
        private IWindowManager WindowsManager { get; set; }

        public override void Commit()
        {
            // Manually commit
            UiConfiguration.CommitTransaction();
            MetroWindowManager.ChangeTheme(UiConfiguration.Theme);
            MetroWindowManager.ChangeThemeAccent(UiConfiguration.ThemeAccent);
            base.Commit();
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
