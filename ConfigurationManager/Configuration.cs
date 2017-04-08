using System.Collections.Generic;

namespace BetterConfigurationManager.ConfigurationManager
{
	public class Configuration : PropertyChangedBase
	{
		public virtual string ProjectConfiguration
		{
			get => projectConfiguration;
			set
			{
				projectConfiguration = value;
				OnPropertyChanged();
			}
		}

		private string projectConfiguration;

		public virtual string ProjectPlatform
		{
			get => projectPlatform;
			set
			{
				projectPlatform = value;
				OnPropertyChanged();
			}
		}

		private string projectPlatform;

		public IEnumerable<string> AvailableProjectConfigurations { get; set; }
		public IEnumerable<string> AvailableProjectPlatforms { get; set; }

		public string SolutionConfiguration { get; set; }
		public string SolutionPlatform { get; set; }

		public bool IsDeployable
		{
			get => isDeployable;
			set
			{
				isDeployable = value;
				OnPropertyChanged();
			}
		}

		private bool isDeployable;

		public virtual bool ShouldDeploy
		{
			get => shouldDeploy;
			set
			{
				shouldDeploy = value;
				OnPropertyChanged();
			}
		}

		private bool shouldDeploy;

		public bool IsBuildable
		{
			get => isBuildable;
			set
			{
				isBuildable = value;
				OnPropertyChanged();
			}
		}

		private bool isBuildable;

		public virtual bool ShouldBuild
		{
			get => shouldBuild;
			set
			{
				shouldBuild = value;
				OnPropertyChanged();
			}
		}

		private bool shouldBuild;
	}
}