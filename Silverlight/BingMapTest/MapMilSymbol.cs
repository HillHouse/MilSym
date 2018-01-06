using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MilSym;

namespace BingMapTest
{
	public partial class MapMilSymbol : MilSymbol
	{
		private ScaleTransform _scaleTransform;

		public MapMilSymbol(string symbolCode,
			double scale = 0.14,
			string labels = "",
			double opacity = 0.7) :
			base(symbolCode, scale: scale, labelString: labels)
		{
			Opacity = opacity;
			LayoutUpdated += SymbolLayoutUpdated;
		}

		// Find the first scale transform index, if there aren't any, return 0
		private int FindScaleTransformIndex(ref TransformGroup tg)
		{
			// Get the RenderTransform and it there isn't one, create one
			tg = RenderTransform as TransformGroup;
			if (tg == null)
			{
				tg = new TransformGroup();
				_scaleTransform = new ScaleTransform();
				tg.Children.Add(_scaleTransform);
				RenderTransform = tg;
				return 0;
			}

			// Find the ScaleTransform index
			for (var i = 0; i < tg.Children.Count; i++)
				if (tg.Children[i] is ScaleTransform)
				{
					_scaleTransform = tg.Children[i] as ScaleTransform;
					return i;
				}
			return 0;
		}

		// Wrap the symbol with a bounding box that points to its location
		public void Wrap()
		{
			var l = Bounds.Left;
			var t = Bounds.Top;
			var h = Bounds.Height;
			var w = Bounds.Width;

			const double offset = 40.0;
			const double b = 10.0;

			TransformGroup tg = null;
			int i = FindScaleTransformIndex(ref tg);
			tg.Children.Insert(i, new TranslateTransform { X = 0.0, Y = -(t + h + offset) });

			// Define and add the polygonal wrapper
			var box = new Polygon
			{
				Points = new PointCollection
	         	{
	         		new Point(l - b, t + h + b),
	         		new Point(-b, t + h + b),
	         		new Point(0, t + h + offset),
	         		new Point(b, t + h + b),
	         		new Point(l + w + b, t + h + b),
	         		new Point(l + w + b, t - b),
	         		new Point(l - b, t - b)
	         	},
				Stroke = new SolidColorBrush(Colors.Red),
				StrokeThickness = 10.0,
				StrokeMiterLimit = 2.0,
				Fill = new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff))
			};
			Children.Insert(0, box);
		}
	}
}
