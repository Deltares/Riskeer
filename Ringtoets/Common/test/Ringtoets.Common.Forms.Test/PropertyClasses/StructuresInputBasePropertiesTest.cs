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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StructuresInputBasePropertiesTest
    {
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
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<InputContextBase<SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.IsInstanceOf<IHasStructureProperty<SimpleStructure>>(properties);
            Assert.IsInstanceOf<IHasForeshoreProfileProperty>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Constructor_ConstructionPropertiesIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleStructuresInputProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("constructionProperties", paramName);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();

            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties());
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);

            // Call
            properties.Data = inputContext;

            // Assert
            SimpleStructureInput input = calculation.InputParameters;
            var expectedFailureProbabilityStructureWithErosion = ProbabilityFormattingHelper.Format(input.FailureProbabilityStructureWithErosion);

            Assert.IsNull(properties.Structure);
            Assert.IsNull(properties.StructureLocation);
            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);
            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreSame(input.AllowedLevelIncreaseStorage, properties.AllowedLevelIncreaseStorage.Data);
            Assert.AreSame(input.StorageStructureArea, properties.StorageStructureArea.Data);
            Assert.AreSame(input.FlowWidthAtBottomProtection, properties.FlowWidthAtBottomProtection.Data);
            Assert.AreSame(input.WidthFlowApertures, properties.WidthFlowApertures.Data);
            Assert.AreSame(input.CriticalOvertoppingDischarge, properties.CriticalOvertoppingDischarge.Data);
            Assert.IsNull(properties.ForeshoreProfile);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.UseBreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.UseForeshore);
            Assert.AreEqual(expectedFailureProbabilityStructureWithErosion, properties.FailureProbabilityStructureWithErosion);
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);
            Assert.AreSame(input.StormDuration, properties.StormDuration.Data);

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
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();

            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    Structure = new SimpleStructure()
                }
            };
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties());
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);

            // Call
            properties.Data = inputContext;

            // Assert
            SimpleStructureInput input = calculation.InputParameters;
            var expectedStructureLocation = new Point2D(new RoundedDouble(0, input.Structure.Location.X), new RoundedDouble(0, input.Structure.Location.Y));
            var expectedFailureProbabilityStructureWithErosion = ProbabilityFormattingHelper.Format(input.FailureProbabilityStructureWithErosion);

            Assert.AreSame(input.Structure, properties.Structure);
            Assert.AreEqual(expectedStructureLocation, properties.StructureLocation);
            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);
            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreSame(input.AllowedLevelIncreaseStorage, properties.AllowedLevelIncreaseStorage.Data);
            Assert.AreSame(input.StorageStructureArea, properties.StorageStructureArea.Data);
            Assert.AreSame(input.FlowWidthAtBottomProtection, properties.FlowWidthAtBottomProtection.Data);
            Assert.AreSame(input.WidthFlowApertures, properties.WidthFlowApertures.Data);
            Assert.AreSame(input.CriticalOvertoppingDischarge, properties.CriticalOvertoppingDischarge.Data);
            Assert.AreSame(input.ForeshoreProfile, properties.ForeshoreProfile);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.UseBreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.UseForeshore);
            Assert.AreEqual(expectedFailureProbabilityStructureWithErosion, properties.FailureProbabilityStructureWithErosion);
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);
            Assert.AreSame(input.StormDuration, properties.StormDuration.Data);

            Assert.AreEqual(1, properties.GetAvailableHydraulicBoundaryLocations().Count());
            CollectionAssert.AreEqual(inputContext.AvailableHydraulicBoundaryLocations, properties.GetAvailableHydraulicBoundaryLocations());

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 5;
            var observerMock = mockRepository.StrictMock<IObserver>();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();

            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    Structure = new SimpleStructure()
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            inputContext.Attach(observerMock);

            var random = new Random(100);
            double newStructureNormalOrientation = random.NextDouble();
            var newStructure = new SimpleStructure();
            ForeshoreProfile newForeshoreProfile = CreateForeshoreProfile();
            HydraulicBoundaryLocation newHydraulicBoundaryLocation = CreateHydraulicBoundaryLocation();

            // Call
            properties.Structure = newStructure;
            properties.StructureNormalOrientation = (RoundedDouble) newStructureNormalOrientation;
            properties.FailureProbabilityStructureWithErosion = "1e-2";
            properties.HydraulicBoundaryLocation = newHydraulicBoundaryLocation;
            properties.ForeshoreProfile = newForeshoreProfile;

            // Assert
            Assert.AreSame(newStructure, properties.Structure);
            Assert.AreEqual(newStructureNormalOrientation, properties.StructureNormalOrientation, properties.StructureNormalOrientation.GetAccuracy());
            Assert.AreEqual(0.01, inputContext.WrappedData.FailureProbabilityStructureWithErosion);
            Assert.AreEqual("1/100", properties.FailureProbabilityStructureWithErosion);
            Assert.AreSame(newHydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);
            Assert.AreSame(newForeshoreProfile, properties.ForeshoreProfile);
            mockRepository.VerifyAll();
        }

        private static HydraulicBoundaryLocation CreateHydraulicBoundaryLocation()
        {
            return new HydraulicBoundaryLocation(0, "", 0, 0);
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return new ForeshoreProfile(new Point2D(0, 0), Enumerable.Empty<Point2D>(), null, new ForeshoreProfile.ConstructionProperties());
        }

        private class SimpleStructure : StructureBase
        {
            public SimpleStructure() : base("Name", "Id", new Point2D(0, 0), 0.0) {}
        }

        private class SimpleStructureInput : StructuresInputBase<SimpleStructure>
        {
            protected override void UpdateStructureParameters() {}
        }

        private class SimpleStructuresInputProperties : StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>
        {
            public SimpleStructuresInputProperties(ConstructionProperties constructionProperties) : base(constructionProperties) {}

            public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
            {
                yield return CreateForeshoreProfile();
            }

            public override IEnumerable<SimpleStructure> GetAvailableStructures()
            {
                yield return new SimpleStructure();
            }

            protected override void AfterSettingStructure() {}
        }

        private class SimpleInputContext : InputContextBase<SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>
        {
            public SimpleInputContext(SimpleStructureInput wrappedData, StructuresCalculation<SimpleStructureInput> calculation, IFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(wrappedData, calculation, failureMechanism, assessmentSection) {}
        }
    }
}