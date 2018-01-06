// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="SymbolCodeUnitTest.cs">
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MilSym.MilSymbol;
    using MilSym.MilSymbol.Schemas;

    /// <summary>
    /// Tests for symbol codes.
    /// </summary>
    [TestClass]
    public class SymbolCodeUnitTest
    {
        /// <summary>
        /// Tests for equipment flags.
        /// </summary>
        [TestMethod]
        public void IsEquipmentTest()
        {
            bool gc = SymbolData.IsEquipment(string.Empty);
            Assert.AreEqual(gc, false);

            // Nonsense code should fail with empty Equipment string
            gc = SymbolData.IsEquipment("ABCD-----------");
            Assert.AreEqual(gc, false);

            // Intelligence lead-in code should pass
            gc = SymbolData.IsEquipment("IBCD-----------");
            Assert.AreEqual(gc, true);
            
            // This should force Warfighting with a check for Equipment
            // which should fail unless we protect when the Equipment string
            // is empty.
            gc = SymbolData.IsEquipment("SBCD-----------");
            Assert.AreEqual(gc, false);

            // Check against emergency management operation equipment
            gc = SymbolData.IsEquipment("EBOD-----------");
            Assert.AreEqual(gc, false);

            // Check against emergency management operation equipment
            gc = SymbolData.IsEquipment("EBODAE---------");
            Assert.AreEqual(gc, true);

            // Check against emergency management infrastructure equipment
            gc = SymbolData.IsEquipment("EBFD-----------");
            Assert.AreEqual(gc, false);

            // Check against emergency management infrastructure equipment
            gc = SymbolData.IsEquipment("EBFDBA---------");
            Assert.AreEqual(gc, true);
        }

        /// <summary>
        /// Tests for BattleDimension.
        /// </summary>
        [TestMethod]
        public void BattleDimensionTest()
        {
            int gc = CategoryBattleDimension.GetCode(string.Empty);
            Assert.AreEqual(gc, 0);
            gc = CategoryBattleDimension.GetCode(null);
            Assert.AreEqual(gc, 0);
            gc = CategoryBattleDimension.GetCode("qqqqqqqqqqqqqqq");
            Assert.AreEqual(gc, 0);
            string str = CategoryBattleDimension.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = CategoryBattleDimension.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = CategoryBattleDimension.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = CategoryBattleDimension.GetName("iqpqqqqqqqqqqqq");
            Assert.AreEqual(str, "Space");
        }

        /// <summary>
        /// Tests for CodingScheme.
        /// </summary>
        [TestMethod]
        public void CodingSchemeTest()
        {
            int gc = CodingScheme.GetCode(string.Empty);
            Assert.AreEqual(gc, 0);
            gc = CodingScheme.GetCode(null);
            Assert.AreEqual(gc, 0);
            gc = CodingScheme.GetCode("qqqqqqqqqqqqqqq");
            Assert.AreEqual(gc, 0);
            string str = CodingScheme.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = CodingScheme.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = CodingScheme.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = CodingScheme.GetName("gqpqqqqqqqqqqqq");
            Assert.AreEqual(str, "Tactical Graphics");
        }

        /// <summary>
        /// Tests for CombinedModifierCode.
        /// </summary>
        [TestMethod]
        public void CombineModifierCodeTest()
        {
            string str = CombinedModifierCode.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = CombinedModifierCode.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = CombinedModifierCode.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = CombinedModifierCode.GetName("gqpqqqqqqqbfqqq");
            Assert.AreEqual(str, "Task Force & Headquarters\nBattalion/Squadron");
            str = CombinedModifierCode.GetName("gqpqqqqqqq-fqqq");
            Assert.AreEqual(str, "Battalion/Squadron");
            str = CombinedModifierCode.GetName("gqpqqqqqqqb-qqq");
            Assert.AreEqual(str, "Task Force & Headquarters");
            str = CombinedModifierCode.GetName("gqpqqqqqqq--qqq");
            Assert.AreEqual(str, string.Empty);
            str = CombinedModifierCode.GetName("gqpqqqqqqq-zqqq");
            Assert.AreEqual(str, string.Empty);
            str = CombinedModifierCode.GetName("gqpqqqqqqqz-qqq");
            Assert.AreEqual(str, string.Empty);
        }

        /// <summary>
        /// Test for the Countries.
        /// </summary>
        [TestMethod]
        public void CountriesTest()
        {
            string gc = Countries.GetCode(string.Empty);
            Assert.AreEqual(gc, string.Empty);
            gc = Countries.GetCode(null);
            Assert.AreEqual(gc, string.Empty);
            gc = Countries.GetCode("qqqqqqqqqqqqqqq");
            Assert.AreEqual(gc, "QQ");
            string str = Countries.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = Countries.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = Countries.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = Countries.GetName("gqpqqqqqqqqqusq");
            Assert.AreEqual(str, "United States");
        }

        /// <summary>
        /// Tests for Echelons.
        /// </summary>
        [TestMethod]
        public void EchelonTest()
        {
            char gc = Echelon.GetCode(string.Empty);
            Assert.AreEqual(gc, (char)0);
            gc = Echelon.GetCode(null);
            Assert.AreEqual(gc, (char)0);
            gc = Echelon.GetCode("qqqqqqqqqqqqqqq");
            Assert.AreEqual(gc, 'Q');
            string str = Echelon.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = Echelon.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = Echelon.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = Echelon.GetName("qqpqqqqqqqqhqqq");
            Assert.AreEqual(str, "Brigade");
        }

        /// <summary>
        /// Tests for Mobility.
        /// </summary>
        [TestMethod]
        public void MobilityTest()
        {
            string gc = Mobility.GetCode(string.Empty);
            Assert.AreEqual(gc, string.Empty);
            gc = Mobility.GetCode(null);
            Assert.AreEqual(gc, string.Empty);
            gc = Mobility.GetCode("qqqqqqqqqqqqqqq");
            Assert.AreEqual(gc, "QQ");
            string str = Mobility.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = Mobility.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = Mobility.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = Mobility.GetName("qqpqqqqqqqmwqqq");
            Assert.AreEqual(str, "Pack Animals");
        }

        /// <summary>
        /// Tests for OrderOfBattle.
        /// </summary>
        [TestMethod]
        public void OrderOfBattleTest()
        {
            char gc = OrderOfBattle.GetCode(string.Empty);
            Assert.AreEqual(gc, (char)0);
            gc = OrderOfBattle.GetCode(null);
            Assert.AreEqual(gc, (char)0);
            gc = OrderOfBattle.GetCode("qqqqqqqqqqqqqqq");
            Assert.AreEqual(gc, 'Q');
            string str = OrderOfBattle.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = OrderOfBattle.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = OrderOfBattle.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = OrderOfBattle.GetName("qqpqqqqqqqqqqqn");
            Assert.AreEqual(str, "Maritime");
        }

        /// <summary>
        /// Tests for StandardIdentity.
        /// </summary>
        [TestMethod]
        public void StandardIdentityTest()
        {
            int gc = StandardIdentity.GetCode(string.Empty);
            Assert.AreEqual(gc, (char)0);
            gc = StandardIdentity.GetCode(null);
            Assert.AreEqual(gc, (char)0);
            gc = StandardIdentity.GetCode("qqqqqqqqqqqqqqq");
            Assert.AreEqual(gc, (char)0);
            string str = StandardIdentity.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = StandardIdentity.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = StandardIdentity.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = StandardIdentity.GetName("qApqqqqqqqqqqqn");
            Assert.AreEqual(str, "Assumed Friend");
        }

        /// <summary>
        /// Tests for Status and OperationalCapacity.
        /// </summary>
        [TestMethod]
        public void StatusOperationalCapacityTest()
        {
            char gc = StatusOperationalCapacity.GetCode(string.Empty);
            Assert.AreEqual(gc, (char)0);
            gc = StatusOperationalCapacity.GetCode(null);
            Assert.AreEqual(gc, (char)0);
            gc = StatusOperationalCapacity.GetCode("qqqqqqqqqqqqqqq");
            Assert.AreEqual(gc, 'Q');
            string str = StatusOperationalCapacity.GetName(string.Empty);
            Assert.AreEqual(str, string.Empty);
            str = StatusOperationalCapacity.GetName(null);
            Assert.AreEqual(str, string.Empty);
            str = StatusOperationalCapacity.GetName("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = StatusOperationalCapacity.GetName("qqpAqqqqqqqqqqn");
            Assert.AreEqual(str, "Anticipated/Planned");
        }

        /// <summary>
        /// Tests for the MilAppendices.
        /// </summary>
        [TestMethod]
        public void MilAppendixTest()
        {
            string str = MilAppendix.Description("qqqqqqqqqqqqqqq");
            Assert.AreEqual(str, string.Empty);
            str = MilAppendix.Description(null);
            Assert.AreEqual(str, string.Empty);
            str = MilAppendix.Description("I*G*SC------***");
            Assert.AreEqual(str, "Signals Intelligence\r\n Ground Track\r\n  Signal Intercept\r\n   Communications\r\n");
        }
    }
}