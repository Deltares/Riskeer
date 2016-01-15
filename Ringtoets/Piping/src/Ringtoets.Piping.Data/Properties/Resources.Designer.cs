﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.Piping.Data.Properties {
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
        public Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Piping.Data.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Berekeningsverslag.
        /// </summary>
        public static string CalculationReport_DisplayName {
            get {
                return ResourceManager.GetString("CalculationReport_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Commentaar.
        /// </summary>
        public static string Comments_DisplayName {
            get {
                return ResourceManager.GetString("Comments_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een kansverdeling moet opgegeven zijn om op basis van die data een rekenwaarde te bepalen..
        /// </summary>
        public static string DesignVariable_GetDesignValue_Distribution_must_be_set {
            get {
                return ResourceManager.GetString("DesignVariable_GetDesignValue_Distribution_must_be_set", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Percentiel moet in het bereik van [0, 1] vallen..
        /// </summary>
        public static string DesignVariable_Percentile_must_be_in_range {
            get {
                return ResourceManager.GetString("DesignVariable_Percentile_must_be_in_range", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geen lagen gevonden voor het profiel..
        /// </summary>
        public static string Error_Cannot_Construct_PipingSoilProfile_Without_Layers {
            get {
                return ResourceManager.GetString("Error_Cannot_Construct_PipingSoilProfile_Without_Layers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kans moet in het bereik van [0, 1] opgegeven worden..
        /// </summary>
        public static string IDistribution_InverseCDF_Probability_must_be_in_range {
            get {
                return ResourceManager.GetString("IDistribution_InverseCDF_Probability_must_be_in_range", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gemiddelde moet groter zijn dan 0..
        /// </summary>
        public static string LognormalDistribution_Mean_must_be_greater_equal_to_zero {
            get {
                return ResourceManager.GetString("LognormalDistribution_Mean_must_be_greater_equal_to_zero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Punten voor een lijn moeten uit elkaar liggen om een lijn te kunnen vormen..
        /// </summary>
        public static string Math2D_LineIntersectionWithLine_Line_points_are_equal {
            get {
                return ResourceManager.GetString("Math2D_LineIntersectionWithLine_Line_points_are_equal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nieuwe berekening.
        /// </summary>
        public static string PipingCalculation_DefaultName {
            get {
                return ResourceManager.GetString("PipingCalculation_DefaultName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nieuwe map.
        /// </summary>
        public static string PipingCalculationGroup_DefaultName {
            get {
                return ResourceManager.GetString("PipingCalculationGroup_DefaultName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de naam van deze groep niet aanpassen, omdat &apos;IsNameEditable&apos; op &apos;false&apos; staat..
        /// </summary>
        public static string PipingCalculationGroup_Setting_readonly_name_error_message {
            get {
                return ResourceManager.GetString("PipingCalculationGroup_Setting_readonly_name_error_message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Berekeningen.
        /// </summary>
        public static string PipingFailureMechanism_Calculations_DisplayName {
            get {
                return ResourceManager.GetString("PipingFailureMechanism_Calculations_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dijken - Piping.
        /// </summary>
        public static string PipingFailureMechanism_DisplayName {
            get {
                return ResourceManager.GetString("PipingFailureMechanism_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een punt in de geometrie voor de profielmeting heeft geen waarde..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen hoogte bepalen op het punt L={0}, omdat de profielmeting verticaal loopt op dat punt..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_verti" +
                        "cal_in_l", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De geometrie die opgegeven werd voor de profielmeting heeft geen waarde..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voor het maken van een segment zijn twee punten nodig..
        /// </summary>
        public static string Segment2D_Constructor_Segment_must_be_created_with_two_points {
            get {
                return ResourceManager.GetString("Segment2D_Constructor_Segment_must_be_created_with_two_points", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Standaard afwijking (σ) moet groter zijn dan 0..
        /// </summary>
        public static string StandardDeviation_Should_be_greater_than_zero {
            get {
                return ResourceManager.GetString("StandardDeviation_Should_be_greater_than_zero", resourceCulture);
            }
        }
    }
}
