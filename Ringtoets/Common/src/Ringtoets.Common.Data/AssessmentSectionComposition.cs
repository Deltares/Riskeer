namespace Ringtoets.Common.Data
{
    /// <summary>
    /// Describes the configuration of an <see cref="IAssessmentSection"/>.
    /// </summary>
    public enum AssessmentSectionComposition
    {
        /// <summary>
        /// The assessment section consists only out of 'dike' elements.
        /// </summary>
        Dike,
        /// <summary>
        /// The assessment section consists only out of 'dune' elements.
        /// </summary>
        Dune,
        /// <summary>
        /// The assessment section consists out of a combination of 'dike' and 'dune' elements
        /// </summary>
        DikeAndDune
    }
}