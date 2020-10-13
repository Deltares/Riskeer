// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.UITypeEditors;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.PropertyClasses;
using Riskeer.Piping.Forms.PropertyClasses.Probabilistic;
using Riskeer.Piping.Forms.PropertyClasses.SemiProbabilistic;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.PropertyClasses.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingInputContextPropertiesTest
    {
        private const int expectedSelectedHydraulicBoundaryLocationPropertyIndex = 1;
        private const int expectedDampingFactorExitPropertyIndex = 2;
        private const int expectedPhreaticLevelExitPropertyIndex = 3;
        private const int expectedPiezometricHeadExitPropertyIndex = 4;

        private const int expectedSurfaceLinePropertyIndex = 5;
        private const int expectedStochasticSoilModelPropertyIndex = 6;
        private const int expectedStochasticSoilProfilePropertyIndex = 7;
        private const int expectedEntryPointLPropertyIndex = 8;
        private const int expectedExitPointLPropertyIndex = 9;
        private const int expectedSeepageLengthPropertyIndex = 10;
        private const int expectedThicknessCoverageLayerPropertyIndex = 11;
        private const int expectedEffectiveThicknessCoverageLayerPropertyIndex = 12;
        private const int expectedThicknessAquiferLayerPropertyIndex = 13;
        private const int expectedDarcyPermeabilityPropertyIndex = 14;
        private const int expectedDiameter70PropertyIndex = 15;
        private const int expectedSaturatedVolumicWeightOfCoverageLayerPropertyIndex = 16;

        private const int expectedSectionNamePropertyIndex = 17;
        private const int expectedSectionLengthPropertyIndex = 18;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new ProbabilisticPipingInputContextProperties(null, AssessmentSectionTestHelper.GetTestAssessmentLevel, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetAssessmentLevelFuncNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var probabilisticPipingCalculation = new ProbabilisticPipingCalculation(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var context = new ProbabilisticPipingInputContext(probabilisticPipingCalculation.InputParameters,
                                                              probabilisticPipingCalculation,
                                                              Enumerable.Empty<PipingSurfaceLine>(),
                                                              Enumerable.Empty<PipingStochasticSoilModel>(),
                                                              failureMechanism,
                                                              assessmentSection);

            // Call
            void Call() => new ProbabilisticPipingInputContextProperties(context, null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getNormativeAssessmentLevelFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PropertyChangeHandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var probabilisticPipingCalculation = new ProbabilisticPipingCalculation(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var context = new ProbabilisticPipingInputContext(probabilisticPipingCalculation.InputParameters,
                                                              probabilisticPipingCalculation,
                                                              Enumerable.Empty<PipingSurfaceLine>(),
                                                              Enumerable.Empty<PipingStochasticSoilModel>(),
                                                              failureMechanism,
                                                              assessmentSection);

            // Call
            void Call() => new ProbabilisticPipingInputContextProperties(context, AssessmentSectionTestHelper.GetTestAssessmentLevel, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("propertyChangeHandler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var probabilisticPipingCalculation = new ProbabilisticPipingCalculation(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var context = new ProbabilisticPipingInputContext(probabilisticPipingCalculation.InputParameters,
                                                              probabilisticPipingCalculation,
                                                              Enumerable.Empty<PipingSurfaceLine>(),
                                                              Enumerable.Empty<PipingStochasticSoilModel>(),
                                                              failureMechanism,
                                                              assessmentSection);

            // Call
            var properties = new ProbabilisticPipingInputContextProperties(context,
                                                                           AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                                           handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ProbabilisticPipingInputContext>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.AreSame(context, properties.Data);

            Assert.IsInstanceOf<LogNormalDistributionDesignVariableProperties>(properties.DampingFactorExit);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.DampingFactorExit));

            Assert.IsInstanceOf<NormalDistributionDesignVariableProperties>(properties.PhreaticLevelExit);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.PhreaticLevelExit));

            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariableProperties>(properties.SeepageLength);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.SeepageLength));

            Assert.IsInstanceOf<LogNormalDistributionDesignVariableProperties>(properties.ThicknessCoverageLayer);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.ThicknessCoverageLayer));

            Assert.IsInstanceOf<LogNormalDistributionDesignVariableProperties>(properties.EffectiveThicknessCoverageLayer);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.EffectiveThicknessCoverageLayer));

            Assert.IsInstanceOf<LogNormalDistributionDesignVariableProperties>(properties.ThicknessAquiferLayer);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.ThicknessAquiferLayer));

            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariableProperties>(properties.DarcyPermeability);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.DarcyPermeability));

            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariableProperties>(properties.Diameter70);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.Diameter70));

            Assert.IsInstanceOf<ShiftedLogNormalDistributionDesignVariableProperties>(properties.SaturatedVolumicWeightOfCoverageLayer);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.SaturatedVolumicWeightOfCoverageLayer));

            mocks.VerifyAll();
        }
    }
}