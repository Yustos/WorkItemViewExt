using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.Behind.Entities;
using YL.Timeline.Controls.MainRegion;
using YL.Timeline.Controls.MainRegion.Ornament;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls
{
	public class ControlRecords : Canvas
	{
		public static readonly DependencyProperty RecordsProperty =
			DependencyProperty.Register("Records", typeof(Record[]),
			typeof(ControlRecords),
			new UIPropertyMetadata(null, (d, doa) =>
			{
				var host = (ControlRecords)d;
				host.UpdateRecords((Record[])doa.NewValue);
			}));

		public Record[] Records
		{
			get
			{
				return (Record[])GetValue(RecordsProperty);
			}
			set
			{
				SetValue(RecordsProperty, value);
			}
		}

		public static readonly DependencyProperty ControllerProperty = ControlTimeLine.ControllerProperty.AddOwner(typeof(ControlRecords));

		public ViewportController Controller
		{
			get
			{
				return (ViewportController)GetValue(ControllerProperty);
			}
			set
			{
				SetValue(ControllerProperty, value);
			}
		}

		public double LastElementWidth { get; set; }

		public ControlRecords()
		{
			ClipToBounds = false;
		}

		private void UpdateRecords(Record[] records)
		{
			var layer = AdornerLayer.GetAdornerLayer(this);
			ControlRecord last = null;
			foreach (var record in records)
			{
				var recordControl = new ControlRecord(record);
				Children.Add(recordControl);
				if (last != null)
				{
					layer.Add(new StateAdorner(recordControl, last, recordControl));
				}
				last = recordControl;
			}
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (Children.Count > 0)
			{
				base.MeasureOverride(constraint);

				var controller = Controller;
				var childs = Children.OfType<ControlRecord>().ToList();
				LastElementWidth = childs.Last().DesiredSize.Width;
				foreach (var child in childs)
				{
					var record = (Record)child.DataContext;
					child.ActualLeft = controller.Interpolate(record.Date, constraint.Width);
				}

				var h = CalculateCollisions(childs);
				return new Size(constraint.Width, h);
			}
			else
			{
				return base.MeasureOverride(constraint);
			}
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			var result = base.ArrangeOverride(arrangeSize);

#warning Not sure.
			var layer = AdornerLayer.GetAdornerLayer(this);
			layer.Update();

			return result;
		}

		private double CalculateCollisions(List<ControlRecord> childs)
		{
#warning Dont calculate or hide invisible records.
			var last = childs.First();
			var firstLine = GetTop(last);
			double max = last.ActualTop + last.DesiredSize.Height;
			foreach (var record in childs.Skip(1))
			{
				var lastTop = GetTop(last);
				var lastLeft = GetLeft(last);
				var curLeft = GetLeft(record);
				if (double.IsNaN(lastTop))
				{
					lastTop = 0;
				}

				if (lastLeft + last.DesiredSize.Width >= curLeft)
				{
					record.ActualTop = lastTop + last.DesiredSize.Height;
				}
				else
				{
					record.ActualTop = 0;
				}

				max = Math.Max(record.ActualTop + record.DesiredSize.Height, max);
				last = record;
			}
			return max;
		}
	}
}
