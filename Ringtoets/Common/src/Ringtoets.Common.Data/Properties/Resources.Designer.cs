﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.Common.Data.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Common.Data.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Vak &apos;{0}&apos; sluit niet aan op de al gedefinieerde vakken van het toetsspoor..
        /// </summary>
        public static string BaseFailureMechanism_AddSection_Section_0_must_connect_to_existing_sections {
            get {
                return ResourceManager.GetString("BaseFailureMechanism_AddSection_Section_0_must_connect_to_existing_sections", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nieuwe map.
        /// </summary>
        public static string CalculationGroup_DefaultName {
            get {
                return ResourceManager.GetString("CalculationGroup_DefaultName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de naam van deze groep niet aanpassen, omdat &apos;IsNameEditable&apos; op &apos;false&apos; staat..
        /// </summary>
        public static string CalculationGroup_Setting_readonly_name_error_message {
            get {
                return ResourceManager.GetString("CalculationGroup_Setting_readonly_name_error_message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De waarde voor de toegestane bijdrage aan faalkans moet in interval [0,100] liggen..
        /// </summary>
        public static string Contribution_Value_should_be_in_interval_0_100 {
            get {
                return ResourceManager.GetString("Contribution_Value_should_be_in_interval_0_100", resourceCulture);
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
        ///   Looks up a localized string similar to Oordeel.
        /// </summary>
        public static string FailureMechanism_AssessmentResult_DisplayName {
            get {
                return ResourceManager.GetString("FailureMechanism_AssessmentResult_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Randvoorwaarden.
        /// </summary>
        public static string FailureMechanism_BoundaryConditions_DisplayName {
            get {
                return ResourceManager.GetString("FailureMechanism_BoundaryConditions_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Locaties.
        /// </summary>
        public static string FailureMechanism_Locations_DisplayName {
            get {
                return ResourceManager.GetString("FailureMechanism_Locations_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen bijdrageoverzicht maken zonder toetsspoor..
        /// </summary>
        public static string FailureMechanismContribution_UpdateContributions_Can_not_create_FailureMechanismContribution_without_FailureMechanism_collection {
            get {
                return ResourceManager.GetString("FailureMechanismContribution_UpdateContributions_Can_not_create_FailureMechanismC" +
                        "ontribution_without_FailureMechanism_collection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen bijdrage element maken zonder een toetsspoor..
        /// </summary>
        public static string FailureMechanismContributionItem_Can_not_create_contribution_item_without_failure_mechanism {
            get {
                return ResourceManager.GetString("FailureMechanismContributionItem_Can_not_create_contribution_item_without_failure" +
                        "_mechanism", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De faalkansbijdrage kan alleen bepaald worden als de norm van het traject groter is dan 0..
        /// </summary>
        public static string FailureMechanismContributionItem_Norm_must_be_larger_than_zero {
            get {
                return ResourceManager.GetString("FailureMechanismContributionItem_Norm_must_be_larger_than_zero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vak moet minstens uit 1 punt bestaan..
        /// </summary>
        public static string FailureMechanismSection_Section_must_have_at_least_1_geometry_point {
            get {
                return ResourceManager.GetString("FailureMechanismSection_Section_must_have_at_least_1_geometry_point", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hydraulische randvoorwaarden.
        /// </summary>
        public static string HydraulicBoundaryConditions_DisplayName {
            get {
                return ResourceManager.GetString("HydraulicBoundaryConditions_DisplayName", resourceCulture);
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
        ///   Looks up a localized string similar to NWOoc.
        /// </summary>
        public static string OtherFailureMechanism_DisplayCode {
            get {
                return ResourceManager.GetString("OtherFailureMechanism_DisplayCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Overig.
        /// </summary>
        public static string OtherFailureMechanism_DisplayName {
            get {
                return ResourceManager.GetString("OtherFailureMechanism_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Referentielijn.
        /// </summary>
        public static string ReferenceLine_DisplayName {
            get {
                return ResourceManager.GetString("ReferenceLine_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De geometrie die opgegeven werd voor de referentielijn heeft geen waarde..
        /// </summary>
        public static string ReferenceLine_SetGeometry_New_geometry_cannot_be_null {
            get {
                return ResourceManager.GetString("ReferenceLine_SetGeometry_New_geometry_cannot_be_null", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een punt in de geometrie voor de referentielijn heeft geen waarde..
        /// </summary>
        public static string ReferenceLine_SetGeometry_New_geometry_has_null_coordinate {
            get {
                return ResourceManager.GetString("ReferenceLine_SetGeometry_New_geometry_has_null_coordinate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Standaard afwijking (σ) moet groter zijn dan of gelijk zijn aan 0..
        /// </summary>
        public static string StandardDeviation_Should_be_greater_than_or_equal_to_zero {
            get {
                return ResourceManager.GetString("StandardDeviation_Should_be_greater_than_or_equal_to_zero", resourceCulture);
            }
        }
    }
}
