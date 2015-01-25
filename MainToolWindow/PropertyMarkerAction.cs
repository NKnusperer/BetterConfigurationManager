using BetterConfigurationManager.ConfigurationManager;

namespace BetterConfigurationManager.MainToolWindow
{
	public class PropertyMarkerAction
	{
		public bool ApplyForEverySolutionPlatformInCurrentSolutionConfiguration { get; set; }
		public string ApplyForSelectedSolutionPlatformInCurrentSolutionConfiguration { get; set; }
		public bool ApplyForEverySolutionConfigurationInCurrentSolutionPlatform { get; set; }
		public string ApplyForSelectedSolutionConfigurationInCurrentSolutionPlatform { get; set; }
		public bool ApplyForEverySolutionConfigurationAndPlatform { get; set; }
		public Project Project { get; set; }
	}
}