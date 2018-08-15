namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Enum representing the exportable failure mechanism types.
    /// </summary>
    public enum ExportableFailureMechanismType
    {
        /// <summary>
        /// Represents the failure mechanism macro stability inwards.
        /// </summary>
        STBI = 1,

        /// <summary>
        /// Represents the failure mechanism macro stability outwards.
        /// </summary>
        STBU = 2,

        /// <summary>
        /// Represents the failure mechanism piping.
        /// </summary>
        STPH = 3,

        /// <summary>
        /// Represents the failure mechanism microstability.
        /// </summary>
        STMI = 4,

        /// <summary>
        /// Represents the failure mechanism wave impact asphalt cover.
        /// </summary>
        AGK = 5,

        /// <summary>
        /// Represents the failure mechanism water pressure asphalt cover.
        /// </summary>
        AWO = 6,

        /// <summary>
        /// Represents the failure mechanism grass cover erosion outwards.
        /// </summary>
        GEBU = 7,

        /// <summary>
        /// Represents the failure mechanism grass cover slipoff outwards.
        /// </summary>
        GABU = 8,

        /// <summary>
        /// Represents the failure mechanism grass cover erosion inwards.
        /// </summary>
        GEKB = 9,

        /// <summary>
        /// Represents the failure mechanism grass cover slipoff inwards.
        /// </summary>
        GABI = 10,

        /// <summary>
        /// Represents the failure mechanism stability stone cover.
        /// </summary>
        ZST = 11,

        /// <summary>
        /// Represents the failure mechanism dune erosion.
        /// </summary>
        DA = 12,

        /// <summary>
        /// Represents the failure mechanism height structurews.
        /// </summary>
        HTKW = 13,

        /// <summary>
        /// Represents the failure mechanism closing structures.
        /// </summary>
        BSKW = 14,

        /// <summary>
        /// Represents the failure mechanism piping structures.
        /// </summary>
        PKW = 15,

        /// <summary>
        /// Represents the failure mechanism stability point structures.
        /// </summary>
        STKWp = 16,

        /// <summary>
        /// Represents the failure mechanism strength stability lengthwise construction.
        /// </summary>
        STKWl = 17,

        /// <summary>
        /// Represents the failure mechanism technical innovation.
        /// </summary>
        INN = 18
    }
}