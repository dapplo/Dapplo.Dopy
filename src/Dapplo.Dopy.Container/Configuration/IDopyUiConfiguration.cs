using System.ComponentModel;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Metro;
using Dapplo.Config.Ini;

namespace Dapplo.Dopy.Container.Configuration
{
    [IniSection("Ui")]
    public interface IDopyUiConfiguration : IUiConfiguration
    {
        [DefaultValue(Themes.BaseLight)]
        Themes Theme { get; set; }

        [DefaultValue(ThemeAccents.Orange)]
        ThemeAccents ThemeAccent { get; set; }
    }
}
