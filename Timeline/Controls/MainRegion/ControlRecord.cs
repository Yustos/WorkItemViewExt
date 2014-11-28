using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls
{
	public class ControlRecord : Border
	{
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
			CornerRadius = new CornerRadius(3);

			Style = CreateStyle(record.State);

			var dockPanel = new DockPanel { LastChildFill = true };

			var tb = new TextBlock();
			tb.Text = string.Format("[{0}] {1}\r\n{2}", record.Rev, record.State, record.Date);
			DockPanel.SetDock(tb, Dock.Top);
			dockPanel.Children.Add(tb);

			if (record.AddedAttachments != 0 || record.RemovedAttachments != 0 || record.AddedChangesets != 0 || record.RemovedChangesets != 0)
			{
				var grid = new Grid();
				for (var i = 0; i < 4; i++)
				{
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				}
				AddDescBox(grid, record.AddedAttachments, "Attachments added: {0}.", 0);
				AddDescBox(grid, -record.RemovedAttachments, "Attachments removed: {0}.", 1);
				AddDescBox(grid, record.AddedChangesets, "Changesets added: {0}.", 2);
				AddDescBox(grid, -record.RemovedChangesets, "Changesets removed: {0}.", 3);

				dockPanel.Children.Add(grid);
			}

			Child = dockPanel;
		}

		private void AddDescBox(Grid grid, int count, string description, int column)
		{
			if (count != 0)
			{
				var tb = new TextBlock
				{
					Text = string.Format("{0:+#;-#}{1}", count, description[0]),
					ToolTip = string.Format(description, count < 0 ? -count : count)
				};
				grid.Children.Add(tb);
				Grid.SetColumn(tb, column);
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
							new Setter()
							{
								Property = BorderThicknessProperty,
								Value = new Thickness(1)
							},
							new Setter()
							{
								Property = MarginProperty,
								Value = new Thickness(1)
							},
							new Setter
							{
								Property = BackgroundProperty,
								Value = new SolidColorBrush(color)
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
							new Setter()
							{
								Property = BorderThicknessProperty,
								Value = new Thickness(2)
							},
							new Setter()
							{
								Property = MarginProperty,
								Value = new Thickness(0)
							},
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
