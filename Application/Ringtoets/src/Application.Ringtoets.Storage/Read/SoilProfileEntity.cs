using System;
using System.Linq;
using Application.Ringtoets.Storage.Read;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class SoilProfileEntity
    {
        public PipingSoilProfile Read(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            if (collector.Contains(this))
            {
                return collector.Get(this);
            }
            var layers = SoilLayerEntities.Select(sl => sl.Read());
            var pipingSoilProfile = new PipingSoilProfile(Name, Convert.ToDouble(Bottom), layers, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = SoilProfileEntityId
            };
            collector.Add(this, pipingSoilProfile);
            return pipingSoilProfile;
        }
    }
}