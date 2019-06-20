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

using System.Linq;
using Dapplo.Windows.Clipboard;

namespace Dapplo.Dopy.Core
{
    /// <summary>
    /// Helps with the format recognisions
    /// </summary>
    public static class ClipboardFormats
    {
        /// <summary>
        /// Used to specify that the clipboard content was "created" by Dopy
        /// </summary>
        public static string Dopy { get; } = "Dapplo.Dopy";

        /// <summary>
        /// CF_UNICODETEXT
        /// </summary>
        public static  string UnicodeText { get; } = "CF_UNICODETEXT";

        public static string FileContents { get; } = "FileContents";
        public static  string Png { get; } = "PNG";
        private static string PngOfficeart { get; } = "PNG+Office Art";
        private static string Format17 { get; } = "Format17";
        private static string Jpg { get; } = "JPG";
        private static string Jfif { get; } = "JFIF";
        private static string JfifOfficeart { get; } = "JFIF+Office Art";
        private static string Gif { get; } = "GIF";
        private static string Bitmap { get; } = "System.Drawing.Bitmap";

        /// <summary>
        /// Test if the ClipboardUpdateInformation was created indirectly by Dopy, so we can excluding this from processing
        /// </summary>
        /// <param name="clipboardUpdateInformation">ClipboardUpdateInformation</param>
        /// <returns>bool if Dopy made this</returns>
        public static bool IsModifiedByDopy(this ClipboardUpdateInformation clipboardUpdateInformation)
        {
            return clipboardUpdateInformation.Formats.Contains(Dopy);
        }
    }
}
