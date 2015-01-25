using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace BetterConfigurationManager.ConfigurationManager
{
	public class VisualStudioConfiguration : Configuration
	{
		public VisualStudioConfiguration(SolutionContext solutionContext,
			EnvDTE.ConfigurationManager nativeConfigurationManager, string solutionConfiguration,
			string solutionPlatform)
		{
			this.solutionContext = solutionContext;
			this.nativeConfigurationManager = nativeConfigurationManager;
			SolutionConfiguration = solutionConfiguration;
			SolutionPlatform = solutionPlatform;
			SetAvailableConfigurationsAndPlatforms();
			SetIsBuildableAndIsDeployable();
		}

		private readonly SolutionContext solutionContext;
		private readonly EnvDTE.ConfigurationManager nativeConfigurationManager;

		private void SetAvailableConfigurationsAndPlatforms()
		{
			AvailableProjectConfigurations =
				ObjectToStringArray(nativeConfigurationManager.ConfigurationRowNames);
			AvailableProjectPlatforms = ObjectToStringArray(nativeConfigurationManager.PlatformNames);
		}

		private static IEnumerable<string> ObjectToStringArray(object obj)
		{
			return ((IEnumerable)obj).Cast<object>().Select(x => x.ToString());
		}

		private void SetIsBuildableAndIsDeployable()
		{
			Configurations configs = nativeConfigurationManager.Platform(ProjectPlatform);
			var nativeConfiguration = configs.Cast<EnvDTE.Configuration>()
				.FirstOrDefault(c => c.ConfigurationName == ProjectConfiguration);
			IsBuildable = nativeConfiguration.IsBuildable;
			IsDeployable = nativeConfiguration.IsDeployable;
		}

		public override string ProjectPlatform
		{
			get
			{
				if (base.ProjectPlatform == null)
					base.ProjectPlatform = solutionContext.PlatformName;
				return base.ProjectPlatform;
			}
			set
			{
				if (base.ProjectPlatform == value)
					return;
				solutionContext.ConfigurationName = ProjectConfiguration + "|" + value;
				base.ProjectPlatform = value;
				SetIsBuildableAndIsDeployable();
			}
		}
		public override string ProjectConfiguration
		{
			get
			{
				if (base.ProjectConfiguration == null)
					base.ProjectConfiguration = solutionContext.ConfigurationName;
				return base.ProjectConfiguration;
			}
			set
			{
				if (base.ProjectConfiguration == value)
					return;
				solutionContext.ConfigurationName = value;
				base.ProjectConfiguration = value;
				SetIsBuildableAndIsDeployable();
			}
		}

		public override bool ShouldDeploy
		{
			get { return solutionContext.ShouldDeploy; }
			set
			{
				solutionContext.ShouldDeploy = value;
				base.ShouldDeploy = value;
			}
		}
		public override bool ShouldBuild
		{
			get { return solutionContext.ShouldBuild; }
			set
			{
				solutionContext.ShouldBuild = value;
				base.ShouldBuild = value;
			}
		}
	}
}