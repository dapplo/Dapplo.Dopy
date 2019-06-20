using System.Diagnostics.CodeAnalysis;
using Dapplo.Config.Language;

namespace Dapplo.Dopy.Translations.Impl
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class ConfigTranslationsImpl : LanguageBase<IConfigTranslations>, IConfigTranslations
    {
        #region Implementation of IConfigTranslations

        public string Filter { get; }

        #endregion

        #region Implementation of IConfigTranslations

        public string Ui { get; }
        public string Theme { get; }

        #endregion
    }
}
