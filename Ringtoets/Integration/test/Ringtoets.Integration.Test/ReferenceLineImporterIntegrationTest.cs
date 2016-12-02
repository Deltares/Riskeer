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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.TestUtils;

namespace Ringtoets.Integration.Test
{
    [TestFixture]
    public class ReferenceLineImporterIntegrationTest : NUnitFormsAssertTest
    {
        [Test]
        public void GivenAssessmentSectionWithReferenceLine_WhenCancellingReferenceLineImport_ThenKeepOriginalReferenceLine()
        {
            // Given
            var originalReferenceLine = new ReferenceLine();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                ReferenceLine = originalReferenceLine
            };

            var handler = new ReferenceLineReplacementHandler();
            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);
            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);

                messageBoxTitle = messageBoxTester.Title;
                messageBoxText = messageBoxTester.Text;

                messageBoxTester.ClickCancel();
            };

            // When
            bool importSuccesful = importer.Import();

            // Then
            Assert.IsFalse(importSuccesful);
            Assert.AreSame(originalReferenceLine, assessmentSection.ReferenceLine);

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            var expectedText = "Na het importeren van een aangepaste ligging van de referentielijn zullen alle geïmporteerde en berekende gegevens van alle toetssporen worden gewist." + Environment.NewLine +
                               Environment.NewLine + "Wilt u doorgaan?";
            Assert.AreEqual(expectedText, messageBoxText);
        }

        [Test]
        public void GivenAssessmentSectionWithReferenceLineAndOtherData_WhenImportingReferenceLine_ThenReferenceLineReplacedAndReferenceLineDependentDataCleared()
        {
            // Given
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mocks = new MockRepository();
            var failureMechanismObserver = mocks.StrictMock<IObserver>();
            failureMechanismObserver.Expect(o => o.UpdateObserver())
                                    .Repeat.Times(assessmentSection.GetFailureMechanisms().Count());
            var assessmentSectionObserver = mocks.StrictMock<IObserver>();
            assessmentSectionObserver.Expect(o => o.UpdateObserver());
            var surfaceLinesObserver = mocks.StrictMock<IObserver>();
            surfaceLinesObserver.Expect(o => o.UpdateObserver());
            var stochasticSoilModelsObserver = mocks.StrictMock<IObserver>();
            stochasticSoilModelsObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            DataImportHelper.ImportReferenceLine(assessmentSection);
            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
            }
            DataImportHelper.ImportPipingSurfaceLines(assessmentSection);
            DataImportHelper.ImportPipingStochasticSoilModels(assessmentSection);

            ReferenceLine originalReferenceLine = assessmentSection.ReferenceLine;

            var handler = new ReferenceLineReplacementHandler();
            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);

                messageBoxTitle = messageBoxTester.Title;
                messageBoxText = messageBoxTester.Text;

                messageBoxTester.ClickOk();
            };

            assessmentSection.Attach(assessmentSectionObserver);
            foreach (var failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                failureMechanism.Attach(failureMechanismObserver);
            }
            assessmentSection.PipingFailureMechanism.StochasticSoilModels.Attach(stochasticSoilModelsObserver);
            assessmentSection.PipingFailureMechanism.SurfaceLines.Attach(surfaceLinesObserver);

            // When
            bool importSuccesful = importer.Import();
            importer.DoPostImportUpdates();

            // Then
            Assert.IsTrue(importSuccesful);
            Assert.AreNotSame(originalReferenceLine, assessmentSection.ReferenceLine);
            Point2D[] point2Ds = assessmentSection.ReferenceLine.Points.ToArray();
            Assert.AreEqual(803, point2Ds.Length);
            Assert.AreEqual(198237.375, point2Ds[123].X, 1e-6);
            Assert.AreEqual(514879.781, point2Ds[123].Y, 1e-6);

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                CollectionAssert.IsEmpty(failureMechanism.Sections);
            }
            CollectionAssert.IsEmpty(assessmentSection.PipingFailureMechanism.SurfaceLines);
            CollectionAssert.IsEmpty(assessmentSection.PipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(assessmentSection.PipingFailureMechanism.CalculationsGroup.Children);

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            var expectedText = "Na het importeren van een aangepaste ligging van de referentielijn zullen alle geïmporteerde en berekende gegevens van alle toetssporen worden gewist." + Environment.NewLine +
                               Environment.NewLine + "Wilt u doorgaan?";
            Assert.AreEqual(expectedText, messageBoxText);

            mocks.VerifyAll();
        }
    }
}