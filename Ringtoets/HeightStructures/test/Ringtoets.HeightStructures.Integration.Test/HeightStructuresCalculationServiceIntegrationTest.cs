﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.HeightStructures.Integration.Test
{
    [TestFixture]
    public class HeightStructuresCalculationServiceIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_InvalidCalculationInputValidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            const string name = "<very nice name>";

            HeightStructuresCalculation calculation = new HeightStructuresCalculation
            {
                Name = name
            };


            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaarde locatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_ValidCalculationInputAndInvalidHydraulicBoundaryDatabase_ReturnsFalse()
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

            HeightStructuresCalculation calculation = new HeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };


            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

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
        public void Validate_ValidCalculationInputAndHydraulicBoundaryDatabase_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            const string name = "<very nice name>";

            HeightStructuresCalculation calculation = new HeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };


            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

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

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            var failureMechanismSection = failureMechanism.Sections.First();
            ExceedanceProbabilityCalculationOutput output = null;

            // Call
            Action call = () => output = HeightStructuresCalculationService.Calculate(calculation, testDataPath, failureMechanismSection, failureMechanismSection.Name, failureMechanism.GeneralInput);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
            });
            Assert.IsNotNull(output);
        }

        [Test]
        public void Calculate_InvalidCalculation_LogStartAndEndAndErrorMessageAndReturnNull()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
                }
            };

            var failureMechanismSection = failureMechanism.Sections.First();
            ExceedanceProbabilityCalculationOutput output = null;

            // Call
            Action call = () => output = HeightStructuresCalculationService.Calculate(calculation, testDataPath, failureMechanismSection, failureMechanismSection.Name, failureMechanism.GeneralInput);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("Hoogte kunstwerk '{0}' niet gelukt.", calculation.Name), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
            Assert.IsNull(output);
        }
    }
}