﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Common.Base;
using Riskeer.Piping.Data.Properties;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Piping.Data.SoilProfile
{
    /// <summary>
    /// A collection of <see cref="PipingStochasticSoilModel"/>. 
    /// The names of the <see cref="PipingStochasticSoilModel"/> elements are unique within 
    /// the collection.
    /// </summary>
    public class PipingStochasticSoilModelCollection : ObservableUniqueItemCollectionWithSourcePath<PipingStochasticSoilModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModelCollection"/>.
        /// </summary>
        public PipingStochasticSoilModelCollection()
            : base(model => model.Name,
                   RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                   Resources.UniqueFeature_Name_FeatureDescription) {}
    }
}