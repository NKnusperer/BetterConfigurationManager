using BetterConfigurationManager.ConfigurationManager;
using NUnit.Framework;

namespace BetterConfigurationManager.Tests
{
	public class ProjectTests
	{
		[Test]
		public void TestSetSolutionContext()
		{
			var project = new Project();
			var configuration = new Configuration
			{
				SolutionConfiguration = "Debug",
				SolutionPlatform = "Any CPU"
			};
			project.AvailableConfigurations.Add(configuration);
			Assert.That(project.ActiveConfiguration, Is.Null);
			project.SetSolutionContext("Debug", "Any CPU");
			Assert.That(project.ActiveConfiguration, Is.EqualTo(configuration));
		}
	}
}