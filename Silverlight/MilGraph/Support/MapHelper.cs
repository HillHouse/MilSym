// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MapHelper.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Helper functions to assist in map-based computations.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilGraph.Support
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Helper functions to assist in map-based computations.
    /// </summary>
    public class MapHelper
    {
        /// <summary>
        /// Earth's radius in kilometers as determined by Microsoft to support their map backgrounds.
        /// </summary>
        private const double EarthRadius = 6367; // radius in km

        /// <summary>
        /// Minimum valid latitude for Bing map backgrounds
        /// </summary>
        private const double MinLatitude = -85.05112878;

        /// <summary>
        /// Maximum valid latitude for Bing map backgrounds
        /// </summary>
        private const double MaxLatitude = 85.05112878;

        /// <summary>
        /// Minimum valid longitude for Bing map backgrounds
        /// </summary>
        private const double MinLongitude = -180;

        /// <summary>
        /// Maximum valid longitude for Bing map backgrounds
        /// </summary>
        private const double MaxLongitude = 180;

        /// <summary>
        /// Gets or sets the particular instance of an IMilSymFactory.
        /// </summary>
        public static IMilSymFactory MilSymFactory { get; set; }

        /// <summary>
        /// Computes the distance between two Point coordinates.
        /// </summary>
        /// <param name="p1">
        /// The first point.
        /// </param>
        /// <param name="p2">
        /// The second point.
        /// </param>
        /// <returns>
        /// The distance between the two Point coordinates.
        /// </returns>
        public static double Hypoteneuse(Point p1, Point p2)
        {
            return Math.Sqrt(((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y)));
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="x">
        /// The degree value to be converted.
        /// </param>
        /// <returns>
        /// The input value converted from degrees to radians.
        /// </returns>
        public static double DegToRad(double x)
        {
            return x * Math.PI / 180;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="x">
        /// The radian value to be converted.
        /// </param>
        /// <returns>
        /// The input value converted from radians to degrees.
        /// </returns>
        public static double RadToDeg(double x)
        {
            return x * 180 / Math.PI;
        }        
        
        /// <summary>
        /// Computes a new map location that is a given arc length (range) and bearing from another map location.
        /// </summary>
        /// <param name="origin">
        /// The starting map location for the computation.
        /// </param>
        /// <param name="bearing">
        /// The bearing (measured clockwise in degrees from true north) angle.
        /// </param>
        /// <param name="arcLength">
        /// The arc length (measured in kilometers).
        /// </param>
        /// <returns>
        /// The new map location at the given range and bearing from the original map location.
        /// </returns>
        public static ILocation CalculateLocation(ILocation origin, double bearing, double arcLength)
        {
            var lat1 = DegToRad(origin.Latitude);
            var lon1 = DegToRad(origin.Longitude);
            var centralAngle = arcLength / EarthRadius;
            var lat2 = Math.Asin((Math.Sin(lat1) * Math.Cos(centralAngle)) + (Math.Cos(lat1) * Math.Sin(centralAngle) * Math.Cos(DegToRad(bearing))));
            var lon2 = lon1 + Math.Atan2(Math.Sin(DegToRad(bearing)) * Math.Sin(centralAngle) * Math.Cos(lat1), Math.Cos(centralAngle) - (Math.Sin(lat1) * Math.Sin(lat2)));
            return MilSymFactory.Location(Order.LatLon, RadToDeg(lat2), RadToDeg(lon2));
        }

        /// <summary>
        /// Computes the map location halfway between two other map locations.
        /// </summary>
        /// <param name="a">
        /// The first map location.
        /// </param>
        /// <param name="b">
        /// The second map location.
        /// </param>
        /// <returns>
        /// The map location halfway between the two provided map locations.
        /// </returns>
        public static ILocation CalculateMidpoint(ILocation a, ILocation b)
        {
            double range = CalculateRange(a, b);
            double bearing = CalculateBearing(a, b);
            return CalculateLocation(a, bearing, range / 2.0);
        }

        /// <summary>
        /// Returns a map location that is 180 degrees from the map location to be mirrored about a center point.
        /// </summary>
        /// <param name="center">
        /// The center map location that will ultimately lie between the mirror map location and its reflection.
        /// </param>
        /// <param name="mirror">
        /// The map location to be reflected (mirrored).
        /// </param>
        /// <returns>
        /// The map location that is mirrored about the center point.
        /// </returns>
        public static ILocation Reflect(ILocation center, ILocation mirror)
        {
            double range = CalculateRange(center, mirror);
            double bearing = CalculateBearing(center, mirror);
            return CalculateLocation(center, bearing + 180, range);
        }

        /// <summary>
        /// Computes a map location that is 1/4 the distance from a first map location towards a second map location.
        /// </summary>
        /// <param name="a">
        /// The first map location.
        /// </param>
        /// <param name="b">
        /// The second map location.
        /// </param>
        /// <returns>
        /// The map location that is 1/4 the distance from a first map location towards a second map location.
        /// </returns>
        public static ILocation CalculateQuarterpoint(ILocation a, ILocation b)
        {
            double range = CalculateRange(a, b);
            double bearing = CalculateBearing(a, b);
            return CalculateLocation(a, bearing, range / 4.0);
        }

        /// <summary>
        /// Given the vectors ab and ac, compute the projection of the vector ac onto a perpendicular to the vector ab. 
        /// </summary>
        /// <param name="a">The starting location for each vector.</param>
        /// <param name="b">The ending location of the first vector.</param>
        /// <param name="c">The ending location of the second vector.</param>
        /// <returns>The ending location of the second vector projected onto a perpendicular to the first vector.</returns>
        public static ILocation CalculatePerpendicular(ILocation a, ILocation b, ILocation c)
        {
            // Compute the angle between ab and ac
            var atobAngle = CalculateBearing(a, b);
            var atocAngle = CalculateBearing(a, c);
            var angle = atocAngle - atobAngle;    // since we're using the cosine, the sign is not important

            var atocRange = CalculateRange(a, c);
            var length = atocRange * Math.Sin(DegToRad(angle));
            var loc = CalculateLocation(a, atobAngle + 90, length);
            return loc;
        }

        /// <summary>
        /// Given the vectors ab and ac, compute the projection of the vector ac onto the vector ab. 
        /// </summary>
        /// <param name="a">The starting location for each vector.</param>
        /// <param name="b">The ending location of the first vector.</param>
        /// <param name="c">The ending location of the second vector.</param>
        /// <returns>The ending location of the second vector projected onto the first vector.</returns>
        public static ILocation CalculateProjection(ILocation a, ILocation b, ILocation c)
        {
            // Compute the angle between ab and ac
            var atobAngle = CalculateBearing(a, b);
            var atocAngle = CalculateBearing(a, c);
            var angle = atocAngle - atobAngle;    // since we're using the cosine, the sign is not important

            // Now we're sort of doing the right triangle thing where the adjacent leg is
            // the cosine of the angle (angle) times the hypotenuse (acRange)
            var atocRange = CalculateRange(a, c);
            var length = atocRange * Math.Cos(DegToRad(angle));

            // Now find the final point by moving by this length along the first line
            var loc = CalculateLocation(a, atobAngle, length);
            return loc;
        }

        /// <summary>
        /// Normalize the vector that starts at a and runs to b.
        /// </summary>
        /// <param name="a">The start of the vector</param>
        /// <param name="b">The end of the vector</param>
        /// <returns>The unit vector running from a to b</returns>
        public static Point Normalize(Point a, Point b)
        {
            var x = b.X - a.X;
            var y = b.Y - a.Y;
            var mag = Math.Sqrt((x * x) + (y * y));
            return new Point(x / mag, y / mag);
        }

        /// <summary>
        /// Computes the projection of ac onto the vector ab. It does this by
        /// computing the unit vector from a to b, computing the dot product of
        /// this vector with the vector from a to c, and scaling the unit
        /// vector from a to b by the dot product, and adding that vector to a.
        /// </summary>
        /// <param name="a">The starting point for the vectors ab and ac</param>
        /// <param name="b">The end point of the vector ab</param>
        /// <param name="c">The end point of the vector ac</param>
        /// <returns>
        /// The point is the projection of ac onto the vector ab.
        /// </returns>
        public static Point CalculateProjection(Point a, Point b, Point c)
        {
            Point p = Normalize(a, b);
            var q = new Point(c.X - a.X, c.Y - a.Y);
            var dot = (p.X * q.X) + (p.Y * q.Y);
            return new Point(a.X + (dot * p.X), a.Y + (dot * p.Y));
        }

        /// <summary>
        /// Compute the angle between vector ab and vector ac in degrees
        /// </summary>
        /// <param name="a">The starting point for the two vectors</param>
        /// <param name="b">The end point for the first vector</param>
        /// <param name="c">The end point for the second vector</param>
        /// <returns>The value of the angle in degrees</returns>
        public static double CalculateAngle(Point a, Point b, Point c)
        {
            Point p = Normalize(a, b);
            Point q = Normalize(a, c);
            var dot = (p.X * q.X) + (p.Y * q.Y);
            var cross = (p.X * q.Y) - (q.X * p.Y);
            return RadToDeg(Math.Atan2(dot, cross));
        }

        /// <summary>
        /// Compute the bearing angle in degrees 
        /// </summary>
        /// <param name="a">The start of the map vector</param>
        /// <param name="b">The end of the map vector</param>
        /// <returns>The bearing angle in degrees</returns>
        public static double CalculateBearing(ILocation a, ILocation b)
        {
            var lat1 = DegToRad(a.Latitude);
            var lon1 = a.Longitude;
            var lat2 = DegToRad(b.Latitude);
            var lon2 = b.Longitude;
            var deltaLon = DegToRad(lon2 - lon1);
            var y = Math.Sin(deltaLon) * Math.Cos(lat2);
            var x = (Math.Cos(lat1) * Math.Sin(lat2)) - (Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon));
            var bearing = (RadToDeg(Math.Atan2(y, x)) + 360.0) % 360.0;
            return bearing;
        }

        /// <summary>
        /// Computes the haversine distance between two map locations.
        /// </summary>
        /// <param name="latlong1">
        /// The first map location.
        /// </param>
        /// <param name="latlong2">
        /// The second map location.
        /// </param>
        /// <returns>
        /// The haversine distance between the two map locations.
        /// </returns>
        public static double CalculateRange(ILocation latlong1, ILocation latlong2)        // haversine distance
        {
            var lat1 = DegToRad(latlong1.Latitude);
            var lon1 = DegToRad(latlong1.Longitude);
            var lat2 = DegToRad(latlong2.Latitude);
            var lon2 = DegToRad(latlong2.Longitude);

            var deltaLat = lat2 - lat1;
            var deltaLon = lon2 - lon1;
            var cordLength = Math.Pow(Math.Sin(deltaLat / 2), 2) +
                (Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(deltaLon / 2), 2));
            var centralAngle = 2 * Math.Atan2(Math.Sqrt(cordLength), Math.Sqrt(1 - cordLength));
            return EarthRadius * centralAngle;
        }

        /// <summary>
        /// Generate a transform that transforms points in pc space to locations in map space.
        /// </summary>
        /// <param name="fromPoints">
        /// The "from" points, typically values from a ResourceDictionary entry.
        /// </param>
        /// <param name="toAnchors">
        /// The "to" anchors, typically map locations.
        /// </param>
        /// <returns>
        /// The 3x3 matrix that represents a transform from the points to the anchors.
        /// </returns>
        public static Matrix33 ComputeTransform(IList<Point> fromPoints, IList<ILocation> toAnchors)
        {
            if (toAnchors == null || fromPoints == null)
            {
                return null;
            }

            var anchorMatrix = new Matrix33(toAnchors);
            var pointMatrix = new Matrix33(fromPoints);
            return pointMatrix.Inverse().Product(anchorMatrix);
        }

        /// <summary>
        /// Generate a transform that transforms points in a space.
        /// </summary>
        /// <param name="fromPoints">
        /// The "from" points.
        /// </param>
        /// <param name="toPixels">
        /// The "to" points.
        /// </param>
        /// <returns>
        /// The 3x3 matrix that represents a transform from the points to the pixels.
        /// </returns>
        public static Matrix33 ComputeTransform(IList<Point> fromPoints, IList<Point> toPixels)
        {
            if (toPixels == null || fromPoints == null)
            {
                return null;
            }

            var pixelMatrix = new Matrix33(toPixels);
            var pointMatrix = new Matrix33(fromPoints);
            return pointMatrix.Inverse().Product(pixelMatrix);
        }

        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level of detail.
        /// </summary>
        /// <param name="levelOfDetail">
        /// Level of detail, from 1 (lowest detail) to 23 (highest detail).
        /// </param>
        /// <returns>
        /// The map width and height in pixels.
        /// </returns>
        public static double MapSize(double levelOfDetail)
        {
            return 256.0 * Math.Pow(2, levelOfDetail);
        }

        /// <summary>
        /// Converts a pixel coordinate into a latitude/longitude coordinate pair for the given level of detail (zoom level)
        /// </summary>
        /// <param name="p">
        /// The pixel coordinate.
        /// </param>
        /// <param name="levelOfDetail">
        /// The level of detail (zoom level).
        /// </param>
        /// <returns>
        /// The ILocation object representing the conversion from pixel space to map coordinate space.
        /// </returns>
        public static ILocation PixelToLatLon(Point p, double levelOfDetail)
        {
            double mapSize = MapSize(levelOfDetail);
            var x = p.X / mapSize;
            var y = p.Y / mapSize;
            var lon = (x * 360.0) - 180.0;
            var sinLat = Math.Exp((0.5 - y) * (4 * Math.PI));
            sinLat = (sinLat - 1.0) / (sinLat + 1.0);
            var lat = Math.Asin(sinLat) * 180.0 / Math.PI;
            return MilSymFactory.Location(Order.LatLon, lat, lon);
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="location">
        /// The Location, in degrees.
        /// </param>
        /// <param name="levelOfDetail">
        /// Level of detail (zoom level), from 1 (lowest detail) to 23 (highest detail).
        /// </param>
        /// <returns>
        /// A Point representing the conversion of the map location to pixels.
        /// </returns>
        public static Point LatLonToPixel(ILocation location, double levelOfDetail)
        {
            var latitude = Clip(location.Latitude, MinLatitude, MaxLatitude);
            var longitude = Clip(location.Longitude, MinLongitude, MaxLongitude);

            double x = (longitude + 180) / 360;
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);
            double y = 0.5 - (Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI));

            double mapSize = MapSize(levelOfDetail);
            return new Point(
                Clip(x * mapSize, 0, mapSize - 1),
                Clip(y * mapSize, 0, mapSize - 1));
        }

        /// <summary>
        /// Converts latitude/longitude pairs to pixels for a given map level of detail.
        /// </summary>
        /// <param name="locations">
        /// The map locations.
        /// </param>
        /// <param name="levelOfDetail">
        /// The map's level of detail (zoom level).
        /// </param>
        /// <returns>
        /// A List of Point objects representing the conversion of the map locations to pixels.
        /// </returns>
        public static IList<Point> LatLonsToPixels(IList<ILocation> locations, double levelOfDetail)
        {
            if (locations == null || locations.Count == 0)
            {
                return null;
            }

            var points = new List<Point>(locations.Count);
            points.AddRange(from location in locations
                            let latitude = Clip(location.Latitude, MinLatitude, MaxLatitude)
                            let longitude = Clip(location.Longitude, MinLongitude, MaxLongitude)
                            let x = (longitude + 180) / 360
                            let sinLatitude = Math.Sin(latitude * Math.PI / 180)
                            let y = 0.5 - (Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI))
                            let mapSize = MapSize(levelOfDetail)
                            select new Point(Clip(x * mapSize, 0, mapSize - 1), Clip(y * mapSize, 0, mapSize - 1)));
            return points;
        }

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }
    }
}
