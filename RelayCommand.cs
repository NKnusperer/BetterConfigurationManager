using System;
using System.Windows.Input;

namespace BetterConfigurationManager
{
	public class RelayCommand<T> : ICommand
	{
		public RelayCommand(Action<T> action, Predicate<T> canExecute = null)
		{
			this.action = action;
			this.canExecute = canExecute;
		}

		protected readonly Action<T> action;
		private readonly Predicate<T> canExecute;

		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (canExecute != null)
					CommandManager.RequerySuggested += value;
			}
			remove
			{
				if (canExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}

		public virtual bool CanExecute(object parameter)
		{
			return canExecute == null || canExecute((T)parameter);
		}

		public virtual void Execute(object parameter)
		{
			action((T)parameter);
		}
	}

	public class RelayCommand : RelayCommand<object>
	{
		public RelayCommand(Action action)
			: base(delegate { action(); }, null) {}
	}
}