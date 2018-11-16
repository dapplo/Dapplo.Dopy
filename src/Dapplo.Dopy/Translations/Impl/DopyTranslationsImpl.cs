using System.Diagnostics.CodeAnalysis;
using Dapplo.Config.Language;
#pragma warning disable 1591

namespace Dapplo.Dopy.Translations.Impl
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class DopyTranslationsImpl : LanguageBase<IDopyTranslations>, IDopyTranslations
    {
        #region Implementation of IDopyTranslations

        public string History { get; }
        public string Delete { get; }
        public string Process { get; }
        public string Restore { get; }
        public string FormatsConfigTitle { get; }
        public string FormatsAvailable { get; }
        public string FormatsSelected { get; }

        #endregion
    }
}
