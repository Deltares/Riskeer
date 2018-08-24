﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.Integration.IO.Helpers;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="SerializableAssessmentSection"/>.
    /// </summary>
    public static class SerializableAssessmentSectionCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="SerializableAssessmentSection"/>
        /// based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate an id for the
        /// <see cref="SerializableAssessmentSection"/>.</param>
        /// <param name="assessmentSectionName">The name of the assessment section.</param>
        /// <param name="geometry">The geometry of the assessment section.</param>
        /// <returns>A <see cref="SerializableAssessmentSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableAssessmentSection Create(UniqueIdentifierGenerator idGenerator,
                                                           string assessmentSectionName,
                                                           IEnumerable<Point2D> geometry)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            return new SerializableAssessmentSection(idGenerator.GetNewId().ToString(),
                                                     assessmentSectionName,
                                                     geometry);
        }
    }
}