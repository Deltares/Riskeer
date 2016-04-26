using System;
using Application.Ringtoets.Storage.Read;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class StochasticSoilProfileEntity
    {
        public StochasticSoilProfile Read(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var profile = new StochasticSoilProfile(Convert.ToDouble(Probability), SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = StochasticSoilProfileEntityId
            };
            ReadSoilProfile(profile, collector);

            return profile;
        }

        private void ReadSoilProfile(StochasticSoilProfile profile, ReadConversionCollector collector)
        {
            profile.SoilProfile = SoilProfileEntity.Read(collector);
        }
    }
}