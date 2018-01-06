// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilTests.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support for testing symbology.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSymUnitTest
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MilSym.MilSymbol;

    /// <summary>
    /// Simple symbology tests.
    /// </summary>
    [TestClass]
    public class MilTests
    {
        /// <summary>
        /// Tests for MilSymbol.
        /// </summary>
        [TestMethod]
        public void MilSymTest()
        {
        }

        /// <summary>
        /// Tests for MilBrush.
        /// </summary>
        [TestMethod]
        public void MilBrushTest()
        {
            MilBrush.ColorScheme = ColorSchemeProperty.Medium;
            ColorSchemeProperty csp = MilBrush.ColorScheme;
            Assert.AreEqual(csp, ColorSchemeProperty.Medium);
            Brush br = MilBrush.FindColorScheme(string.Empty);
            Assert.AreEqual(br, null);
            br = MilBrush.FindColorScheme(null);
            Assert.AreEqual(br, null);
            br = MilBrush.FindColorScheme("qqqqqqqqqqqqqqq");
            Assert.AreEqual(br, null);
            br = MilBrush.FindColorScheme("qFqqqqFqqqqFqqC");
            Assert.AreEqual(br, MilBrush.MediumPurple);
            br = MilBrush.FindColorScheme("qHqqqqFqqqqFqqC");
            Assert.AreEqual(br, MilBrush.MediumRed);
            br = MilBrush.FindColorScheme("qFqqqqFqqqqFqqq");
            Assert.AreEqual(br, MilBrush.MediumBlue);
            br = MilBrush.FindColorScheme("qHqqqqFqqqqFqqq");
            Assert.AreEqual(br, MilBrush.MediumRed);
            br = MilBrush.FindColorScheme("qNqqqqFqqqqFqqq");
            Assert.AreEqual(br, MilBrush.MediumGreen);
            br = MilBrush.FindColorScheme("qUqqqqFqqqqFqqq");
            Assert.AreEqual(br, MilBrush.MediumYellow);
            Style st = MilBrush.GetFill(null);
            Assert.AreEqual(st, null);
            st = MilBrush.GetFill(MilBrush.MediumYellow);
            Assert.IsNotNull(st);
            Style stb = MilBrush.GetFill(MilBrush.MediumYellow);
            Assert.AreEqual(st, stb);
            st = MilBrush.GetLinePresent(null);
            Assert.AreEqual(st, null);
            st = MilBrush.GetLinePresent(MilBrush.MediumYellow);
            Assert.IsNotNull(st);
            stb = MilBrush.GetLinePresent(MilBrush.MediumYellow);
            Assert.AreEqual(st, stb);
        }

        /// <summary>
        /// Tests for MilHats.
        /// </summary>
        [TestMethod]
        public void MilHatsTest()
        {
            var ms = new MilSymbol("IHGqSCC---" + "BC" + "qqq");
            var tf = new[] { "BC", "DG", "EI", "GM" };
            var heights = new[] { -253, -293.3999, -253, -320.3999 };
            for (int i = 0; i < tf.Length; i++)
            {
                string sc = "IHGqSCC---" + tf[i] + "qqq";
                double d = MilHats.Generate(ms, sc);
                Assert.IsTrue(Math.Abs(heights[i] - d) < 0.0001);
            }
        }
    }
}