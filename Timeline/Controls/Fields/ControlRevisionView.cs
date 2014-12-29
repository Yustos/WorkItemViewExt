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
using System.Windows.Media.Imaging;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.Fields.Converters;
using YL.Timeline.Controls.MainRegion;
using YL.Timeline.Entities;
using YL.Timeline.Entities.RecordDetails;

namespace YL.Timeline.Controls.Fields
{
	public class ControlRevisionView : DockPanel
	{
		private static readonly Brush _changedBrush = new SolidColorBrush(Color.FromArgb(255, 240, 255, 240));
		private static readonly BitmapSource _closeSource = Properties.Resources.Close.ToSource();

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

		public static readonly DependencyProperty UserProperty = DependencyProperty.Register(
			"User",
			typeof(string),
			typeof(ControlRevisionView),
			new FrameworkPropertyMetadata(null)
		);

		public string User
		{
			get
			{
				return (string)GetValue(UserProperty);
			}
			set
			{
				SetValue(UserProperty, value);
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

			var closeButton = new Button { Content = new Image() { Source = _closeSource, Height=12, Width=13 }, Margin = new Thickness(0, 0, 0, 0) };
			closeButton.Click += CloseButtonClick;
			DockPanel.SetDock(closeButton, Dock.Right);
			title.Children.Add(closeButton);

			var titleTextBox = new TextBlock();
			titleTextBox.SetBinding(TextBlock.TextProperty, new Binding("Title") { Source = this });
			title.Children.Add(titleTextBox);
			Children.Add(title);


			var userTextBlock = new TextBlock();
			userTextBlock.SetBinding(TextBlock.TextProperty, new Binding("User") { Source = this });
			DockPanel.SetDock(userTextBlock, Dock.Top);
			Children.Add(userTextBlock);

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

			var attachmentsLinkStyle = new Style(typeof(TextBlock));
			attachmentsLinkStyle.Setters.Add(new EventSetter(Hyperlink.ClickEvent, (RoutedEventHandler)AttachmentEventSetterOnHandler));
			attachmentsLinkStyle.Setters.Add(new Setter(TextBlock.ToolTipProperty, new Binding("Uri")));
			_attachmentsGrid.Columns.Add(new DataGridHyperlinkColumn
			{
				Header = "Name",
				Binding = new Binding("Name"),
				Width = new DataGridLength(1, DataGridLengthUnitType.Star),
				ElementStyle = attachmentsLinkStyle
			});

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

			var changesetLinkStyle = new Style(typeof(TextBlock));
			changesetLinkStyle.Setters.Add(new EventSetter(Hyperlink.ClickEvent, (RoutedEventHandler)ChangesetEventSetterOnHandler));
			changesetLinkStyle.Setters.Add(new Setter(TextBlock.ToolTipProperty, new Binding("Uri")));

			_changesetsGrid.Columns.Add(new DataGridHyperlinkColumn
			{
				Header = "Comment",
				Binding = new Binding("Comment"),
				Width = new DataGridLength(1, DataGridLengthUnitType.Star),
				ElementStyle = changesetLinkStyle
			});
			DockPanel.SetDock(_changesetsGrid, Dock.Bottom);
			Children.Add(_changesetsGrid);
			var changesetsTitle = new TextBlock { Text = "Changesets" };
			DockPanel.SetDock(changesetsTitle, Dock.Bottom);
			Children.Add(changesetsTitle);

			// Fields
			var nameFieldStyle = new Style(typeof(TextBlock));
			nameFieldStyle.Setters.Add(new EventSetter(Hyperlink.ClickEvent, (RoutedEventHandler)AttachmentEventSetterOnHandler));
			nameFieldStyle.Setters.Add(new Setter(TextBlock.ToolTipProperty, new Binding("ReferenceName")));
			_fieldsGrid.Columns.Add(new DataGridTextColumn { Header = "Name",
				Binding = new Binding("Name"),
				Width = new DataGridLength(1, DataGridLengthUnitType.Star),
				ElementStyle = nameFieldStyle
			});
			_fieldsGrid.Columns.Add(new DataGridTextColumn { Header = "Original", Binding = new Binding("OriginalValue"), Width = 64 });
			_fieldsGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value"), Width = 64 });
			_fieldsGrid.LoadingRow += FieldsGridLoadingRow;
			Children.Add(_fieldsGrid);

			Loaded += ControlRevisionView_Loaded;
		}

		private void ChangesetEventSetterOnHandler(object sender, RoutedEventArgs e)
		{
			var changeset = (Changeset)((Hyperlink)e.OriginalSource).DataContext;
			var controller = ControlRevisionsView.GetController(this);
			controller.ShowChangeset(changeset.Id);
		}

		private void AttachmentEventSetterOnHandler(object sender, RoutedEventArgs e)
		{
			var attachment = (Attachment)((Hyperlink)e.OriginalSource).DataContext;
			Process.Start(attachment.Uri.ToString());
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			var parent = Helpers.FindParent<ControlRevisionsView>(this);
			var list = (IList)parent.ItemsSource;
			list.Remove(DataContext);
		}

		private void FieldsGridLoadingRow(object sender, DataGridRowEventArgs e)
		{
			var field = (Field)e.Row.DataContext;
			if (!string.Equals(Convert.ToString(field.OriginalValue), Convert.ToString(field.Value)))
			{
				e.Row.Background = _changedBrush;
				if (field.IsChangedByUser)
				{
					e.Row.FontWeight = FontWeights.Bold;
				}
			}
		}

		private void ControlRevisionView_Loaded(object sender, RoutedEventArgs e)
		{
			var record = (Record)DataContext;
			Title = string.Format("Id: {0}, Rev: {1}", record.Owner.Id, record.Rev);
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
								User = record.Details.ChangedBy +
									(record.Details.ChangedBy == record.Details.AuthorizedAs ? null : " via " + record.Details.AuthorizedAs);
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
