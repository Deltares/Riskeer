using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    public class ReferenceLinePersistor
    {
        private readonly ReferenceLineConverter converter;
        private IRingtoetsEntities context;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationEntityPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public ReferenceLinePersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }

            context = ringtoetsContext;
            converter = new ReferenceLineConverter();
        }

        /// <summary>
        /// Inserts the <paramref name="referenceLine"/> as points into the <paramref name="entityCollection"/>.
        /// </summary>
        /// <param name="entityCollection">The collection where the entities are added.</param>
        /// <param name="referenceLine">The reference line which will be added tot the <paramref name="entityCollection"/>
        ///  as entities.</param>
        public void InsertModel(ICollection<ReferenceLinePointEntity> entityCollection, ReferenceLine referenceLine)
        {
            if (referenceLine != null)
            {
                if (entityCollection == null)
                {
                    throw new ArgumentNullException("entityCollection");
                }
                context.Set<ReferenceLinePointEntity>().RemoveRange(entityCollection);
                converter.ConvertModelToEntity(referenceLine, entityCollection);
            }
        }

        /// <summary>
        /// Creates a new <see cref="ReferenceLine"/> based on the information in <paramref name="entityCollection"/>.
        /// </summary>
        /// <param name="entityCollection">The database entity containing the information to set on the new model.</param>
        /// <returns>A new <see cref="ReferenceLine"/> with properties set from the database.</returns>
        public ReferenceLine LoadModel(ICollection<ReferenceLinePointEntity> entityCollection)
        {
            if (entityCollection == null)
            {
                throw new ArgumentNullException("entityCollection");
            }
            return converter.ConvertEntityToModel(entityCollection);
        }
    }
}