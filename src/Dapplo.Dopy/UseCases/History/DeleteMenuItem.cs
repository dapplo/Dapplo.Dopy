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

using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Dopy.Core.Entities;
using Dapplo.Dopy.Core.Repositories;
using Dapplo.Dopy.Translations;
using Dapplo.Log;
using MahApps.Metro.IconPacks;

namespace Dapplo.Dopy.UseCases.History
{
    /// <summary>
    /// This makes a delete of a clip possible
    /// </summary>
    [Menu("historymenu")]
    public sealed class DeleteMenuItem : ClickableMenuItem<Clip>
    {
        private readonly IClipRepository _clipRepository;

        private static readonly LogSource Log = new LogSource();

        /// <summary>
        /// The constructor for the history MenuItem
        /// </summary>
        /// <param name="dopyContextMenuTranslations"></param>
        /// <param name="clipRepository">IClipRepository to perform actions on</param>
        public DeleteMenuItem(IDopyTranslations dopyContextMenuTranslations,
            IClipRepository clipRepository)
        {
            _clipRepository = clipRepository;
            // automatically update the DisplayName
            dopyContextMenuTranslations.CreateDisplayNameBinding(this, nameof(IDopyTranslations.Delete));
            Id = "B_Delete";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Delete
            };
        }

        /// <inheritdoc />
        public override void Click(Clip clip)
        {
            Log.Debug().WriteLine("Id = {0}", clip.Id);
            _clipRepository.Delete(clip);
        }
    }
}
