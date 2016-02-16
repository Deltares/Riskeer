﻿using System;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class represents a location for which there are points along the surface line
    /// which are characterizing for that surface line.
    /// </summary>
    public class PipingCharacteristicPointsLocation
    {
        /// <summary>
        /// Creates an instance of <see cref="PipingCharacteristicPointsLocation"/>.
        /// </summary>
        /// <param name="name">The name of the location for which the <see cref="PipingCharacteristicPointsLocation"/>
        /// defines characteristic points.</param>
        public PipingCharacteristicPointsLocation(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "Cannot make a definition of characteristic points for an unknown location.");
            }
            Name = name;
        }

        /// <summary>
        /// Gets the name of the location for which the <see cref="PipingCharacteristicPointsLocation"/> defines
        /// characteristic points.
        /// </summary>
        public string Name { get; private set; }
    }
}