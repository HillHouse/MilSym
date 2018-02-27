// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Matrix33.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   A useful 3x3 matrix class for transforming a set of three points to and from a map.
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
#if WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
#endif

    /// <summary>
    /// A useful 3x3 matrix class for transforming a set of three points to and from a map.
    /// </summary>
    public class Matrix33
    {
        /// <summary>
        /// Private member for the third element in the first row.
        /// </summary>
        private double m13 = 1.0;

        /// <summary>
        /// Private member for the third element in the second row.
        /// </summary>
        private double m23 = 1.0;

        /// <summary>
        /// Private member for the third element in the third row.
        /// </summary>
        private double m33 = 1.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix33"/> class.
        /// </summary>
        public Matrix33()
        {
            this.M11 = 1.0;  // these are the only non-zero members
            this.M22 = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix33"/> class.
        /// </summary>
        /// <param name="pts">
        /// The three map locations in the collection correspond to the three rows in the matrix.
        /// </param>
        public Matrix33(IEnumerable<ILocation> pts) 
            : this(pts.ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix33"/> class.
        /// </summary>
        /// <param name="pts">
        /// The three points in the collection correspond to the three rows in the matrix.
        /// </param>
        public Matrix33(IEnumerable<Point> pts)
            : this(pts.ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix33"/> class.
        /// </summary>
        /// <param name="pts">
        /// The three map locations in the list correspond to the three rows in the matrix.
        /// </param>
        public Matrix33(IList<ILocation> pts)
        {
            if (pts == null || pts.Count < 3)
            {
                return;
            }

            this.M11 = pts[0].Longitude;
            this.M12 = pts[0].Latitude;
            this.M21 = pts[1].Longitude;
            this.M22 = pts[1].Latitude;
            this.M31 = pts[2].Longitude;
            this.M32 = pts[2].Latitude;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix33"/> class.
        /// </summary>
        /// <param name="pts">
        /// The three points in the list correspond to the three rows in the matrix.
        /// </param>
        public Matrix33(IList<Point> pts)
        {
            if (pts == null || pts.Count < 3)
            {
                return;
            }

            this.M11 = pts[0].X;
            this.M12 = pts[0].Y;
            this.M21 = pts[1].X;
            this.M22 = pts[1].Y;
            this.M31 = pts[2].X;
            this.M32 = pts[2].Y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix33"/> class.
        /// </summary>
        /// <param name="m11">
        /// The M11 member.
        /// </param>
        /// <param name="m12">
        /// The M12 member.
        /// </param>
        /// <param name="m21">
        /// The M21 member.
        /// </param>
        /// <param name="m22">
        /// The M22 member.
        /// </param>
        /// <param name="m31">
        /// The M30 member.
        /// </param>
        /// <param name="m32">
        /// The M32 member.
        /// </param>
        public Matrix33(double m11, double m12, double m21, double m22, double m31, double m32)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M21 = m21;
            this.M22 = m22;
            this.M31 = m31;
            this.M32 = m32;
        }

        /// <summary>
        /// Gets or sets the MilSymFactory, mostly for the ability to generate ILocation.
        /// </summary>
        public static IMilSymFactory MilSymFactory { private get; set; }

        /// <summary>
        /// Gets or sets M11, the first element in the first row.
        /// </summary>
        public double M11 { get; set; }

        /// <summary>
        /// Gets or sets M12, the second element in the first row.
        /// </summary>
        public double M12 { get; set; }
        
        /// <summary>
        /// Gets or sets M21, the first element in the second row.
        /// </summary>
        public double M21 { get; set; }
        
        /// <summary>
        /// Gets or sets M22, the second element in the second row.
        /// </summary>
        public double M22 { get; set; }
        
        /// <summary>
        /// Gets or sets M31, the first element in the third row.
        /// </summary>
        public double M31 { get; set; }
        
        /// <summary>
        /// Gets or sets M32, the second element in the third row.
        /// </summary>
        public double M32 { get; set; }

        /// <summary>
        /// Computes the determinant of the matrix to support the inverse computation.
        /// </summary>
        /// <returns>
        /// The value of the determinant.
        /// </returns>
        public double Determinant()
        {
            return (this.M11 * (this.M22 - this.M32))  
                 - (this.M21 * (this.M12 - this.M32))  
                 + (this.M31 * (this.M12 - this.M22));
        }

        /// <summary>
        /// Returns the inverse of the matrix.
        /// </summary>
        /// <returns>
        /// The inverse of the matrix.
        /// </returns>
        public Matrix33 Inverse()
        {
            double d = this.Determinant();
            var inv = new Matrix33
            {
                M11 = (this.M22 - this.M32) / d,
                M21 = (this.M31 - this.M21) / d,
                M31 = ((this.M21 * this.M32) - (this.M22 * this.M31)) / d,
                M12 = (this.M32 - this.M12) / d,
                M22 = (this.M11 - this.M31) / d,
                M32 = ((this.M31 * this.M12) - (this.M11 * this.M32)) / d,
                m13 = (this.M12 - this.M22) / d,
                m23 = (this.M21 - this.M11) / d,
                m33 = ((this.M11 * this.M22) - (this.M12 * this.M21)) / d
            };
            return inv;
        }

        /// <summary>
        /// Computes the product of two matrices.
        /// </summary>
        /// <param name="n">
        /// The matrix to multiply against - this x n.
        /// </param>
        /// <returns>
        /// The product of the two matrices.
        /// </returns>
        public Matrix33 Product(Matrix33 n)
        {
            // Don't bother with the _m entries since they're not being used.
            // Better not concatenate anything though.
            var p = new Matrix33
            {
                M11 = (this.M11 * n.M11) + (this.M12 * n.M21) + (this.m13 * n.M31),
                M12 = (this.M11 * n.M12) + (this.M12 * n.M22) + (this.m13 * n.M32),
                M21 = (this.M21 * n.M11) + (this.M22 * n.M21) + (this.m23 * n.M31),
                M22 = (this.M21 * n.M12) + (this.M22 * n.M22) + (this.m23 * n.M32),
                M31 = (this.M31 * n.M11) + (this.M32 * n.M21) + (this.m33 * n.M31),
                M32 = (this.M31 * n.M12) + (this.M32 * n.M22) + (this.m33 * n.M32)
            };
            return p;
        }

        /// <summary>
        /// Transforms the ILocation according to the matrix.
        /// </summary>
        /// <param name="loc">
        /// The location to be transformed.
        /// </param>
        /// <returns>
        /// The transformed location.
        /// </returns>
        public ILocation Transform(ILocation loc)
        {
            var x = (loc.Longitude * this.M11) + (loc.Latitude * this.M21) + this.M31;
            var y = (loc.Longitude * this.M12) + (loc.Latitude * this.M22) + this.M32;
            if (MilSymFactory == null)
            {
                return null;
            }

            return MilSymFactory.Location(Order.LatLon, y, x);
        }

        /// <summary>
        /// Transforms the list of Points to ILocations according to the matrix.
        /// </summary>
        /// <param name="pc">
        /// The list of Points to be transformed.
        /// </param>
        /// <returns>
        /// The transformed Points as ILocations.
        /// </returns>
        public IList<ILocation> Transform(IList<Point> pc)
        {
            if (MilSymFactory == null)
            {
                return null;
            }

            return
                pc.Select(
                    p =>
                    MilSymFactory.Location(
                        Order.LatLon,
                        (p.X * this.M12) + (p.Y * this.M22) + this.M32,
                        (p.X * this.M11) + (p.Y * this.M21) + this.M31)).ToList();
        }

        /// <summary>
        /// Transforms the collection of Points to ILocations according to the matrix.
        /// </summary>
        /// <param name="pc">
        /// The collection of Points to be transformed.
        /// </param>
        /// <returns>
        /// The transformed Points as ILocations.
        /// </returns>
        public IList<ILocation> Transform(PointCollection pc)
        {
            return
                pc.Select(
                    p =>
                    MilSymFactory.Location(
                        Order.LatLon,
                        (p.X * this.M12) + (p.Y * this.M22) + this.M32,
                        (p.X * this.M11) + (p.Y * this.M21) + this.M31)).ToList();
        }

        /// <summary>
        /// Override ToString for a better format.
        /// </summary>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(
                "{0} {1} {2}\n{3} {4} {5}\n{6} {7} {8}",
                this.M11,
                this.M12,
                this.m13,
                this.M21,
                this.M22,
                this.m23,
                this.M31,
                this.M32,
                this.m33);
            return sb.ToString();
        }
    }
}