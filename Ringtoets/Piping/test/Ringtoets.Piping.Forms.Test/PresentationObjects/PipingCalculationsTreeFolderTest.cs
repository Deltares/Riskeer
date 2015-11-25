﻿using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingCalculationsTreeFolderTest
    {
        [Test]
        public void ParameteredConstructor_FailureMechanismWithOneCalculation_ExpectedValues()
        {
            // Setup
            const string folderName = "Berekeningen";
            var failureMechanism = new PipingFailureMechanism();
            AddTestSurfaceLines(failureMechanism);
            AddTestSoilProfiles(failureMechanism);

            // Call
            var calculationsFolder = new PipingCalculationsTreeFolder(folderName, failureMechanism);

            // Assert
            Assert.IsInstanceOf<CategoryTreeFolder>(calculationsFolder);
            Assert.AreEqual(folderName, calculationsFolder.Name);
            Assert.AreSame(failureMechanism, calculationsFolder.ParentFailureMechanism);

            object[] contentsAsArray = calculationsFolder.Contents.OfType<object>().ToArray();
            Assert.AreEqual(1, contentsAsArray.Length);
            var calculationPresentationObjects = contentsAsArray.Cast<PipingCalculationContext>()
                                                                .ToArray();
            foreach (var pipingCalculationContext in calculationPresentationObjects)
            {
                CollectionAssert.AreEqual(failureMechanism.SurfaceLines, pipingCalculationContext.AvailablePipingSurfaceLines);
                CollectionAssert.AreEqual(failureMechanism.SoilProfiles, pipingCalculationContext.AvailablePipingSoilProfiles);
            }
            CollectionAssert.AreEqual(failureMechanism.Calculations, calculationPresentationObjects
                                                                         .Select(pci => pci.WrappedPipingCalculation)
                                                                         .ToArray());
            Assert.AreEqual(TreeFolderCategory.General, calculationsFolder.Category);
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismWithOneEmptyCalculationGroup_ExpectedValues()
        {
            // Setup
            const string folderName = "Berekeningen";

            var group = new PipingCalculationGroup();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Calculations.Clear();
            failureMechanism.Calculations.Add(group);

            // Call
            var calculationsFolder = new PipingCalculationsTreeFolder(folderName, failureMechanism);

            // Assert
            Assert.IsInstanceOf<CategoryTreeFolder>(calculationsFolder);
            Assert.AreEqual(folderName, calculationsFolder.Name);
            Assert.AreSame(failureMechanism, calculationsFolder.ParentFailureMechanism);

            object[] contentsAsArray = calculationsFolder.Contents.OfType<object>().ToArray();
            Assert.AreEqual(1, contentsAsArray.Length);
            CollectionAssert.AreEqual(new[]{group}, contentsAsArray);
        }

        private void AddTestSurfaceLines(PipingFailureMechanism failureMechanism)
        {
            var collection = (ICollection<RingtoetsPipingSurfaceLine>)failureMechanism.SurfaceLines;
            collection.Add(new RingtoetsPipingSurfaceLine{Name = "A"});
            collection.Add(new RingtoetsPipingSurfaceLine{Name = "B"});
        }

        private void AddTestSoilProfiles(PipingFailureMechanism failureMechanism)
        {
            var collection = (ICollection<PipingSoilProfile>)failureMechanism.SoilProfiles;
            collection.Add(new TestPipingSoilProfile());
            collection.Add(new TestPipingSoilProfile());
        }
    }
}