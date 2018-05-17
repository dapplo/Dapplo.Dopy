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
using Dapplo.Addons;
using Dapplo.Log;
using Rssdp;

namespace Dapplo.Dopy.Sharing.Services
{
    /// <summary>
    /// Adds the server for sharing and locating other dopy versions
    /// </summary>
    [ServiceOrder(int.MaxValue)]
    public class ShareServer : IStartup, IShutdown
    {
        private static readonly LogSource Log = new LogSource();
        private SsdpDeviceLocator _deviceLocator;
        private SsdpDevicePublisher _publisher;

        /// <inheritdoc />
        public void Start()
        {
            PublishDevice();
            BeginSearch();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _publisher.Dispose();
            _deviceLocator.DeviceAvailable -= DeviceAvailable;
            _deviceLocator.Dispose();
        }

        /// <summary>
        /// Publish this Dopy as a SSDP device
        /// </summary>
        private void PublishDevice()
        {
            // As this is a sample, we are only setting the minimum required properties.
            var deviceDefinition = new SsdpRootDevice()
            {
                CacheLifetime = TimeSpan.FromMinutes(30), //How long SSDP clients can cache this info.
                Location = new Uri("http://mydevice/descriptiondocument.xml"), // Must point to the URL that serves your devices UPnP description document. 
                DeviceType = "urn:schemas-upnp-org:device:Basic:1",
                FriendlyName = "Dapplo.Dopy sharing service",
                Manufacturer = "Dapplo",
                ModelName = "Share service",
                ModelNumber = "1.0",
                Icons = {
                    new SsdpDeviceIcon {
                        MimeType = "image/svg+xml",
                        ColorDepth = 256,
                        Height = 64,
                        Width = 64,
                        Url = new Uri("http://dapplo.net/images/d.svg")
                    }
                },
                ModelDescription = "Dapplo.Dopy is a clipboard sharing service",
                ManufacturerUrl = new Uri("http://dapplo.net"),
                Uuid = Guid.NewGuid().ToString() // This must be a globally unique value that survives reboots etc. Get from storage or embedded hardware etc.
            };
            deviceDefinition.ToDescriptionDocument();

            deviceDefinition.AddService(new SsdpService
            {
                ServiceVersion = 1,
                Uuid = Guid.NewGuid().ToString(),
                ServiceType = "urn:schemas-upnp-org:service:Dimming:1",
                ScpdUrl = new Uri("/dopy.xml", UriKind.Relative),
                EventSubUrl = new Uri("/Dopy/CopyClip", UriKind.Relative),
                ControlUrl = new Uri("/Dopy/PasteClip", UriKind.Relative),
            });
            _publisher = new SsdpDevicePublisher();
            _publisher.AddDevice(deviceDefinition);
        }

        /// <summary>
        /// Track the SSDP devices, to see where we can share to
        /// </summary>
        private void BeginSearch()
        {
            _deviceLocator = new SsdpDeviceLocator
            {
                // (Optional) Set the filter so we only see notifications for devices we care about 
                // (can be any search target value i.e device type, uuid value etc - any value that appears in the 
                // DiscoverdSsdpDevice.NotificationType property or that is used with the searchTarget parameter of the Search method).
                NotificationFilter = "upnp:rootdevice"
            };


            // Connect our event handler so we process devices as they are found
            _deviceLocator.DeviceAvailable += DeviceAvailable;

            // Enable listening for notifications (optional)
            _deviceLocator.StartListeningForNotifications();

            // Perform a search so we don't have to wait for devices to broadcast notifications 
            // again to get any results right away (notifications are broadcast periodically).
            _deviceLocator.SearchAsync();
        }

        /// <summary>
        /// Processes the discovery information
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="deviceAvailableEventArgs">DeviceAvailableEventArgs</param>
        private static async void DeviceAvailable(object sender, DeviceAvailableEventArgs deviceAvailableEventArgs)
        {
            try
            {
                var fullDevice = await deviceAvailableEventArgs.DiscoveredDevice.GetDeviceInfo();
                Log.Info().WriteLine("Found {0} - {1} at {2}", fullDevice.FriendlyName, deviceAvailableEventArgs.DiscoveredDevice.Usn, deviceAvailableEventArgs.DiscoveredDevice.DescriptionLocation);
            }
            catch (Exception ex)
            {
                Log.Error().WriteLine(ex, "Couldn't get details for {0} at {1}", deviceAvailableEventArgs.DiscoveredDevice.Usn, deviceAvailableEventArgs.DiscoveredDevice.DescriptionLocation);
            }
        }

    }
}
