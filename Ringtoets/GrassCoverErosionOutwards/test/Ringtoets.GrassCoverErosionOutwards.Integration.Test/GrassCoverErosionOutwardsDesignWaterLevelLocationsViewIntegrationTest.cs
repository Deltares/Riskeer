﻿using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.Integration.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsViewIntegrationTest
    {
        private const int locationCalculateColumnIndex = 0;

        private Form testForm;
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(false, false, "De bijdrage van dit toetsspoor is nul.")]
        [TestCase(true, false, "De bijdrage van dit toetsspoor is nul.")]
        [TestCase(false, true, "Er zijn geen berekeningen geselecteerd.")]
        [TestCase(true, true, "")]
        public void CalculateForSelectedButton_FailureMechanismContributionChanged_ButtonAndErrorMessageSyncedAccordingly(bool rowSelected, bool contributionAfterChangeNotZero, string expectedErrorMessage)
        {
            // Setup
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();
            view.AssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            if (rowSelected)
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var rows = dataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            if (!contributionAfterChangeNotZero)
            {
                failureMechanism.Contribution = 5;
            }
            view.FailureMechanism = failureMechanism;

            // Precondition
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && !contributionAfterChangeNotZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreNotEqual(expectedErrorMessage, errorProvider.GetError(button));

            // Call
            failureMechanism.Contribution = contributionAfterChangeNotZero ? 5 : 0;
            view.AssessmentSection.NotifyObservers();

            // Assert
            Assert.AreEqual(rowSelected && contributionAfterChangeNotZero, button.Enabled);
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        private GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowDesignWaterLevelLocationsView()
        {
            var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView()
        {
            var view = ShowDesignWaterLevelLocationsView();
            view.Data = new ObservableList<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(1.23)
                },
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(2.45)
                }
            };
            return view;
        }
    }
}