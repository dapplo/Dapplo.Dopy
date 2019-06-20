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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace Dapplo.Dopy.Core
{
    /// <summary>
    /// Container for images, supporting Exif orientation
    /// </summary>
    public class ImageContainer
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The actual image
        /// </summary>
        public ImageSource ImageData { get; private set; }

        /// <summary>
        /// Exif information
        /// </summary>
        public IEnumerable<MetadataExtractor.Directory> Metadata { get; private set; } = Enumerable.Empty<MetadataExtractor.Directory>();

        /// <summary>
        /// Dummy
        /// </summary>
        public ImageContainer()
        {
            ImageData = new BitmapImage();
        }

        /// <summary>
        /// Dummy
        /// </summary>
        public ImageContainer(string filename)
        {
            Filename = filename;
            ReadFrom(filename);
        }

        /// <summary>
        /// Constructor for memoryStream
        /// </summary>
        public ImageContainer(MemoryStream memoryStream)
        {
            ReadFrom(memoryStream);
        }

        /// <summary>
        /// handle reading from a memorystream
        /// </summary>
        private void ReadFrom(MemoryStream memoryStream)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);

            Metadata = ImageMetadataReader.ReadMetadata(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var bitmapImage = new BitmapImage();
            ImageData = bitmapImage;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
        }

        /// <summary>
        /// handle reading from a file
        /// </summary>
        public void ReadFrom(string filename)
        {
            using (var filestream = File.OpenRead(filename))
            {
                var memoryStream = new MemoryStream();
                filestream.CopyTo(memoryStream);
                ReadFrom(memoryStream);
            }
        }

        /// <summary>
        /// Returns the rotation
        /// </summary>
        public int Rotation
        {
            get
            {
                var orientation = (ushort?)Metadata?.OfType<ExifIfd0Directory>().FirstOrDefault()?.GetObject(ExifDirectoryBase.TagOrientation);
                if (!orientation.HasValue)
                {
                    return 0;
                }
                switch (orientation.Value)
                {
                    case 6:
                        return 90;
                    case 8:
                        return 270;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            }
        }
    }

}
