// <copyright file="ProgramInformationTableDescriptorTests.cs" company="INTV Funhouse">
// Copyright (c) 2018-2019 All Rights Reserved
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

using System;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramInformationTableDescriptorTests
    {
        [Fact]
        public void ProgramInformationTableDescriptor_DefaultConstructor_InitializesAsExpected()
        {
            var descriptor = new ProgramInformationTableDescriptor();

            Assert.Null(descriptor.FilePath.Path);
            Assert.Null(descriptor.Factory);
        }

        [Fact]
        public void ProgramInformationTableDescriptor_ConstructorWithNullArguments_DoesNotThrow()
        {
            var descriptor = new ProgramInformationTableDescriptor(StorageLocation.Null, null);

            Assert.Null(descriptor.FilePath.Path);
            Assert.Null(descriptor.Factory);
        }

        [Fact]
        public void ProgramInformationTableDescriptor_ConstructorWithArguments_InitializesCorrectly()
        {
            var factory = new Func<StorageLocation, IProgramInformationTable>(p =>
                {
                    return null;
                });
            var filePath = new StorageLocation(@"C:\Program Files\lol.xml");
            var descriptor = new ProgramInformationTableDescriptor(filePath, factory);

            Assert.Equal(filePath, descriptor.FilePath);
            Assert.Equal(factory, descriptor.Factory);
        }
    }
}
