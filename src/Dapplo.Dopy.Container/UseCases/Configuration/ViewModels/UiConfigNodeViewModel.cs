using System.ComponentModel.Composition;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Dopy.Container.Translations;
using Dapplo.Dopy.Shared;

namespace Dapplo.Dopy.Container.UseCases.Configuration.ViewModels
{
    /// <summary>
    /// This represents a node in the config
    /// </summary>
    [Export(typeof(IConfigScreen))]
    public sealed class UiConfigNodeViewModel : ConfigNode
    {
        public IConfigTranslations ConfigTranslations { get; }

        [ImportingConstructor]
        public UiConfigNodeViewModel(IConfigTranslations configTranslations)
        {
            ConfigTranslations = configTranslations;

            // automatically update the DisplayName
            ConfigTranslations.CreateDisplayNameBinding(this, nameof(IConfigTranslations.Ui));

            // automatically update the DisplayName
            CanActivate = false;
            Id = nameof(ConfigIds.Ui);
        }
    }
}
