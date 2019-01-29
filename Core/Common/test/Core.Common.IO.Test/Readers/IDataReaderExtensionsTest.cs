// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Data;
using System.Globalization;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.IO.Test.Readers
{
    [TestFixture]
    public class IDataReaderExtensionsTest
    {
        [Test]
        public void Read_NullDataReader_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((IDataReader) null).Read<Int32>("column");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dataReader", paramName);
        }

        [Test]
        public void Read_NullColumnName_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dataReader = mockRepository.Stub<IDataReader>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => dataReader.Read<Int32>(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("columnName", paramName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Read_NotExistingColumnName_ThrowsArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dataReader = mockRepository.Stub<IDataReader>();
            const string columnName = "SomeColumn";
            dataReader.Stub(dr => dr[columnName]).Throw(new IndexOutOfRangeException());
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => dataReader.Read<Int32>(columnName);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                test, "Column \'SomeColumn\' not defined for data row.");
            string paramName = exception.ParamName;
            Assert.AreEqual("columnName", paramName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Read_NumberWithInvalidFormat_ThrowsInvalidCastException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dataReader = mockRepository.Stub<IDataReader>();
            const string columnName = "SomeColumn";
            dataReader.Stub(dr => dr[columnName]).Return("3..2");
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => dataReader.Read<int>(columnName);

            // Assert
            string message = Assert.Throws<ConversionException>(test).Message;
            Assert.AreEqual("Value read from data reader ('3..2') is an incorrect format to transform to type System.Int32.", message);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Read_UsingTypeNotMatchingColumnType_ThrowsInvalidCastException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dataReader = mockRepository.Stub<IDataReader>();
            const string columnName = "SomeColumn";
            const double value = 3.9;
            dataReader.Stub(dr => dr[columnName]).Return(value);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => dataReader.Read<IDataReader>(columnName);

            // Assert
            string message = Assert.Throws<ConversionException>(test).Message;
            string expectedMessage = string.Format(CultureInfo.CurrentCulture,
                                                   "Value read from data reader ('{0}') could not be cast to desired type System.Data.IDataReader.",
                                                   value);
            Assert.AreEqual(expectedMessage, message);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Read_ColumnValueTooLargeForUsedType_ThrowsInvalidCastException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dataReader = mockRepository.Stub<IDataReader>();
            const string columnName = "SomeColumn";
            dataReader.Stub(dr => dr[columnName]).Return(3e139);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => dataReader.Read<int>(columnName);

            // Assert
            string message = Assert.Throws<ConversionException>(test).Message;
            Assert.AreEqual("Value read from data reader ('3E+139') was too large to convert to type System.Int32.", message);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Read_UsingSameTypeAsColumnType_ReturnsTypedValue()
        {
            // Setup
            const string columnName = "SomeColumn";
            const string testValue = "testValue";

            var mockRepository = new MockRepository();
            var dataReader = mockRepository.Stub<IDataReader>();
            dataReader.Stub(dr => dr[columnName]).Return(testValue);
            mockRepository.ReplayAll();

            // Call
            var result = dataReader.Read<string>(columnName);

            // Assert
            Assert.AreEqual(testValue, result);

            mockRepository.VerifyAll();
        }
    }
}