using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace BetterConfigurationManager
{
	/// <summary>
	/// http://www.mztools.com/articles/2013/MZ2013029.aspx
	/// </summary>
	internal class DteInitializer : IVsShellPropertyEvents
	{
		internal DteInitializer(IVsShell shellService, Action callback)
		{
			this.shellService = shellService;
			this.callback = callback;
			// Set an event handler to detect when the IDE is fully initialized
			int hr = this.shellService.AdviseShellPropertyChanges(this, out cookie);
			ErrorHandler.ThrowOnFailure(hr);
		}

		private readonly IVsShell shellService;
		private readonly Action callback;
		private uint cookie;

		int IVsShellPropertyEvents.OnShellPropertyChange(int propId, object var)
		{
			if (propId != (int)__VSSPROPID.VSSPROPID_Zombie)
				return VSConstants.S_OK;
			var isZombie = (bool)var;
			if (isZombie)
				return VSConstants.S_OK;
			// Release the event handler to detect when the IDE is fully initialized
			int hr = shellService.UnadviseShellPropertyChanges(cookie);
			ErrorHandler.ThrowOnFailure(hr);
			cookie = 0;
			callback();
			return VSConstants.S_OK;
		}
	}
}