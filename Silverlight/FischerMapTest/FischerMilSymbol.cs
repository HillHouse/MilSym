using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;

namespace FischerMapTest
{
	public partial class MapMilSymbol
	{
		private LocationRect _locationRect;
		private MapBase _mapBase;
		private double _saveZoom;

		public void SymbolLayoutUpdated(object sender, EventArgs e)
		{
			// Find the base map and save it for future reference
			if (_mapBase == null)
			{
				var fe = this as FrameworkElement;
				do
				{
					fe = fe.Parent as FrameworkElement;
					if (fe == null) return;
				} while (!(fe is MapLayer));
				var ml = fe as MapLayer;
				_mapBase = ml.ParentMap;	// need the _mapBase for the zoom factor
				if (_mapBase == null)
					return;

				// Need the location to avoid recomputing the size unnecessarily
				var loc = GetValue(MapLayer.PositionProperty) as Location;
				_locationRect = new LocationRect(loc, 1.0, 1.0);
			}

			// If we're not rendering don't bother
			if (!_mapBase.BoundingRectangle.Intersects(_locationRect))
				return;

			// If zoom didn't change, don't bother
			double zoom = (_mapBase.ZoomLevel);
			if (_saveZoom == zoom)	// don't recompute if we have the same zoom value
				return;
			_saveZoom = zoom;

			// Get the scale transform the first time, shouldn't actually be necessary 
			if (_scaleTransform == null)
			{
				TransformGroup tg = null;
				FindScaleTransformIndex(ref tg);
			}

			// Any old scale scheme will work here
			if (_scaleTransform != null)
				_scaleTransform.ScaleX =
				_scaleTransform.ScaleY = Scale * Math.Pow(zoom, 0.75) / 5.0;
		}
	}
}