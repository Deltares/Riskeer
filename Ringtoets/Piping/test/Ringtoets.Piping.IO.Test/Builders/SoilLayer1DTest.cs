using System;
using System.Drawing;
using NUnit.Framework;
using Ringtoets.Piping.IO.Builders;

namespace Ringtoets.Piping.IO.Test.Builders
{
    [TestFixture]
    public class SoilLayer1DTest
    {
        [Test]
        public void Constructor_WithTop_TopSet()
        {
            // Setup
            var random = new Random(22);
            var top = random.NextDouble();

            // Call
            var layer = new SoilLayer1D(top);

            // Assert
            Assert.AreEqual(top, layer.Top);
            Assert.IsNull(layer.AbovePhreaticLevel);
            Assert.IsNull(layer.BelowPhreaticLevel);
            Assert.IsNull(layer.DryUnitWeight);
            Assert.IsNull(layer.IsAquifer);
            Assert.IsNull(layer.MaterialName);
            Assert.IsNull(layer.Color);
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(1.0+1e-12)]
        [TestCase(2.0)]
        public void AsPipingSoilLayer_PropertiesSetWithDifferentIsAquifer_PropertiesAreSetInPipingSoilLayer(double isAquifer)
        {
            // Setup
            var random = new Random(22);
            var top = random.NextDouble();
            var materialName = "materialX";
            var abovePhreaticLevel = random.NextDouble();
            var belowPhreaticLevel = random.NextDouble();
            var dryUnitWeight = random.NextDouble();
            var color = Color.BlanchedAlmond;

            var layer = new SoilLayer1D(top)
            {
                MaterialName = materialName,
                IsAquifer = isAquifer,
                AbovePhreaticLevel = abovePhreaticLevel,
                BelowPhreaticLevel = belowPhreaticLevel,
                DryUnitWeight = dryUnitWeight,
                Color = color.ToArgb()
            };

            // Call
            var result = layer.AsPipingSoilLayer();

            // Assert
            Assert.AreEqual(top, result.Top);
            Assert.AreEqual(isAquifer.Equals(1.0), result.IsAquifer);
            Assert.AreEqual(abovePhreaticLevel, result.AbovePhreaticLevel);
            Assert.AreEqual(belowPhreaticLevel, result.BelowPhreaticLevel);
            Assert.AreEqual(dryUnitWeight, result.DryUnitWeight);
            Assert.AreEqual(materialName, result.MaterialName);
            Assert.AreEqual(Color.FromArgb(color.ToArgb()), result.Color);
        }

        [Test]
        public void AsPipingSoilLayer_PropertiesSetWithNullMaterialName_MaterialNameEmptyInPipingSoilLayer()
        {
            // Setup
            var random = new Random(22);
            var top = random.NextDouble();
            var layer = new SoilLayer1D(top);

            // Call
            var result = layer.AsPipingSoilLayer();

            // Assert
            Assert.IsEmpty(result.MaterialName);
        }
    }
}