using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Denotes a <see cref="SurfaceLinePointEntity"/> as being used to mark a particular
    /// characteristic point of a <see cref="RingtoetsPipingSurfaceLine"/>.
    /// </summary>
    public enum CharacteristicPointType
    {
        /// <summary>
        /// Corresponds <see cref="SurfaceLinePointEntity"/> to <see cref="RingtoetsPipingSurfaceLine.DikeToeAtRiver"/>.
        /// </summary>
        DikeToeAtRiver = 1,
        /// <summary>
        /// Corresponds <see cref="SurfaceLinePointEntity"/> to <see cref="RingtoetsPipingSurfaceLine.DikeToeAtPolder"/>.
        /// </summary>
        DikeToeAtPolder = 2,
        /// <summary>
        /// Corresponds <see cref="SurfaceLinePointEntity"/> to <see cref="RingtoetsPipingSurfaceLine.DitchDikeSide"/>.
        /// </summary>
        DitchDikeSide = 3,
        /// <summary>
        /// Corresponds <see cref="SurfaceLinePointEntity"/> to <see cref="RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/>.
        /// </summary>
        BottomDitchDikeSide = 4,
        /// <summary>
        /// Corresponds <see cref="SurfaceLinePointEntity"/> to <see cref="RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/>.
        /// </summary>
        BottomDitchPolderSide = 5,
        /// <summary>
        /// Corresponds <see cref="SurfaceLinePointEntity"/> to <see cref="RingtoetsPipingSurfaceLine.DitchPolderSide"/>.
        /// </summary>
        DitchPolderSide = 6
    }
}