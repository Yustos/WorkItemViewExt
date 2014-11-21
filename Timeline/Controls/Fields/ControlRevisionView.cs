using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using YL.Timeline.Entities;
using YL.Timeline.Interaction;

namespace YL.Timeline.Controls.Fields
{
	public class ControlRevisionView : DockPanel
	{
		private static LoaderCommand _loader;

		public static readonly DependencyProperty InfoLoaderCommandProperty = DependencyProperty.RegisterAttached(
			"InfoLoaderCommand",
			typeof(LoaderCommand),
			typeof(ControlRevisionView),
			new FrameworkPropertyMetadata(null, (d, doa) =>
			{
				_loader = (LoaderCommand)doa.NewValue;
			})
		);
		public static void SetInfoLoaderCommand(UIElement element, LoaderCommand value)
		{
			element.SetValue(InfoLoaderCommandProperty, value);
		}
		public static LoaderCommand GetInfoLoaderCommand(UIElement element)
		{
			return (LoaderCommand)element.GetValue(InfoLoaderCommandProperty);
		}

		private readonly DataGrid _grid = new DataGrid { AutoGenerateColumns = false, HeadersVisibility = DataGridHeadersVisibility.Column};

		private static AdornerLayer _layer;

		private SelectionLinkAdorner _adorner;

		public ControlRevisionView()
		{
			LastChildFill = true;

			/*var title = new DockPanel { LastChildFill = true };
			DockPanel.SetDock(title, Dock.Top);

			var closeTextBox = new TextBlock() { Text = "X" };
			DockPanel.SetDock(closeTextBox, Dock.Right);
			title.Children.Add(closeTextBox);

			var titleTextBox = new TextBlock();
			titleTextBox.SetBinding(TextBlock.TextProperty, new Binding("Rev"));
			
			title.Children.Add(titleTextBox);
			Children.Add(title);*/

			
			_grid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name"), Width = 128 });
			_grid.Columns.Add(new DataGridTextColumn { Header = "Original", Binding = new Binding("OriginalValue"), Width = 64 });
			_grid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value"), Width = 64 });
			DockPanel.SetDock(_grid, Dock.Top);
			Children.Add(_grid);

			Loaded += ControlRevisionView_Loaded;
			Unloaded += ControlRevisionView_Unloaded;
		}

		private void ControlRevisionView_Loaded(object sender, RoutedEventArgs e)
		{
			var recordControl = (ControlRecord)DataContext;

			if (_layer == null)
			{
				_layer = AdornerLayer.GetAdornerLayer(FindParentForLayer(this));
			}
			_adorner = new SelectionLinkAdorner(recordControl, this);
			_layer.Add(_adorner);
			
			var record = (Record)recordControl.DataContext;
			var fields = _loader.LoadInfo(record.Owner.Id, record.Rev);
			_grid.ItemsSource = fields;
		}

		private void ControlRevisionView_Unloaded(object sender, RoutedEventArgs e)
		{
			_layer.Remove(_adorner);
		}

		public static ControlRevisionsView FindParentForLayer(DependencyObject dependencyObject)
		{
			var parent = VisualTreeHelper.GetParent(dependencyObject);
			if (parent == null) return null;
			var parentT = parent as ControlRevisionsView;
			return parentT ?? FindParentForLayer(parent);
		}
	}
}
