using System.Diagnostics.CodeAnalysis;
using Dapplo.Config.Language;

namespace Dapplo.Dopy.Container.Translations.Impl
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class MainContextMenuTranslationsImpl : LanguageBase<IMainContextMenuTranslations>, IMainContextMenuTranslations
    {
        #region Implementation of IMainContextMenuTranslations

        public string Exit { get; }
        public string Title { get; }

        #endregion
    }
}
