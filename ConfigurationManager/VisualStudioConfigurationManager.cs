using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace BetterConfigurationManager.ConfigurationManager
{
	public class VisualStudioConfigurationManager : ConfigurationManager
	{
		public override void Reload()
		{
			ClearData();
			LoadConfigurations();
		}

		private void LoadConfigurations()
		{
			SetAvailableConfigurations();
			SetActiveConfiguration();
			SetProjects();
			FireSolutionContextChanged();
			IsSolutionAvailable = true;
		}

		private void SetAvailableConfigurations()
		{
			foreach (SolutionConfiguration2 solutionConfiguration in GetSolutionConfigurations())
			{
				string solutionConfigurationName = solutionConfiguration.Name;
				string solutionPlatformName = solutionConfiguration.PlatformName;
				if (!AvailableSolutionConfigurations.Contains(solutionConfigurationName))
					AvailableSolutionConfigurations.Add(solutionConfigurationName);
				if (!AvailableSolutionPlatforms.Contains(solutionPlatformName))
					AvailableSolutionPlatforms.Add(solutionPlatformName);
			}
		}

		private SolutionConfigurations GetSolutionConfigurations()
		{
			var solution = (Solution2)dte.Solution;
			var solutionBuild = (SolutionBuild2)solution.SolutionBuild;
			return solutionBuild.SolutionConfigurations;
		}

		private DTE2 dte;

		private void SetActiveConfiguration()
		{
			var solutionConfiguration =
				(SolutionConfiguration2)dte.Solution.SolutionBuild.ActiveConfiguration;
			ActiveSolutionPlatform = solutionConfiguration.PlatformName;
			ActiveSolutionConfiguration = solutionConfiguration.Name;
		}

		private void SetProjects()
		{
			SolutionConfigurations solutionConfigurations = GetSolutionConfigurations();
			var nativeProjects = new List<EnvDTE.Project>();
			NavigateSolution(nativeProjects);
			foreach (EnvDTE.Project nativeProject in nativeProjects.OrderBy(p => p.Name))
				Projects.Add(new VisualStudioProject(nativeProject, solutionConfigurations));
			UpdateProjectsSolutionContext();
		}

		private void NavigateSolution(List<EnvDTE.Project> nativeProjects)
		{
			foreach (EnvDTE.Project nativeProject in dte.Solution.Projects)
				NavigateProject(nativeProject, nativeProjects);
		}

		private void NavigateProject(EnvDTE.Project nativeProject,
			List<EnvDTE.Project> nativeProjects)
		{
			if (nativeProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
				NavigateProjectItems(nativeProject.ProjectItems, nativeProjects);
			else if (nativeProject.Object != null)
				nativeProjects.Add(nativeProject);
		}

		private void NavigateProjectItems(ProjectItems nativeProjectItems,
			List<EnvDTE.Project> nativeProjects)
		{
			if (nativeProjectItems == null)
				return;
			foreach (ProjectItem nativeProjectItem in nativeProjectItems)
				if (nativeProjectItem.SubProject != null)
					NavigateProject(nativeProjectItem.SubProject, nativeProjects);
		}

		public void SetDte(DTE2 dte)
		{
			this.dte = dte;
			solutionEvents = this.dte.Events.SolutionEvents;
			solutionEvents.Opened += LoadConfigurations;
			solutionEvents.AfterClosing += OnSolutionClosing;
			if (this.dte.Solution.IsOpen)
				LoadConfigurations();
		}

		private SolutionEvents solutionEvents;

		private void OnSolutionClosing()
		{
			ClearData();
		}
	}
}