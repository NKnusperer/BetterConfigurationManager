using System;
using System.Collections.Generic;
using System.Linq;
using BetterConfigurationManager.ConfigurationManager;
using BetterConfigurationManager.MainToolWindow;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BetterConfigurationManager.Tests
{
	public class MainToolWindowViewModelTests
	{
		[Test]
		public void AvailableProjectConfigurationsReturnsDistinctList()
		{
			var viewModel = GetViewModel();
			Assert.That(viewModel.AvailableProjectConfigurationsInCurrentSolutionContext, Is.EqualTo(new[] { "Debug", "Release" }));
		}

		private static MainToolWindowViewModel GetViewModel()
		{
			return new MainToolWindowViewModel
			{
				ConfigurationManager = new MockConfigurationManager()
			};
		}

		private class MockConfigurationManager : ConfigurationManager.ConfigurationManager
		{
			public MockConfigurationManager()
			{
				var projectNames = new[] { "Namespace.FirstProject", "Namespace.SecondProject" };
				var platforms = new[] { "Any CPU", "x86" };
				var configurations = new[] { "Debug", "Release" };
				foreach (string name in projectNames)
				{
					var project = new Project { Name = name };
					foreach (string platform in platforms)
					{
						foreach (string configuration in configurations)
						{
							project.AvailableConfigurations.Add(new Configuration
							{
								SolutionConfiguration = configuration,
								SolutionPlatform = platform,
								ProjectConfiguration = configuration,
								AvailableProjectConfigurations = configurations,
								ProjectPlatform = platform,
								AvailableProjectPlatforms = platforms
							});
						}
					}
					Projects.Add(project);
				}
				ActiveSolutionConfiguration = "Debug";
				ActiveSolutionPlatform = "Any CPU";
			}
		}

		[Test]
		public void AvailableProjectPlatformsReturnsDistinctList()
		{
			var viewModel = GetViewModel();
			Assert.That(viewModel.AvailableProjectPlatformsInCurrentSolutionContext, 
				Is.EqualTo(new[] { "Any CPU", "x86" }));
		}

		[Test]
		public void BuildEveryProjectOnlyAppliesToBuildableConfigurations()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[0].ActiveConfiguration.IsBuildable = true;
			viewModel.BuildEveryProjectInCurrentSolutionContext = true;
			Assert.That(projects[0].ActiveConfiguration.IsBuildable, Is.True);
			Assert.That(projects[1].ActiveConfiguration.IsBuildable, Is.False);
		}

		[Test]
		public void BuildEveryProjectIsTrueIfEveryBuildableProjectIsSelected()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[0].ActiveConfiguration.IsBuildable = true;
			projects[0].ActiveConfiguration.ShouldBuild = true;
			Assert.That(viewModel.BuildEveryProjectInCurrentSolutionContext.Value, Is.True);
		}

		[Test]
		public void BuildEveryProjectIsFalseIfNoBuildableProjectIsAvailable()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[0].ActiveConfiguration.IsBuildable = true;
			Assert.That(viewModel.BuildEveryProjectInCurrentSolutionContext.Value, Is.False);
		}

		[Test]
		public void BuildEveryProjectIsNullIfNotAllBuildableProjectsHaveTheSameFlag()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[0].ActiveConfiguration.IsBuildable = true;
			projects[1].ActiveConfiguration.IsBuildable = true;
			projects[1].ActiveConfiguration.ShouldBuild = true;
			Assert.That(viewModel.BuildEveryProjectInCurrentSolutionContext.HasValue, Is.False);
		}

		[Test]
		public void BuildEveryProjectIsNullIfNoProjectExists()
		{
			var viewModel = GetViewModel();
			viewModel.ConfigurationManager.ClearData();
			Assert.That(viewModel.BuildEveryProjectInCurrentSolutionContext.HasValue, Is.False);
		}

		[Test]
		public void DeployEveryProjectOnlyAppliesToDeployableConfigurations()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[0].ActiveConfiguration.IsDeployable = true;
			viewModel.DeployEveryProjectInCurrentSolutionContext = true;
			Assert.That(projects[0].ActiveConfiguration.IsDeployable, Is.True);
			Assert.That(projects[1].ActiveConfiguration.IsDeployable, Is.False);
		}

		[Test]
		public void DeployEveryProjectIsTrueIfEveryDeployableProjectIsSelected()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[0].ActiveConfiguration.IsDeployable = true;
			projects[0].ActiveConfiguration.ShouldDeploy = true;
			Assert.That(viewModel.DeployEveryProjectInCurrentSolutionContext.Value, Is.True);
		}

		[Test]
		public void DeployEveryProjectIsFalseIfNoDeployableProjectIsAvailable()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[0].ActiveConfiguration.IsDeployable = true;
			Assert.That(viewModel.DeployEveryProjectInCurrentSolutionContext.Value, Is.False);
		}

		[Test]
		public void DeployEveryProjectIsNullIfNotAllDeployableProjectsHaveTheSameFlag()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[0].ActiveConfiguration.IsDeployable = true;
			projects[1].ActiveConfiguration.IsDeployable = true;
			projects[1].ActiveConfiguration.ShouldDeploy = true;
			Assert.That(viewModel.DeployEveryProjectInCurrentSolutionContext.HasValue, Is.False);
		}

		[Test]
		public void DeployEveryProjectIsNullIfNoProjectExists()
		{
			var viewModel = GetViewModel();
			viewModel.ConfigurationManager.ClearData();
			Assert.That(viewModel.DeployEveryProjectInCurrentSolutionContext.HasValue, Is.False);
		}

		[Test]
		public void ConfigurationForEveryProjectInitiallyHasValueIfEveryProjectHasSameConfiguration()
		{
			var viewModel = GetViewModel();
			Assert.That(viewModel.ConfigurationForEveryProjectInCurrentSolutionContext,
				Is.EqualTo("Debug"));
		}

		[Test]
		public void ConfigurationForEveryProjectInitiallyIsNullIfNotEveryProjectHasSameConfiguration()
		{
			var viewModel = GetViewModel();
			viewModel.ConfigurationManager.Projects[0].ActiveConfiguration.ProjectConfiguration =
				"Release";
			Assert.That(viewModel.ConfigurationForEveryProjectInCurrentSolutionContext, Is.Null);
		}

		[Test]
		public void ConfigurationForEveryProjectOnlyAppliesToConfigurationsWhichSupportIt()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[1].ActiveConfiguration.AvailableProjectConfigurations = new[] { "CustomConfig" };
			viewModel.ConfigurationForEveryProjectInCurrentSolutionContext = "Release";
			Assert.That(projects[0].ActiveConfiguration.ProjectConfiguration, Is.EqualTo("Release"));
			Assert.That(projects[1].ActiveConfiguration.ProjectConfiguration, Is.Not.EqualTo("Release"));
		}

		[Test]
		public void PlatformForEveryProjectInitiallyHasValueIfEveryProjectHasSamePlatform()
		{
			var viewModel = GetViewModel();
			Assert.That(viewModel.PlatformForEveryProjectInCurrentSolutionContext, Is.EqualTo("Any CPU"));
		}

		[Test]
		public void PlatformForEveryProjectInitiallyIsNullIfNotEveryProjectHasSameConfiguration()
		{
			var viewModel = GetViewModel();
			viewModel.ConfigurationManager.Projects[0].ActiveConfiguration.ProjectPlatform =
				"x86";
			Assert.That(viewModel.PlatformForEveryProjectInCurrentSolutionContext, Is.Null);
		}

		[Test]
		public void PlatformForEveryProjectOnlyAppliesToConfigurationsWhichSupportIt()
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			projects[1].ActiveConfiguration.AvailableProjectPlatforms = new[] { "CustomPlatform" };
			viewModel.PlatformForEveryProjectInCurrentSolutionContext = "x86";
			Assert.That(projects[0].ActiveConfiguration.ProjectPlatform, Is.EqualTo("x86"));
			Assert.That(projects[1].ActiveConfiguration.ProjectPlatform, Is.Not.EqualTo("x86"));
		}

		[Test]
		public void TestSetShouldBuildProjects()
		{
			TestSetActionForAllPossibleConfigurations(
				(viewModel, action) => viewModel.SetShouldBuildProjects(action),
				project => project.ActiveConfiguration.ShouldBuild = true,
				configuration => configuration.IsBuildable = true,
				configuration => configuration.ShouldBuild, Is.True);
		}

		private static void TestSetActionForAllPossibleConfigurations(
			Action<MainToolWindowViewModel, PropertyMarkerAction> setAction,
			Action<Project> initialProjectState, Action<Configuration> initialConfigurationsState,
			Func<Configuration, object> configurationPropertyToTest,
			IResolveConstraint expectedConfigurationPropertyValueExpression)
		{
			AssertApplyForEverySolutionPlatformInCurrentSolutionConfiguration(setAction,
				initialProjectState, initialConfigurationsState, configurationPropertyToTest,
				expectedConfigurationPropertyValueExpression);
			AssertApplyForSelectedSolutionPlatformInCurrentSolutionConfiguration(setAction,
				initialProjectState, initialConfigurationsState, configurationPropertyToTest,
				expectedConfigurationPropertyValueExpression);
			AssertApplyForEverySolutionConfigurationInCurrentSolutionPlatform(setAction,
				initialProjectState, initialConfigurationsState, configurationPropertyToTest,
				expectedConfigurationPropertyValueExpression);
			AssertApplyForSelectedSolutionConfigurationInCurrentSolutionPlatform(setAction,
				initialProjectState, initialConfigurationsState, configurationPropertyToTest,
				expectedConfigurationPropertyValueExpression);
			AssertApplyForEverySolutionConfigurationAndPlatform(setAction, initialProjectState,
				initialConfigurationsState, configurationPropertyToTest,
				expectedConfigurationPropertyValueExpression);
		}

		private static void AssertApplyForEverySolutionPlatformInCurrentSolutionConfiguration(
			Action<MainToolWindowViewModel, PropertyMarkerAction> setAction, 
			Action<Project> initialProjectState, Action<Configuration> initialConfigurationsState,
			Func<Configuration, object> configurationPropertyToTest,
			IResolveConstraint expectedConfigurationPropertyValueExpression)
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			var project = projects[0];
			initialProjectState(project);
			var configurations = GetConfigurationsForSolutionConfiguration(project, "Debug");
			foreach (Configuration configuration in configurations)
				initialConfigurationsState(configuration);
			setAction(viewModel, new PropertyMarkerAction
			{
				Project = project,
				ApplyForEverySolutionPlatformInCurrentSolutionConfiguration = true
			});
			foreach (Configuration configuration in configurations)
				Assert.That(configurationPropertyToTest(configuration), expectedConfigurationPropertyValueExpression);
		}

		private static IEnumerable<Configuration> GetConfigurationsForSolutionConfiguration(
			Project project, string configuration)
		{
			return project.AvailableConfigurations.Where(c => c.SolutionConfiguration == configuration);
		}


		private static void AssertApplyForSelectedSolutionPlatformInCurrentSolutionConfiguration(
			Action<MainToolWindowViewModel, PropertyMarkerAction> setAction,
			Action<Project> initialProjectState, Action<Configuration> initialConfigurationsState,
			Func<Configuration, object> configurationPropertyToTest,
			IResolveConstraint expectedConfigurationPropertyValueExpression)
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			var project = projects[0];
			initialProjectState(project);
			var configuration = GetConfiguration(project, "Debug", "x86");
			initialConfigurationsState(configuration);
			setAction(viewModel, new PropertyMarkerAction
			{
				Project = project,
				ApplyForSelectedSolutionPlatformInCurrentSolutionConfiguration = "x86"
			});
			Assert.That(configurationPropertyToTest(configuration), 
				expectedConfigurationPropertyValueExpression);
		}

		private static Configuration GetConfiguration(Project project, string solutionConfiguration,
			string solutionPlatform)
		{
			return project.AvailableConfigurations.First(p =>
						p.SolutionConfiguration == solutionConfiguration &&
						p.SolutionPlatform == solutionPlatform);
		}

		private static void AssertApplyForEverySolutionConfigurationInCurrentSolutionPlatform(
			Action<MainToolWindowViewModel, PropertyMarkerAction> setAction,
			Action<Project> initialProjectState, Action<Configuration> initialConfigurationsState,
			Func<Configuration, object> configurationPropertyToTest,
			IResolveConstraint expectedConfigurationPropertyValueExpression)
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			var project = projects[0];
			initialProjectState(project);
			var configurations = GetConfigurationsForSolutionPlatform(project, "Any CPU");
			foreach (Configuration configuration in configurations)
				initialConfigurationsState(configuration);
			setAction(viewModel, new PropertyMarkerAction
			{
				Project = project,
				ApplyForEverySolutionConfigurationInCurrentSolutionPlatform = true
			});
			foreach (Configuration configuration in configurations)
				Assert.That(configurationPropertyToTest(configuration),
					expectedConfigurationPropertyValueExpression);
		}

		private static IEnumerable<Configuration> GetConfigurationsForSolutionPlatform(
			Project project, string platform)
		{
			return project.AvailableConfigurations.Where(c => c.SolutionPlatform == platform);
		}

		private static void AssertApplyForSelectedSolutionConfigurationInCurrentSolutionPlatform(
			Action<MainToolWindowViewModel, PropertyMarkerAction> setAction,
			Action<Project> initialProjectState, Action<Configuration> initialConfigurationsState,
			Func<Configuration, object> configurationPropertyToTest,
			IResolveConstraint expectedConfigurationPropertyValueExpression)
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			var project = projects[0];
			initialProjectState(project);
			var configuration = GetConfiguration(project, "Release", "Any CPU");
			initialConfigurationsState(configuration);
			setAction(viewModel, new PropertyMarkerAction
			{
				Project = project,
				ApplyForSelectedSolutionConfigurationInCurrentSolutionPlatform = "Release"
			});
			Assert.That(configurationPropertyToTest(configuration), 
				expectedConfigurationPropertyValueExpression);
		}

		private static void AssertApplyForEverySolutionConfigurationAndPlatform(
			Action<MainToolWindowViewModel, PropertyMarkerAction> setAction,
			Action<Project> initialProjectState, Action<Configuration> initialConfigurationsState,
			Func<Configuration, object> configurationPropertyToTest,
			IResolveConstraint expectedConfigurationPropertyValueExpression)
		{
			var viewModel = GetViewModel();
			var projects = viewModel.ConfigurationManager.Projects;
			var project = projects[0];
			initialProjectState(project);
			foreach (Configuration configuration in project.AvailableConfigurations)
				initialConfigurationsState(configuration);
			setAction(viewModel, new PropertyMarkerAction
			{
				Project = project,
				ApplyForEverySolutionConfigurationAndPlatform = true
			});
			foreach (Configuration configuration in project.AvailableConfigurations)
				Assert.That(configurationPropertyToTest(configuration),
					expectedConfigurationPropertyValueExpression);
		}

		[Test]
		public void TestSetShouldDeployProjects()
		{
			TestSetActionForAllPossibleConfigurations(
				(viewModel, action) => viewModel.SetShouldDeployProjects(action),
				project => project.ActiveConfiguration.ShouldDeploy = true,
				configuration => configuration.ShouldDeploy = true,
				configuration => configuration.ShouldDeploy, Is.True);
		}

		[Test]
		public void TestSetProjectsConfiguration()
		{
			TestSetActionForAllPossibleConfigurations(
				(viewModel, action) => viewModel.SetProjectsConfiguration(action),
				project => project.ActiveConfiguration.ProjectConfiguration = "Release",
				configuration => configuration.ProjectConfiguration = "Release",
				configuration => configuration.ProjectConfiguration, Is.EqualTo("Release"));
		}

		[Test]
		public void TestSetProjectsPlatform()
		{
			TestSetActionForAllPossibleConfigurations(
				(viewModel, action) => viewModel.SetProjectsPlatform(action),
				project => project.ActiveConfiguration.ProjectPlatform = "x86",
				configuration => configuration.ProjectPlatform = "x86",
				configuration => configuration.ProjectPlatform, Is.EqualTo("x86"));
		}
	}
}