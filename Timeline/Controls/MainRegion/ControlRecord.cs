using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls
{
	public class ControlRecord : Border
	{
		private static readonly BitmapSource[] _icons = new[]
			{
				Properties.Resources.AttachmentAdd.ToSource(),
				Properties.Resources.AttachmentRemove.ToSource(),
				Properties.Resources.ChangesetAdd.ToSource(),
				Properties.Resources.ChangesetRemove.ToSource()
			};

		internal double ActualLeft
		{
			get
			{
				var v = Canvas.GetLeft(this);
				if (double.IsNaN(v))
				{
					v = 0;
				}
				return v;
			}
			set
			{
				Canvas.SetLeft(this, value);
			}
		}

		internal double ActualTop
		{
			get
			{
				var v = Canvas.GetTop(this);
				if (double.IsNaN(v))
				{
					v = 0;
				}
				return v;
			}
			set
			{
				Canvas.SetTop(this, value);
			}
		}

		internal ControlRecord(Record record)
		{
			DataContext = record;
			BorderBrush = new SolidColorBrush(Color.FromRgb(130, 166, 207));
			BorderThickness = new Thickness(1);
			CornerRadius = new CornerRadius(3);

			Style = CreateStyle(record.State);

			var dockPanel = new DockPanel { LastChildFill = true };

			if (record.AddedAttachments != 0 || record.RemovedAttachments != 0 || record.AddedChangesets != 0 || record.RemovedChangesets != 0)
			{
				var grid = new Grid();
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

				AddDescBox(grid, 0, record.AddedAttachments, "Attachments added: {0}.", 0, 0);
				AddDescBox(grid, 1, -record.RemovedAttachments, "Attachments removed: {0}.", 0, 1);
				AddDescBox(grid, 2, record.AddedChangesets, "Changesets added: {0}.", 1, 0);
				AddDescBox(grid, 3, -record.RemovedChangesets, "Changesets removed: {0}.", 1, 1);

				dockPanel.Children.Add(grid);
				DockPanel.SetDock(grid, Dock.Right);
			}

			var stack = new StackPanel();

			stack.Children.Add(new TextBlock
			{
				Text = string.Format("[{0}] {1}", record.Rev, record.Date),
				ToolTip = string.Format("Work item: {1}{0}Revision #: {2}{0}Changed date: {3}", Environment.NewLine, record.Owner.Id, record.Rev, record.Date)
			});
			stack.Children.Add(new TextBlock { Text = record.State, ToolTip = "State" });


			if (record.DisplayFields != null)
			{
				foreach (var field in record.DisplayFields)
				{
					var fieldBlock = new TextBlock();
					fieldBlock.Inlines.Add(field.Name);
					fieldBlock.Inlines.Add(": ");
					fieldBlock.Inlines.Add(new Run(Convert.ToString(field.Value)) { FontWeight = FontWeights.Bold });

					var tooltipBlock = new TextBlock();
					tooltipBlock.Inlines.Add(field.ReferenceName);
					tooltipBlock.Inlines.Add(": ");
					tooltipBlock.Inlines.Add(new Run(Convert.ToString(field.OriginalValue)) { FontWeight = FontWeights.Bold });
					tooltipBlock.Inlines.Add(" -> ");
					tooltipBlock.Inlines.Add(new Run(Convert.ToString(field.Value)) { FontWeight = FontWeights.Bold });

					fieldBlock.ToolTip = tooltipBlock;

					stack.Children.Add(fieldBlock);
				}
				
				//description += Environment.NewLine + string.Join(Environment.NewLine, record.DisplayFields);
			}
				
			DockPanel.SetDock(stack, Dock.Top);
			dockPanel.Children.Add(stack);

			Child = dockPanel;
		}

		private void AddDescBox(Grid icons, int iconIndex, int count, string description, int row, int column)
		{
			if (count != 0)
			{
				var stack = new StackPanel
				{
					Orientation = Orientation.Horizontal,
					ToolTip = string.Format(description, count < 0 ? -count : count)
				};

				stack.Children.Add(new Image { Source = _icons[iconIndex] });
				stack.Children.Add(new TextBlock { Text = count.ToString() });

				icons.Children.Add(stack);
				Grid.SetRow(stack, row);
				Grid.SetColumn(stack, column);
			}
		}

		private Style CreateStyle(string state)
		{
			var color = string.IsNullOrEmpty(state) ? Color.FromRgb(196, 218, 241) : ColorGenerator.GetColor(state);
			color.A = 255;
			var result = new Style();
			result.Triggers.Add(new Trigger()
					{
						Property = Selector.IsSelectedProperty,
						Value = false,
						Setters =
						{
							new Setter
							{
								Property = BackgroundProperty,
								Value = new LinearGradientBrush(
									color,
									Color.FromArgb(70, color.R, color.G, color.B),  0)
							}
						}
					});
			color.R -= 20;
			color.G -= 20;
			color.B -= 20;

			result.Triggers.Add(new Trigger()
				{
					Property = Selector.IsSelectedProperty,
					Value = true,
					Setters =
						{
							new Setter
							{
								Property = BackgroundProperty,
								Value = new SolidColorBrush(color)
							}
						}
				});
			return result;
		}
	}
}
