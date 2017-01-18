// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Used to make paths strongly typed, see <see cref="TestHelper.GetTestDataPath(TestDataPath)"/>
    /// </summary>
    public class TestDataPath
    {
        public string Path { get; private set; }

        public static implicit operator TestDataPath(string path)
        {
            return new TestDataPath
            {
                Path = path
            };
        }

        public static class Application
        {
            public static class Ringtoets
            {
                public static readonly TestDataPath Storage = System.IO.Path.Combine("Application", "Ringtoets", "test", "Application.Ringtoets.Storage.Test");
            }
        }

        public static class Core
        {
            public static class Common
            {
                public static readonly TestDataPath Gui = System.IO.Path.Combine("Core", "Common", "test", "Core.Common.Gui.Test");
                public static readonly TestDataPath Utils = System.IO.Path.Combine("Core", "Common", "test", "Core.Common.Utils.Test");
                public static readonly TestDataPath IO = System.IO.Path.Combine("Core", "Common", "test", "Core.Common.IO.Test");
            }

            public static class Components
            {
                public static class Gis
                {
                    public static readonly TestDataPath IO = System.IO.Path.Combine("Core", "Components", "test", "Core.Components.Gis.IO.Test");
                }
            }
        }

        public static class Migration
        {
            public static class Core
            {
                public static readonly TestDataPath Storage = System.IO.Path.Combine("Migration", "Core", "test", "Migration.Core.Storage.Test");
            }
        }

        public static class Ringtoets
        {
            public static class ClosingStructures
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "ClosingStructures", "test", "Ringtoets.ClosingStructures.IO.Test");
            }

            public static class Common
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Common", "test", "Ringtoets.Common.IO.Test");
                public static readonly TestDataPath Service = System.IO.Path.Combine("Ringtoets", "Common", "test", "Ringtoets.Common.Service.Test");
            }

            public static class DuneErosion
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "DuneErosion", "test", "Ringtoets.DuneErosion.IO.Test");
            }

            public static class GrassCoverErosionInwards
            {
                public static readonly TestDataPath Integration = System.IO.Path.Combine("Ringtoets", "GrassCoverErosionInwards", "test", "Ringtoets.GrassCoverErosionInwards.Integration.Test");
            }

            public static class GrassCoverErosionOutwards
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "GrassCoverErosionOutwards", "test", "Ringtoets.GrassCoverErosionOutwards.IO.Test");
            }

            public static class HeightStructures
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "HeightStructures", "test", "Ringtoets.HeightStructures.IO.Test");
                public static readonly TestDataPath Integration = System.IO.Path.Combine("Ringtoets", "HeightStructures", "test", "Ringtoets.HeightStructures.Integration.Test");
            }

            public static class HydraRing
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "HydraRing", "test", "Ringtoets.HydraRing.IO.Test");
                public static readonly TestDataPath Calculation = System.IO.Path.Combine("Ringtoets", "HydraRing", "test", "Ringtoets.HydraRing.Calculation.Test");
            }

            public static class Integration
            {
                public static readonly TestDataPath Forms = System.IO.Path.Combine("Ringtoets", "Integration", "test", "Ringtoets.Integration.Forms.Test");
                public static readonly TestDataPath Service = System.IO.Path.Combine("Ringtoets", "Integration", "test", "Ringtoets.Integration.Service.Test");
                public static readonly TestDataPath Plugin = System.IO.Path.Combine("Ringtoets", "Integration", "test", "Ringtoets.Integration.Plugin.Test");
            }

            public static class Piping
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Piping", "test", "Ringtoets.Piping.IO.Test");
                public static readonly TestDataPath Plugin = System.IO.Path.Combine("Ringtoets", "Piping", "test", "Ringtoets.Piping.Plugin.Test");
            }

            public static class Revetment
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Revetment", "test", "Ringtoets.Revetment.IO.Test");
            }

            public static class StabilityPointStructures
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "StabilityPointStructures", "test", "Ringtoets.StabilityPointStructures.IO.Test");
            }

            public static class StabilityStoneCover
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "StabilityStoneCover", "test", "Ringtoets.StabilityStoneCover.IO.Test");
            }

            public static class WaveImpactAsphaltCover
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "WaveImpactAsphaltCover", "test", "Ringtoets.WaveImpactAsphaltCover.IO.Test");
            }
        }
    }
}