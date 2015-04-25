using System.Runtime.InteropServices;
using BetterConfigurationManager.ConfigurationManager;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BetterConfigurationManager.MainToolWindow
{
	[Guid("37fe8527-01e7-45dc-b1ef-5a8b12618770")]
	public class MainToolWindowInitialize : ToolWindowPane
	{
		public MainToolWindowInitialize()
			: base(null)
		{
			Caption = Resources.ToolWindowTitle;
			BitmapResourceID = 301;
			BitmapIndex = 1;
			viewModel = new MainToolWindowViewModel();
			viewModel.ConfigurationManager = new VisualStudioConfigurationManager();
			var view = new MainToolWindowView();
			view.DataContext = viewModel;
			base.Content = view;
		}

		private readonly MainToolWindowViewModel viewModel;

		public override async void OnToolWindowCreated()
		{
			base.OnToolWindowCreated();
			await InitializeDte();
		}

		private async System.Threading.Tasks.Task InitializeDte()
		{
			dte = GetService(typeof(SDTE)) as DTE2;
			if (dte == null) // The IDE is not yet fully initialized
			{
				var shellService = GetService(typeof(SVsShell)) as IVsShell;
				new DteInitializer(shellService, async () => await InitializeDte());
			}
			else
			{
				// ReSharper disable once MaximumChainedReferences
				dte.Events.DTEEvents.OnStartupComplete += OnDteEventsStartupComplete;
				await TryInitializeVisualStudioConfigurationManager();
			}
		}

		private DTE2 dte;

		private async void OnDteEventsStartupComplete()
		{
			await TryInitializeVisualStudioConfigurationManager();
		}

		private async System.Threading.Tasks.Task TryInitializeVisualStudioConfigurationManager()
		{
			if (dte == null)
				return;
			dte.Events.DTEEvents.OnStartupComplete -= OnDteEventsStartupComplete;
			var configurationManager = (VisualStudioConfigurationManager)viewModel.ConfigurationManager;
			await configurationManager.SetDte(dte);
		}
	}
}