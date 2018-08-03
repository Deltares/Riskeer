using System;
using System.ComponentModel;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Data.AssemblyTool
{
    /// <summary>
    /// The cpnverter that <see cref="ManualFailureMechanismSectionAssemblyCategoryGroup"/>
    /// to <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
    /// </summary>
    public static class ManualFailureMechanismSectionAssemblyCategoryGroupConverter
    {
        /// <summary>
        /// Converts a <see cref="ManualFailureMechanismSectionAssemblyCategoryGroup"/> into a
        /// <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="categoryGroup">The <see cref="ManualFailureMechanismSectionAssemblyCategoryGroup"/> to convert.</param>
        /// <returns>The <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on <paramref name="categoryGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryGroup"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryGroup"/> is a valid value, but not supported.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup Convert(ManualFailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            if (!Enum.IsDefined(typeof(ManualFailureMechanismSectionAssemblyCategoryGroup), categoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(categoryGroup),
                                                       (int) categoryGroup,
                                                       typeof(ManualFailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (categoryGroup)
            {
                case ManualFailureMechanismSectionAssemblyCategoryGroup.None:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}