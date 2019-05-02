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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Dapplo.CaliburnMicro.Toasts.ViewModels;
using Dapplo.Log;

namespace Dapplo.Dopy.OpenUri.ViewModels
{
    /// <summary>
    /// The ViewModel which presents the user a toast where (s)he can select to open Uri's on the clipboard
    /// </summary>
    public class OpenUriToastViewModel : ToastBaseViewModel
    {
        private static readonly LogSource Log = new LogSource();
        
        public ObservableCollection<Uri> Uris { get; }

        /// <summary>
        /// Create the ViewModel for the OpenUriToastViewModel
        /// </summary>
        /// <param name="uris">Uris</param>
        public OpenUriToastViewModel(IEnumerable<Uri> uris)
        {
            Uris = new ObservableCollection<Uri>(uris);
        }

        /// <summary>
        /// Place the modified clip to the clipboard, called from the view
        /// </summary>
        public void Open(Uri uri)
        {
            try
            {
                // In dotnet core we need shell-execute
                var processStartInfo = new ProcessStartInfo(uri.AbsoluteUri)
                {
                    UseShellExecute = true
                };
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Log.Error().WriteLine(ex, "Couldn't open url {0} in the default browser.");
            }
        }
    }
}
