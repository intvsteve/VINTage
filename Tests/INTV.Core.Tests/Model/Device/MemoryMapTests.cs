// <copyright file="MemoryMapTests.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model.Device;
using Xunit;

namespace INTV.Core.Tests.Model.Device
{
    public class MemoryMapTests
    {
        public static IEnumerable<object[]> MemoryMapTestData
        {
            get
            {
                yield return new object[] { (ushort)0, (ushort)0, null };
                yield return new object[] { (ushort)0x100, (ushort)32, null };
                yield return new object[] { (ushort)0x200, (ushort)32, new ushort[] { 0x4200, 0x8200, 0xC200 } };
            }
        }

        [Theory]
        [MemberData("MemoryMapTestData")]
        public void MemoryMap_ValidateProperConstruction(ushort baseAddress, ushort size, IEnumerable<ushort> aliases)
        {
            var memoryMap = new MemoryMap(baseAddress, size, aliases);
            var expectedAliases = aliases == null ? Enumerable.Empty<ushort>() : aliases;

            Assert.Equal(baseAddress, memoryMap.BaseAddress);
            Assert.Equal(size, memoryMap.Size);
            Assert.Equal(expectedAliases, memoryMap.Aliases);
        }

        [Fact]
        public void MemoryMapSubclass_ValidateProperConstruction()
        {
            var myCoolDevice = new MyCoolDevice();
            var expectedRegisters = Enum.GetValues(typeof(MyCoolDeviceRegister)).Cast<MyCoolDeviceRegister>();
            var expectedRegisterAddresses = new ushort[] { 0x0D00, 0x0D02, 0x0D04, 0x0D06 };

            var deviceRegisterAddresses = expectedRegisters.Select(r => myCoolDevice.GetMemoryLocationForRegister(r));

            Assert.Equal(expectedRegisters, myCoolDevice.Registers);
            Assert.Equal(expectedRegisterAddresses, deviceRegisterAddresses);
        }

        private enum MyCoolDeviceRegister
        {
            /// <summary>Register 0.</summary>
            R0,

            /// <summary>Register 1.</summary>
            R1,

            /// <summary>Register 2.</summary>
            R2,

            /// <summary>Register 3.</summary>
            R3
        }

        private class MyCoolDevice : MemoryMap<MyCoolDeviceRegister>
        {
            public MyCoolDevice()
                : base(0x0D00, 32, new ushort[] { 0x4D00, 0x8D00, 0xCD00 })
            {
                InitializeRegisterMemoryMap();
            }

            public void DoubleRegisterARegister()
            {
                AddRegister(MyCoolDeviceRegister.R2, 0x0D02);
            }

            protected override void InitializeRegisterMemoryMap()
            {
                AddRegister(MyCoolDeviceRegister.R0, 0x0D00);
                AddRegister(MyCoolDeviceRegister.R1, 0x0D02);
                AddRegister(MyCoolDeviceRegister.R2, 0x0D04);
                AddRegister(MyCoolDeviceRegister.R3, 0x0D06);
            }
        }
    }
}
