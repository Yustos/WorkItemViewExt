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
using YL.Timeline.Entities;

namespace YL.Timeline.Controls
{
	public class ControlRecords : Canvas
	{
		private readonly SortedList<Record, ControlRecord> _records = new SortedList<Record, ControlRecord>(new RecordComparer());

		private readonly Dictionary<double, double> _lines = new Dictionary<double, double>();

		private readonly List<Tuple<double, double, double, string>> _states = new List<Tuple<double, double, double, string>>();

		private readonly ControlRecord _lastElement;

		private readonly ViewportController _controller;

#warning Not good.
		internal SortedList<Record, ControlRecord> RecordControls
		{
			get
			{
				return _records;
			}
		}

		private readonly List<ControlPath> _paths = new List<ControlPath>();

		public ControlRecords(ViewportController controller, Record[] records)
		{
			_controller = controller;
			ClipToBounds = false;
			foreach (var record in records)
			{
				var recordControl = new ControlRecord(record);
				Children.Add(recordControl);
				_records.Add(record, recordControl);
				_lastElement = recordControl;
			}
		}

		internal void UpdateViewport()
		{
			InvalidateMeasure();
			InvalidateVisual();
		}

		protected override Size MeasureOverride(Size constraint)
		{
			base.MeasureOverride(constraint);

			_lastElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			foreach (var record in _records)
			{
				record.Value.ActualLeft = _controller.Interpolate(record.Key, constraint.Width, _lastElement.DesiredSize.Width);
			}

			var height = CalculateCollisions();
			return new Size(constraint.Width, height);
		}

		protected override void OnRender(DrawingContext dc)
		{
			foreach (var state in _states)
			{
				if (state.Item4 == null)
				{
					continue;
				}
				dc.DrawRectangle(new SolidColorBrush(ColorGenerator.GetColor(state.Item4)), null,
						new Rect(state.Item1, state.Item3, state.Item2 - state.Item1, 16));
			}
		}

		private double CalculateCollisions()
		{
#warning Dont calculate or hide invisible records.
			_lines.Clear();
			_states.Clear();

			KeyValuePair<Record, ControlRecord> last = _records.First();
			var firstLine = GetTop(last.Value);
			_lines[double.IsNaN(firstLine) ? 0 : firstLine] = last.Value.DesiredSize.Height;
			double max = last.Value.DesiredSize.Height;
			foreach (var record in _records.Skip(1))
			{
				var lastTop = GetTop(last.Value);
				var lastLeft = GetLeft(last.Value);
				var curLeft = GetLeft(record.Value);
				if (double.IsNaN(lastTop))
				{
					lastTop = 0;
				}

				if (lastLeft + last.Value.DesiredSize.Width >= curLeft)
				{
					record.Value.ActualTop = lastTop + last.Value.DesiredSize.Height;
					max = Math.Max(lastTop + last.Value.DesiredSize.Height + record.Value.DesiredSize.Height, max);
					_lines[lastTop + last.Value.DesiredSize.Height] = record.Value.DesiredSize.Height;
				}
				else
				{
					record.Value.ActualTop = 0;
				}

				_states.Add(new Tuple<double, double, double, string>(lastLeft, curLeft, record.Value.ActualTop + record.Value.DesiredSize.Height / 2 - 10, last.Key.State));

				last = record;
			}
			return max;
		}
	}
}
