using System.ComponentModel.Composition;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Entities;
using Dapplo.Dopy.Translations;
using Dapplo.Log;
using MahApps.Metro.IconPacks;

namespace Dapplo.Dopy.UseCases.History
{
    /// <summary>
    /// This makes a delete of a clip possible
    /// </summary>
    [Export("historyMenu", typeof(IMenuItem))]
    public sealed class DeleteMenuItem : ClickableMenuItem<Clip>
    {
        private static readonly LogSource Log = new LogSource();
        /// <summary>
        /// The constructor for the history MenuItem
        /// </summary>
        /// <param name="dopyContextMenuTranslations"></param>
        [ImportingConstructor]
        public DeleteMenuItem(IDopyTranslations dopyContextMenuTranslations)
        {
            // automatically update the DisplayName
            dopyContextMenuTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.Delete));
            Id = "A_Delete";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Delete,
            };
        }

        /// <inheritdoc />
        public override void Click(Clip clip)
        {
            Log.Debug().WriteLine("Id = {0}", clip.Id);
        }
    }
}
