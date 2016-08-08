namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// This interface describes components that obtain results from the output files of a Hydra-Ring calculation.
    /// </summary>
    public interface IHydraRingFileParser
    {
        /// <summary>
        /// Tries to parse output from a file in the <paramref name="workingDirectory"/> based on a <paramref name="sectionId"/>.
        /// </summary>
        /// <param name="workingDirectory">The path to the directory which contains the output of the Hydra-Ring type I calculation.</param>
        /// <param name="sectionId">The section id to get the output for.</param>
        void Parse(string workingDirectory, int sectionId);
    }
}