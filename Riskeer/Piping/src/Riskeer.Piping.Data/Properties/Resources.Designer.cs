﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Riskeer.Piping.Data.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.Piping.Data.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to De waarde moet binnen het bereik {0} liggen..
        /// </summary>
        public static string GeneralPipingInput_WaterVolumetricWeight_must_be_in_Range_0_ {
            get {
                return ResourceManager.GetString("GeneralPipingInput_WaterVolumetricWeight_must_be_in_Range_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to STPH.
        /// </summary>
        public static string PipingFailureMechanism_DisplayCode {
            get {
                return ResourceManager.GetString("PipingFailureMechanism_DisplayCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dijken en dammen - Piping.
        /// </summary>
        public static string PipingFailureMechanism_DisplayName {
            get {
                return ResourceManager.GetString("PipingFailureMechanism_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het uittredepunt moet landwaarts van het intredepunt liggen..
        /// </summary>
        public static string PipingInput_EntryPointL_greater_or_equal_to_ExitPointL {
            get {
                return ResourceManager.GetString("PipingInput_EntryPointL_greater_or_equal_to_ExitPointL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het gespecificeerde punt moet op het profiel liggen (bereik {0})..
        /// </summary>
        public static string PipingInput_ValidatePointOnSurfaceLine_Length_must_be_in_Range_0_ {
            get {
                return ResourceManager.GetString("PipingInput_ValidatePointOnSurfaceLine_Length_must_be_in_Range_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to naam.
        /// </summary>
        public static string UniqueFeature_Name_FeatureDescription {
            get {
                return ResourceManager.GetString("UniqueFeature_Name_FeatureDescription", resourceCulture);
            }
        }
    }
}
