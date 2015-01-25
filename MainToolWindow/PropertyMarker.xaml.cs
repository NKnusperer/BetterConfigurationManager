using System.Windows;
using System.Windows.Input;
using BetterConfigurationManager.ConfigurationManager;

namespace BetterConfigurationManager.MainToolWindow
{
	public partial class PropertyMarker
	{
		public PropertyMarker()
		{
			InitializeComponent();
			// http://stackoverflow.com/q/11333028/483931
			NameScope.SetNameScope(PropertyMarkerContextMenu, NameScope.GetNameScope(this));
			SetUpCommands();
		}

		private void SetUpCommands()
		{
			ApplyForEverySolutionPlatformCommand = new RelayCommand(ApplyForEverySolutionPlatform);
			ApplyForSpecificSolutionPlatformCommand =
				new RelayCommand<string>(ApplyForSpecificSolutionPlatform);
			ApplyForEverySolutionConfigurationCommand =
				new RelayCommand(ApplyForEverySolutionConfiguration);
			ApplyForSpecificSolutionConfigurationCommand =
				new RelayCommand<string>(ApplyForSpecificSolutionConfiguration);
			ApplyForEverySolutionConfigurationAndPlatformCommand =
				new RelayCommand(ApplyForEverySolutionConfigurationAndPlatform);
		}

		public ICommand ApplyForEverySolutionPlatformCommand { get; private set; }

		private void ApplyForEverySolutionPlatform()
		{
			CallCommand(new PropertyMarkerAction { ApplyForEverySolutionPlatformInCurrentSolutionConfiguration = true });
		}

		private void CallCommand(PropertyMarkerAction action)
		{
			action.Project = Project;
			if (Command != null)
				Command.Execute(action);
		}

		public Project Project
		{
			get { return (Project)GetValue(ProjectProperty); }
			set { SetValue(ProjectProperty, value); }
		}

		public static readonly DependencyProperty ProjectProperty =
			DependencyProperty.Register("Project", typeof(Project), typeof(PropertyMarker));

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(PropertyMarker));

		public ICommand ApplyForSpecificSolutionPlatformCommand { get; private set; }

		private void ApplyForSpecificSolutionPlatform(string solutionPlatform)
		{
			CallCommand(new PropertyMarkerAction { ApplyForSelectedSolutionPlatformInCurrentSolutionConfiguration = solutionPlatform });
		}

		public ICommand ApplyForEverySolutionConfigurationCommand { get; private set; }

		private void ApplyForEverySolutionConfiguration()
		{
			CallCommand(new PropertyMarkerAction { ApplyForEverySolutionConfigurationInCurrentSolutionPlatform = true });
		}

		public ICommand ApplyForSpecificSolutionConfigurationCommand { get; private set; }

		private void ApplyForSpecificSolutionConfiguration(string solutionConfiguration)
		{
			CallCommand(new PropertyMarkerAction
			{
				ApplyForSelectedSolutionConfigurationInCurrentSolutionPlatform = solutionConfiguration
			});
		}

		public ICommand ApplyForEverySolutionConfigurationAndPlatformCommand { get; private set; }

		private void ApplyForEverySolutionConfigurationAndPlatform()
		{
			CallCommand(new PropertyMarkerAction
			{
				ApplyForEverySolutionConfigurationAndPlatform = true
			});
		}

		public ConfigurationManager.ConfigurationManager ConfigurationManager
		{
			get
			{
				return (ConfigurationManager.ConfigurationManager)GetValue(ConfigurationManagerProperty);
			}
			set { SetValue(ConfigurationManagerProperty, value); }
		}

		public static readonly DependencyProperty ConfigurationManagerProperty =
			DependencyProperty.Register("ConfigurationManager",
				typeof(ConfigurationManager.ConfigurationManager), typeof(PropertyMarker),
				new FrameworkPropertyMetadata(null));
	}
}