using System;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class SoilLayerEntity
    {
        public PipingSoilLayer Read()
        {
            return new PipingSoilLayer(Convert.ToDouble(Top))
            {
                StorageId = SoilLayerEntityId,
                IsAquifer = Convert.ToBoolean(IsAquifer)
            };
        }
    }
}