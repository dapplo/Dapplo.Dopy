﻿//  Dapplo - building blocks for desktop applications
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

using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Dopy.Core;
using Dapplo.Dopy.Translations;

namespace Dapplo.Dopy.UseCases.Configuration.ViewModels
{
    /// <summary>
    /// This represents a node in the config
    /// </summary>
    public sealed class UiConfigNodeViewModel : ConfigNode
    {
        public IConfigTranslations ConfigTranslations { get; }

        public UiConfigNodeViewModel(IConfigTranslations configTranslations)
        {
            ConfigTranslations = configTranslations;

            // automatically update the DisplayName
            ConfigTranslations.CreateDisplayNameBinding(this, nameof(IConfigTranslations.Ui));

            // automatically update the DisplayName
            CanActivate = false;
            Id = nameof(ConfigIds.Ui);
        }
    }
}
