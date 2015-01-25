using EnvDTE;
using EnvDTE80;

namespace BetterConfigurationManager.ConfigurationManager
{
	public class VisualStudioProject : Project
	{
		public VisualStudioProject(EnvDTE.Project nativeProject,
			SolutionConfigurations solutionConfigurations)
		{
			this.nativeProject = nativeProject;

			Name = nativeProject.Name;
			SetAvailableConfigurations(solutionConfigurations);
		}

		private readonly EnvDTE.Project nativeProject;

		private void SetAvailableConfigurations(SolutionConfigurations solutionConfigurations)
		{
			foreach (SolutionConfiguration2 solutionConfiguration in solutionConfigurations)
			{
				string configuration = solutionConfiguration.Name;
				string platform = solutionConfiguration.PlatformName;
				foreach (SolutionContext solutionContext in solutionConfiguration.SolutionContexts)
					if (solutionContext.ProjectName == nativeProject.UniqueName)
						AvailableConfigurations.Add(new VisualStudioConfiguration(solutionContext,
							nativeProject.ConfigurationManager, configuration, platform));
			}
		}
	}
}