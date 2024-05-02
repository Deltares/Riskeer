// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.IO.Converters;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableAssessmentSection"/>
    /// with assembly results.
    /// </summary>
    public static class ExportableAssessmentSectionFactory
    {
        /// <summary>
        /// Creates an <see cref="ExportableAssessmentSection"/> with assembly results based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create an <see cref="ExportableAssessmentSection"/> for.</param>
        /// <returns>An <see cref="ExportableAssessmentSection"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when no reference line is set for <paramref name="assessmentSection"/>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when assembly results are invalid and cannot be exported.</exception>
        public static ExportableAssessmentSection CreateExportableAssessmentSection(IdentifierGenerator idGenerator, AssessmentSection assessmentSection)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var registry = new ExportableModelRegistry();

            List<ExportableFailureMechanismSectionCollection> failureMechanismSectionCollections = CreateExportableFailureMechanismSectionCollections(
                idGenerator, registry, assessmentSection);

            return new ExportableAssessmentSection(
                IdentifierGenerator.GenerateId(Resources.ExportableAssessmentSection_IdPrefix, assessmentSection.Id),
                assessmentSection.Name,
                assessmentSection.ReferenceLine.Points,
                failureMechanismSectionCollections,
                CreateExportableAssessmentSectionAssemblyResult(idGenerator, assessmentSection),
                CreateExportableFailureMechanisms(idGenerator, registry, assessmentSection));
        }

        /// <summary>
        /// Creates an <see cref="ExportableAssessmentSectionAssemblyResult"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="assessmentSection">The assessment section to create an
        /// <see cref="ExportableAssessmentSectionAssemblyResult"/> for.</param>
        /// <returns>An <see cref="ExportableAssessmentSectionAssemblyResult"/> with assembly result.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly result cannot be created for
        /// <paramref name="assessmentSection"/>.</exception>
        private static ExportableAssessmentSectionAssemblyResult CreateExportableAssessmentSectionAssemblyResult(IdentifierGenerator idGenerator,
                                                                                                                 AssessmentSection assessmentSection)
        {
            AssessmentSectionAssemblyResultWrapper assemblyResultWrapper = AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);
            AssessmentSectionAssemblyResult assemblyResult = assemblyResultWrapper.AssemblyResult;
            return new ExportableAssessmentSectionAssemblyResult(
                idGenerator.GetUniqueId(Resources.ExportableTotalAssemblyResult_IdPrefix),
                ConvertAssemblyGroup(assemblyResult.AssemblyGroup), assemblyResult.Probability,
                ExportableAssemblyMethodConverter.ConvertTo(assemblyResultWrapper.AssemblyGroupMethod),
                ExportableAssemblyMethodConverter.ConvertTo(assemblyResultWrapper.ProbabilityMethod));
        }

        /// <summary>
        /// Converts an <see cref="AssessmentSectionAssemblyGroup"/> into an <see cref="ExportableAssessmentSectionAssemblyGroup"/>
        /// .
        /// </summary>
        /// <param name="assemblyGroup">The <see cref="AssessmentSectionAssemblyGroup"/> to convert.</param>
        /// <returns>The converted <see cref="ExportableAssessmentSectionAssemblyGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        private static ExportableAssessmentSectionAssemblyGroup ConvertAssemblyGroup(AssessmentSectionAssemblyGroup assemblyGroup)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyGroup), assemblyGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyGroup),
                                                       (int) assemblyGroup,
                                                       typeof(AssessmentSectionAssemblyGroup));
            }

            switch (assemblyGroup)
            {
                case AssessmentSectionAssemblyGroup.APlus:
                    return ExportableAssessmentSectionAssemblyGroup.APlus;
                case AssessmentSectionAssemblyGroup.A:
                    return ExportableAssessmentSectionAssemblyGroup.A;
                case AssessmentSectionAssemblyGroup.B:
                    return ExportableAssessmentSectionAssemblyGroup.B;
                case AssessmentSectionAssemblyGroup.C:
                    return ExportableAssessmentSectionAssemblyGroup.C;
                case AssessmentSectionAssemblyGroup.D:
                    return ExportableAssessmentSectionAssemblyGroup.D;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanism"/>
        /// for failure mechanisms that are in assembly based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created items.</param>
        /// <param name="assessmentSection">The assessment section to create a collection of
        /// <see cref="ExportableFailureMechanism"/> with probability for.</param>
        /// <returns>A collection of <see cref="ExportableFailureMechanism"/> based on failure
        /// mechanisms that are in assembly.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for
        /// <paramref name="assessmentSection"/>.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when assembly results are invalid and cannot be exported.</exception>
        private static IEnumerable<ExportableFailureMechanism> CreateExportableFailureMechanisms(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry, AssessmentSection assessmentSection)
        {
            var exportableFailureMechanisms = new List<ExportableFailureMechanism>();

            AddGenericFailureMechanismWhenInAssembly<PipingFailureMechanism, AdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.Piping, assessmentSection,
                PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                PipingFailureMechanismAssemblyFactory.AssembleSection);

            AddGenericFailureMechanismWhenInAssembly<GrassCoverErosionInwardsFailureMechanism, AdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.GrassCoverErosionInwards, assessmentSection,
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection);

            AddGenericFailureMechanismWhenInAssembly<MacroStabilityInwardsFailureMechanism, AdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.MacroStabilityInwards, assessmentSection,
                MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleSection);

            AddGenericFailureMechanismWhenInAssembly<MicrostabilityFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.Microstability, assessmentSection,
                FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            AddGenericFailureMechanismWhenInAssembly<StabilityStoneCoverFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.StabilityStoneCover, assessmentSection,
                StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            AddGenericFailureMechanismWhenInAssembly<WaveImpactAsphaltCoverFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.WaveImpactAsphaltCover, assessmentSection,
                WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            AddGenericFailureMechanismWhenInAssembly<WaterPressureAsphaltCoverFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.WaterPressureAsphaltCover, assessmentSection,
                FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            AddGenericFailureMechanismWhenInAssembly<GrassCoverErosionOutwardsFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.GrassCoverErosionOutwards, assessmentSection,
                GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            AddGenericFailureMechanismWhenInAssembly<GrassCoverSlipOffOutwardsFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.GrassCoverSlipOffOutwards, assessmentSection,
                FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            AddGenericFailureMechanismWhenInAssembly<GrassCoverSlipOffInwardsFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.GrassCoverSlipOffInwards, assessmentSection,
                FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            AddGenericFailureMechanismWhenInAssembly<HeightStructuresFailureMechanism, AdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.HeightStructures, assessmentSection,
                HeightStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                StructuresFailureMechanismAssemblyFactory.AssembleSection<HeightStructuresInput>);

            AddGenericFailureMechanismWhenInAssembly<ClosingStructuresFailureMechanism, AdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.ClosingStructures, assessmentSection,
                ClosingStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                StructuresFailureMechanismAssemblyFactory.AssembleSection<ClosingStructuresInput>);

            AddGenericFailureMechanismWhenInAssembly<PipingStructureFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.PipingStructure, assessmentSection,
                PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            AddGenericFailureMechanismWhenInAssembly<StabilityPointStructuresFailureMechanism, AdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.StabilityPointStructures, assessmentSection,
                StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                StructuresFailureMechanismAssemblyFactory.AssembleSection<StabilityPointStructuresInput>);

            AddGenericFailureMechanismWhenInAssembly<DuneErosionFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                idGenerator, registry, exportableFailureMechanisms, assessmentSection.DuneErosion, assessmentSection,
                DuneErosionFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass));

            exportableFailureMechanisms.AddRange(
                assessmentSection.SpecificFailureMechanisms.Where(fm => fm.InAssembly)
                                 .Select(fm => ExportableFailureMechanismFactory.CreateExportableSpecificFailureMechanism(
                                             idGenerator, registry, fm, assessmentSection,
                                             FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                                             (sr, sfm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass))));

            return exportableFailureMechanisms;
        }

        /// <summary>
        /// Adds a generic failure mechanism to the <paramref name="exportableFailureMechanisms"/> when it is in assembly.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created items.</param>
        /// <param name="exportableFailureMechanisms">The <see cref="List{T}"/> with <see cref="ExportableFailureMechanism"/>
        /// to add the failure mechanism to.</param>
        /// <param name="failureMechanism">The failure mechanism to export.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <param name="assembleFailureMechanismFunc">The <see cref="Func{T1,T2,TResult}"/> to assemble the failure mechanism.</param>
        /// <param name="assembleFailureMechanismSectionFunc">The <see cref="Func{T1,T2,T3,TResult}"/> to assemble the failure
        /// mechanism section.</param>
        /// <typeparam name="TFailureMechanism">The type of failure mechanism.</typeparam>
        /// <typeparam name="TSectionResult">The type of section result.</typeparam>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="failureMechanism"/>.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when assembly results are invalid and cannot be exported.</exception>
        private static void AddGenericFailureMechanismWhenInAssembly<TFailureMechanism, TSectionResult>(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry, List<ExportableFailureMechanism> exportableFailureMechanisms,
            TFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
            Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> assembleFailureMechanismFunc,
            Func<TSectionResult, TFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> assembleFailureMechanismSectionFunc)
            where TFailureMechanism : IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            if (failureMechanism.InAssembly)
            {
                exportableFailureMechanisms.Add(
                    ExportableFailureMechanismFactory.CreateExportableGenericFailureMechanism(
                        idGenerator, registry, failureMechanism, assessmentSection, assembleFailureMechanismFunc, assembleFailureMechanismSectionFunc));
            }
        }

        private static List<ExportableFailureMechanismSectionCollection> CreateExportableFailureMechanismSectionCollections(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry, IAssessmentSection assessmentSection)
        {
            return assessmentSection.GetFailureMechanisms()
                                    .Concat(assessmentSection.SpecificFailureMechanisms)
                                    .Where(fm => fm.InAssembly)
                                    .Select(fm => ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                                                idGenerator, registry, fm.Sections))
                                    .ToList();
        }
    }
}