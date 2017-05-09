using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dapplo.Dopy.Container.Utils
{
    /// <summary>
    /// Workaround for culture aware binding
    /// </summary>
    public class CultureAwareBinding : Binding
    {
        public CultureAwareBinding()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                ConverterCulture = new CultureInfo("de-DE");
            }
            else
            {
                ConverterCulture = CultureInfo.CurrentCulture;
            }
        }
    }
}
