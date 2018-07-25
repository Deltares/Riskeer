// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingInputTest
    {
        private static IEnumerable<TestCaseData> DifferentSurfaceLineProperties
        {
            get
            {
                yield return new TestCaseData(new Point3D(3, 0, 0), new Point3D(3, 0, 0))
                    .SetName("DifferentDikeToeAtRiver");
                yield return new TestCaseData(new Point3D(2, 0, 3), new Point3D(4, 0, 2))
                    .SetName("DifferentDikeToeAtPolder");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var phreaticLevelExit = new NormalDistribution(3)
            {
                Mean = (RoundedDouble) 0,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var dampingFactorExit = new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.7,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var generalInputParameters = new GeneralPipingInput();

            // Call
            var inputParameters = new PipingInput(generalInputParameters);

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(inputParameters);
            Assert.IsInstanceOf<ICalculationInputWithHydraulicBoundaryLocation>(inputParameters);

            DistributionAssert.AreEqual(phreaticLevelExit, inputParameters.PhreaticLevelExit);
            DistributionAssert.AreEqual(dampingFactorExit, inputParameters.DampingFactorExit);

            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);

            Assert.AreEqual(generalInputParameters.UpliftModelFactor, inputParameters.UpliftModelFactor);
            Assert.AreEqual(generalInputParameters.SellmeijerModelFactor, inputParameters.SellmeijerModelFactor);
            Assert.AreEqual(generalInputParameters.CriticalHeaveGradient, inputParameters.CriticalHeaveGradient);
            Assert.AreEqual(generalInputParameters.SellmeijerReductionFactor, inputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(generalInputParameters.Gravity, inputParameters.Gravity);
            Assert.AreEqual(generalInputParameters.WaterKinematicViscosity, inputParameters.WaterKinematicViscosity);
            Assert.AreEqual(generalInputParameters.WaterVolumetricWeight, inputParameters.WaterVolumetricWeight);
            Assert.AreEqual(generalInputParameters.SandParticlesVolumicWeight, inputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(generalInputParameters.WhitesDragCoefficient, inputParameters.WhitesDragCoefficient);
            Assert.AreEqual(generalInputParameters.BeddingAngle, inputParameters.BeddingAngle);
            Assert.AreEqual(generalInputParameters.MeanDiameter70, inputParameters.MeanDiameter70);

            Assert.IsNaN(inputParameters.ExitPointL);
            Assert.AreEqual(2, inputParameters.ExitPointL.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.EntryPointL);
            Assert.AreEqual(2, inputParameters.EntryPointL.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.AssessmentLevel);
            Assert.AreEqual(2, inputParameters.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.IsFalse(inputParameters.UseAssessmentLevelManualInput);
        }

        [Test]
        public void Constructor_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingInput(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void ExitPointL_Always_SameNumberOfDecimalsAsSurfaceLineLocalGeometry()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = CreateSurfaceLine()
            };

            // Call
            RoundedPoint2DCollection localGeometry = pipingInput.SurfaceLine.LocalGeometry;

            // Assert
            Assert.AreEqual(localGeometry.NumberOfDecimalPlaces, pipingInput.ExitPointL.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(1.23456)]
        [TestCase(3.5)]
        public void ExitPointL_ExitPointEqualSmallerThanEntryPoint_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                EntryPointL = (RoundedDouble) 3.5
            };

            // Call
            TestDelegate call = () => pipingInput.ExitPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(5.4)]
        [TestCase(1.006)]
        [TestCase(-0.005)]
        [TestCase(-5.4)]
        public void ExitPointL_ExitPointNotOnSurfaceLine_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = RoundedDouble.NaN;

            // Call
            TestDelegate call = () => input.ExitPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 1,0]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(-1e-3, Description = "Valid ExitPointL due to rounding to 0.0")]
        [TestCase(0.1004)]
        [TestCase(0.50)]
        public void ExitPointL_SetToNew_ValueIsRounded(double exitPointValue)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = RoundedDouble.NaN;

            int originalNumberOfDecimalPlaces = input.ExitPointL.NumberOfDecimalPlaces;

            // Call
            input.ExitPointL = (RoundedDouble) exitPointValue;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.ExitPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(exitPointValue, input.ExitPointL, input.ExitPointL.GetAccuracy());
        }

        [Test]
        public void EntryPointL_Always_SameNumberOfDecimalsAsSurfaceLineLocalGeometry()
        {
            // Setup
            PipingSurfaceLine surfaceLine = CreateSurfaceLine();
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            RoundedPoint2DCollection localGeometry = surfaceLine.LocalGeometry;

            // Assert
            Assert.AreEqual(localGeometry.NumberOfDecimalPlaces, pipingInput.EntryPointL.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(5.0)]
        [TestCase(3.5)]
        public void EntryPointL_EntryPointEqualOrGreaterThanExitPoint_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 3.5
            };

            // Call
            TestDelegate call = () => pipingInput.EntryPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(5.4)]
        [TestCase(1.006)]
        [TestCase(-0.005)]
        [TestCase(-5.4)]
        public void EntryPointL_EntryPointNotOnSurfaceLine_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = RoundedDouble.NaN;
            // Call
            TestDelegate call = () => input.EntryPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 1,0]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(-1e-3, Description = "Valid EntryPointL due to rounding to 0.0")]
        [TestCase(0.005)]
        [TestCase(0.1004)]
        [TestCase(0.50)]
        public void EntryPointL_SetToNew_ValueIsRounded(double entryPointValue)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = RoundedDouble.NaN;

            int originalNumberOfDecimalPlaces = input.EntryPointL.NumberOfDecimalPlaces;

            // Call
            input.EntryPointL = (RoundedDouble) entryPointValue;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.EntryPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(entryPointValue, input.EntryPointL, input.EntryPointL.GetAccuracy());
        }

        [Test]
        public void SurfaceLine_WithDikeToes_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(1, 0, 2));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(2, 0, 3));

            // Call
            input.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(1, input.EntryPointL, input.EntryPointL.GetAccuracy());
            Assert.AreEqual(2, input.ExitPointL, input.ExitPointL.GetAccuracy());
        }

        [Test]
        public void SurfaceLine_DikeToesBeyondSetExitPointL_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            input.SurfaceLine = surfaceLine;
            input.EntryPointL = (RoundedDouble) 0;
            input.ExitPointL = (RoundedDouble) 1;

            // Call
            input.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(2, input.EntryPointL, input.EntryPointL.GetAccuracy());
            Assert.AreEqual(3, input.ExitPointL, input.ExitPointL.GetAccuracy());
        }

        [Test]
        public void SurfaceLine_DikeToesBeforeSetEntryPointL_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            input.SurfaceLine = surfaceLine;
            input.ExitPointL = (RoundedDouble) 5;
            input.EntryPointL = (RoundedDouble) 4;

            // Call
            input.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(2, input.EntryPointL, input.EntryPointL.GetAccuracy());
            Assert.AreEqual(3, input.ExitPointL, input.ExitPointL.GetAccuracy());
        }

        [Test]
        public void SynchronizeEntryAndExitPointInput_SurfaceLineNull_EntryPointLAndExitPointLNaN()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput())
            {
                EntryPointL = (RoundedDouble) 3,
                ExitPointL = (RoundedDouble) 5
            };

            // Call
            input.SynchronizeEntryAndExitPointInput();

            // Assert
            Assert.IsNaN(input.EntryPointL);
            Assert.IsNaN(input.ExitPointL);
        }

        [Test]
        public void SynchronizeEntryAndExitPointInput_DikeToesBeyondSetExitPointL_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            input.SurfaceLine = surfaceLine;
            input.EntryPointL = (RoundedDouble) 0;
            input.ExitPointL = (RoundedDouble) 1;

            // Call
            input.SynchronizeEntryAndExitPointInput();

            // Assert
            Assert.AreEqual(2, input.EntryPointL, input.EntryPointL.GetAccuracy());
            Assert.AreEqual(3, input.ExitPointL, input.ExitPointL.GetAccuracy());
        }

        [Test]
        public void SynchronizeEntryAndExitPointInput_DikeToesBeforeSetEntryPointL_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            input.SurfaceLine = surfaceLine;
            input.ExitPointL = (RoundedDouble) 5;
            input.EntryPointL = (RoundedDouble) 4;

            // Call
            input.SynchronizeEntryAndExitPointInput();

            // Assert
            Assert.AreEqual(2, input.EntryPointL, input.EntryPointL.GetAccuracy());
            Assert.AreEqual(3, input.ExitPointL, input.ExitPointL.GetAccuracy());
        }

        [Test]
        public void IsEntryAndExitPointInputSynchronized_SurfaceLineNull_ReturnFalse()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            // Call
            bool synchronized = input.IsEntryAndExitPointInputSynchronized;

            // Assert
            Assert.IsFalse(synchronized);
        }

        [Test]
        public void IsEntryAndExitPointInputSynchronized_SurfaceLineAndInputInSync_ReturnTrue()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            // Call
            bool synchronized = input.IsEntryAndExitPointInputSynchronized;

            // Assert
            Assert.IsTrue(synchronized);
        }

        [Test]
        [TestCaseSource(nameof(DifferentSurfaceLineProperties))]
        public void IsEntryAndExitPointInputSynchronized_SurfaceLineAndInputNotInSync_ReturnFalse(Point3D newDikeToeAtRiver, Point3D newDikeToeAtPolder)
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            input.SurfaceLine.SetDikeToeAtRiverAt(newDikeToeAtRiver);
            input.SurfaceLine.SetDikeToeAtPolderAt(newDikeToeAtPolder);

            // Call
            bool synchronized = input.IsEntryAndExitPointInputSynchronized;

            // Assert
            Assert.IsFalse(synchronized);
        }

        [Test]
        public void GivenSurfaceLineSet_WhenSurfaceLineNull_ThenEntryAndExitPointsNaN()
        {
            // Given
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            input.SurfaceLine = surfaceLine;
            input.ExitPointL = (RoundedDouble) 5;
            input.EntryPointL = (RoundedDouble) 4;

            // When
            input.SurfaceLine = null;

            // Then
            Assert.IsNaN(input.EntryPointL);
            Assert.IsNaN(input.ExitPointL);
        }

        [Test]
        public void PhreaticLevelExit_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var random = new Random(22);
            var input = new PipingInput(new GeneralPipingInput());
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(3)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.PhreaticLevelExit = distributionToSet;

            // Assert
            DistributionTestHelper.AssertDistributionCorrectlySet(input.PhreaticLevelExit, distributionToSet, expectedDistribution);
        }

        [Test]
        public void DampingFactorExit_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var random = new Random(22);
            var input = new PipingInput(new GeneralPipingInput());
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new LogNormalDistribution(3)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.DampingFactorExit = distributionToSet;

            // Assert
            DistributionTestHelper.AssertDistributionCorrectlySet(input.DampingFactorExit, distributionToSet, expectedDistribution);
        }

        [Test]
        public void AssessmentLevel_SetToNew_ValueIsRounded()
        {
            // Setup
            const double assessmentLevel = 1.111111;
            var input = new PipingInput(new GeneralPipingInput());

            int originalNumberOfDecimalPlaces = input.AssessmentLevel.NumberOfDecimalPlaces;

            // Call
            input.AssessmentLevel = (RoundedDouble) assessmentLevel;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(assessmentLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new PipingInput(new GeneralPipingInput());

            PipingTestDataGenerator.SetRandomDataToPipingInput(original);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new PipingInput(new GeneralPipingInput());

            PipingTestDataGenerator.SetRandomDataToPipingInput(original);

            original.SurfaceLine = null;
            original.StochasticSoilModel = null;
            original.StochasticSoilProfile = null;
            original.HydraulicBoundaryLocation = null;

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        private static PipingSurfaceLine CreateSurfaceLine()
        {
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(2, 0, 2)
            });

            return surfaceLine;
        }
    }
}