//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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
        public override void Rollback()
        {
            // Nothing to do
        }

        /// <inheritdoc />
        public override void Terminate()
        {
            // Nothing to do
        }

        /// <inheritdoc />
        public override void Commit()
        {
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
