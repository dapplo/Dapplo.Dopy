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

using Dapplo.CaliburnMicro.Toasts.ViewModels;
using Dapplo.Dopy.Core.Entities;
using Dapplo.Dopy.Core.Extensions;

namespace Dapplo.Dopy.Addon.SimplifyStacktrace.ViewModels
{
    /// <summary>
    /// The ViewModel which presents the user a toast where (s)he can select to update the clipboard with a cleaned stacktrace.
    /// </summary>
    public class CleanStacktraceToastViewModel : ToastBaseViewModel
    {
        private readonly Clip _clip;
        private readonly JavaStacktraceCleaner _javaStacktraceCleaner;

        /// <summary>
        /// Create the ViewModel for the JavaStacktraceCleaner which detected changes
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <param name="javaStacktraceCleaner">JavaStacktraceCleaner</param>
        public CleanStacktraceToastViewModel(Clip clip, JavaStacktraceCleaner javaStacktraceCleaner)
        {
            _clip = clip;
            _javaStacktraceCleaner = javaStacktraceCleaner;
        }

        /// <summary>
        /// Place the modified clip to the clipboard, called from the view
        /// </summary>
        public void Cleanup()
        {
            _clip.ClipboardText = _javaStacktraceCleaner.CleanStacktrace;
            _clip.IsModifiedByDopy = true;
            _clip.PlaceOnClipboard();
            TryClose(true);
        }
    }
}
