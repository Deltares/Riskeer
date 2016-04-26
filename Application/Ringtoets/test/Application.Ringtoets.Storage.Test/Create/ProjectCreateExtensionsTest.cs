using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class ProjectCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var project = new Project();

            // Call
            TestDelegate test = () => project.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsProjectEntityWithDescription()
        {
            // Setup
            var testdescription = "testDescription";
            var project = new Project
            {
                Description = testdescription
            };
            var collector = new CreateConversionCollector();

            // Call
            var entity = project.Create(collector);

            // Assert
            Assert.NotNull(entity);
            Assert.AreEqual(testdescription, entity.Description);
        }

        [Test]
        public void Create_WithAssessmentSections_AddsSectionsToEntity()
        {
            // Setup
            var project = new Project
            {
                Items =
                {
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                }
            };
            var collector = new CreateConversionCollector();

            // Call
            var entity = project.Create(collector);

            // Assert
            Assert.AreEqual(1, entity.AssessmentSectionEntities.Count);
        }
    }
}