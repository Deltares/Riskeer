// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsViewInfoTest : ViewInfoTest<GrassCoverErosionOutwardsDesignWaterLevelLocationsView>
    {
        protected override Type DataType
        {
            get
            {
                return typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext);
            }
        }

        protected override Type ViewDataType
        {
            get
            {
                return typeof(IEnumerable<HydraulicBoundaryLocation>);
                
            }
        }

        protected override PluginBase CreatePlugin()
        {
            return new GrassCoverErosionOutwardsPlugin();
        }
    }

    public abstract class ViewInfoTest<TView>
    {
        private PluginBase plugin;
        private ViewInfo Info { get; set; }

        protected abstract Type DataType { get; }
        protected abstract Type ViewDataType { get; }

        [SetUp]
        public void SetUp()
        {
            plugin = CreatePlugin();
            Info = plugin.GetViewInfos().FirstOrDefault(vi => vi.ViewType == typeof(TView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [TestCase]
        public void Initialized_Always_DataTypeAndViewTypeAsExpected()
        {
            Assert.NotNull(Info, "Expected a viewInfo definition for views with type {0}.", typeof(TView));
            Assert.AreEqual(DataType, Info.DataType);
        }

        protected virtual PluginBase CreatePlugin()
        {
            return null;
        }
    }
}