using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BetterConfigurationManager.ConfigurationManager
{
	public abstract class ConfigurationManager : PropertyChangedBase
	{
		protected ConfigurationManager()
		{
			AvailableSolutionConfigurations = new ObservableCollection<string>();
			AvailableSolutionPlatforms = new ObservableCollection<string>();
			Projects = new ObservableCollection<Project>();
		}

		public ObservableCollection<string> AvailableSolutionConfigurations { get; }
		public ObservableCollection<string> AvailableSolutionPlatforms { get; }
		public ObservableCollection<Project> Projects { get; }
		public event Action SolutionContextChanged;

		public string ActiveSolutionConfiguration
		{
			get => activeSolutionConfiguration;
			set
			{
				activeSolutionConfiguration = value;
				UpdateProjectsSolutionContext();
				FireSolutionContextChanged();
				OnPropertyChanged();
			}
		}

		private string activeSolutionConfiguration;

		protected void UpdateProjectsSolutionContext()
		{
			if (!ValidSolutionContextSet)
				return;
			foreach (Project project in Projects)
				project.SetSolutionContext(ActiveSolutionConfiguration, ActiveSolutionPlatform);
		}

		public bool ValidSolutionContextSet 
			=> Projects.Count > 0 && ActiveSolutionConfiguration != null && ActiveSolutionPlatform != null;

		public string ActiveSolutionPlatform
		{
			get => activeSolutionPlatform;
			set
			{
				activeSolutionPlatform = value;
				UpdateProjectsSolutionContext();
				FireSolutionContextChanged();
				OnPropertyChanged();
			}
		}

		private string activeSolutionPlatform;

		protected void FireSolutionContextChanged()
		{
			if (!ValidSolutionContextSet)
				return;
			SolutionContextChanged?.Invoke();
		}

		public bool IsSolutionAvailable
		{
			get => isSolutionAvailable;
			set
			{
				isSolutionAvailable = value;
				OnPropertyChanged();
			}
		}

		private bool isSolutionAvailable;

		public string StatusText
		{
			get => statusText;
			set
			{
				statusText = value;
				OnPropertyChanged();
			}
		}

		private string statusText;

		public bool ShowStatusText
		{
			get => showStatusText;
			set
			{
				showStatusText = value;
				OnPropertyChanged();
			}
		}

		private bool showStatusText;

		public void ClearData()
		{
			ActiveSolutionPlatform = null;
			ActiveSolutionConfiguration = null;
			AvailableSolutionConfigurations.Clear();
			AvailableSolutionPlatforms.Clear();
			Projects.Clear();
			IsSolutionAvailable = false;
		}

		public abstract Task Reload();
	}
}