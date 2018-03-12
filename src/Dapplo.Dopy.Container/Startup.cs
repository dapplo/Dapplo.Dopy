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

#region using

using System;
using System.Windows;
using Dapplo.CaliburnMicro.Dapp;
using Dapplo.Log;
using Dapplo.Log.Loggers;

#endregion

namespace Dapplo.Dopy.Container
{
    /// <summary>
    ///     This takes care or starting the Application
    /// </summary>
    public static class Startup
    {
        /// <summary>
        ///     Start the application
        /// </summary>
        [STAThread]
        public static void Main()
        {


#if DEBUG
            // Initialize a debug logger for Dapplo packages
            LogSettings.RegisterDefaultLogger<DebugLogger>(LogLevels.Verbose);
#endif
            var application = new Dapplication("Dapplo.Dopy", "06486F0F-0DBC-4912-9C5C-5C9C777BA34E")
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };

            // Load the Application.Demo.* assemblies
            application.Bootstrapper.FindAndLoadAssemblies("Dapplo.Dopy*");

            application.Run();
        }
    }
}