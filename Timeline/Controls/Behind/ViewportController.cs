using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using YL.Timeline.Controls.Behind.Entities;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.Behind
{
	public class ViewportController : INotifyPropertyChanged
	{
		private readonly TimelineInput _input;

		private double _start;
		private double _scale;
		private double _viewportWidth;
		private double _lastElementWidth;
		private AggregatedPositions _aggregatedPositions;

		public double Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
				OnPropertyChanged();
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
				OnPropertyChanged();
			}
		}

		public double ViewportWidth
		{
			get
			{
				return _viewportWidth;
			}
			set
			{
				_viewportWidth = value;
				UpdateAggregatedPositions();
				OnPropertyChanged();
			}
		}

		public double LastElementWidth
		{
			get
			{
				return _lastElementWidth;
			}
			set
			{
				_lastElementWidth = value;
				OnPropertyChanged();
			}
		}

		public TimelineInput Input
		{
			get
			{
				return _input;
			}
		}

		public AggregatedPositions AggregatedPositions
		{
			get
			{
				return _aggregatedPositions;
			}
			set
			{
				_aggregatedPositions = value;
				OnPropertyChanged();
			}
		}

		public ViewportController(TimelineInput input)
		{
			_input = input;
			Scale = 1;
			Start = 0;
			input.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == "Items")
					{
						UpdateAggregatedPositions();
					}
				};
		}

		internal double Interpolate(DateTime date)
		{
			var viewportWidth = ViewportWidth;
			var step = viewportWidth / (_input.Max.Ticks - _input.Min.Ticks);
			return step * (date.Ticks - _input.Min.Ticks);
		}

		internal double Interpolate(DateTime date, double viewportWidth, double lastElementWidth)
		{
			viewportWidth = viewportWidth - lastElementWidth;
			var step = viewportWidth / (_input.Max.Ticks - _input.Min.Ticks);
			var startPos = viewportWidth * Start;
			return (step * (date.Ticks - _input.Min.Ticks) - startPos) * Scale;
		}

		private void UpdateAggregatedPositions()
		{
			var hash = new SortedDictionary<double, int>();
			var step = ViewportWidth / (Input.Max.Ticks - Input.Min.Ticks);
			var max = 0;

			if (Input.Items != null)
			{
				foreach (var item in Input.Items)
				{
					foreach (var record in item.Records)
					{
						var position = (record.Date.Ticks - Input.Min.Ticks) * step;
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
			}

			//Smooth(hash);
			AggregatedPositions = new AggregatedPositions
				{
					Apex = max,
					Aggregations = hash
				};
		}

		private void Smooth(SortedDictionary<double, int> hash)
		{
			var keys = hash.Keys.ToArray();
			for (var i = 0; i < keys.Length - 1; i++)
			{
				var prev = keys[i];
				var next = keys[i+1];
				if (next - prev <= 4)
				{
					var newKey = (prev + next) / 2;
					var newVal = Math.Max(hash[prev], hash[next]);
					hash.Remove(prev);
					hash.Remove(next);
					hash.Add(newKey, newVal);
					i++;
				}
			}
		}

		/*internal Dictionary<int, DateTime> AggregateLabels(double viewportWidth)
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
		}*/

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
