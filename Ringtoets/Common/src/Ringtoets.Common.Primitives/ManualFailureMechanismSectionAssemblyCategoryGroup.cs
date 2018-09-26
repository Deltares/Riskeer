using Core.Common.Util.Attributes;
using RingtoetsCommonPrimitivesResources = Ringtoets.Common.Primitives.Properties.Resources;

namespace Ringtoets.Common.Primitives
{
    public enum ManualFailureMechanismSectionAssemblyCategoryGroup
    {
        /// <summary>
        /// No option has been selected for this failure
        /// mechanism section.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_None_DisplayName))]
        None = 1,

        /// <summary>
        /// The failure mechanism section is not applicable.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_NotApplicable_DisplayName))]
        NotApplicable = 2,

        /// <summary>
        /// Represents the assembly category Iv.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_Iv_DisplayName))]
        Iv = 3,

        /// <summary>
        /// Represents the assembly category IIv.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_IIv_DisplayName))]
        IIv = 4,

        /// <summary>
        /// Represents the assembly category Vv.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_Vv_DisplayName))]
        Vv = 5,

        /// <summary>
        /// Represents the assembly category VIIv.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_VIIv_DisplayName))]
        VIIv = 6
    }
}