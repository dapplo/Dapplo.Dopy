using System.ComponentModel.Composition;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Dopy.Translations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Dapplo.Dopy.Configuration;
using Dapplo.Windows.Clipboard;

namespace Dapplo.Dopy.UseCases.Configuration.ViewModels
{
    /// <summary>
    /// This config-screen makes it possible to select the formats to include / exclude in the database
    /// </summary>
    [Export(typeof(IConfigScreen))]
    public sealed class FormatsViewModel : ConfigScreen
    {
        /// <summary>
        /// Translations for the view
        /// </summary>
        public IDopyTranslations DopyTranslations { get; }

        /// <summary>
        /// Configuration for the view
        /// </summary>
        public IDopyConfiguration DopyConfiguration { get; }

        /// <summary>
        /// The list of all clipboard formats which are available
        /// </summary>
        public ObservableCollection<string> AvailableFormats { get; }

        /// <summary>
        /// The list of all clipboard formats which are selected
        /// </summary>
        public ObservableCollection<string> SelectedFormats { get; }

        /// <summary>
        /// 
        /// </summary>
        [ImportingConstructor]
        public FormatsViewModel(
            IDopyTranslations dopyTranslations,
            IDopyConfiguration dopyConfiguration)
        {
            DopyTranslations = dopyTranslations;
            DopyConfiguration = dopyConfiguration;
            dopyTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.FormatsConfigTitle));
            Id = "F_Formats";
            SelectedFormats = new ObservableCollection<string>(DopyConfiguration.IncludeFormats);
            using (ClipboardNative.Lock())
            {
                AvailableFormats = new ObservableCollection<string>(ClipboardNative.AvailableFormats());
            }
        }

        /// <inheritdoc />
        public override void Commit()
        {
            base.Commit();
            DopyConfiguration.IncludeFormats = new List<string>(SelectedFormats);
        }

#if DEBUG
        /// <summary>
        /// Design-time only
        /// </summary>
        public FormatsViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                AvailableFormats = new ObservableCollection<string> {"CF_TEXT", "PNG"};
            }
        }
#endif
    }
}
