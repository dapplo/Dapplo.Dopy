//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2017 Dapplo
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

#region using

using System.ComponentModel;
using Dapplo.Language;

#endregion

namespace Dapplo.Dopy.Translations
{
    /// <summary>
    /// Translations for Dopy
    /// </summary>
    [Language("Dopy")]
    public interface IDopyTranslations : ILanguage, INotifyPropertyChanged
    {
        /// <summary>
        /// The translation for the history window and menu item
        /// </summary>
        [DefaultValue("History")]
        string History { get; }

        /// <summary>
        /// The translation for the delete menu item
        /// </summary>
        [DefaultValue("Delete")]
        string Delete { get; }

        /// <summary>
        /// The translation for the process of the context menu of a clipboard entry
        /// </summary>
        [DefaultValue("Process")]
        string Process { get; }

        /// <summary>
        /// The translation for the restore menu item
        /// </summary>
        [DefaultValue("Restore")]
        string Restore { get; }

        /// <summary>
        /// The translation for the formats configuration
        /// </summary>
        [DefaultValue("Formats")]
        string FormatsConfigTitle { get; }

        /// <summary>
        /// The translation in the formats configuration for the available formats 
        /// </summary>
        [DefaultValue("Available formats")]
        string FormatsAvailable { get; }

        /// <summary>
        /// The translation in the formats configuration for the selected formats 
        /// </summary>
        [DefaultValue("Selected formats")]
        string FormatsSelected { get; }
    }
}