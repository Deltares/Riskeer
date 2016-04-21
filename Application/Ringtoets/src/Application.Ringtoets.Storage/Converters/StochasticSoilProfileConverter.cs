using System;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Converts <see cref="StochasticSoilProfile"/> and underlying objects into a <see cref="StochasticSoilProfileEntity"/>
    /// and underlying entities; and vice versa. 
    /// </summary>
    public class StochasticSoilProfileConverter : IEntityConverter<StochasticSoilProfile, StochasticSoilProfileEntity>
    {
        public StochasticSoilProfile ConvertEntityToModel(StochasticSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var layers = entity.SoilProfileEntity.SoilLayerEntities.Select(sl => new PipingSoilLayer((double) sl.Top)
            {
                IsAquifer = sl.IsAquifer == 1
            });

            return new StochasticSoilProfile((double) entity.Probability, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new PipingSoilProfile(entity.SoilProfileEntity.Name, (double) entity.SoilProfileEntity.Bottom, layers, SoilProfileType.SoilProfile1D, -1),
                StorageId = entity.StochasticSoilProfileEntityId
            };
        }

        public void ConvertModelToEntity(StochasticSoilProfile modelObject, StochasticSoilProfileEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.StochasticSoilProfileEntityId = modelObject.StorageId;
            entity.Probability = Convert.ToDecimal(modelObject.Probability);
            entity.SoilProfileEntity = new SoilProfileEntity
            {
                Bottom = Convert.ToDecimal(modelObject.SoilProfile.Bottom),
                Name = modelObject.SoilProfile.Name
            };

            foreach (var sl in modelObject.SoilProfile.Layers)
            {
                var layerEntity = new SoilLayerEntity
                {
                    IsAquifer = sl.IsAquifer ? (byte) 1 : (byte) 0,
                    Top = Convert.ToDecimal(sl.Top)
                };

                entity.SoilProfileEntity.SoilLayerEntities.Add(layerEntity);
            }
        }
    }
}