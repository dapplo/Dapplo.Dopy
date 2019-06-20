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

using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Dapplo.Dopy.Core.Entities;
using Dapplo.Windows.Clipboard;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.Messages;

namespace Dapplo.Dopy.Core.Extensions
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
        /// Test if the clip has an image
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <returns>true if it has image content</returns>
        public static bool HasImage(this Clip clip)
        {
            if (clip?.Contents == null)
            {
                return false;
            }
            if (clip.Filenames.Any(file => file.EndsWith(".png") && File.Exists(file)))
            {
                return true;
            }
            return clip.Contents.ContainsKey(ClipboardFormats.Png);
        }

        /// <summary>
        /// Get the "best" image from a clip
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <returns>image content</returns>
        public static ImageContainer GetImage(this Clip clip)
        {
            if (clip?.Contents == null)
            {
                return null;
            }

            var imageFile = clip.Filenames.FirstOrDefault(file => file.EndsWith(".png") && File.Exists(file));
            if (imageFile != null)
            {
                return new ImageContainer(imageFile);
            }
            if (clip.Contents.ContainsKey(ClipboardFormats.Png))
            {
                return new ImageContainer(clip.Contents[ClipboardFormats.Png]);
            }
            return null;
        }

        /// <summary>
        /// Write the OwnerIcon of the clip to a memorystream
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <returns>MemoryStream you will need to dispose this yourself</returns>
        public static MemoryStream OwnerIconAsStream(this Clip clip)
        {
            if (clip?.OwnerIcon == null)
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
            var decoder = new PngBitmapDecoder(iconMemoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            clip.OwnerIcon = decoder.Frames[0];
        }

        /// <summary>
        /// Place the clip back onto the clipboard
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <param name="fromExisting">bool</param>
        public static void PlaceOnClipboard(this Clip clip, bool fromExisting = false)
        {
            var handle = WinProcHandler.Instance.Handle;

            if (InteropWindowFactory.CreateFor(clip.OriginalWindowHandle).Exists())
            {
                handle = clip.OriginalWindowHandle;
            }
            // TODO: Prevent detecting the restore, especially if Dopy doesn't "paste" with it's Window handle
            using (var clipboardAccessToken = ClipboardNative.Access(handle))
            {
                clipboardAccessToken.ClearContents();
                // Make the clipboard as modified by DOPY
                if (fromExisting ||clip.IsModifiedByDopy)
                {
                    clipboardAccessToken.SetAsUnicodeString($"On {DateTime.Now:O}", ClipboardFormats.Dopy);
                }
                foreach (var key in clip.Contents.Keys)
                {
                    clipboardAccessToken.SetAsStream(key, clip.Contents[key]);
                }
                if (!string.IsNullOrEmpty(clip.ClipboardText))
                {
                    clipboardAccessToken.SetAsUnicodeString(clip.ClipboardText);
                }
            }
        }
    }
}
