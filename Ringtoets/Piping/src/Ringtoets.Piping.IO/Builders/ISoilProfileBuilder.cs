using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Builders
{
    public interface ISoilProfileBuilder
    {
        /// <summary>
        /// Creates a new instances of the <see cref="PipingSoilProfile"/> based on the layer definitions.
        /// </summary>
        /// <returns>A new <see cref="PipingSoilProfile"/>.</returns>
        PipingSoilProfile Build();
    }
}