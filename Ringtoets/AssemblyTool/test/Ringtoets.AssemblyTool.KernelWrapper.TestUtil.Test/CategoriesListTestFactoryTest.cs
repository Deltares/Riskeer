using Assembly.Kernel.Model.CategoryLimits;
using NUnit.Framework;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class CategoriesListTestFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionCategories_Always_ReturnsValidCategoriesList()
        {
            // Call
            CategoriesList<FmSectionCategory> categories = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

            // Assert
            Assert.IsNotNull(categories);

            FmSectionCategory[] sectionCategories = categories.Categories;
            Assert.AreEqual(4, sectionCategories);

            Assert.AreEqual(0, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.25, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.25, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.5, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.5, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.75, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.75, sectionCategories[0].LowerLimit);
            Assert.AreEqual(1, sectionCategories[0].UpperLimit);
        }

        [Test]
        public void CreateFailureMechanismCategories_Always_ReturnsValidCategoriesList()
        {
            // Call
            CategoriesList<FailureMechanismCategory> categories = CategoriesListTestFactory.CreateFailureMechanismCategories();

            // Assert
            Assert.IsNotNull(categories);

            FailureMechanismCategory[] sectionCategories = categories.Categories;
            Assert.AreEqual(4, sectionCategories);

            Assert.AreEqual(0, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.25, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.25, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.5, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.5, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.75, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.75, sectionCategories[0].LowerLimit);
            Assert.AreEqual(1, sectionCategories[0].UpperLimit);
        }

        [Test]
        public void CreateAssessmentSectionCategories_Always_ReturnsValidCategoriesList()
        {
            // Call
            CategoriesList<AssessmentSectionCategory> categories = CategoriesListTestFactory.CreateAssessmentSectionCategories();

            // Assert
            Assert.IsNotNull(categories);

            AssessmentSectionCategory[] sectionCategories = categories.Categories;
            Assert.AreEqual(4, sectionCategories);

            Assert.AreEqual(0, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.25, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.25, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.5, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.5, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.75, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.75, sectionCategories[0].LowerLimit);
            Assert.AreEqual(1, sectionCategories[0].UpperLimit);
        }
    }
}