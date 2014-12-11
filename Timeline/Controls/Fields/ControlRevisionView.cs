using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.Fields.Converters;
using YL.Timeline.Controls.MainRegion;
using YL.Timeline.Entities;
using YL.Timeline.Entities.RecordDetails;

namespace YL.Timeline.Controls.Fields
{
	public class ControlRevisionView : DockPanel
	{
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title",
			typeof(string),
			typeof(ControlRevisionView),
			new FrameworkPropertyMetadata(null)
		);

		public string Title
		{
			get
			{
				return (string)GetValue(TitleProperty);
			}
			set
			{
				SetValue(TitleProperty, value);
			}
		}

		public static readonly DependencyProperty OnlyChangedProperty = DependencyProperty.Register(
			"OnlyChanged",
			typeof(bool),
			typeof(ControlRevisionView),
			new FrameworkPropertyMetadata(false, (d, doe) =>
			{
				var host = (ControlRevisionView)d;
				var onlyChanged = (bool)doe.NewValue;
				host.ApplyFilter(onlyChanged);
			})
		);

		public bool OnlyChanged
		{
			get
			{
				return (bool)GetValue(OnlyChangedProperty);
			}
			set
			{
				SetValue(OnlyChangedProperty, value);
			}
		}

		private static readonly Style HeaderStyle = new Style(typeof(DataGridColumnHeader));

		private readonly DataGrid _fieldsGrid = new DataGrid { AutoGenerateColumns = false, HeadersVisibility = DataGridHeadersVisibility.Column, IsReadOnly = true };
		private readonly DataGrid _attachmentsGrid = new DataGrid { AutoGenerateColumns = false, HeadersVisibility = DataGridHeadersVisibility.Column, IsReadOnly = true };
		private readonly DataGrid _changesetsGrid = new DataGrid { AutoGenerateColumns = false, HeadersVisibility = DataGridHeadersVisibility.Column, IsReadOnly = true };

		static ControlRevisionView()
		{
			var setter = new Setter
				{
					Property = ToolTipProperty,
					Value = "Added/Deleted"
				};
			var trigger = new Trigger{
				Property = IsMouseOverProperty,
				Value = true};
			trigger.Setters.Add(setter);
			HeaderStyle.Triggers.Add(trigger);
		}

		public ControlRevisionView()
		{
			Width = 256;

			LastChildFill = true;
			var title = new DockPanel { LastChildFill = true };
			DockPanel.SetDock(title, Dock.Top);

			var filterCheckBox = new CheckBox();
			filterCheckBox.Margin = new Thickness(2);
			filterCheckBox.ToolTip = "Only changed fields";
			filterCheckBox.SetBinding(CheckBox.IsCheckedProperty, new Binding("OnlyChanged") { Source = this });
			title.Children.Add(filterCheckBox);

			var titleText = new TextBlock();
			titleText.Text = "Fields: ";
			title.Children.Add(titleText);

			var closeTextBox = new TextBlock() { Text = "x", Margin = new Thickness(0, 0, 0, 8) };
			closeTextBox.MouseLeftButtonUp += CloseTextBoxMouseLeftButtonUp;
			DockPanel.SetDock(closeTextBox, Dock.Right);
			title.Children.Add(closeTextBox);

			var titleTextBox = new TextBlock();
			titleTextBox.SetBinding(TextBlock.TextProperty, new Binding("Title") { Source = this });
			title.Children.Add(titleTextBox);
			Children.Add(title);

			// Attachments
			var attachmentsImgFactory = new FrameworkElementFactory(typeof(Image));
			attachmentsImgFactory.SetValue(Image.SourceProperty, new Binding("IsAdded") { Converter = new BoolToImageConverter() });
			_attachmentsGrid.Columns.Add(new DataGridTemplateColumn
			{
				HeaderStyle = HeaderStyle,
				Width = 16,
				CellTemplate = new DataTemplate
				{
					VisualTree = attachmentsImgFactory
				}});

			_attachmentsGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
			DockPanel.SetDock(_attachmentsGrid, Dock.Bottom);
			Children.Add(_attachmentsGrid);
			var attachmentsTitle = new TextBlock { Text = "Attachments" };
			DockPanel.SetDock(attachmentsTitle, Dock.Bottom);
			Children.Add(attachmentsTitle);

			// Changesets
			var changesetsImgFactory = new FrameworkElementFactory(typeof(Image));
			changesetsImgFactory.SetValue(Image.SourceProperty, new Binding("IsAdded") { Converter = new BoolToImageConverter() });

			_changesetsGrid.Columns.Add(new DataGridTemplateColumn
			{
				HeaderStyle = HeaderStyle,
				Width = 16,
				CellTemplate = new DataTemplate
				{
					VisualTree = changesetsImgFactory
				}
			});

			_changesetsGrid.Columns.Add(new DataGridHyperlinkColumn { 
				Header = "Uri", 
				Binding = new Binding("Uri"),
				ContentBinding = new Binding("Uri"),
				Width = new DataGridLength(1, DataGridLengthUnitType.Star),
			});
			_changesetsGrid.Columns.Add(new DataGridTextColumn { Header = "Comment", Binding = new Binding("Comment"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
			DockPanel.SetDock(_changesetsGrid, Dock.Bottom);
			Children.Add(_changesetsGrid);
			var changesetsTitle = new TextBlock { Text = "Changesets" };
			DockPanel.SetDock(changesetsTitle, Dock.Bottom);
			Children.Add(changesetsTitle);

			// Fields
			_fieldsGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
			_fieldsGrid.Columns.Add(new DataGridTextColumn { Header = "Original", Binding = new Binding("OriginalValue"), Width = 64 });
			_fieldsGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value"), Width = 64 });
			_fieldsGrid.LoadingRow += FieldsGridLoadingRow;
			Children.Add(_fieldsGrid);

			Loaded += ControlRevisionView_Loaded;
		}

		private void FieldsGridLoadingRow(object sender, DataGridRowEventArgs e)
		{
			var field = (Field)e.Row.DataContext;
			e.Row.FontWeight = string.Equals(Convert.ToString(field.OriginalValue), Convert.ToString(field.Value)) ? FontWeights.Normal : FontWeights.Bold;
		}

		private void CloseTextBoxMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var parent = Helpers.FindParent<ControlRevisionsView>(this);
			var list = (IList)parent.ItemsSource;
			list.Remove(DataContext);
		}

		private void ControlRevisionView_Loaded(object sender, RoutedEventArgs e)
		{
			var record = (Record)DataContext;
			Title = string.Format("{0} [{1}]", record.Owner.Id, record.Rev);
			var controller = ControlRevisionsView.GetController(this);
			Task.Factory.StartNew(() =>
				{
					record.EnsureDetails();
				}).
				ContinueWith(r => {
					if (r.Exception == null)
					{
						Dispatcher.Invoke(
							() =>
							{
								_fieldsGrid.ItemsSource = record.Details.Fields;
								_attachmentsGrid.ItemsSource = record.Details.Attachments;
								_changesetsGrid.ItemsSource = record.Details.Changesets;

								ApplyFilter(OnlyChanged);
							});
					}
					else
					{
						Dispatcher.Invoke(() => controller.Log(Convert.ToString(r.Exception)));
					}
				});
		}

		private void ApplyFilter(bool onlyChanged)
		{
			ICollectionView view = CollectionViewSource.GetDefaultView(_fieldsGrid.ItemsSource);
			if (view != null)
			{
				view.Filter = (a) =>
				{
					var f = (Field)a;
					return !onlyChanged || !string.Equals(Convert.ToString(f.OriginalValue), Convert.ToString(f.Value));
				};
			}
		}
	}
}
