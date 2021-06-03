// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Factories;
using Riskeer.Integration.Forms.Properties;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class is a view showing reference line map data for an assessment section.
    /// </summary>
    public partial class AssessmentSectionReferenceLineView : UserControl, IMapView
    {
        private readonly MapLineData referenceLineMapData;

        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionReferenceLineView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionReferenceLineView(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();

            AssessmentSection = assessmentSection;

            MapDataCollection = new MapDataCollection(Resources.AssessmentSectionMap_DisplayName);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();

            MapDataCollection.Add(referenceLineMapData);
        }

        public object Data { get; set; }

        public IMapControl Map => riskeerMapControl.MapControl;

        /// <summary>
        /// Gets the <see cref="IAssessmentSection"/>.
        /// </summary>
        protected IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the <see cref="MapDataCollection"/>.
        /// </summary>
        protected MapDataCollection MapDataCollection { get; }

        protected override void OnLoad(EventArgs e)
        {
            CreateObservers();

            SetAllMapDataFeatures();

            riskeerMapControl.SetAllData(MapDataCollection, AssessmentSection.BackgroundData);

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            referenceLineObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates observers for the related data.
        /// </summary>
        protected virtual void CreateObservers()
        {
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection
            };

            referenceLineObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection.ReferenceLine
            };
        }

        /// <summary>
        /// Sets all map data features.
        /// </summary>
        protected virtual void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
        }

        #region ReferenceLine MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            referenceLineMapData.Features = RiskeerMapDataFeaturesFactory.CreateReferenceLineFeatures(AssessmentSection.ReferenceLine, AssessmentSection.Id, AssessmentSection.Name);
        }

        #endregion
    }
}