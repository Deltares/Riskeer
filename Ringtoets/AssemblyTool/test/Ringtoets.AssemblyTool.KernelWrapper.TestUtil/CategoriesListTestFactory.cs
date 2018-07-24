using System;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class that creates valid instances of <see cref="CategoriesList{TCategory}"/>
    /// which can be used for testing.
    /// </summary>
    public static class CategoriesListTestFactory
    {
        private static readonly Random random = new Random(21);

        /// <summary>
        /// Creates a valid instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="FmSectionCategory"/>.
        /// </summary>
        /// <returns>An instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="FmSectionCategory"/>.</returns>
        public static CategoriesList<FmSectionCategory> CreateFailureMechanismSectionCategories()
        {
            return new CategoriesList<FmSectionCategory>(new []
            {
                new FmSectionCategory(random.NextEnumValue<EFmSectionCategory>(), 0, 0.25),
                new FmSectionCategory(random.NextEnumValue<EFmSectionCategory>(), 0.25, 0.5),
                new FmSectionCategory(random.NextEnumValue<EFmSectionCategory>(), 0.5, 0.75),
                new FmSectionCategory(random.NextEnumValue<EFmSectionCategory>(), 0.75, 1.0)
            });
        }

        /// <summary>
        /// Creates a valid instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="FailureMechanismCategory"/>.
        /// </summary>
        /// <returns>An instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="FailureMechanismCategory"/>.</returns>
        public static CategoriesList<FailureMechanismCategory> CreateFailureMechanismCategories()
        {
            return new CategoriesList<FailureMechanismCategory>(new[]
            {
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0, 0.25),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.25, 0.5),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.5, 0.75),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.75, 1.0)
            });
        }

        /// <summary>
        /// Creates a valid instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="AssessmentSectionCategory"/>.
        /// </summary>
        /// <returns>An instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="AssessmentSectionCategory"/>.</returns>
        public static CategoriesList<AssessmentSectionCategory> CreateAssessmentSectionCategories()
        {
            return new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0, 0.25),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.25, 0.5),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.5, 0.75),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.75, 1.0)
            });
        }
    }
}