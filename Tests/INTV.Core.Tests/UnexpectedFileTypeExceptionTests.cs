// <copyright file="UnexpectedFileTypeExceptionTests.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using Xunit;

namespace INTV.Core.Tests
{
    public class UnexpectedFileTypeExceptionTests
    {
        [Fact]
        public void UnexpectedFileTypeException_CallConstructorWithExpectedTypeName_HasCorrectMessage()
        {
            var testFileTypeName = "TestFileType";
            var exception = new UnexpectedFileTypeException(testFileTypeName);

            Assert.NotEmpty(exception.Message);
            Assert.Equal(testFileTypeName, exception.ExpectedTypeName);
        }

        [Fact]
        public void UnexpectedFileTypeException_CallConstructorWithExpectedTypeNameAndMessage_HasCorrectMessage()
        {
            var testFileTypeName = "AnotherTestFileType";
            var testMessage = "This is a test of the message.";
            var exception = new UnexpectedFileTypeException(testFileTypeName, testMessage);

            Assert.Equal(testMessage, exception.Message);
            Assert.Equal(testFileTypeName, exception.ExpectedTypeName);
        }
    }
}
