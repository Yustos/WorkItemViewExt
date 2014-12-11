using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.Fields.Ornament
{
	public class SelectionLinkAdorner : Adorner
	{
		private readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromArgb(80, 0, 200, 0)), 2);

		private readonly UIElement _from;
		private readonly UIElement _to;


		public static readonly DependencyProperty InvalidateIsVisibleProperty = DependencyProperty.Register(
			"InvalidateIsVisible",
			typeof(bool),
			typeof(SelectionLinkAdorner),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
		);

		public bool InvalidateIsVisible
		{
			get
			{
				return (bool)GetValue(InvalidateIsVisibleProperty);
			}
			set
			{
				SetValue(InvalidateIsVisibleProperty, value);
			}
		}


		public SelectionLinkAdorner(UIElement adornedElement, UIElement from, UIElement to)
			: base(adornedElement)
		{
			_from = from;
			_to = to;
			SetBinding(InvalidateIsVisibleProperty, new Binding("IsVisible") { Source = _from });
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			InvalidateVisual();
			return base.ArrangeOverride(finalSize);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var from = _from.IsVisible
				? _from.TranslatePoint(new Point(_from.DesiredSize.Width / 2, _from.DesiredSize.Height), this)
				: ((FrameworkElement)((FrameworkElement)_from).Parent).TranslatePoint(new Point(0, 0), this);
			_pen.DashStyle = _from.IsVisible ? DashStyles.Solid : DashStyles.Dot;

			var to = _to.TranslatePoint(new Point(_to.DesiredSize.Width / 2, 0), this);
			
			drawingContext.DrawLine(_pen, from, to);
		}
	}
}
