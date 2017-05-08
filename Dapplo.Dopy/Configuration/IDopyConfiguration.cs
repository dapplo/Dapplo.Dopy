using System.Collections.Generic;
using System.ComponentModel;
using Dapplo.Ini;
using Dapplo.InterfaceImpl.Extensions;

namespace Dapplo.Dopy.Configuration
{
    [IniSection("Dopy")]
    public interface IDopyConfiguration : IIniSection, IDefaultValue
    {
        [DefaultValue("PNG,CF_UNICODETEXT,CF_WAVE,HTML Format")]
        IList<string> CopyAlways { get; set; }
    }
}
