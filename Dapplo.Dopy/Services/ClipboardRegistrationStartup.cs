using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Threading;
using Dapplo.Addons;
using Dapplo.Windows.Clipboard;

namespace Dapplo.Dopy.Services
{
    [StartupAction]
	public class ClipboardRegistrationStartup : IStartupAction
	{
	    private readonly SynchronizationContext _uiSynchronizationContext;
	    private IDisposable _clipboardMonitor;

        [ImportingConstructor]
	    public ClipboardRegistrationStartup(
	        [Import("ui", typeof(SynchronizationContext))]SynchronizationContext uiSynchronizationContext)
	    {
	        _uiSynchronizationContext = uiSynchronizationContext;
	    }

	    /// <summary>
	    /// Start will register all needed services
	    /// </summary>
	    public void Start()
	    {
	        _clipboardMonitor = ClipboardMonitor.OnUpdate.SubscribeOn(_uiSynchronizationContext).Subscribe(clipboardContents =>
	        {
                // TODO: Do something with the content
	        });
	    }

    }
}
