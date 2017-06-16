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

using System;
using System.IO;
using System.Windows.Media.Imaging;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Windows.Clipboard;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.Messages;

namespace Dapplo.Dopy.Shared.Extensions
{
    /// <summary>
    /// Helper methods for the clip object
    /// </summary>
    public static class ClipExtensions
    {
        /// <summary>
        /// Test if the clip has text
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <returns>true if it has text content</returns>
        public static bool HasText(this Clip clip)
        {
            return clip.OriginalFormats.Contains(ClipboardFormats.UnicodeText);
        }

        /// <summary>
        /// Write the OwnerIcon of the clip to a memorystream
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <returns>MemoryStream you will need to dispose this yourself</returns>
        public static MemoryStream OwnerIconAsStream(this Clip clip)
        {
            if (clip.OwnerIcon == null)
            {
                return null;
            }
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(clip.OwnerIcon));
            var memoryStreamResult = new MemoryStream();
            encoder.Save(memoryStreamResult);
            return memoryStreamResult;
        }

        /// <summary>
        /// Read the stream into the OwnerIcon of the clip
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <param name="iconMemoryStream">MemoryStream</param>
        public static void OwnerIconFromStream(this Clip clip, MemoryStream iconMemoryStream)
        {
            if (iconMemoryStream == null)
            {
                return;
            }
            var decoder = new PngBitmapDecoder(iconMemoryStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            clip.OwnerIcon = decoder.Frames[0];
        }

        /// <summary>
        /// Place the clip back onto the clipboard
        /// </summary>
        /// <param name="clip">Clip</param>
        public static void PlaceOnClipboard(this Clip clip)
        {
            var handle = WinProcHandler.Instance.Handle;

            if (InteropWindowFactory.CreateFor(clip.OriginalWindowHandle).Exists())
            {
                handle = clip.OriginalWindowHandle;
            }
            // TODO: Prevent detecting the restore, especially if Dopy doesn't "paste" with it's Window handle
            using (ClipboardNative.Lock(handle))
            {
                ClipboardNative.Clear();
                // Make the clipboard as modified by DOPY
                if (clip.IsModifiedByDopy)
                {
                    ClipboardNative.SetAsUnicodeString($"On {DateTime.Now:O}", ClipboardFormats.Dopy);
                }
                foreach (var key in clip.Contents.Keys)
                {
                    ClipboardNative.SetAsStream(key, clip.Contents[key]);
                }
                if (!string.IsNullOrEmpty(clip.ClipboardText))
                {
                    ClipboardNative.SetAsUnicodeString(clip.ClipboardText);
                }
            }
        }
    }
}
