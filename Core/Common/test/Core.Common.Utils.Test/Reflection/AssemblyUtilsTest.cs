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

using System;
using System.IO;
using System.Reflection;
using Core.Common.Utils.Reflection;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Reflection
{
    [TestFixture]
    public class AssemblyUtilsTest
    {
        [Test]
        public void GetAssemblyInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyUtils.AssemblyInfo assemblyInfo = AssemblyUtils.GetAssemblyInfo(assembly);
            Assert.AreEqual("Core.Common.Utils.Test", assemblyInfo.Title);
        }

        [Test]
        public void GetAssemblyInfo_LocationIsEmpty_ReturnEmptyAssemblyInfo()
        {
            // Setup
            var assemblyWithoutLocation = new MockedAssemblyWithoutLocation();

            // Call
            AssemblyUtils.AssemblyInfo assemblyInfo = AssemblyUtils.GetAssemblyInfo(assemblyWithoutLocation);

            // Assert
            Assert.IsNull(assemblyInfo.Company);
            Assert.IsNull(assemblyInfo.Copyright);
            Assert.IsNull(assemblyInfo.Description);
            Assert.IsNull(assemblyInfo.Product);
            Assert.IsNull(assemblyInfo.Title);
            Assert.IsNull(assemblyInfo.Version);
        }

        [Test]
        public void GetAssemblyInfo_ForThisTestProjectAssmebly_ReturnAssmeblyInfoWithExpectedValues()
        {
            // Setup
            Assembly assembly = Assembly.GetAssembly(GetType());

            // Call
            AssemblyUtils.AssemblyInfo assemblyInfo = AssemblyUtils.GetAssemblyInfo(assembly);

            // Assert
            Assert.AreEqual("Deltares", assemblyInfo.Company);
            Assert.AreEqual("Copyright � Deltares 2016", assemblyInfo.Copyright);
            Assert.IsEmpty(assemblyInfo.Description);
            Assert.AreEqual("Core.Common.Utils.Test", assemblyInfo.Product);
            Assert.AreEqual("Core.Common.Utils.Test", assemblyInfo.Title);
            StringAssert.StartsWith("16.3.1.", assemblyInfo.Version);
        }

        [Test]
        public void GetExecutingAssemblyInfo_ReturnAssmeblyInfoForAssemblyUtilsAssembly()
        {
            // Setup
            Assembly assembly = Assembly.GetAssembly(typeof(AssemblyUtils));
            AssemblyUtils.AssemblyInfo assemblyInfo = AssemblyUtils.GetAssemblyInfo(assembly);

            // Call
            AssemblyUtils.AssemblyInfo executingAssemblyInfo = AssemblyUtils.GetExecutingAssemblyInfo();

            // Assert
            Assert.AreEqual(assemblyInfo.Company, executingAssemblyInfo.Company);
            Assert.AreEqual(assemblyInfo.Copyright, executingAssemblyInfo.Copyright);
            Assert.AreEqual(assemblyInfo.Description, executingAssemblyInfo.Description);
            Assert.AreEqual(assemblyInfo.Product, executingAssemblyInfo.Product);
            Assert.AreEqual(assemblyInfo.Title, executingAssemblyInfo.Title);
            Assert.AreEqual(assemblyInfo.Version, executingAssemblyInfo.Version);
        }

        [Test]
        public void GetTypeByName_ForThisTestSuite_ReturnTypeOfThisTestSuite()
        {
            // Setup
            var typeName = GetType().FullName;

            // Call
            var returnedType = AssemblyUtils.GetTypeByName(typeName);

            // Assert
            Assert.AreEqual(GetType(), returnedType);
        }

        [Test]
        public void GetTypeByName_ForNonexistingClass_ReturnNull()
        {
            // Call
            var returnedType = AssemblyUtils.GetTypeByName("I.Dont.Exist");

            // Assert
            Assert.IsNull(returnedType);
        }

        [Test]
        public void GetAssemblyResourceStream_ForEmbeddedResource_ReturnStream()
        {
            // Call
            var stream = AssemblyUtils.GetAssemblyResourceStream(GetType().Assembly, "testFile.txt");

            // Assert
            using (var reader = new StreamReader(stream))
            {
                Assert.AreEqual("test test 1 2 3", reader.ReadToEnd());
            }
        }

        [Test]
        public void GetAssemblyResourceStream_ForNonexistingEmbeddedResource_ThrowArgumentException()
        {
            // Call
            TestDelegate call = () => AssemblyUtils.GetAssemblyResourceStream(GetType().Assembly, "I do not exist.txt");

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        private class MockedAssemblyWithoutLocation : Assembly
        {
            public override string Location
            {
                get
                {
                    return "";
                }
            }
        }
    }
}