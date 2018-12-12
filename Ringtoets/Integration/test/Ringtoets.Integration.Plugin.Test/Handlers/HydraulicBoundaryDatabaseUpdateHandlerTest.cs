// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.HydraRing.IO.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.Plugin.Handlers;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseUpdateHandlerTest : NUnitFormTest
    {

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseUpdateHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            Assert.IsInstanceOf<IHydraulicBoundaryDatabaseUpdateHandler>(handler);
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.IsConfirmationRequired(null, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void IsConfirmationRequired_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.IsConfirmationRequired(new HydraulicBoundaryDatabase(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseNotLinked_ReturnsFalse()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));
            
            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(new HydraulicBoundaryDatabase(), ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsFalse(confirmationRequired);
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseLinkedAndReadDatabaseSameVersion_ReturnsFalse()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "some/file/path",
                Version = "version"
            };

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(database, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsFalse(confirmationRequired);
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseLinkedAndReadDatabaseDifferentVersion_ReturnsTrue()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "some/file/path",
                Version = "1"
            };

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(database, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsTrue(confirmationRequired);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_ClickDialog_ReturnTrueIfOkAndFalseIfCancel(bool clickOk)
        {
            // Setup
            string dialogTitle = null, dialogMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                dialogTitle = tester.Title;
                dialogMessage = tester.Text;
                if (clickOk)
                {
                    tester.ClickOk();
                }
                else
                {
                    tester.ClickCancel();
                }
            };

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(clickOk, result);

            Assert.AreEqual("Bevestigen", dialogTitle);
            Assert.AreEqual("U heeft een ander hydraulische belastingendatabase bestand geselecteerd. Als gevolg hiervan moet de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden." + Environment.NewLine +
                            Environment.NewLine +
                            "Wilt u doorgaan?",
                            dialogMessage);
        }

        [Test]
        public void Update_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.Update(null, ReadHydraulicBoundaryDatabaseTestFactory.Create(), ReadHydraulicLocationConfigurationDatabaseTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Update_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.Update(new HydraulicBoundaryDatabase(), null, ReadHydraulicLocationConfigurationDatabaseTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.Update(new HydraulicBoundaryDatabase(), ReadHydraulicBoundaryDatabaseTestFactory.Create(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicLocationConfigurationDatabase", exception.ParamName);
        }
    }
}