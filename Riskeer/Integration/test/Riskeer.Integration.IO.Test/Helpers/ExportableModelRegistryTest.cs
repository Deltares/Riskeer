// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.TestUtil;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class ExportableModelRegistryTest
    {
        /// <summary>
        /// Test class to test the <see cref="ExportableModelRegistry"/> for the combination of 
        /// <see cref="TDataModel"/> and <see cref="TExportableModel"/>.
        /// </summary>
        /// <typeparam name="TDataModel">The data model.</typeparam>
        /// <typeparam name="TExportableModel">The exportable model.</typeparam>
        private abstract class RegistryTest<TDataModel, TExportableModel>
            where TDataModel : class
            where TExportableModel : class
        {
            private readonly Action<ExportableModelRegistry, TExportableModel, TDataModel> registerToRegistry;
            private readonly Func<ExportableModelRegistry, TDataModel, bool> containsInRegistry;
            private readonly Func<ExportableModelRegistry, TDataModel, TExportableModel> getFromRegistry;

            /// <summary>
            /// Creates a new instance of <see cref="RegistryTest{T,T}"/>.
            /// </summary>
            /// <param name="registerToRegistry">The action to perform to register the data model
            /// to the registry.</param>
            /// <param name="containsInRegistry">The action to perform to check whether the data 
            /// model is registered in the registry.</param>
            /// <param name="getFromRegistry">The action to perform to get the data model from
            ///  the registry.</param>
            /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
            /// <example>
            /// <code>public DerivedRegistryTest() : base(
            /// (r, e, m) => r.Register(e, m),
            /// (r, m) => r.Contains(m),
            /// (r, m) => r.Get(m)) {}
            /// </code>
            /// </example>
            protected RegistryTest(Action<ExportableModelRegistry, TExportableModel, TDataModel> registerToRegistry,
                                   Func<ExportableModelRegistry, TDataModel, bool> containsInRegistry,
                                   Func<ExportableModelRegistry, TDataModel, TExportableModel> getFromRegistry)
            {
                if (registerToRegistry == null)
                {
                    throw new ArgumentNullException(nameof(registerToRegistry));
                }

                if (containsInRegistry == null)
                {
                    throw new ArgumentNullException(nameof(containsInRegistry));
                }

                if (getFromRegistry == null)
                {
                    throw new ArgumentNullException(nameof(getFromRegistry));
                }

                this.registerToRegistry = registerToRegistry;
                this.containsInRegistry = containsInRegistry;
                this.getFromRegistry = getFromRegistry;
            }

            [Test]
            public void Register_WithNullExportableModel_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new ExportableModelRegistry();

                // Call
                void Call() => registerToRegistry(registry, null, CreateDataModel());

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("exportableModel", exception.ParamName);
            }

            [Test]
            public void Register_WithNullDataModel_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new ExportableModelRegistry();

                // Call
                void Call() => registerToRegistry(registry, CreateExportableModel(), null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("model", exception.ParamName);
            }

            [Test]
            public void Contains_DataModelNull_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new ExportableModelRegistry();

                // Call
                void Call() => containsInRegistry(registry, null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("model", exception.ParamName);
            }

            [Test]
            public void Contains_DataModelAdded_ReturnsTrue()
            {
                // Setup
                var registry = new ExportableModelRegistry();
                TDataModel dataModel = CreateDataModel();
                registerToRegistry(registry, CreateExportableModel(), dataModel);

                // Call
                bool result = containsInRegistry(registry, dataModel);

                // Assert
                Assert.IsTrue(result);
            }

            [Test]
            public void Contains_OtherDataModelAdded_ReturnsFalse()
            {
                // Setup
                var registry = new ExportableModelRegistry();
                registerToRegistry(registry, CreateExportableModel(), CreateDataModel());

                // Call
                bool result = containsInRegistry(registry, CreateDataModel());

                // Assert
                Assert.IsFalse(result);
            }

            [Test]
            public void Contains_ExportableModelRegistryEmpty_ReturnsFalse()
            {
                // Setup
                var registry = new ExportableModelRegistry();
                TDataModel dataModel = CreateDataModel();

                // Call
                bool result = containsInRegistry(registry, dataModel);

                // Assert
                Assert.IsFalse(result);
            }

            [Test]
            public void Get_DataModelNull_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new ExportableModelRegistry();

                // Call
                void Call() => getFromRegistry(registry, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
                Assert.AreEqual("model", paramName);
            }

            [Test]
            public void Get_DataModelAdded_ReturnsEntity()
            {
                // Setup
                var registry = new ExportableModelRegistry();
                TDataModel dataModel = CreateDataModel();
                TExportableModel exportableModel = CreateExportableModel();

                registerToRegistry(registry, exportableModel, dataModel);

                // Call
                TExportableModel result = getFromRegistry(registry, dataModel);

                // Assert
                Assert.AreSame(exportableModel, result);
            }

            [Test]
            public void Get_NoDataModelAdded_ThrowsInvalidOperationException()
            {
                // Setup
                var registry = new ExportableModelRegistry();
                TDataModel dataModel = CreateDataModel();

                // Call
                void Call() => getFromRegistry(registry, dataModel);

                // Assert
                Assert.Throws<InvalidOperationException>(Call);
            }

            [Test]
            public void Get_OtherDataModelAdded_ThrowsInvalidOperationException()
            {
                // Setup
                var registry = new ExportableModelRegistry();
                registerToRegistry(registry, CreateExportableModel(), CreateDataModel());

                // Call
                void Call() => getFromRegistry(registry, CreateDataModel());

                // Assert
                Assert.Throws<InvalidOperationException>(Call);
            }

            /// <summary>
            /// Creates a new instance of <see cref="TDataModel"/>.
            /// </summary>
            /// <returns>An instance of <see cref="TDataModel"/>.</returns>
            protected abstract TDataModel CreateDataModel();

            /// <summary>
            /// Creates a new instance of <see cref="TExportableModel"/>.
            /// </summary>
            /// <returns></returns>
            protected abstract TExportableModel CreateExportableModel();
        }

        [TestFixture]
        private class CombinedFailureMechanismSectionAssemblyResultTest : RegistryTest<CombinedFailureMechanismSectionAssemblyResult,
            ExportableCombinedFailureMechanismSection>
        {
            public CombinedFailureMechanismSectionAssemblyResultTest() : base(
                (r, e, m) => r.Register(m, e),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override CombinedFailureMechanismSectionAssemblyResult CreateDataModel()
            {
                return CombinedFailureMechanismSectionAssemblyResultTestFactory.Create();
            }

            protected override ExportableCombinedFailureMechanismSection CreateExportableModel()
            {
                return ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection();
            }
        }

        [TestFixture]
        private class FailureMechanismSectionTest : RegistryTest<FailureMechanismSection,
            ExportableFailureMechanismSection>
        {
            public FailureMechanismSectionTest() : base(
                (r, e, m) => r.Register(m, e),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override FailureMechanismSection CreateDataModel()
            {
                return FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            }

            protected override ExportableFailureMechanismSection CreateExportableModel()
            {
                return ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            }
        }

        [TestFixture]
        private class FailureMechanismSectionResultTest : RegistryTest<FailureMechanismSectionResult,
            ExportableFailureMechanismSectionAssemblyResult>
        {
            public FailureMechanismSectionResultTest() : base(
                (r, e, m) => r.Register(m, e),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override FailureMechanismSectionResult CreateDataModel()
            {
                return FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            }

            protected override ExportableFailureMechanismSectionAssemblyResult CreateExportableModel()
            {
                ExportableFailureMechanismSection exportableSection =
                    ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
                return ExportableFailureMechanismSectionAssemblyResultTestFactory.Create(exportableSection, 21);
            }
        }
    }
}