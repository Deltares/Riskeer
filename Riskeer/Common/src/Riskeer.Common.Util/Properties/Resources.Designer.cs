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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Riskeer.Common.Util.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.Common.Util.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string MetaData_ID {
            get {
                return ResourceManager.GetString("MetaData_ID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Naam.
        /// </summary>
        public static string MetaData_Name {
            get {
                return ResourceManager.GetString("MetaData_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:f0}.
        /// </summary>
        public static string ReturnPeriodFormat {
            get {
                return ResourceManager.GetString("ReturnPeriodFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is geen geldige Riskeer of Ringtoets projectbestand versie..
        /// </summary>
        public static string RiskeerVersionHelper_Version_0_Not_Valid {
            get {
                return ResourceManager.GetString("RiskeerVersionHelper_Version_0_Not_Valid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waterstanden bij vaste doelkans.
        /// </summary>
        public static string WaterLevelCalculationsForNormTargetProbabilities_DisplayName {
            get {
                return ResourceManager.GetString("WaterLevelCalculationsForNormTargetProbabilities_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waterstanden bij vrije doelkans.
        /// </summary>
        public static string WaterLevelCalculationsForUserDefinedTargetProbabilities_DisplayName {
            get {
                return ResourceManager.GetString("WaterLevelCalculationsForUserDefinedTargetProbabilities_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Golfhoogten bij doelkans.
        /// </summary>
        public static string WaveHeightCalculationsForUserDefinedTargetProbabilities_DisplayName {
            get {
                return ResourceManager.GetString("WaveHeightCalculationsForUserDefinedTargetProbabilities_DisplayName", resourceCulture);
            }
        }
    }
}
