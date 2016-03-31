using System;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Builders
{
    public class SoilProfileBuilder1DTest
    {
        [Test]
        public void Build_WithOutLayers_ThrowsSoilProfileBuilderException()
        {
            // Setup
            var profileName = "SomeProfile";
            var builder = new SoilProfileBuilder1D(profileName, 0.0, 0);

            // Call
            TestDelegate test = () => builder.Build();

            // Assert
            Assert.Throws<SoilProfileBuilderException>(test);
        }

        [Test]
        public void Build_WithSingleLayer_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            var profileName = "SomeProfile";
            var random = new Random(22);
            var bottom = random.NextDouble();
            var top = random.NextDouble();
            const long pipingSoilProfileId = 1234L;
            var builder = new SoilProfileBuilder1D(profileName, bottom, pipingSoilProfileId);
            builder.Add(new PipingSoilLayer(top)
            {
                IsAquifer = true
            });

            // Call
            PipingSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(1, soilProfile.Layers.Count());
            Assert.AreEqual(SoilProfileType.SoilProfile1D, soilProfile.SoilProfileType);
            Assert.AreEqual(pipingSoilProfileId, soilProfile.PipingSoilProfileId);
            Assert.AreEqual(top, soilProfile.Layers.ToArray()[0].Top);
            Assert.AreEqual(bottom, soilProfile.Bottom);
        }

        [Test]
        public void Build_WithMultipleLayers_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            var profileName = "SomeProfile";
            var random = new Random(22);
            var bottom = random.NextDouble();
            var top = bottom + random.NextDouble();
            var top2 = bottom + random.NextDouble();
            const long pipingSoilProfileId = 1234L;

            var builder = new SoilProfileBuilder1D(profileName, bottom, pipingSoilProfileId);
            builder.Add(new PipingSoilLayer(top)
            {
                IsAquifer = true
            });
            builder.Add(new PipingSoilLayer(top2));

            // Call
            PipingSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(2, soilProfile.Layers.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                top,
                top2
            }, soilProfile.Layers.Select(l => l.Top));
            Assert.AreEqual(bottom, soilProfile.Bottom);
            Assert.AreEqual(pipingSoilProfileId, soilProfile.PipingSoilProfileId);
        }
    }
}