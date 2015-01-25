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

		public List<Configuration> AvailableConfigurations { get; private set; }

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				OnPropertyChanged();
			}
		}

		private string name;

		public Configuration ActiveConfiguration
		{
			get { return activeConfiguration; }
			set
			{
				activeConfiguration = value;
				OnPropertyChanged();
			}
		}

		private Configuration activeConfiguration;

		public void SetSolutionContext(string solutionConfiguration, string solutionPlatform)
		{
			ActiveConfiguration = AvailableConfigurations.FirstOrDefault(c =>
				c.SolutionConfiguration == solutionConfiguration &&
				c.SolutionPlatform == solutionPlatform);
		}
	}
}