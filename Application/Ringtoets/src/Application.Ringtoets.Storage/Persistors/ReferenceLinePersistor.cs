using System.Collections.Generic;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    public class ReferenceLinePersistor
    {
        private readonly ReferenceLineConverter converter = new ReferenceLineConverter();

        /// <summary>
        /// Updates a (new) entity based on the values of <paramref name="model"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">The entity to update.</param>
        /// <param name="model">The model to use to update the entity.</param>
        public void UpdateModel(ICollection<ReferenceLinePointEntity> parentNavigationProperty, ReferenceLine model)
        {
        }

        /// <summary>
        /// Inserts a new entity based on the values of <paramref name="model"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">The collection where the new entity is added.</param>
        /// <param name="model">The model to use to update the entity.</param>
        public void InsertModel(ICollection<ReferenceLinePointEntity> parentNavigationProperty, ReferenceLine model)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ReferenceLine"/> based on the information in <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The database entity containing the information to set on the new model.</param>
        /// <returns>A new <see cref="ReferenceLine"/> with properties set from the database.</returns>
        public ReferenceLine LoadModel(ICollection<ReferenceLinePointEntity> entity)
        {
            return converter.ConvertEntityToModel(entity);
        }
    }
}