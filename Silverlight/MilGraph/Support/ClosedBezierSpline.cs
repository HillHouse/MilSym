// <copyright file="ClosedBezierSpline.cs" company="Oleg V. Polikarpotchkin">
// Copyright © 2009 Oleg V. Polikarpotchkin. All Right Reserved
// </copyright>
// <author>Oleg V. Polikarpotchkin</author>
// <email>ov-p@yandex.ru</email>
// <date>2009-02-28</date>
// <summary>Method to calculate Closed Bezier Spline Control Points.</summary>

namespace MilSym.MilGraph.Support
{
    using System.Collections.Generic;
    using System.Windows;
    using NumericalRecipes;

    /// <summary>
    /// Closed Bezier Spline Control Points calculation.
    /// </summary>
    public static class ClosedBezierSpline
    {
        /// <summary>
        /// Gets or sets the MilSymFactory.
        /// </summary>
        public static IMilSymFactory MilSymFactory { get; set; }

        /// <summary>
        /// Get Closed Bezier Spline Control Points.
        /// </summary>
        /// <param name="knots">Input Knot Bezier spline points.</param>
        /// <param name="firstControlPoints">Output First Control points array of the same 
        /// length as the <paramref name="knots"/> array.</param>
        /// <param name="secondControlPoints">Output Second Control points array of of the same
        /// length as the <paramref name="knots"/> array.</param>
        public static void GetCurveControlPoints(
            IList<ILocation> knots, 
            out ILocationCollection firstControlPoints, 
            out ILocationCollection secondControlPoints)
        {
            firstControlPoints = MilSymFactory.LocationCollection();
            secondControlPoints = MilSymFactory.LocationCollection();

            int n = knots.Count;
            if (n <= 2)
            {
                return;
            }

            // Calculate first Bezier control points

            // The matrix.
            var a = new double[n];
            var b = new double[n];
            var c = new double[n];
            for (int i = 0; i < n; ++i)
            {
                a[i] = 1;
                b[i] = 4;
                c[i] = 1;
            }

            // Right hand side vector for points X coordinates.
            var rhs = new double[n];
            for (int i = 0; i < n; ++i)
            {
                int j = (i == n - 1) ? 0 : i + 1;
                rhs[i] = (4 * knots[i].Longitude) + (2 * knots[j].Longitude);
            }

            // Solve the system for X.
            double[] x = Cyclic.Solve(a, b, c, 1, 1, rhs);

            // Right hand side vector for points Y coordinates.
            for (int i = 0; i < n; ++i)
            {
                int j = (i == n - 1) ? 0 : i + 1;
                rhs[i] = (4 * knots[i].Latitude) + (2 * knots[j].Latitude);
            }

            // Solve the system for Y.
            double[] y = Cyclic.Solve(a, b, c, 1, 1, rhs);

            // Fill output arrays.
            for (int i = 0; i < n; ++i)
            {
                // First control point.
                firstControlPoints.Add(MilSymFactory.Location(Order.LatLon, y[i], x[i]));

                // Second control point.
                secondControlPoints.Add(
                    MilSymFactory.Location(Order.LatLon, (2 * knots[i].Latitude) - y[i], (2 * knots[i].Longitude) - x[i]));
            }
        }

        /// <summary>
        /// Get the curve control points.
        /// </summary>
        /// <param name="count">
        /// The point count.
        /// </param>
        /// <param name="knots">
        /// The knots.
        /// </param>
        /// <param name="firstControlPoints">
        /// The first set of control points.
        /// </param>
        /// <param name="secondControlPoints">
        /// The second set of control points.
        /// </param>
        public static void GetCurveControlPoints(
            int count,
            IList<Point> knots,
            out IList<Point> firstControlPoints,
            out IList<Point> secondControlPoints)
        {
            firstControlPoints = new List<Point>(count);
            secondControlPoints = new List<Point>(count);

            if (count <= 2)
            {
                return;
            }

            // Calculate first Bezier control points

            // The matrix.
            var a = new double[count];
            var b = new double[count];
            var c = new double[count];
            for (int i = 0; i < count; ++i)
            {
                a[i] = 1;
                b[i] = 4;
                c[i] = 1;
            }

            // Right hand side vector for points X coordinates.
            var rhs = new double[count];
            for (int i = 0; i < count; ++i)
            {
                int j = (i == count - 1) ? 0 : i + 1;
                rhs[i] = (4 * knots[i].X) + (2 * knots[j].X);
            }

            // Solve the system for X.
            double[] x = Cyclic.Solve(a, b, c, 1, 1, rhs);

            // Right hand side vector for points Y coordinates.
            for (int i = 0; i < count; ++i)
            {
                int j = (i == count - 1) ? 0 : i + 1;
                rhs[i] = (4 * knots[i].Y) + (2 * knots[j].Y);
            }

            // Solve the system for Y.
            double[] y = Cyclic.Solve(a, b, c, 1, 1, rhs);

            // Fill output arrays.
            for (int i = 0; i < count; ++i)
            {
                // First control point.
                firstControlPoints.Add(new Point(x[i], y[i]));
                
                // Second control point.
                secondControlPoints.Add(new Point(
                    (2 * knots[i].X) - x[i],
                    (2 * knots[i].Y) - y[i]));
            }
        }
    }
}
