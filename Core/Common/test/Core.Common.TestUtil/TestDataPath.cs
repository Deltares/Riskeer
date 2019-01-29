// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

        public static class Core
        {
            public static class Common
            {
                public static readonly TestDataPath Util = System.IO.Path.Combine("Core", "Common", "test", "Core.Common.Util.Test");
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

        public static class Ringtoets
        {
            public static class AssemblyTool
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "AssemblyTool", "test", "Riskeer.AssemblyTool.IO.Test");
            }

            public static class ClosingStructures
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "ClosingStructures", "test", "Riskeer.ClosingStructures.IO.Test");
            }

            public static class Common
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "Common", "test", "Riskeer.Common.IO.Test");
                public static readonly TestDataPath Service = System.IO.Path.Combine("Riskeer", "Common", "test", "Riskeer.Common.Service.Test");
            }

            public static class GrassCoverErosionInwards
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "GrassCoverErosionInwards", "test", "Riskeer.GrassCoverErosionInwards.IO.Test");
                public static readonly TestDataPath Integration = System.IO.Path.Combine("Riskeer", "GrassCoverErosionInwards", "test", "Riskeer.GrassCoverErosionInwards.Integration.Test");
            }

            public static class GrassCoverErosionOutwards
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "GrassCoverErosionOutwards", "test", "Riskeer.GrassCoverErosionOutwards.IO.Test");
            }

            public static class HeightStructures
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "HeightStructures", "test", "Riskeer.HeightStructures.IO.Test");
                public static readonly TestDataPath Integration = System.IO.Path.Combine("Riskeer", "HeightStructures", "test", "Riskeer.HeightStructures.Integration.Test");
            }

            public static class HydraRing
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "HydraRing", "test", "Riskeer.HydraRing.IO.Test");
                public static readonly TestDataPath Calculation = System.IO.Path.Combine("Riskeer", "HydraRing", "test", "Riskeer.HydraRing.Calculation.Test");
            }

            public static class Integration
            {
                public static readonly TestDataPath Forms = System.IO.Path.Combine("Riskeer", "Integration", "test", "Riskeer.Integration.Forms.Test");
                public static readonly TestDataPath Service = System.IO.Path.Combine("Riskeer", "Integration", "test", "Riskeer.Integration.Service.Test");
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "Integration", "test", "Riskeer.Integration.IO.Test");
                public static readonly TestDataPath Plugin = System.IO.Path.Combine("Riskeer", "Integration", "test", "Riskeer.Integration.Plugin.Test");
                public static readonly TestDataPath TestUtil = System.IO.Path.Combine("Riskeer", "Integration", "test", "Riskeer.Integration.TestUtil.Test");
            }

            public static class MacroStabilityInwards
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "MacroStabilityInwards", "test", "Riskeer.MacroStabilityInwards.IO.Test");
            }

            public static class Migration
            {
                public static readonly TestDataPath Core = System.IO.Path.Combine("Riskeer", "Migration", "test", "Riskeer.Migration.Core.Test");
            }

            public static class Piping
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "Piping", "test", "Riskeer.Piping.IO.Test");
            }

            public static class Revetment
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "Revetment", "test", "Riskeer.Revetment.IO.Test");
            }

            public static class StabilityPointStructures
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Riskeer", "StabilityPointStructures", "test", "Riskeer.StabilityPointStructures.IO.Test");
            }

            public static class Storage
            {
                public static readonly TestDataPath Core = System.IO.Path.Combine("Riskeer", "Storage", "test", "Riskeer.Storage.Core.Test");
            }
        }
    }
}