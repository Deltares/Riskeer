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

using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Riskeer.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextPropertiesIntegrationTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void DikeProfile_CalculationDikeProfileSetToOtherSection_SecondSectionResultCalculationSetFirstSectionResultCalculationNull()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            DikeProfile dikeProfile1 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.51, 0.51), "id1");
            DikeProfile dikeProfile2 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.51, 1.51), "id2");

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile1
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile1,
                dikeProfile2
            }, "path");

            failureMechanism.CalculationsGroup.Children.Add(calculation);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("firstSection", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(1.1, 1.1)
                }),
                new FailureMechanismSection("secondSection", new[]
                {
                    new Point2D(1.1, 1.1),
                    new Point2D(2.2, 2.2)
                })
            });

            GrassCoverErosionInwardsFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults.ToArray();
            sectionResults[0].Calculation = calculation;

            var inputContext = new GrassCoverErosionInwardsInputContext(calculation.InputParameters, calculation, failureMechanism, assessmentSection);

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, customHandler);

            // Call
            properties.DikeProfile = dikeProfile2;

            // Assert
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation, sectionResults[1].Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DikeProfile_CalculationDikeProfileSetToOtherSection_SecondSectionResultCalculationUnchangedFirstSectionResultCalculationNull()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            DikeProfile dikeProfile1 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.51, 0.51), "id1");
            DikeProfile dikeProfile2 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.51, 1.51), "id2");

            var calculation1 = new GrassCoverErosionInwardsCalculation
            {
                Name = "firstCalculation",
                InputParameters =
                {
                    DikeProfile = dikeProfile1
                }
            };
            var calculation2 = new GrassCoverErosionInwardsCalculation
            {
                Name = "secondCalculation",
                InputParameters =
                {
                    DikeProfile = dikeProfile2
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile1,
                dikeProfile2
            }, "path");

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("firstSection", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(1.1, 1.1)
                }),
                new FailureMechanismSection("secondSection", new[]
                {
                    new Point2D(1.1, 1.1),
                    new Point2D(2.2, 2.2)
                })
            });

            GrassCoverErosionInwardsFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults.ToArray();
            sectionResults[0].Calculation = calculation1;
            sectionResults[1].Calculation = calculation2;

            var inputContext = new GrassCoverErosionInwardsInputContext(calculation1.InputParameters, calculation1, failureMechanism, assessmentSection);

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, customHandler);

            // Call
            properties.DikeProfile = dikeProfile2;

            // Assert
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation2, sectionResults[1].Calculation);
            mockRepository.VerifyAll();
        }
    }
}