// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="LabelsUnitTest.cs">
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
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MilSym.MilSymbol;

    /// <summary>
    /// Methods for testing labels.
    /// </summary>
    [TestClass]
    public class LabelsUnitTest
    {
        /// <summary>
        /// The TestMethod for the LabelsUnitTest.
        /// </summary>
        [TestMethod]
        [Description("Test parsing a label's string")]
        public void TestMethod()
        {
            // Test to make sure that we're parsing label strings
            IDictionary<string, string> lo = MilLabels.Generate("C=%%^&,F=78", null);
            Assert.AreNotEqual(null, lo);
            Assert.AreEqual(lo["C"], "%%^&", "Should be the value of the first string.");
            Assert.AreEqual(lo["F"], "78", "Should be the value of the second string.");

            // Test to make sure that we're inserting the proper number of new lines for labels
            var style = new Style(typeof(TextBlock));   // set arbitrary TextBlock style
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, 80.0));

            // Generate labels for bottom left and right lines
            lo = MilLabels.Generate("J=JJ,Z=ZZ", null);

            // Should have four lines on the left and on the right
            var right = MilLabels.GenerateRight("SNACMFJ---****E", lo, style);
            Assert.AreEqual(right.Inlines.Count, 4, "Should be the number of lines on the right side");
            var left = MilLabels.GenerateLeft("SNACMFJ---****E", lo, style);
            Assert.AreEqual(left.Inlines.Count, 4, "Should be the number of lines on the left side");
        }
    }
}