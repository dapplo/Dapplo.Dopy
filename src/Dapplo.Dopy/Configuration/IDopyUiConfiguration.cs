//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.Dopy
// 
//  Dapplo.Dopy is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.Dopy is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.Dopy. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

using System.ComponentModel;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.Config.Ini;

namespace Dapplo.Dopy.Configuration
{
    /// <summary>
    /// This specifies the configuration for MahApps.Metro
    /// </summary>
    [IniSection("Ui")]
    public interface IDopyUiConfiguration : IUiConfiguration, IMetroUiConfiguration
    {
        /// <inheritdoc cref="IMetroUiConfiguration" />
        [DefaultValue("Light")]
        new string Theme { get; set; }

        /// <inheritdoc cref="IMetroUiConfiguration" />
        [DefaultValue("Orange")]
        new string ThemeColor { get; set; }
    }
}
