using System.Diagnostics.CodeAnalysis;
using Dapplo.Config.Language;

namespace Dapplo.Dopy.Container.Translations.Impl
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class CoreTranslationsImpl : LanguageBase<ICoreTranslations>, ICoreTranslations
    {
        #region Implementation of ICoreTranslations

        public string Cancel { get; }
        public string Ok { get; }

        #endregion

        #region Implementation of ICoreTranslations

        public string Configure { get; }

        #endregion
    }
}
