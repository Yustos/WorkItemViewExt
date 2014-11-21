using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YL.Timeline.Controls.Behind.Entities;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.Behind
{
	public class ViewportController
	{
		private readonly TimelineInput _input;

		private double _start;
		private double _scale;

		public double Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
				Relayout();
			}
		}

		public double Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
				Relayout();
			}
		}

		public TimelineInput Input
		{
			get
			{
				return _input;
			}
		}

		public event EventHandler Relayouting;

		public ViewportController(TimelineInput input)
		{
			_input = input;
			Scale = 1;
			Start = 0;
		}

		internal double Interpolate(Record record, double viewportWidth, double lastElementWidth)
		{
			viewportWidth = viewportWidth - lastElementWidth;
			var step = viewportWidth / (_input.Max.Ticks - _input.Min.Ticks);
			var startPos = viewportWidth * Start;
			return (step * (record.Date.Ticks - _input.Min.Ticks) - startPos) * Scale;
		}

		internal AggregatedPositions AggregatePosisions(double viewportWidth)
		{
			var hash = new Dictionary<int, int>();
			var step = viewportWidth / (Input.Max.Ticks - Input.Min.Ticks);
			var max = 0;

			foreach (var item in Input.Items)
			{
				foreach (var record in item.Records)
				{
					var position = (int)((record.Date.Ticks - Input.Min.Ticks) * step + step);
					int count;
					if (hash.TryGetValue(position, out count))
					{
						count++;
					}
					else
					{
						count = 1;
					}
					hash[position] = count;
					if (count > max)
					{
						max = count;
					}
				}
			}
			return new AggregatedPositions
				{
					Apex = max,
					Aggregations = hash
				};
		}

		internal Dictionary<int, DateTime> AggregateLabels(double viewportWidth)
		{
			var hash = new Dictionary<int, DateTime>();
			var step = viewportWidth / (Input.Max.Ticks - Input.Min.Ticks);
			foreach (var item in Input.Items)
			{
				foreach (var record in item.Records)
				{
					var position = (int)((record.Date.Ticks - Input.Min.Ticks) * step + step);
					hash[position] = record.Date;
				}
			}
			return hash;
		}

		internal void Relayout()
		{
			if (Relayouting != null)
			{
				Relayouting(this, new EventArgs());
			}
		}
	}
}
