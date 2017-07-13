using System;
using System.Collections.Generic;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Represents a single illustration point to be used in 
    /// <see cref="IllustrationPointsControl"/>.
    /// </summary>
    public class IllustrationPointControlItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointControlItem"/>.
        /// </summary>
        /// <param name="source">The wrapped source object.</param>
        /// <param name="windDirectionName">The name of the wind direction.</param>
        /// <param name="closingSituation">The closing situation of the illustration
        /// point.</param>
        /// <param name="stochasts">The associated stochasts.</param>
        /// <param name="beta">The beta of the illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when parameter 
        /// <paramref name="source"/>, <paramref name="windDirectionName"/>,
        /// <paramref name="closingSituation"/> or <paramref name="stochasts"/> is 
        /// <c>null</c>.</exception>
        public IllustrationPointControlItem(object source,
                                            string windDirectionName,
                                            string closingSituation,
                                            IEnumerable<Stochast> stochasts,
                                            RoundedDouble beta)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (windDirectionName == null)
            {
                throw new ArgumentNullException(nameof(windDirectionName));
            }
            if (closingSituation == null)
            {
                throw new ArgumentNullException(nameof(closingSituation));
            }
            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }

            Source = source;
            WindDirectionName = windDirectionName;
            ClosingSituation = closingSituation;
            Beta = beta;
            Stochasts = stochasts;
        }

        /// <summary>
        /// Gets the wrapped source object.
        /// </summary>
        public object Source { get; }

        /// <summary>
        /// Gets the wind direction name.
        /// </summary>
        public string WindDirectionName { get; }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingSituation { get; }

        /// <summary>
        /// Gets the beta.
        /// </summary>
        public RoundedDouble Beta { get; }

        /// <summary>
        /// Gets the associated stochasts.
        /// </summary>
        public IEnumerable<Stochast> Stochasts { get; }
    }
}