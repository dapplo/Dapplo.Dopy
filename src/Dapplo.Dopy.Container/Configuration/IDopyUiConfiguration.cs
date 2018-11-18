using System.ComponentModel;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Metro;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.Config.Ini;

namespace Dapplo.Dopy.Container.Configuration
{
    [IniSection("Ui")]
    public interface IDopyUiConfiguration : IUiConfiguration, IMetroUiConfiguration
    {
        [DefaultValue(Themes.Light)]
        new Themes Theme { get; set; }

        [DefaultValue(ThemeAccents.Orange)]
        new ThemeAccents ThemeAccent { get; set; }
    }
}
