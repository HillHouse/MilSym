// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="WindBarb.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for generating the wind barbs defined in MIL STD-2525C
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilSymbol
{
    using System;
    using System.Collections.Generic;
    using System.Text;
#if WINDOWS_UWP
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Shapes;
#else
    using System.Windows.Markup;
    using System.Windows.Shapes;
#endif

    /// <summary>
    /// Generates the wind barbs for weather symbology
    /// </summary>
    internal static class WindBarb
    {
        /// <summary>
        /// The header for parsing some XAML code strings.
        /// </summary>
        private const string XmlnsOpen =
            "' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >";

        /// <summary>
        /// A list of XAML strings that define cloud cover
        /// </summary>
        private static readonly string[] Clouds = new[]
        {
            // 0 - nothing
            string.Empty,
            
            // 1 - vertical line 
             "<Path Stroke='Black' StrokeThickness='5' Data='M0,-25 0,25",
            
             // 2 - one quarter 
             "<Path Fill='Black' Data='M0,0 L0,-22 A22,22 90 0 1 22,0Z",
             
             // 3 - one quarter plus vertical line 
             "<Path Fill='Black' Stroke='Black' StrokeThickness='5' Data='M2.5,0 L2.5,-20 A20,20 90 0 1 20,0Z M0,-25 0,25",
            
             // 4 - half 
             "<Path Fill='Black' Data='M0,0 L0,-22 A22,22 180 1 1 0,22Z",
            
             // 5 - half + horizontal line 
             "<Path Fill='Black' Stroke='Black' StrokeThickness='5' Data='M2.5,0 L2.5,-20 A20,20 180 1 1 2.5,20Z M0,0 -25,0",
            
             // 6 - three quarter 
             "<Path Fill='Black' Data='M0,0 L0,-22 A22,22 270 1 1 -22,0Z",
            
             // 7 - filled black + vertical white line 
             "<Path Fill='Black' Data='M5,0 L5,-22 A22,22 180 0 1 5,22Z M-5,0 L-5,-22 A22,22 180 0 0 -5,22Z",
            
             // 8 - overcast filled black 
             "<Path Fill='Black' Data='M-22,-0.1 A22,22 360 1 1 -22,0Z",
             
             // 9 - sky obscured 
             "<Path Stroke='Black' StrokeThickness='5' Data='M-18,-18 L18,18 M18,-18 L-18,18"
        };

        /// <summary>
        /// The primary method for generating a wind barb when the hemisphere is specified.
        /// </summary>
        /// <param name="speedIn">
        /// The speed (in knots).
        /// </param>
        /// <param name="direction">
        /// The direction (in degrees).
        /// </param>
        /// <param name="southernHemisphere">
        /// A boolean that is true when the wind barb is in the southern hemisphere.
        /// </param>
        /// <returns>
        /// A Shape representing the wind barb.
        /// </returns>
        public static Shape GenerateWind(double? speedIn, double direction, bool southernHemisphere)
        {
            var sb = new StringBuilder("<Path Fill='Black' Stroke='Black' StrokeThickness='10' Data=");
            if (!speedIn.HasValue || (double)speedIn < 0.0 || (double)speedIn > 200.0)
            {
                sb.Append("'M0,-25 0,-155 M-30,-125 30,-185 M30,-125 -30,-185");
                return Rotate(sb, direction, southernHemisphere);
            }

            var list = BuildKnotsList((double)speedIn);
            if (list.Count == 0)
            {
                sb.Append("'M0,-25 0,-150");
                return Rotate(sb, direction, southernHemisphere);
            }

            return DrawBarbs(sb, list, direction, southernHemisphere);
        }

        /// <summary>
        /// The primary method for generating a wind barb, defaulting to the northern hemisphere.
        /// </summary>
        /// <param name="speedIn">
        /// The speed (in knots).
        /// </param>
        /// <param name="direction">
        /// The direction (in degrees).
        /// </param>
        /// <returns>
        /// A Shape representing the wind barb.
        /// </returns>
        public static Shape GenerateWind(double? speedIn, double direction)
        {
            return GenerateWind(speedIn, direction, false);
        }

        /// <summary>
        /// Generates the Shape for the desired cloud cover.
        /// </summary>
        /// <param name="scale">
        /// The scale to apply to the cloud cover.
        /// </param>
        /// <param name="cloudCover">
        /// The cloud cover value.
        /// </param>
        /// <returns>
        /// A Shape representing the desired cloud cover at the requested scale.
        /// </returns>
        public static Shape GenerateCloudCover(double scale, int cloudCover)
        {
            // Investigating a different way to set the Clouds compared to say, Mobility
            if (cloudCover < 1 || cloudCover > 9)
            {
                return null;
            }

            var sb = new StringBuilder(Clouds[cloudCover]);
            return Scale(sb, scale);
        }

        /// <summary>
        /// Adds a scale transformation to the passed in XAML string.
        /// </summary>
        /// <param name="sb">
        /// The string to keep appending to.
        /// </param>
        /// <param name="scale">
        /// The scale to apply via the transformation matrix.
        /// </param>
        /// <returns>
        /// The Shape generated by the resulting XAML string.
        /// </returns>
        private static Shape Scale(StringBuilder sb, double scale)
        {
            sb.Append(XmlnsOpen);
            sb.AppendFormat("<Path.RenderTransform><TransformGroup>");
            sb.AppendFormat("<ScaleTransform ScaleX='{0}' ScaleY='{0}'/>", scale);
            sb.AppendFormat("</TransformGroup></Path.RenderTransform></Path>");
#if SILVERLIGHT
            return XamlReader.Load(sb.ToString()) as Shape;
#elif WINDOWS_UWP
            return XamlReader.Load(sb.ToString()) as Shape;
#else
            return XamlReader.Parse(sb.ToString()) as Shape; 
#endif
        }

        /// <summary>
        /// Adds a rotate transformation to the passed in XAML string.
        /// </summary>
        /// <param name="sb">
        /// The XAML string up to this point.
        /// </param>
        /// <param name="direction">
        /// The angle (in degrees) to rotate the wind barb.
        /// </param>
        /// <param name="southernHemisphere">
        /// Whether the barb is in the southern hemisphere.
        /// </param>
        /// <returns>
        /// The new shape defined by the rotation angle.
        /// </returns>
        private static Shape Rotate(StringBuilder sb, double direction, bool southernHemisphere)
        {
            sb.Append(XmlnsOpen);
            sb.Append("<Path.RenderTransform><TransformGroup>");
            if (southernHemisphere)
            {
                sb.Append("<ScaleTransform ScaleX='-1' ScaleY='1'/>");
            }

            sb.AppendFormat(
                "<RotateTransform Angle='{0}'/></TransformGroup></Path.RenderTransform></Path>",
                (int)Math.Round(direction));
#if SILVERLIGHT
            return XamlReader.Load(sb.ToString()) as Shape;
#elif WINDOWS_UWP
            return XamlReader.Load(sb.ToString()) as Shape;
#else
            return XamlReader.Parse(sb.ToString()) as Shape;
#endif
        }

        /// <summary>
        /// Generates the list of barbs on the side of the indicator for a given speed.
        /// </summary>
        /// <param name="speed">
        /// The speed (in knots).
        /// </param>
        /// <returns>
        /// A collection of integers that is used to build the barbs on the side of the indicator.
        /// </returns>
        private static ICollection<int> BuildKnotsList(double speed)
        {
            var knots50 = (int)Math.Floor(speed / 50.0);
            speed -= knots50 * 50.0;
            var knots10 = (int)Math.Floor(speed / 10.0);
            speed -= knots10 * 10.0;
            var knots5 = 0;
            if (speed >= 2.5 && speed < 7.5)
            {
                knots5 = 1;
            }

            if (speed >= 7.5)
            {
                knots10++;
            }

            if (knots10 >= 5)
            {
                knots50++;
                knots10 = 0;
            }

            var list = new List<int>();
            for (int i = 0; i < knots5; i++)
            {
                list.Add(5);
            }

            for (int i = 0; i < knots10; i++)
            {
                list.Add(10);
            }

            for (int i = 0; i < knots50; i++)
            {
                list.Add(50);
            }

            return list;            
        }

        /// <summary>
        /// Gets the starting location for barbs. Five knots is a special case
        /// </summary>
        /// <param name="list">
        /// The list from the BuildKnotsList method.
        /// </param>
        /// <returns>
        /// The starting location for the barb location on the side on the indicator, in pixels.
        /// </returns>
        private static int GetStart(ICollection<int> list)
        {
            // Barbs are 30 pixels apart

            // If 4+ barbs, place first barb at 60
            // If 3 barbs, place first barb at 90
            // If 2 barbs, place first barb at 120
            // If 1 barb, place at 150 unless 5 knots - then place at 120
            switch (list.Count)
            {
                case 1:
                    if (list.Contains(5))
                    {
                        return -120;
                    }

                    return -150;
                case 2:
                    return -120;
                case 3:
                    return -90;
                default:
                    return -60;
            }
        }

        /// <summary>
        /// Add the barbs to the side of the indicator.
        /// </summary>
        /// <param name="sb">
        /// The string to which to add the barbs.
        /// </param>
        /// <param name="list">
        /// The list of barb indicators from BuildKnotsList.
        /// </param>
        /// <param name="direction">
        /// The direction to display the indicator.
        /// </param>
        /// <param name="southernHemisphere">
        /// A flag indicating whether the indicator is in the southern hemisphere.
        /// </param>
        /// <returns>
        /// A Shape representing the requested wind barb.
        /// </returns>
        private static Shape DrawBarbs(
            StringBuilder sb, 
            ICollection<int> list, 
            double direction, 
            bool southernHemisphere)
        {
            int startAt = GetStart(list); 
            sb.AppendFormat("'M0,-25 0,{0}", Math.Min(-150, -150 - (30 * (list.Count - 4))));
            foreach (var type in list)
            {
                switch (type)
                {
                    case 5:
                        sb.AppendFormat(" M0,{0} 35,{1}", startAt, startAt - 20);
                        break;
                    case 10:
                        sb.AppendFormat(" M0,{0} 70,{1}", startAt, startAt - 40);
                        break;
                    case 50:
                        sb.AppendFormat(" M0,{0} 0,{1} 52,{1} 0,{0}", startAt, startAt - 30);
                        break;
                }

                startAt -= 30;
            }

            return Rotate(sb, direction, southernHemisphere);
        }
    }
}
