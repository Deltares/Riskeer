using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds the information to export the assembly result of a failure mechanism.
    /// </summary>
    public class ExportableFailureMechanismAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyCategory">The assembly result.</param>
        public ExportableFailureMechanismAssemblyResult(ExportableAssemblyMethod assemblyMethod,
                                                        FailureMechanismAssemblyCategoryGroup assemblyCategory)
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
        public FailureMechanismAssemblyCategoryGroup AssemblyCategory { get; }
    }
}