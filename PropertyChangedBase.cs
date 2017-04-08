using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BetterConfigurationManager.Annotations;


namespace BetterConfigurationManager
{
	public abstract class PropertyChangedBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler == null)
				return;
			Action handlerAction = () => handler(this, new PropertyChangedEventArgs(propertyName));
			Application.Current.Dispatcher.InvokeAsync(handlerAction);
		}
	}
}