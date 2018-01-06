// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="PointHelper.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Methods to simplify Point-based computations.
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
    using System.Windows;

    /// <summary>
    /// Methods to simplify Point-based computations.
    /// </summary>
    public class PointHelper
    {
        /// <summary>
        /// Computes the sum of a and b.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The sum: a + b.</returns>
        public static Point Add(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Computes the difference between a and b (a - b).
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The difference: a - b.</returns>
        public static Point Minus(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// Computes the point midway between a and b.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The average: (a + b) / 2.</returns>
        public static Point MidPoint(Point a, Point b)
        {
            return new Point((a.X + b.X) / 2.0, (a.Y + b.Y) / 2.0);
        }

        /// <summary>
        /// Computes a normal pointing to the right of the vector ab. 
        /// </summary>
        /// <param name="a">
        /// The first point.
        /// </param>
        /// <param name="b">
        /// The second point.
        /// </param>
        /// <returns>
        /// The normal pointing to the right of the vector ab.
        /// </returns>
        public static Point RightNormal(Point a, Point b)
        {
            return new Point(b.Y - a.Y, a.X - b.X);
        }

        /// <summary>
        /// Computes a normal pointing to the left of the vector ab. 
        /// </summary>
        /// <param name="a">
        /// The first point.
        /// </param>
        /// <param name="b">
        /// The second point.
        /// </param>
        /// <returns>
        /// The normal pointing to the left of the vector ab.
        /// </returns>
        public static Point LeftNormal(Point a, Point b)
        {
            return new Point(a.Y - b.Y, b.X - a.X);
        }

        /// <summary>
        /// Computes a unit normal pointing to the right of the vector ab. 
        /// </summary>
        /// <param name="a">
        /// The first point.
        /// </param>
        /// <param name="b">
        /// The second point.
        /// </param>
        /// <returns>
        /// The unit normal pointing to the right of the vector ab.
        /// </returns>
        public static Point RightUnitNormal(Point a, Point b)
        {
            double m = Magnitude(a, b);
            return new Point((b.Y - a.Y) / m, (a.X - b.X) / m);
        }

        /// <summary>
        /// Computes a unit normal pointing to the left of the vector ab. 
        /// </summary>
        /// <param name="a">
        /// The first point.
        /// </param>
        /// <param name="b">
        /// The second point.
        /// </param>
        /// <returns>
        /// The unit normal pointing to the left of the vector ab.
        /// </returns>
        public static Point LeftUnitNormal(Point a, Point b)
        {
            double m = Magnitude(a, b);
            return new Point((a.Y - b.Y) / m, (b.X - a.X) / m);
        }

        /// <summary>
        /// Computes the difference of two points as a vector.
        /// </summary>
        /// <param name="a">
        /// The first point.
        /// </param>
        /// <param name="b">
        /// The second point.
        /// </param>
        /// <returns>
        /// The difference between a and b, b - a, as a Point vector.
        /// </returns>
        public static Point Vector(Point a, Point b)
        {
            return new Point(b.X - a.X, b.Y - a.Y);
        }

        /// <summary>
        /// Computes the unit vector in the direction of the Point.
        /// </summary>
        /// <param name="a">
        /// The point for which to create the unit vector.
        /// </param>
        /// <returns>
        /// The unit vector in the direction of Point a.
        /// </returns>
        public static Point UnitVector(Point a)
        {
            double mag = Math.Sqrt((a.X * a.X) + (a.Y * a.Y));
            return new Point(a.X / mag, a.Y / mag);
        }

        /// <summary>
        /// Determines whether a point is inside a polygon or not. 
        /// See http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm#wn_PinPolygon.
        /// </summary>
        /// <param name="p">
        /// The point to test against the polygon.
        /// </param>
        /// <param name="count">
        /// The number of points in the polygon.
        /// </param>
        /// <param name="pts">
        /// The points in the polygon.
        /// </param>
        /// <returns>
        /// A boolean value that is true if the point is inside the polygon.
        /// </returns>
        public static bool PointInPolygon(Point p, int count, IList<Point> pts)
        {
            int wn = 0;    // the winding number counter

            // Loop through all edges of the polygon
            for (int i = 0; i < count; i++)
            {   
                // edge from pts[i] to pts[i+1]
                if (pts[i].Y <= p.Y)
                {
                    // start Y <= p.Y
                    if (pts[i + 1].Y > p.Y)
                    {
                        // an upward crossing
                        if (IsLeft(pts[i], pts[i + 1], p) > 0.0)
                        {
                            ++wn; // p is left of edge, so have a valid up intersect
                        }
                    }
                }
                else
                {
                    // start Y > p.Y (no test needed)
                    if (pts[i + 1].Y <= p.Y)
                    {
                        // a downward crossing
                        if (IsLeft(pts[i], pts[i + 1], p) < 0.0)
                        { 
                            --wn; // p is right of edge, so have a valid down intersect
                        }
                    }
                }
            }

            return wn != 0;
        }

        /// <summary>
        /// Normalize the vector that starts at a and runs to b.
        /// </summary>
        /// <param name="a">The start of the vector.</param>
        /// <param name="b">The end of the vector.</param>
        /// <returns>the unit vector running from a to b.</returns>
        public static Point UnitVector(Point a, Point b)
        {
            var x = b.X - a.X;
            var y = b.Y - a.Y;
            var mag = Math.Sqrt((x * x) + (y * y));
            return new Point(x / mag, y / mag);
        }

        /// <summary>
        /// Returns the dot product of Points 'a' and 'b'.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>Returns a.X * b.X + a.Y * b.Y.</returns>
        public static double DotProduct(Point a, Point b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        /// <summary>
        /// Computes the new Point p = a + c * b
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="c">The scale factor for the second point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>Returns the linear combination a + c * b..</returns>
        public static Point LinearCombination(Point a, double c, Point b)
        {
            return new Point(a.X + (c * b.X), a.Y + (c * b.Y));
        }

        /// <summary>
        /// Computes the distance between Points 'a' and 'b'. Order is unimportant.
        /// </summary>
        /// <param name="a">The first point</param>
        /// <param name="b">The second point</param>
        /// <returns>Returns the 2D Cartesian distance between two points.</returns>
        public static double Magnitude(Point a, Point b)
        {
            return Math.Sqrt(((b.X - a.X) * (b.X - a.X)) + ((b.Y - a.Y) * (b.Y - a.Y)));
        }

        /// <summary>
        /// Determines whether p2 is to the left (+) or right (-) of the vector from p0 to p1.
        /// </summary>
        /// <param name="p0">
        /// The first point of the vector.
        /// </param>
        /// <param name="p1">
        /// The second point of the vector.
        /// </param>
        /// <param name="p2">
        /// The point to check to see if it is to the left or right of the vector.
        /// </param>
        /// <returns>
        /// A double whose sign determines whether the point is to the left (+) or right (-) of the vector.
        /// </returns>
        private static double IsLeft(Point p0, Point p1, Point p2)
        {
            return ((p1.X - p0.X) * (p2.Y - p0.Y)) - ((p2.X - p0.X) * (p1.Y - p0.Y));
        }
    }
}
