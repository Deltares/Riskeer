namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class contains the column names that are used when querying the DSoil-Model database.
    /// </summary>
    internal static class SoilProfileDatabaseColumns
    {
        internal const string ProfileCount = "ProfileCount";
        internal const string Dimension = "Dimension";
        internal const string IsAquifer = "IsAquifer";
        internal const string ProfileName = "ProfileName";
        internal const string IntersectionX = "IntersectionX";
        internal const string Bottom = "Bottom";
        internal const string Top = "Top";
        internal const string LayerGeometry = "LayerGeometry";
        internal const string AbovePhreaticLevel = "AbovePhreaticLevel";
        internal const string BelowPhreaticLevel = "BelowPhreaticLevel";
        internal const string DryUnitWeight = "DryUnitWeight";
        internal const string LayerCount = "LayerCount";
    }
}