using System.Collections.Generic;

namespace BetterConfigurationManager.ConfigurationManager
{
	public class Configuration : PropertyChangedBase
	{
		public virtual string ProjectConfiguration
		{
			get { return projectConfiguration; }
			set
			{
				projectConfiguration = value;
				OnPropertyChanged();
			}
		}

		private string projectConfiguration;

		public virtual string ProjectPlatform
		{
			get { return projectPlatform; }
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
			get { return isDeployable; }
			set
			{
				isDeployable = value;
				OnPropertyChanged();
			}
		}

		private bool isDeployable;

		public virtual bool ShouldDeploy
		{
			get { return shouldDeploy; }
			set
			{
				shouldDeploy = value;
				OnPropertyChanged();
			}
		}

		private bool shouldDeploy;

		public bool IsBuildable
		{
			get { return isBuildable; }
			set
			{
				isBuildable = value;
				OnPropertyChanged();
			}
		}

		private bool isBuildable;

		public virtual bool ShouldBuild
		{
			get { return shouldBuild; }
			set
			{
				shouldBuild = value;
				OnPropertyChanged();
			}
		}

		private bool shouldBuild;
	}
}