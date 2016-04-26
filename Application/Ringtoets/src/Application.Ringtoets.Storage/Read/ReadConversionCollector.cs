using System.Collections.Generic;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read
{
    public class ReadConversionCollector
    {
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = new Dictionary<SoilProfileEntity, PipingSoilProfile>(new ReferenceEqualityComparer<SoilProfileEntity>());

        internal void Add(SoilProfileEntity entity, PipingSoilProfile profile)
        {
            soilProfiles[entity] = profile;
        }

        internal bool Contains(SoilProfileEntity entity)
        {
            return soilProfiles.ContainsKey(entity);
        }

        internal PipingSoilProfile Get(SoilProfileEntity soilProfileEntity)
        {
            return soilProfiles[soilProfileEntity];
        }
    }
}