using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;

namespace BetterConfigurationManager.ConfigurationManager
{
	public class VisualStudioConfigurationManager : ConfigurationManager
	{
		public override async Task Reload()
		{
			ClearData();
			ShowStatusText = true;
			SetAvailableConfigurations();
			SetActiveConfiguration();
			await SetProjects();
			FireSolutionContextChanged();
			ShowStatusText = false;
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
			StatusText = "Getting active solution configurations...";
			var solutionConfiguration =
				(SolutionConfiguration2)dte.Solution.SolutionBuild.ActiveConfiguration;
			ActiveSolutionPlatform = solutionConfiguration.PlatformName;
			ActiveSolutionConfiguration = solutionConfiguration.Name;
		}

		private async Task SetProjects()
		{
			var loadedProjects = new List<Project>();
			await Task.Run(() =>
			{
				SolutionConfigurations solutionConfigurations = GetSolutionConfigurations();
				var nativeProjects = new List<EnvDTE.Project>();
				NavigateSolution(nativeProjects);
				nativeProjects = nativeProjects.OrderBy(p => p.Name).ToList();
				foreach (var nativeProject in nativeProjects.Select((x, i) => new { Value = x, Index = i })
					)
				{
					StatusText = FormatProjectLoadingMessage(nativeProjects.Count, nativeProject.Index,
						nativeProject.Value.Name);
					loadedProjects.Add(new VisualStudioProject(nativeProject.Value, solutionConfigurations));
				}
			});
			foreach (Project loadedProject in loadedProjects)
				Projects.Add(loadedProject);
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

		private static string FormatProjectLoadingMessage(int availableProjectsCount,
			int currentProjectIndex, string currentProjectname)
		{
			string header = CenterString(string.Format("Loading Project {0}/{1}:", currentProjectIndex, 
				availableProjectsCount), currentProjectname.Length);
			return header + Environment.NewLine + CenterString(currentProjectname, header.Length);
		}

		private static string CenterString(string input, int width)
		{
			if (input.Length >= width)
				return input;
			int leftPadding = (width - input.Length) / 2;
			int rightPadding = width - input.Length - leftPadding;
			return new string(' ', leftPadding) + input + new string(' ', rightPadding);
		}

		public async Task SetDte(DTE2 dte)
		{
			this.dte = dte;
			solutionEvents = this.dte.Events.SolutionEvents;
			solutionEvents.Opened += async () => await Reload();
			solutionEvents.AfterClosing += OnSolutionClosing;
			if (this.dte.Solution.IsOpen)
				await Reload();
		}

		private SolutionEvents solutionEvents;

		private void OnSolutionClosing()
		{
			ClearData();
		}
	}
}