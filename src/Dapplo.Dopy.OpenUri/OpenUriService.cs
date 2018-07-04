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
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.Dopy.OpenUri.ViewModels;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Shared.Extensions;
using Dapplo.Dopy.Shared.Repositories;

namespace Dapplo.Dopy.OpenUri
{
    /// <summary>
    /// A service to process 
    /// </summary>
    [Service(nameof(OpenUriService), nameof(CaliburnStartOrder.CaliburnMicroBootstrapper))]
    public class OpenUriService : IStartup
    {
        private static readonly Regex UriRegex = new Regex(@"([a-z]+://[a-zA-Z0-9-_]+(?::[0-9]+)?[^\s]+)", RegexOptions.Compiled);
        private readonly IClipRepository _clipRepository;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// The importing constructor
        /// </summary>
        /// <param name="eventAggregator">IEventAggregator</param>
        /// <param name="clipRepository">IClipRepository</param>
        public OpenUriService(IEventAggregator eventAggregator, IClipRepository clipRepository)
        {
            _clipRepository = clipRepository;
            _eventAggregator = eventAggregator;
        }

        /// <inheritdoc />
        public void Startup()
        {
            _clipRepository.Updates.Where(args => args.CrudAction == CrudActions.Create).Subscribe(OnNext);
        }

        /// <summary>
        /// Handle the clipboard update
        /// </summary>
        /// <param name="repositoryUpdateArgs"></param>
        private void OnNext(RepositoryUpdateArgs<Clip> repositoryUpdateArgs)
        {
            var clip = repositoryUpdateArgs.Entity;
            if (!clip.HasText())
            {
                return;
            }

            var matches = UriRegex.Matches(clip.ClipboardText);
            var uris = matches.Cast<Match>().Select(match => new Uri(match.Value)).ToList();
            if (uris.Count == 0)
            {
                return;
            }

            ShowToast(uris);
        }

        /// <summary>
        /// This takes care of creating the view model, publishing it, and disposing afterwards
        /// </summary>
        /// <param name="uris">IEnumerable with uris</param>
        private void ShowToast(IEnumerable<Uri> uris)
        {
            var viewModel = new OpenUriToastViewModel(uris);

            // Show the ViewModel as toast 
            _eventAggregator.PublishOnUIThread(viewModel);
        }
    }
}
