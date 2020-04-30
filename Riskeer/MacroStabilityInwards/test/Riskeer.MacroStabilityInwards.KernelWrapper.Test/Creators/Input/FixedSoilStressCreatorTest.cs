using System;
using System.Collections.Generic;
using System.Linq;
using Deltares.MacroStability.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;
using SoilLayer = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class FixedSoilStressCreatorTest
    {
        [Test]
        public void Create_LayerLookupNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FixedSoilStressCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("layerLookup", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsFixedSoilStresses()
        {
            // Setup
            var lookup = new Dictionary<SoilLayer, LayerWithSoil>
            {
                {
                    CreateSoilLayer(new Point2D[0], true, 22),
                    CreateLayerWithSoil(new Point2D[0], "Material 1")
                },
                {
                    CreateSoilLayer(new Point2D[0], false),
                    CreateLayerWithSoil(new Point2D[0], "Material 2")
                },
                {
                    CreateSoilLayer(new Point2D[0], true, 23),
                    CreateLayerWithSoil(new Point2D[0], "Material 3")
                },
                {
                    CreateSoilLayer(new Point2D[0], true, 24),
                    CreateLayerWithSoil(new Point2D[0], "Material 4")
                }
            };

            // Call
            IEnumerable<FixedSoilStress> fixedSoilStresses = FixedSoilStressCreator.Create(lookup);

            // Assert
            IEnumerable<KeyValuePair<SoilLayer, LayerWithSoil>> lookUpWithPop = lookup.Where(l => l.Key.UsePop);
            Assert.AreEqual(lookUpWithPop.Count(), fixedSoilStresses.Count());

            for (var i = 0; i < fixedSoilStresses.Count(); i++)
            {
                KeyValuePair<SoilLayer, LayerWithSoil> keyValuePair = lookUpWithPop.ElementAt(i);
                FixedSoilStress fixedSoilStress = fixedSoilStresses.ElementAt(i);
                
                Assert.AreEqual(keyValuePair.Value.Soil, fixedSoilStress.Soil);
                Assert.AreEqual(keyValuePair.Key.Pop, fixedSoilStress.CenterStressValue.POP);
            }
        }

        private static SoilLayer CreateSoilLayer(IEnumerable<Point2D> outerRing1, bool usePop, int seed = 21)
        {
            return new SoilLayer(outerRing1, new SoilLayer.ConstructionProperties
            {
                UsePop = usePop,
                Pop = usePop ? new Random(seed).NextDouble() : double.NaN
            }, Enumerable.Empty<SoilLayer>());
        }

        private static LayerWithSoil CreateLayerWithSoil(IEnumerable<Point2D> outerRing1, string materialName)
        {
            return new LayerWithSoil(outerRing1, Enumerable.Empty<IEnumerable<Point2D>>(), new Soil(materialName), false, WaterpressureInterpolationModel.Automatic);
        }
    }
}