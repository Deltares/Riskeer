using System;
using Application.Ringtoets.Storage.Read;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class StochasticSoilModelEntity
    {
        public StochasticSoilModel Read(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var model = new StochasticSoilModel(-1, Name, SegmentName)
            {
                StorageId = StochasticSoilModelEntityId
            };
            ReadStochasticSoilProfiles(model, collector);

            return model;
        }

        private void ReadStochasticSoilProfiles(StochasticSoilModel model, ReadConversionCollector collector)
        {
            foreach (var stochasticSoilProfileEntity in StochasticSoilProfileEntities)
            {
                model.StochasticSoilProfiles.Add(stochasticSoilProfileEntity.Read(collector));
            }
        }
    }
}