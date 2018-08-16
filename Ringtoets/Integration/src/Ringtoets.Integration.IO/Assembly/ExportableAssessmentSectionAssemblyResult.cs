using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export the assembly result
    /// of an assessment section.
    /// </summary>
    public class ExportableAssessmentSectionAssemblyResult
    {
        /// <summary>
        /// Creates an instance of <see cref="ExportableAssessmentSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyCategory">The assembly result.</param>
        public ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod assemblyMethod,
                                                         AssessmentSectionAssemblyCategoryGroup assemblyCategory)
        {
            AssemblyMethod = assemblyMethod;
            AssemblyCategory = assemblyCategory;
        }

        /// <summary>
        /// Gets the assembly method that was used to assembly the assembly result.
        /// </summary>
        public ExportableAssemblyMethod AssemblyMethod { get; }

        /// <summary>
        /// Gets the assembly category.
        /// </summary>
        public AssessmentSectionAssemblyCategoryGroup AssemblyCategory { get; }
    }
}