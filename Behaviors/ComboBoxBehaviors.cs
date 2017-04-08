using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace BetterConfigurationManager.Behaviors
{
	public class ComboBoxBehaviors : DependencyObject
	{
		public static ICommand GetCommand(DependencyObject dependencyObject) 
			=> (ICommand)dependencyObject.GetValue(CommandProperty);

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached("Command", typeof(ICommand),
				typeof(ComboBoxBehaviors), new UIPropertyMetadata(null, SetCommandCallback));

		private static void SetCommandCallback(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var selector = (Selector)dependencyObject;
			if (selector != null)
				selector.SelectionChanged += SelectionChanged;
		}

		private static void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selector = (Selector)sender;
			var command = selector?.GetValue(CommandProperty) as ICommand;
			command?.Execute(selector.SelectedItem);
		}

		public static void SetCommand(DependencyObject dependencyObject, ICommand command) 
			=> dependencyObject.SetValue(CommandProperty, command);

		public static readonly DependencyProperty DefaultTextProperty =
			DependencyProperty.RegisterAttached("DefaultText", typeof(string),
				typeof(ComboBoxBehaviors), new UIPropertyMetadata(null, SetDefaultTextCallback));

		private static void SetDefaultTextCallback(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var comboBox = dependencyObject as ComboBox;
			if (comboBox == null)
				return;
			SetDefaultText(dependencyObject, args.NewValue.ToString());
		}

		public static void SetDefaultText(DependencyObject dependencyObject, string value)
		{
			var comboBox = (ComboBox)dependencyObject;
			RefreshDefaultText(comboBox, value);
			comboBox.SelectionChanged += (sender, args) =>
				RefreshDefaultText((ComboBox)sender, GetDefaultText((ComboBox)sender));
			dependencyObject.SetValue(DefaultTextProperty, value);
		}

		public static string GetDefaultText(DependencyObject dependencyObject) 
			=> (string)dependencyObject.GetValue(DefaultTextProperty);

		private static void RefreshDefaultText(ComboBox combo, string text)
		{
			if (combo.SelectedIndex == -1 && !string.IsNullOrEmpty(text))
			{
				// Show DefaultText
				var visual = new TextBlock
				{
					FontStyle = FontStyles.Italic,
					Text = text,
					Foreground = Brushes.Gray
				};

				combo.Background = new VisualBrush(visual)
				{
					Stretch = Stretch.None,
					AlignmentX = AlignmentX.Left,
					AlignmentY = AlignmentY.Center,
					Transform = new TranslateTransform(3, 0)
				};
			}
			else
				// Hide DefaultText
				combo.Background = null;
		}
	}
}