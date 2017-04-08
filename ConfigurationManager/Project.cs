using System.Collections.Generic;
using System.Linq;

namespace BetterConfigurationManager.ConfigurationManager
{
	public class Project : PropertyChangedBase
	{
		public Project()
		{
			AvailableConfigurations = new List<Configuration>();
		}

		public List<Configuration> AvailableConfigurations { get; }

		public string Name
		{
			get => name;
			set
			{
				name = value;
				OnPropertyChanged();
			}
		}

		private string name;

		public Configuration ActiveConfiguration
		{
			get => activeConfiguration;
			set
			{
				activeConfiguration = value;
				OnPropertyChanged();
			}
		}

		private Configuration activeConfiguration;

		public void SetSolutionContext(string solutionConfiguration, string solutionPlatform)
			=> ActiveConfiguration = AvailableConfigurations
			.FirstOrDefault(c => c.SolutionConfiguration == solutionConfiguration &&
				c.SolutionPlatform == solutionPlatform);
	}
}