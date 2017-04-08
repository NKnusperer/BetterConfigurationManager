using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using BetterConfigurationManager.MainToolWindow;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BetterConfigurationManager
{
	[PackageRegistration(UseManagedResourcesOnly = true),
	InstalledProductRegistration("#110", "#112", "1.2", IconResourceID = 400),
	ProvideMenuResource("Menus.ctmenu", 1),
	ProvideToolWindow(typeof(MainToolWindowInitialize), Style = VsDockStyle.MDI, MultiInstances = false),
	Guid(Guids.BetterConfigurationManagerPackageId)]
	public sealed class BetterConfigurationManagerPackage : Package
	{
		protected override void Initialize()
		{
			base.Initialize();
			var menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (menuCommandService == null)
				return;
			var commandID = new CommandID(Guids.BetterConfigurationManagerMenuId,
				(int)PkgCmdIDList.BetterConfigurationManagerMenu);
			var menuCommand = new MenuCommand(ShowToolWindow, commandID);
			menuCommandService.AddCommand(menuCommand);
		}

		private void ShowToolWindow(object sender, EventArgs e)
		{
			ToolWindowPane window = FindToolWindow(typeof(MainToolWindowInitialize), 0, true);
			if ((window == null) || (window.Frame == null))
				throw new NotSupportedException(Resources.CanNotCreateWindow);
			var windowFrame = (IVsWindowFrame)window.Frame;
			ErrorHandler.ThrowOnFailure(windowFrame.Show());
		}
	}
}