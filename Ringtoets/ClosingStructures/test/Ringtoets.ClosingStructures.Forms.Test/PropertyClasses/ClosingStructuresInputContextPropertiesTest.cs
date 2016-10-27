// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructuresInputContextPropertiesTest
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 0;
        private const int stormDurationPropertyIndex = 1;
        private const int deviationWaveDirectionPropertyIndex = 2;
        private const int insideWaterLevelPropertyIndex = 3;
        private const int structurePropertyIndex = 4;
        private const int structureLocationPropertyIndex = 5;
        private const int structureNormalOrientationPropertyIndex = 6;
        private const int inflowModelTypePropertyIndex = 7;
        private const int widthFlowAperturesPropertyIndex = 8;
        private const int areaFlowAperturesPropertyIndex = 9;
        private const int identicalAperturesPropertyIndex = 10;
        private const int flowWidthAtBottomProtectionPropertyIndex = 11;
        private const int storageStructureAreaPropertyIndex = 12;
        private const int allowedLevelIncreaseStoragePropertyIndex = 13;
        private const int levelCrestStructureNotClosingPropertyIndex = 14;
        private const int thresholdHeightOpenWeirPropertyIndex = 15;
        private const int criticalOvertoppingDischargePropertyIndex = 16;
        private const int probabilityOpenStructureBeforeFloodingPropertyIndex = 17;
        private const int failureProbabilityOpenStructurePropertyIndex = 18;
        private const int failureProbabilityReparationPropertyIndex = 19;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 20;
        private const int foreshoreProfilePropertyIndex = 21;
        private const int useBreakWaterPropertyIndex = 22;
        private const int useForeshorePropertyIndex = 23;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 24;
        private const int drainCoefficientPropertyIndex = 25;
        private const int factorStormDurationOpenStructurePropertyIndex = 26;

        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ClosingStructuresInputContextProperties();

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<ClosingStructure, ClosingStructuresInput, StructuresCalculation<ClosingStructuresInput>, ClosingStructuresFailureMechanism>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var properties = new ClosingStructuresInputContextProperties();

            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);

            // Call
            properties.Data = inputContext;

            // Assert
            ClosingStructuresInput input = calculation.InputParameters;
            var expectedProbabilityOpenStructureBeforeFlooding = ProbabilityFormattingHelper.Format(input.ProbabilityOpenStructureBeforeFlooding);
            var expectedFailureProbabilityOpenStructure = ProbabilityFormattingHelper.Format(input.FailureProbabilityOpenStructure);
            var expectedFailureProbabilityReparation = ProbabilityFormattingHelper.Format(input.FailureProbabilityReparation);

            Assert.AreEqual(input.DeviationWaveDirection, properties.DeviationWaveDirection);
            Assert.AreSame(input.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.AreEqual(input.InflowModelType, properties.InflowModelType);
            Assert.AreSame(input.AreaFlowApertures, properties.AreaFlowApertures.Data);
            Assert.AreEqual(input.IdenticalApertures, properties.IdenticalApertures);
            Assert.AreSame(input.LevelCrestStructureNotClosing, properties.LevelCrestStructureNotClosing.Data);
            Assert.AreSame(input.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.AreEqual(expectedProbabilityOpenStructureBeforeFlooding, properties.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(expectedFailureProbabilityOpenStructure, properties.FailureProbabilityOpenStructure);
            Assert.AreEqual(expectedFailureProbabilityReparation, properties.FailureProbabilityReparation);
            Assert.AreSame(input.DrainCoefficient, properties.DrainCoefficient.Data);
            Assert.AreEqual(input.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetNewInputContextInstanceWithData_ReturnCorrectPropertyValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(0, "", 0, 0)
                }
            };
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                ForeshoreProfiles =
                {
                    new TestForeshoreProfile()
                },
                ClosingStructures =
                {
                    new TestClosingStructure()
                }
            };
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(),
                    HydraulicBoundaryLocation = CreateHydraulicBoundaryLocation(),
                    ForeshoreProfile = CreateForeshoreProfile()
                }
            };

            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties();

            // Call
            properties.Data = inputContext;

            // Assert
            ClosingStructuresInput input = calculation.InputParameters;
            Assert.AreEqual(input.DeviationWaveDirection, properties.DeviationWaveDirection);

            var availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles().ToArray();
            Assert.AreEqual(1, availableForeshoreProfiles.Length);
            CollectionAssert.AreEqual(failureMechanism.ForeshoreProfiles, availableForeshoreProfiles);

            var availableStructures = properties.GetAvailableStructures().ToArray();
            Assert.AreEqual(1, availableStructures.Length);
            CollectionAssert.AreEqual(failureMechanism.ClosingStructures, availableStructures);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 1;
            var observerMock = mockRepository.StrictMock<IObserver>();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();

            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            input.Attach(observerMock);

            var random = new Random(100);
            double newDeviationWaveDirection = random.NextDouble();

            // Call
            properties.DeviationWaveDirection = (RoundedDouble)newDeviationWaveDirection;

            // Assert
            Assert.AreEqual(newDeviationWaveDirection, properties.DeviationWaveDirection, properties.DeviationWaveDirection.GetAccuracy());
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);

            // Call
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(27, dynamicProperties.Count);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[structureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[widthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[modelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[stormDurationPropertyIndex].DisplayName);

            PropertyDescriptor deviationWaveDirectionProperty = dynamicProperties[deviationWaveDirectionPropertyIndex];
            Assert.AreEqual(hydraulicDataCategory, deviationWaveDirectionProperty.Category);
            Assert.AreEqual("Afwijking golfrichting [°]", deviationWaveDirectionProperty.DisplayName);
            Assert.AreEqual("Afwijking van de golfrichting.", deviationWaveDirectionProperty.Description);

            mockRepository.VerifyAll();
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return new ForeshoreProfile(new Point2D(0, 0), Enumerable.Empty<Point2D>(), new BreakWater(BreakWaterType.Caisson, 0), new ForeshoreProfile.ConstructionProperties());
        }

        private static HydraulicBoundaryLocation CreateHydraulicBoundaryLocation()
        {
            return new HydraulicBoundaryLocation(0, "name", 0.0, 1.1);
        }
    }
}