using System.ComponentModel.Composition;
using System.Windows.Media;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Container.Translations;
using MahApps.Metro.IconPacks;

namespace Dapplo.Dopy.Container.UseCases.ContextMenu
{
    /// <summary>
    ///     This will add an extry for the title to the context menu
    /// </summary>
    [Export("contextmenu", typeof(IMenuItem))]
    public sealed class TitleMenuItem : MenuItem
    {
        [ImportingConstructor]
        public TitleMenuItem(IMainContextMenuTranslations contextMenuTranslations)
        {
            // automatically update the DisplayName
            contextMenuTranslations.CreateDisplayNameBinding(this, nameof(IMainContextMenuTranslations.Title));
            Id = "A_Title";
            Style = MenuItemStyles.Title;

            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Clipboard
            };
            this.ApplyIconForegroundColor(Brushes.SaddleBrown);
        }
    }

}
