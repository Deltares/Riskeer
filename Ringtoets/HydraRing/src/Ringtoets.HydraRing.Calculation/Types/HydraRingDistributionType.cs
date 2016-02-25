namespace Ringtoets.HydraRing.Calculation.Types
{
    /// <summary>
    /// Enumeration that defines the distribution types supported by Hydra-Ring.
    /// </summary>
    /// <remarks>
    /// The integer values correspond to distribution ids defined within Hydra-Ring.
    /// </remarks>
    public enum HydraRingDistributionType
    {
        Deterministic = 0,
        Normal = 2,
        LogNormal = 4 // Also applies to shifted log normal distributions
    }
}