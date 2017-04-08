using System.Threading.Tasks;
using BetterConfigurationManager.ConfigurationManager;

namespace BetterConfigurationManager.MainToolWindow
{
	public class DesignTimeMainToolWindowViewModel : MainToolWindowViewModel
	{
		public DesignTimeMainToolWindowViewModel()
		{
			ConfigurationManager = new DesignTimeConfigurationManager();
		}

		private class DesignTimeConfigurationManager : ConfigurationManager.ConfigurationManager
		{
			public DesignTimeConfigurationManager()
			{
				IsSolutionAvailable = true;
				AvailableSolutionConfigurations.Add("Debug");
				AvailableSolutionPlatforms.Add("Any CPU");
				ActiveSolutionConfiguration = "Debug";
				ActiveSolutionPlatform = "Any CPU";
				ShowStatusText = true;
				StatusText = "Loading...";

				var firstProject = new Project { Name = "Namespace.FirstProject" };
				firstProject.ActiveConfiguration = new Configuration
				{
					ProjectConfiguration = "Debug",
					AvailableProjectConfigurations = new[] { "Debug" },
					ProjectPlatform = "Any CPU",
					AvailableProjectPlatforms = new[] { "Any CPU" },
					IsDeployable = false,
					IsBuildable = true,
					ShouldBuild = true,
					ShouldDeploy = false
				};
				Projects.Add(firstProject);

				var secondProject = new Project { Name = "Namespace.SecondProject" };
				secondProject.ActiveConfiguration = new Configuration
				{
					ProjectConfiguration = "Debug",
					AvailableProjectConfigurations = new[] { "Debug" },
					ProjectPlatform = "x86",
					AvailableProjectPlatforms = new[] { "x86" },
					IsDeployable = true,
					IsBuildable = true,
					ShouldBuild = true,
					ShouldDeploy = true
				};
				Projects.Add(secondProject);
			}

			public override Task Reload() => null;
		}
	}
}