﻿using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export an assembly result for a failure mechanism section.
    /// </summary>
    public class ExportableSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyCategory">The assembly result.</param>
        public ExportableSectionAssemblyResult(ExportableFailureMechanismSectionAssemblyMethod assemblyMethod,
                                               FailureMechanismSectionAssemblyCategoryGroup assemblyCategory)
        {
            AssemblyMethod = assemblyMethod;
            AssemblyCategory = assemblyCategory;
        }

        /// <summary>
        ///  Gets the assembly method that was used to assemble the assembly result.
        /// </summary>
        public ExportableFailureMechanismSectionAssemblyMethod AssemblyMethod { get; }

        /// <summary>
        /// Gets the assembly category.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup AssemblyCategory { get; }
    }
}