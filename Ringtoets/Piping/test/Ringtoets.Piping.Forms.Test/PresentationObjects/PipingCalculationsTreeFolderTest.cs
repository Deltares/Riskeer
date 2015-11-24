using System.Collections.Generic;
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
        public void ParameteredConstructor_ExpectedValues()
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
            var calculationInputPresentationObjects = calculationsFolder.Contents
                                                                        .OfType<PipingCalculationInputs>()
                                                                        .ToArray();
            foreach (var pipingCalculationInputs in calculationInputPresentationObjects)
            {
                CollectionAssert.AreEqual(failureMechanism.SurfaceLines, pipingCalculationInputs.AvailablePipingSurfaceLines);
                CollectionAssert.AreEqual(failureMechanism.SoilProfiles, pipingCalculationInputs.AvailablePipingSoilProfiles);
            }
            CollectionAssert.AreEqual(failureMechanism.Calculations, calculationInputPresentationObjects
                                                                         .Select(pci => pci.PipingData)
                                                                         .ToArray());
            Assert.AreEqual(TreeFolderCategory.General, calculationsFolder.Category);
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