using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using BetterConfigurationManager.ConfigurationManager;

namespace BetterConfigurationManager.MainToolWindow
{
	public class MainToolWindowViewModel : PropertyChangedBase
	{
		public MainToolWindowViewModel()
		{
			ShouldBuildProjectChangedCommand = new RelayCommand(ShouldBuildProjectChanged);
			ShouldDeployProjectChangedCommand = new RelayCommand(ShouldDeployProjectChanged);
			ProjectConfigurationChangedCommand = new RelayCommand(ProjectConfigurationChanged);
			ProjectPlatformChangedCommand = new RelayCommand(ProjectPlatformChanged);
			SetShouldBuildProjectsCommand = new RelayCommand<PropertyMarkerAction>(SetShouldBuildProjects);
			SetShouldDeployProjectsCommand = new RelayCommand<PropertyMarkerAction>(SetShouldDeployProjects);
			SetProjectsConfigurationCommand = new RelayCommand<PropertyMarkerAction>(SetProjectsConfiguration);
			SetProjectsPlatformCommand = new RelayCommand<PropertyMarkerAction>(SetProjectsPlatform);
			ReloadCommand = new RelayCommand(async () => await configurationManager.Reload());
		}

		public ICommand ShouldBuildProjectChangedCommand { get; private set; }

		private void ShouldBuildProjectChanged()
		{
			OnPropertyChanged("BuildEveryProjectInCurrentSolutionContext");
		}

		public ICommand ShouldDeployProjectChangedCommand { get; private set; }

		private void ShouldDeployProjectChanged()
		{
			OnPropertyChanged("DeployEveryProjectInCurrentSolutionContext");
		}

		public ICommand ProjectConfigurationChangedCommand { get; private set; }

		private void ProjectConfigurationChanged()
		{
			OnPropertyChanged("ConfigurationForEveryProjectInCurrentSolutionContext");
		}

		public ICommand ProjectPlatformChangedCommand { get; private set; }

		private void ProjectPlatformChanged()
		{
			OnPropertyChanged("PlatformForEveryProjectInCurrentSolutionContext");
		}

		public ICommand SetShouldBuildProjectsCommand { get; private set; }

		public void SetShouldBuildProjects(PropertyMarkerAction action)
		{
			ApplyAction(action, (project, configuration) => configuration.IsBuildable,
				(project, configuration) =>	configuration.ShouldBuild = 
					project.ActiveConfiguration.ShouldBuild);
		}

		private void ApplyAction(PropertyMarkerAction action,
			Func<Project, Configuration, bool> canApplyAction, Action<Project, Configuration> applyAction)
		{
			var configurationFilter = GetConfigurationFilter(action);
			IEnumerable<Project> projects = GetProjectsToApplyActionFor(action);
			foreach (Project project in projects)
			{
				var configurations = project.AvailableConfigurations.Where(configurationFilter);
				ApplyAction(configurations, configuration => canApplyAction(project, configuration), 
					configuration => applyAction(project, configuration));
			}
		}

		private Func<Configuration, bool> GetConfigurationFilter(PropertyMarkerAction action)
		{
			if (action.ApplyForEverySolutionPlatformInCurrentSolutionConfiguration)
				return c => c.SolutionConfiguration == configurationManager.ActiveSolutionConfiguration;
			if (action.ApplyForSelectedSolutionPlatformInCurrentSolutionConfiguration != null)
				return c => c.SolutionConfiguration == configurationManager.ActiveSolutionConfiguration &&
						c.SolutionPlatform == action.ApplyForSelectedSolutionPlatformInCurrentSolutionConfiguration;
			if (action.ApplyForEverySolutionConfigurationInCurrentSolutionPlatform)
				return c => c.SolutionPlatform == configurationManager.ActiveSolutionPlatform;
			if (action.ApplyForSelectedSolutionConfigurationInCurrentSolutionPlatform != null)
				return c => c.SolutionConfiguration == action.ApplyForSelectedSolutionConfigurationInCurrentSolutionPlatform &&
					c.SolutionPlatform == configurationManager.ActiveSolutionPlatform;
			if (action.ApplyForEverySolutionConfigurationAndPlatform)
				return delegate { return true; };
			throw new ArgumentException("action");
		}

		private ConfigurationManager.ConfigurationManager configurationManager;

		private IEnumerable<Project> GetProjectsToApplyActionFor(PropertyMarkerAction action)
		{
			if (action.Project == null)
				return configurationManager.Projects;
			return new[] { action.Project };
		}

		private static void ApplyAction(IEnumerable<Configuration> configurations,
			Func<Configuration, bool> canApplyAction, Action<Configuration> applyAction)
		{
			foreach (Configuration configuration in configurations)
				if (canApplyAction(configuration))
					applyAction(configuration);
		}

		public ICommand SetShouldDeployProjectsCommand { get; private set; }

		public void SetShouldDeployProjects(PropertyMarkerAction action)
		{
			ApplyAction(action, (project, configuration) => configuration.IsDeployable,
				(project, configuration) => configuration.ShouldDeploy = 
					project.ActiveConfiguration.ShouldDeploy);
		}

		public ICommand SetProjectsConfigurationCommand { get; private set; }

		public void SetProjectsConfiguration(PropertyMarkerAction action)
		{
			ApplyAction(action, (project, configuration) => 
				configuration.AvailableProjectConfigurations.
				Contains(project.ActiveConfiguration.ProjectConfiguration),
				(project, configuration) => configuration.ProjectConfiguration = 
					project.ActiveConfiguration.ProjectConfiguration);
		}

		public ICommand SetProjectsPlatformCommand { get; private set; }

		public void SetProjectsPlatform(PropertyMarkerAction action)
		{
			ApplyAction(action, (project, configuration) =>
				configuration.AvailableProjectPlatforms.
				Contains(project.ActiveConfiguration.ProjectPlatform),
				(project, configuration) => configuration.ProjectPlatform =
					project.ActiveConfiguration.ProjectPlatform);
		}

		public ICommand ReloadCommand { get; private set; }

		public ConfigurationManager.ConfigurationManager ConfigurationManager
		{
			get { return configurationManager; }
			set
			{
				configurationManager = value;
				configurationManager.SolutionContextChanged += OnConfigurationManagerSolutionContextChanged;
			}
		}

		private void OnConfigurationManagerSolutionContextChanged()
		{
			OnPropertyChanged("AvailableProjectConfigurationsInCurrentSolutionContext");
			OnPropertyChanged("AvailableProjectPlatformsInCurrentSolutionContext");
			OnPropertyChanged("BuildEveryProjectInCurrentSolutionContext");
			OnPropertyChanged("DeployEveryProjectInCurrentSolutionContext");
			OnPropertyChanged("ConfigurationForEveryProjectInCurrentSolutionContext");
			OnPropertyChanged("PlatformForEveryProjectInCurrentSolutionContext");
		}

		public bool? BuildEveryProjectInCurrentSolutionContext
		{
			get
			{
				var shouldBuildFlags = ConfigurationManager.Projects.
					Select(project => project.ActiveConfiguration).
					Where(configuration => configuration.IsBuildable).
					Select(configuration => configuration.ShouldBuild).ToList();
				return EvaluateShouldBuildOrShouldDeployFlags(shouldBuildFlags);
			}
			set
			{
				ApplyAction(GetActiveConfigurationForEveryProject(), c => c.IsBuildable,
					c => c.ShouldBuild = value.Value);
			}
		}

		private static bool? EvaluateShouldBuildOrShouldDeployFlags(List<bool> flags)
		{
			if (flags.Count == 0)
				return null;
			if (flags.All(flag => flag))
				return true;
			if (flags.All(flag => !flag))
				return false;
			return null;
		}

		private IEnumerable<Configuration> GetActiveConfigurationForEveryProject()
		{
			return ConfigurationManager.Projects.Select(p => p.ActiveConfiguration);
		}

		public bool? DeployEveryProjectInCurrentSolutionContext
		{
			get
			{
				var shouldDeployFlags =	ConfigurationManager.Projects.
					Select(project => project.ActiveConfiguration).
					Where(configuration => configuration.IsDeployable).
					Select(configuration => configuration.ShouldDeploy).ToList();
				return EvaluateShouldBuildOrShouldDeployFlags(shouldDeployFlags);
			}
			set
			{
				ApplyAction(GetActiveConfigurationForEveryProject(), c => c.IsDeployable,
					c => c.ShouldDeploy = value.Value);
			}
		}

		public IEnumerable<string> AvailableProjectConfigurationsInCurrentSolutionContext
		{
			get
			{
				return GetActiveConfigurationForEveryProject().
					SelectMany(c => c.AvailableProjectConfigurations).Distinct();
			}
		}

		public IEnumerable<string> AvailableProjectPlatformsInCurrentSolutionContext
		{
			get
			{
				return GetActiveConfigurationForEveryProject().SelectMany(c => c.AvailableProjectPlatforms).
					Distinct();
			}
		}

		public string ConfigurationForEveryProjectInCurrentSolutionContext
		{
			get
			{
				var configurations = configurationManager.Projects.
					Select(c => c.ActiveConfiguration.ProjectConfiguration).ToList();
				return AllItemsAreEqual(configurations) ? configurations[0] : null;
			}
			set
			{
				ApplyAction(GetActiveConfigurationForEveryProject(),
					c => c.AvailableProjectConfigurations.Contains(value), c => c.ProjectConfiguration = value);
			}
		}

		private static bool AllItemsAreEqual(List<string> items)
		{
			if (items.Count == 0)
				return false;
			return items.All(item => item == items[0]);
		}

		public string PlatformForEveryProjectInCurrentSolutionContext
		{
			get
			{
				var configurations =
					configurationManager.Projects.Select(c => c.ActiveConfiguration.ProjectPlatform).ToList();
				return AllItemsAreEqual(configurations) ? configurations[0] : null;
			}
			set
			{
				ApplyAction(GetActiveConfigurationForEveryProject(),
					c => c.AvailableProjectPlatforms.Contains(value), c => c.ProjectPlatform = value);
			}
		}
	}
}