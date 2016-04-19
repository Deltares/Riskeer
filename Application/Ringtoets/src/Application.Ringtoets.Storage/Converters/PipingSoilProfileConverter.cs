using System;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Converters
{
    public class PipingSoilProfileConverter : IEntityConverter<PipingSoilProfile, SoilProfileEntity>
    {
        public PipingSoilProfile ConvertEntityToModel(SoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var layers = entity.SoilLayerEntities.Select(sl => new PipingSoilLayer((double) sl.Top)
            {
                IsAquifer = sl.IsAquifer == 1
            });

            return new PipingSoilProfile(entity.Name, (double) entity.Bottom, layers, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = entity.SoilProfileEntityId
            };
        }

        public void ConvertModelToEntity(PipingSoilProfile modelObject, SoilProfileEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.SoilProfileEntityId = modelObject.StorageId;
            entity.Bottom = Convert.ToDecimal(modelObject.Bottom);
            entity.Name = modelObject.Name;

            foreach (var sl in modelObject.Layers)
            {
                var layerEntity = new SoilLayerEntity
                {
                    IsAquifer = sl.IsAquifer ? (byte) 1 : (byte) 0,
                    Top = Convert.ToDecimal(sl.Top)
                };

                entity.SoilLayerEntities.Add(layerEntity);
            }
        }
    }
}