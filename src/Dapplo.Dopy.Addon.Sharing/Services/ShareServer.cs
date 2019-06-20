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
using System.Reactive.Linq;
using Dapplo.Addons;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Shared.Repositories;

namespace Dapplo.Dopy.Sharing.Services
{
    /// <summary>
    /// Adds the server for sharing and locating other dopy versions
    /// </summary>
    [Service(nameof(ShareServer))]
    public class ShareServer : IStartup
    {
        private readonly IClipRepository _clipRepository;

        /// <summary>
        /// Constructor for the dependencies
        /// </summary>
        /// <param name="clipRepository"></param>
        public ShareServer(IClipRepository clipRepository)
        {
            _clipRepository = clipRepository;
            
        }
        /// <inheritdoc />
        public void Startup()
        {
            _clipRepository.Updates.Where(args => args.CrudAction == CrudActions.Create).Subscribe(PublishChange);
        }

        private void PublishChange(RepositoryUpdateArgs<Clip> clipUpdate)
        {

        }
    }
}
