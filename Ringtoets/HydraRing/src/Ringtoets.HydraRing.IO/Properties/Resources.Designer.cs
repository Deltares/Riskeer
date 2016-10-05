﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.HydraRing.IO.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.HydraRing.IO.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Kon geen locaties verkrijgen van de database..
        /// </summary>
        public static string Error_HydraulicBoundaryLocation_read_from_database {
            get {
                return ResourceManager.GetString("Error_HydraulicBoundaryLocation_read_from_database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database..
        /// </summary>
        public static string HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string HydraulicBoundaryLocation_Id {
            get {
                return ResourceManager.GetString("HydraulicBoundaryLocation_Id", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Naam.
        /// </summary>
        public static string HydraulicBoundaryLocation_Name {
            get {
                return ResourceManager.GetString("HydraulicBoundaryLocation_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Golfhoogte.
        /// </summary>
        public static string HydraulicBoundaryLocation_WaveHeight {
            get {
                return ResourceManager.GetString("HydraulicBoundaryLocation_WaveHeight", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het opgegeven bestandspad ({0}) is niet geldig..
        /// </summary>
        public static string HydraulicDatabaseHelper_ValidatePathForCalculation_Invalid_path_0_ {
            get {
                return ResourceManager.GetString("HydraulicDatabaseHelper_ValidatePathForCalculation_Invalid_path_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bevragen van de database is mislukt..
        /// </summary>
        public static string HydraulicLocationConfigurationSqLiteDatabaseReader_Critical_Unexpected_Exception {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationSqLiteDatabaseReader_Critical_Unexpected_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er zijn meerdere resultaten gevonden, wat niet voor zou mogen komen. Neem contact op met de leverancier. Het eerste resultaat zal worden gebruikt..
        /// </summary>
        public static string HydraulicLocationConfigurationSqLiteDatabaseReader_GetLocationIdFromDatabase_Ambiguous_Row_Found_Take_First {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationSqLiteDatabaseReader_GetLocationIdFromDatabase_Ambi" +
                        "guous_Row_Found_Take_First", resourceCulture);
            }
        }
    }
}
