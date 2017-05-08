using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Dapplo.Addons;
using Dapplo.Dopy.Configuration;
using Dapplo.Dopy.Entities;
using Dapplo.Log;
using Dapplo.Windows.Clipboard;
using Dapplo.Windows.Desktop;

namespace Dapplo.Dopy.Services
{
    /// <summary>
    /// This service takes care of automatically storing every clipboard change to the IClipboardRepository
    /// </summary>
    [StartupAction, ShutdownAction]
    public class ClipboardStoreService : IStartupAction, IShutdownAction
    {
        private static readonly LogSource Log = new LogSource();
        private IDisposable _clipboardMonitor;
        private readonly SynchronizationContext _uiSynchronizationContext;
        private readonly IClipRepository _clipRepository;
        private readonly IDopyConfiguration _dopyConfiguration;

        /// <summary>
        /// Initializes the needed depedencies
        /// </summary>
        /// <param name="clipRepository">IClipRepository</param>
        /// <param name="dopyConfiguration">Configuration</param>
        /// <param name="uiSynchronizationContext">SynchronizationContext to register the Clipboard Monitor with</param>
        [ImportingConstructor]
        public ClipboardStoreService(
            IClipRepository clipRepository,
            IDopyConfiguration dopyConfiguration,
            [Import("ui", typeof(SynchronizationContext))]SynchronizationContext uiSynchronizationContext)
        {
            _clipRepository = clipRepository;
            _uiSynchronizationContext = uiSynchronizationContext;
            _dopyConfiguration = dopyConfiguration;
        }

        /// <summary>
        /// Start will register all needed services
        /// </summary>
        public void Start()
        {
            _clipboardMonitor = ClipboardMonitor.OnUpdate.SubscribeOn(_uiSynchronizationContext).Synchronize().Subscribe(clipboardContents =>
            {

                Log.Info().WriteLine("Processing clipboard id {0}", clipboardContents.Id);
                var interopWindow = InteropWindowFactory.CreateFor(clipboardContents.OwnerHandle);

                using (var process = Process.GetProcessById(interopWindow.GetProcessId()))
                {
                    string productName = process.ProcessName;
                    try
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(process.MainModule.FileName);
                        productName = versionInfo.ProductName;
                    }
                    catch (Exception ex)
                    {
                        Log.Error().WriteLine(ex, "Problem retrieving process information for a process with ID {0} and name {1}", process.Id, process.ProcessName);
                    }
                    var clip = new Clip
                    {
                        WindowTitle = interopWindow.GetCaption(),
                        ProcessName = process.ProcessName,
                        ProductName = productName,
                        Formats = clipboardContents.Formats.ToList()
                    };
                    using (ClipboardNative.Lock())
                    {
                        foreach (var format in clipboardContents.Formats)
                        {
                            if (!_dopyConfiguration.CopyAlways.Contains(format))
                            {
                                continue;
                            }
                            clip.Contents[format] = ClipboardNative.GetAsStream(format);
                        }
                    }
                    _clipRepository.Insert(clip);
                }
            });
        }

        public void Shutdown()
        {
            _clipboardMonitor?.Dispose();
        }
    }
}
