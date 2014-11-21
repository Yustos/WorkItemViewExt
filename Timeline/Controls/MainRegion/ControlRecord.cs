using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls
{
	public class ControlRecord : Border
	{
		private static Style SelectedStyle = new Style()
			{
				Triggers =
				{
					new Trigger()
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
							new Setter
							{
								Property = BackgroundProperty,
								Value = new SolidColorBrush(Color.FromRgb(186, 208, 231))
							}
						}
					},
					new Trigger()
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
							new Setter
							{
								Property = BackgroundProperty,
								Value = new SolidColorBrush(Color.FromRgb(196, 218, 241))
							}
						}
					}
				},
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
			CornerRadius = new CornerRadius(3);

			Style = SelectedStyle;

			var tb = new TextBlock();
			tb.Text = string.Format("[{0}] {1}\r\n{2}", record.Rev, record.State, record.Date); ;
			Child = tb;
		}
	}
}
