using System.ComponentModel;
using Dapplo.CaliburnMicro.Metro;
using Dapplo.Ini;
using Dapplo.InterfaceImpl.Extensions;

namespace Dapplo.Dopy.Container.Configuration
{
    [IniSection("Ui")]
    public interface IUiConfiguration : IIniSection, ITransactionalProperties
    {
        [DefaultValue(Themes.BaseLight)]
        Themes Theme { get; set; }

        [DefaultValue(ThemeAccents.Orange)]
        ThemeAccents ThemeAccent { get; set; }
    }
}
