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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationServiceIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_NoHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    DikeProfile = new DikeProfile(new Point2D(0, 0))
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
                }
            };

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0))
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NoDikeProfile_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen dijkprofiel geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_ValidInput_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0))
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        public void Calculate_ValidCalculation_LogStartAndEndAndReturnOutput()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = new DikeProfile(new Point2D(0, 0))
                    {
                        Orientation = (RoundedDouble) 5.5,
                        DikeGeometry =
                        {
                            new RoughnessPoint(new Point2D(1.1, 2.2), 0.6),
                            new RoughnessPoint(new Point2D(3.3, 4.4), 0.7)
                        },
                        DikeHeight = (RoundedDouble) 10
                    }
                }
            };

            var failureMechanismSection = assessmentSection.GrassCoverErosionInwards.Sections.First();
            ExceedanceProbabilityCalculationOutput output = null;

            // Call
            Action call = () => output = GrassCoverErosionInwardsCalculationService.Calculate(calculation,
                                                                                              testDataPath,
                                                                                              failureMechanismSection,
                                                                                              failureMechanismSection.Name,
                                                                                              assessmentSection.GrassCoverErosionInwards.GeneralInput);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
            });
            Assert.IsNotNull(output);
        }

        [Test]
        public void Calculate_InvalidCalculation_LogStartAndEndAndErrorMessageAndReturnNull()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
                }
            };

            var failureMechanismSection = assessmentSection.GrassCoverErosionInwards.Sections.First();
            ExceedanceProbabilityCalculationOutput output = null;

            // Call
            Action call = () => output = GrassCoverErosionInwardsCalculationService.Calculate(calculation,
                                                                                              testDataPath,
                                                                                              failureMechanismSection,
                                                                                              failureMechanismSection.Name,
                                                                                              assessmentSection.GrassCoverErosionInwards.GeneralInput);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("De berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt.", calculation.Name), msgs[1]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
            Assert.IsNull(output);
        }

        private void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }
        }
    }
}