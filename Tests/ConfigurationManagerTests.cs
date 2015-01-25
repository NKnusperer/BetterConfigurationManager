using BetterConfigurationManager.ConfigurationManager;
using NUnit.Framework;

namespace BetterConfigurationManager.Tests
{
	public class ConfigurationManagerTests
	{
		[Test]
		public void ProjectSolutionContextIsSetWhenSettingActiveConfiguration()
		{
			var configurationManager = new MockConfigurationManager();
			Assert.That(configurationManager.Projects[0].ActiveConfiguration, Is.Null);
			configurationManager.ActiveSolutionConfiguration = "Debug";
			configurationManager.ActiveSolutionPlatform = "Any CPU";
			var activeConfiguration = configurationManager.Projects[0].ActiveConfiguration;
			Assert.That(activeConfiguration, Is.Not.Null);
			Assert.That(activeConfiguration.SolutionConfiguration, Is.EqualTo("Debug"));
			Assert.That(activeConfiguration.SolutionPlatform, Is.EqualTo("Any CPU"));
		}

		private class MockConfigurationManager : ConfigurationManager.ConfigurationManager
		{
			public MockConfigurationManager()
			{
				var firstProject = new Project { Name = "Namespace.FirstProject" };
				firstProject.AvailableConfigurations.Add(new Configuration
				{
					SolutionConfiguration = "Debug",
					SolutionPlatform = "Any CPU",
					ProjectConfiguration = "Debug",
					AvailableProjectConfigurations = new[] { "Debug" },
					ProjectPlatform = "Any CPU",
					AvailableProjectPlatforms = new[] { "Any CPU" }
				});
				Projects.Add(firstProject);
			}
		}

		[Test]
		public void TestClearData()
		{
			var configurationManager = new MockConfigurationManager();
			configurationManager.ClearData();
			Assert.That(configurationManager.ActiveSolutionPlatform, Is.Null);
			Assert.That(configurationManager.ActiveSolutionConfiguration, Is.Null);
			Assert.That(configurationManager.AvailableSolutionConfigurations, Is.Empty);
			Assert.That(configurationManager.AvailableSolutionPlatforms, Is.Empty);
			Assert.That(configurationManager.Projects, Is.Empty);
		}

		[Test]
		public void ValidSolutionContextSetIsOnlyTrueIfProjectsExistAndConfigurationAndPlatformWasSet()
		{
			var configurationManager = new MockConfigurationManager();
			configurationManager.ClearData();
			Assert.That(configurationManager.ValidSolutionContextSet, Is.False);
			configurationManager.Projects.Add(new Project());
			Assert.That(configurationManager.ValidSolutionContextSet, Is.False);
			configurationManager.ActiveSolutionConfiguration = "Debug";
			Assert.That(configurationManager.ValidSolutionContextSet, Is.False);
			configurationManager.ActiveSolutionPlatform = "Any CPU";
			Assert.That(configurationManager.ValidSolutionContextSet, Is.True);
		}
	}
}