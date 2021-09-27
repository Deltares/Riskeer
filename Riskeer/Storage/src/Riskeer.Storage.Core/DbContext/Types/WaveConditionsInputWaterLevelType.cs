namespace Riskeer.Storage.Core.DbContext
{
    /// <summary>
    /// Enumeration that defines the possible water level types in the application for wave condition inputs.
    /// </summary>
    public enum WaveConditionsInputWaterLevelType
    {
        /// <summary>
        /// No water level.
        /// </summary>
        None = 1,
        
        /// <summary>
        /// The water level corresponding to the lower limit norm.
        /// </summary>
        LowerLimit = 2,
        
        /// <summary>
        /// The water level corresponding to the signaling norm.
        /// </summary>
        Signaling = 3,
        
        /// <summary>
        /// The water level corresponding to a user defined target probability.
        /// </summary>
        UserDefinedTargetProbability = 4
    }
}