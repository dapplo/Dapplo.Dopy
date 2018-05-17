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
using Caliburn.Micro;
using Dapplo.Addons;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Shared.Extensions;
using Dapplo.Dopy.Shared.Repositories;
using Dapplo.Dopy.SimplifyStacktrace.ViewModels;

namespace Dapplo.Dopy.SimplifyStacktrace
{
    /// <summary>
    /// A service to process 
    /// </summary>
    public class StacktraceService : IStartup
    {
        private readonly IClipRepository _clipRepository;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// The importing constructor
        /// </summary>
        /// <param name="eventAggregator">IEventAggregator</param>
        /// <param name="clipRepository">IClipRepository</param>
        public StacktraceService(IEventAggregator eventAggregator, IClipRepository clipRepository)
        {
            _clipRepository = clipRepository;
            _eventAggregator = eventAggregator;
        }

        /// <inheritdoc />
        public void Start()
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

            var javaStacktraceCleaner = new JavaStacktraceCleaner(clip.ClipboardText);
            if (!javaStacktraceCleaner.IsStacktrace && !javaStacktraceCleaner.HasModifications)
            {
                return;
            }

            ShowToast(clip, javaStacktraceCleaner);
        }

        /// <summary>
        /// This takes care of creating the view model, publishing it, and disposing afterwards
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <param name="javaStacktraceCleaner">JavaStacktraceCleaner</param>
        private void ShowToast(Clip clip, JavaStacktraceCleaner javaStacktraceCleaner)
        {
            var viewModel = new CleanStacktraceToastViewModel(clip, javaStacktraceCleaner);

            // Show the ViewModel as toast 
            _eventAggregator.PublishOnUIThread(viewModel);
        }
    }
}
