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

		public ObservableCollection<string> AvailableSolutionConfigurations { get; private set; }
		public ObservableCollection<string> AvailableSolutionPlatforms { get; private set; }
		public ObservableCollection<Project> Projects { get; private set; }
		public event Action SolutionContextChanged;

		public string ActiveSolutionConfiguration
		{
			get { return activeSolutionConfiguration; }
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
		{
			get
			{
				return Projects.Count > 0 && ActiveSolutionConfiguration != null &&
							ActiveSolutionPlatform != null;
			}
		}

		public string ActiveSolutionPlatform
		{
			get { return activeSolutionPlatform; }
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
			if (SolutionContextChanged != null)
				SolutionContextChanged();
		}

		public bool IsSolutionAvailable
		{
			get { return isSolutionAvailable; }
			set
			{
				isSolutionAvailable = value;
				OnPropertyChanged();
			}
		}

		private bool isSolutionAvailable;

		public string StatusText
		{
			get { return statusText; }
			set
			{
				statusText = value;
				OnPropertyChanged();
			}
		}

		private string statusText;

		public bool ShowStatusText
		{
			get { return showStatusText; }
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