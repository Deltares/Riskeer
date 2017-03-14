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

namespace Ringtoets.Piping.IO.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Piping.IO.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Het bestand is niet geschikt om karakteristieke punten uit te lezen: koptekst komt niet overeen met wat verwacht wordt..
        /// </summary>
        public static string CharacteristicPointsCsvReader_File_invalid_header {
            get {
                return ResourceManager.GetString("CharacteristicPointsCsvReader_File_invalid_header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to locatie &apos;{0}&apos;.
        /// </summary>
        public static string CharacteristicPointsCsvReader_LocationName_0_ {
            get {
                return ResourceManager.GetString("CharacteristicPointsCsvReader_LocationName_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Regel heeft geen ID..
        /// </summary>
        public static string CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_ID {
            get {
                return ResourceManager.GetString("CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_ID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ontbrekend scheidingsteken &apos;{0}&apos;..
        /// </summary>
        public static string CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_separator_0_ {
            get {
                return ResourceManager.GetString("CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_separat" +
                        "or_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het aantal kolommen voor deze locatie komt niet overeen met het aantal kolommen in de koptekst..
        /// </summary>
        public static string CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Location_lacks_values_for_characteristic_points {
            get {
                return ResourceManager.GetString("CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Location_lacks_val" +
                        "ues_for_characteristic_points", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er kan geen 1D-profiel bepaald worden wanneer segmenten in een 2D laag verticaal lopen op de gekozen positie: x = {0}..
        /// </summary>
        public static string Error_Can_not_determine_1D_profile_with_vertical_segments_at_X_0_ {
            get {
                return ResourceManager.GetString("Error_Can_not_determine_1D_profile_with_vertical_segments_at_X_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Karakteristiek punt heeft een coördinaatwaarde die niet omgezet kan worden naar een getal..
        /// </summary>
        public static string Error_CharacteristicPoint_has_not_double {
            get {
                return ResourceManager.GetString("Error_CharacteristicPoint_has_not_double", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Karakteristiek punt heeft een coördinaatwaarde die te groot of te klein is om ingelezen te worden..
        /// </summary>
        public static string Error_CharacteristicPoint_parsing_causes_overflow {
            get {
                return ResourceManager.GetString("Error_CharacteristicPoint_parsing_causes_overflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kon geen ondergrondschematisaties verkrijgen uit de database..
        /// </summary>
        public static string Error_SoilProfile_read_from_database {
            get {
                return ResourceManager.GetString("Error_SoilProfile_read_from_database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geen geldige X waarde gevonden om intersectie te maken uit 2D profiel &apos;{0}&apos;..
        /// </summary>
        public static string Error_SoilProfileBuilder_cant_determine_intersect_SoilProfileName_0_at_double_NaN {
            get {
                return ResourceManager.GetString("Error_SoilProfileBuilder_cant_determine_intersect_SoilProfileName_0_at_double_NaN" +
                        "", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Profielschematisatie heeft een coördinaatwaarde die niet omgezet kan worden naar een getal..
        /// </summary>
        public static string Error_SurfaceLine_has_not_double {
            get {
                return ResourceManager.GetString("Error_SurfaceLine_has_not_double", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Profielschematisatie heeft een coördinaatwaarde die te groot of te klein is om ingelezen te worden..
        /// </summary>
        public static string Error_SurfaceLine_parsing_causes_overflow {
            get {
                return ResourceManager.GetString("Error_SurfaceLine_parsing_causes_overflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///
        ///&lt;!--
        ///Copyright (C) Stichting Deltares 2016. All rights reserved.
        ///
        ///This file is part of Ringtoets.
        ///
        ///Ringtoets is free software: you can redistribute it and/or modify
        ///it under the terms of the GNU General Public License as published by
        ///the Free Software Foundation, either version 3 of the License, or
        ///(at your option) any later version.
        ///
        ///This program is distributed in the hope that it will be useful,
        ///but WITHOUT ANY WARRANTY; without even the implied warrant [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PipingConfiguratieSchema {
            get {
                return ResourceManager.GetString("PipingConfiguratieSchema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} 
        ///Er is geen berekeningenconfiguratie geïmporteerd..
        /// </summary>
        public static string PipingConfigurationImporter_HandleCriticalFileReadError_Error_0_no_configuration_imported {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_HandleCriticalFileReadError_Error_0_no_configuration_" +
                        "imported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen berekeningenconfiguratie..
        /// </summary>
        public static string PipingConfigurationImporter_ProgressText_Reading_configuration {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ProgressText_Reading_configuration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Valideren berekeningenconfiguratie..
        /// </summary>
        public static string PipingConfigurationImporter_ProgressText_Validating_imported_data {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ProgressText_Validating_imported_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een waarde van &apos;{0}&apos; als intredepunt is ongeldig..
        /// </summary>
        public static string PipingConfigurationImporter_ReadEntryExitPoint_Entry_point_invalid {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadEntryExitPoint_Entry_point_invalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een waarde van &apos;{0}&apos; als uittredepunt is ongeldig..
        /// </summary>
        public static string PipingConfigurationImporter_ReadEntryExitPoint_Exit_point_invalid {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadEntryExitPoint_Exit_point_invalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De locatie met hydraulische randvoorwaarden &apos;{0}&apos; bestaat niet..
        /// </summary>
        public static string PipingConfigurationImporter_ReadHydraulicBoundaryLocation_Hydraulic_boundary_location_0_does_not_exist {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadHydraulicBoundaryLocation_Hydraulic_boundary_loca" +
                        "tion_0_does_not_exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het stochastische ondergrondmodel &apos;{0}&apos; bestaat niet..
        /// </summary>
        public static string PipingConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_exist {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_do" +
                        "es_not_exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het stochastische ondergrondmodel &apos;{0}&apos;doorkruist de profielschematisatie &apos;{1}&apos; niet..
        /// </summary>
        public static string PipingConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_intersect_with_surfaceLine_1 {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_do" +
                        "es_not_intersect_with_surfaceLine_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen stochastisch ondergrondmodel opgegeven bij ondergrondschematisatie &apos;{0}&apos;..
        /// </summary>
        public static string PipingConfigurationImporter_ReadStochasticSoilProfile_No_soil_model_provided_for_soil_profile_with_name_0 {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadStochasticSoilProfile_No_soil_model_provided_for_" +
                        "soil_profile_with_name_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De ondergrondschematisatie &apos;{0}&apos; bestaat niet binnen het stochastische ondergrondmodel &apos;{1}&apos;..
        /// </summary>
        public static string PipingConfigurationImporter_ReadStochasticSoilProfile_Stochastic_soil_profile_0_does_not_exist_within_soil_model_1 {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadStochasticSoilProfile_Stochastic_soil_profile_0_d" +
                        "oes_not_exist_within_soil_model_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een gemiddelde van &apos;{0}&apos; is ongeldig voor stochast &apos;{1}&apos;..
        /// </summary>
        public static string PipingConfigurationImporter_ReadStochasts_Invalid_mean_0_for_stochast_with_name_1 {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadStochasts_Invalid_mean_0_for_stochast_with_name_1" +
                        "", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een standaardafwijking van &apos;{0}&apos; is ongeldig voor stochast &apos;{1}&apos;..
        /// </summary>
        public static string PipingConfigurationImporter_ReadStochasts_Invalid_standard_deviation_0_for_stochast_with_name_1 {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadStochasts_Invalid_standard_deviation_0_for_stocha" +
                        "st_with_name_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen profielschematisatie, maar wel een intrede- of uittredepunt opgegeven..
        /// </summary>
        public static string PipingConfigurationImporter_ReadSurfaceLine_EntryPointL_or_ExitPointL_defined_without_SurfaceLine {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadSurfaceLine_EntryPointL_or_ExitPointL_defined_wit" +
                        "hout_SurfaceLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De profielschematisatie &apos;{0}&apos; bestaat niet..
        /// </summary>
        public static string PipingConfigurationImporter_ReadSurfaceLine_SurfaceLine_0_does_not_exist {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ReadSurfaceLine_SurfaceLine_0_does_not_exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Berekening &apos;{1}&apos; is overgeslagen..
        /// </summary>
        public static string PipingConfigurationImporter_ValidateCalculation_Error_message_0_calculation_1_skipped {
            get {
                return ResourceManager.GetString("PipingConfigurationImporter_ValidateCalculation_Error_message_0_calculation_1_ski" +
                        "pped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database..
        /// </summary>
        public static string PipingSoilProfileReader_Critical_Unexpected_value_on_column {
            get {
                return ResourceManager.GetString("PipingSoilProfileReader_Critical_Unexpected_value_on_column", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De database heeft niet de vereiste versie informatie. Vereiste versie is &apos;{0}&apos;..
        /// </summary>
        public static string PipingSoilProfileReader_Database_incorrect_version_requires_Version_0_ {
            get {
                return ResourceManager.GetString("PipingSoilProfileReader_Database_incorrect_version_requires_Version_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ondergrondschematisatie bevat geen geldige waarde in kolom &apos;{0}&apos;..
        /// </summary>
        public static string PipingSoilProfileReader_Profile_has_invalid_value_on_Column_0_ {
            get {
                return ResourceManager.GetString("PipingSoilProfileReader_Profile_has_invalid_value_on_Column_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ondergrondschematisatie &apos;{0}&apos;.
        /// </summary>
        public static string PipingSoilProfileReader_SoilProfileName_0_ {
            get {
                return ResourceManager.GetString("PipingSoilProfileReader_SoilProfileName_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand is niet geschikt om profielschematisaties uit te lezen (Verwachte koptekst: locationid;X1;Y1;Z1)..
        /// </summary>
        public static string PipingSurfaceLinesCsvReader_File_invalid_header {
            get {
                return ResourceManager.GetString("PipingSurfaceLinesCsvReader_File_invalid_header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Regel heeft geen ID..
        /// </summary>
        public static string PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_ID {
            get {
                return ResourceManager.GetString("PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_ID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ontbrekend scheidingsteken &apos;{0}&apos;..
        /// </summary>
        public static string PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_separator_0_ {
            get {
                return ResourceManager.GetString("PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_separator_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Profielschematisatie heeft een teruglopende geometrie (punten behoren een oplopende set L-coördinaten te hebben in het lokale coördinatenstelsel)..
        /// </summary>
        public static string PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry {
            get {
                return ResourceManager.GetString("PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Profielschematisatie heeft een geometrie die een lijn met lengte 0 beschrijft..
        /// </summary>
        public static string PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_zero_length {
            get {
                return ResourceManager.GetString("PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_zero_length", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voor de profielschematisatie ontbreken er waardes om een 3D (X,Y,Z) punt aan te maken..
        /// </summary>
        public static string PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_lacks_values_for_coordinate_triplet {
            get {
                return ResourceManager.GetString("PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_lacks_values_for_coordinate_trip" +
                        "let", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to profielschematisatie &apos;{0}&apos;.
        /// </summary>
        public static string PipingSurfaceLinesCsvReader_SurfaceLineName_0_ {
            get {
                return ResourceManager.GetString("PipingSurfaceLinesCsvReader_SurfaceLineName_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen ondergrondmodellen lezen. Mogelijk bestaat de &apos;{0}&apos; tabel niet..
        /// </summary>
        public static string SoilDatabaseConstraintsReader_VerifyConstraints_Can_not_read_StochasticSoilModel_Perhaps_table_missing {
            get {
                return ResourceManager.GetString("SoilDatabaseConstraintsReader_VerifyConstraints_Can_not_read_StochasticSoilModel_" +
                        "Perhaps_table_missing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen ondergrondschematisaties lezen. Mogelijk bestaat de &apos;{0}&apos; tabel niet..
        /// </summary>
        public static string SoilDatabaseConstraintsReader_VerifyConstraints_Can_not_read_StochasticSoilProfile_Perhaps_table_missing {
            get {
                return ResourceManager.GetString("SoilDatabaseConstraintsReader_VerifyConstraints_Can_not_read_StochasticSoilProfil" +
                        "e_Perhaps_table_missing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er zijn stochastische ondergrondschematisaties zonder geldige kans van voorkomen..
        /// </summary>
        public static string SoilDatabaseConstraintsReader_VerifyConstraints_Invalid_StochasticSoilProfile_probability {
            get {
                return ResourceManager.GetString("SoilDatabaseConstraintsReader_VerifyConstraints_Invalid_StochasticSoilProfile_pro" +
                        "bability", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Namen van ondergrondmodellen zijn niet uniek..
        /// </summary>
        public static string SoilDatabaseConstraintsReader_VerifyConstraints_Non_unique_StochasticSoilModel_names {
            get {
                return ResourceManager.GetString("SoilDatabaseConstraintsReader_VerifyConstraints_Non_unique_StochasticSoilModel_na" +
                        "mes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Onverwachte fout tijdens het verifiëren van unieke ondergrondmodelnamen..
        /// </summary>
        public static string SoilDatabaseConstraintsReader_VerifyConstraints_Unexpected_error_while_verifying_unique_StochasticSoilModel_names {
            get {
                return ResourceManager.GetString("SoilDatabaseConstraintsReader_VerifyConstraints_Unexpected_error_while_verifying_" +
                        "unique_StochasticSoilModel_names", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Onverwachte fout tijdens het verifiëren van kansen van voorkomen voor profielen..
        /// </summary>
        public static string SoilDatabaseConstraintsReader_VerifyConstraints_Unexpected_error_while_verifying_valid_StochasticSoilProfile_probability {
            get {
                return ResourceManager.GetString("SoilDatabaseConstraintsReader_VerifyConstraints_Unexpected_error_while_verifying_" +
                        "valid_StochasticSoilProfile_probability", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verzadigd gewicht.
        /// </summary>
        public static string SoilLayer_BelowPhreaticLevelDistribution_Description {
            get {
                return ResourceManager.GetString("SoilLayer_BelowPhreaticLevelDistribution_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Korrelgrootte.
        /// </summary>
        public static string SoilLayer_DiameterD70Distribution_Description {
            get {
                return ResourceManager.GetString("SoilLayer_DiameterD70Distribution_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Doorlatendheid.
        /// </summary>
        public static string SoilLayer_PermeabilityDistribution_Description {
            get {
                return ResourceManager.GetString("SoilLayer_PermeabilityDistribution_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;{0}&apos; is niet lognormaal verdeeld..
        /// </summary>
        public static string SoilLayer_Stochastic_parameter_0_has_no_lognormal_distribution {
            get {
                return ResourceManager.GetString("SoilLayer_Stochastic_parameter_0_has_no_lognormal_distribution", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;{0}&apos; is niet verschoven lognormaal verdeeld..
        /// </summary>
        public static string SoilLayer_Stochastic_parameter_0_has_no_shifted_lognormal_distribution {
            get {
                return ResourceManager.GetString("SoilLayer_Stochastic_parameter_0_has_no_shifted_lognormal_distribution", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De segmenten van de geometrie van de laag vormen geen lus..
        /// </summary>
        public static string SoilLayer2D_Error_Loop_contains_disconnected_segments {
            get {
                return ResourceManager.GetString("SoilLayer2D_Error_Loop_contains_disconnected_segments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Coördinaat van een punt bevat ongeldige waarde..
        /// </summary>
        public static string SoilLayer2DReader_Could_not_parse_point_location {
            get {
                return ResourceManager.GetString("SoilLayer2DReader_Could_not_parse_point_location", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het XML-document dat de geometrie beschrijft voor de laag is niet geldig..
        /// </summary>
        public static string SoilLayer2DReader_Geometry_contains_no_valid_xml {
            get {
                return ResourceManager.GetString("SoilLayer2DReader_Geometry_contains_no_valid_xml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De geometrie is leeg..
        /// </summary>
        public static string SoilLayer2DReader_Geometry_is_null {
            get {
                return ResourceManager.GetString("SoilLayer2DReader_Geometry_is_null", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kon geen stochastische ondergrondmodellen verkrijgen uit de database..
        /// </summary>
        public static string StochasticSoilModelDatabaseReader_Failed_to_read_database {
            get {
                return ResourceManager.GetString("StochasticSoilModelDatabaseReader_Failed_to_read_database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De uitgelezen ondergrondschematisatie &apos;{0}&apos; wordt niet gebruikt in een van de stochastische ondergrondmodellen..
        /// </summary>
        public static string StochasticSoilModelImporter_CheckIfAllProfilesAreUsed_SoilProfile_0_is_not_used_in_any_stochastic_soil_model {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_CheckIfAllProfilesAreUsed_SoilProfile_0_is_not_used_i" +
                        "n_any_stochastic_soil_model", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Controleren van ondergrondschematisaties..
        /// </summary>
        public static string StochasticSoilModelImporter_CheckIfAllProfilesAreUsed_Start_checking_soil_profiles {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_CheckIfAllProfilesAreUsed_Start_checking_soil_profile" +
                        "s", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} 
        ///Het bestand wordt overgeslagen..
        /// </summary>
        public static string StochasticSoilModelImporter_CriticalErrorMessage_0_File_Skipped {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_CriticalErrorMessage_0_File_Skipped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Dit stochastische ondergrondmodel wordt overgeslagen..
        /// </summary>
        public static string StochasticSoilModelImporter_GetStochasticSoilModelReadResult_Error_0_stochastic_soil_model_skipped {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_GetStochasticSoilModelReadResult_Error_0_stochastic_s" +
                        "oil_model_skipped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van de stochastische ondergrondmodellen..
        /// </summary>
        public static string StochasticSoilModelImporter_GetStochasticSoilModelReadResult_Reading_stochastic_soil_models_from_database {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_GetStochasticSoilModelReadResult_Reading_stochastic_s" +
                        "oil_models_from_database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stochastische ondergrondmodellen importeren afgebroken. Geen data ingelezen..
        /// </summary>
        public static string StochasticSoilModelImporter_Import_Import_canceled {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_Import_Import_canceled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ondergrondschematisatie &apos;{0}&apos; is meerdere keren gevonden in ondergrondmodel &apos;{1}&apos;. Kansen van voorkomen worden opgeteld..
        /// </summary>
        public static string StochasticSoilModelImporter_MergeStochasticSoilProfiles_Multiple_SoilProfile_0_used_in_StochasticSoilModel_1_Probabilities_added {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_MergeStochasticSoilProfiles_Multiple_SoilProfile_0_us" +
                        "ed_in_StochasticSoilModel_1_Probabilities_added", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van de D-Soil Model database..
        /// </summary>
        public static string StochasticSoilModelImporter_Reading_database {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_Reading_database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van de ondergrondschematisatie uit de D-Soil Model database..
        /// </summary>
        public static string StochasticSoilModelImporter_ReadingSoilProfiles {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_ReadingSoilProfiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} 
        ///Deze ondergrondschematisatie wordt overgeslagen..
        /// </summary>
        public static string StochasticSoilModelImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skip" +
                        "ped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er zijn geen ondergrondschematisaties gevonden in het stochastische ondergrondmodel &apos;{0}&apos;. Dit model wordt overgeslagen..
        /// </summary>
        public static string StochasticSoilModelImporter_ValidateStochasticSoilModel_No_profiles_found_in_stochastic_soil_model_0 {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_ValidateStochasticSoilModel_No_profiles_found_in_stoc" +
                        "hastic_soil_model_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het stochastische ondergrondmodel &apos;{0}&apos; heeft een ongespecificeerde ondergrondschematisatie. Dit model wordt overgeslagen..
        /// </summary>
        public static string StochasticSoilModelImporter_ValidateStochasticSoilModel_SoilModel_0_with_stochastic_soil_profile_without_profile {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_ValidateStochasticSoilModel_SoilModel_0_with_stochast" +
                        "ic_soil_profile_without_profile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De som van de kansen van voorkomen in het stochastich ondergrondmodel &apos;{0}&apos; is niet gelijk aan 100%..
        /// </summary>
        public static string StochasticSoilModelImporter_ValidateStochasticSoilModel_Sum_of_probabilities_of_stochastic_soil_model_0_is_not_correct {
            get {
                return ResourceManager.GetString("StochasticSoilModelImporter_ValidateStochasticSoilModel_Sum_of_probabilities_of_s" +
                        "tochastic_soil_model_0_is_not_correct", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De ondergrondschematisatie verwijst naar een ongeldige waarde..
        /// </summary>
        public static string StochasticSoilProfileDatabaseReader_StochasticSoilProfile_has_invalid_value {
            get {
                return ResourceManager.GetString("StochasticSoilProfileDatabaseReader_StochasticSoilProfile_has_invalid_value", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot;?&gt;
        ///
        ///&lt;!--
        ///Copyright (C) Stichting Deltares 2016. All rights reserved.
        ///
        ///This file is part of Ringtoets.
        ///
        ///Ringtoets is free software: you can redistribute it and/or modify
        ///it under the terms of the GNU General Public License as published by
        ///the Free Software Foundation, either version 3 of the License, or
        ///(at your option) any later version.
        ///
        ///This program is distributed in the hope that it will be useful,
        ///but WITHOUT ANY WARRANTY; without even the implied warranty of
        ///MERCHANTABI [rest of string was truncated]&quot;;.
        /// </summary>
        public static string XmlGeometrySchema {
            get {
                return ResourceManager.GetString("XmlGeometrySchema", resourceCulture);
            }
        }
    }
}
