using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Configuration;
using ICoreTranslations = Dapplo.Dopy.Container.Translations.ICoreTranslations;
using IConfigTranslations = Dapplo.Dopy.Container.Translations.IConfigTranslations;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using MahApps.Metro.IconPacks;

namespace Dapplo.Dopy.Container.UseCases.Configuration.ViewModels
{
    /// <summary>
    ///     The settings view model is, well... for the settings :)
    ///     It is a conductor where one item is active.
    /// </summary>
    [Export]
    public sealed class ConfigViewModel : Config<IConfigScreen>
    {
        [Export("contextmenu", typeof(IMenuItem))]
        private ClickableMenuItem ConfigureMenuItem { get; }

        /// <summary>
        /// Is used from View
        /// </summary>
        public IConfigTranslations ConfigTranslations { get; }

        /// <summary>
        /// Is used from View
        /// </summary>
        public ICoreTranslations CoreTranslations { get; }

        [ImportingConstructor]
        public ConfigViewModel(
            [ImportMany] IEnumerable<Lazy<IConfigScreen>> configScreens,
            IWindowManager windowsManager,
            IConfigTranslations configTranslations,
            ICoreTranslations coreTranslations)
        {
            ConfigScreens = configScreens;
            ConfigTranslations = configTranslations;
            CoreTranslations = coreTranslations;

            // automatically update the DisplayName
            CoreTranslations.CreateDisplayNameBinding(this, nameof(ICoreTranslations.Cancel));

            ConfigureMenuItem = new ClickableMenuItem
            {
                Id = "D_Configure",
                Icon = new PackIconMaterial
                {
                    Kind = PackIconMaterialKind.Settings
                },
                ClickAction = clickedItem =>
                {
                    // Prevent should it multiple times
                    if (!IsActive)
                    {
                        windowsManager.ShowDialog(this);
                    }
                }
            };
            CoreTranslations.CreateDisplayNameBinding(ConfigureMenuItem, nameof(ICoreTranslations.Configure));
        }
    }

}
