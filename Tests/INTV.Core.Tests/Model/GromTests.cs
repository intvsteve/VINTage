// <copyright file="GromTests.cs" company="INTV Funhouse">
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

using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class GromTests
    {
        [Fact]
        public void Grom_Name_IsCorrect()
        {
            var grom = new Grom();

            Assert.Equal("GROM", grom.Name);
        }

        [Fact]
        public void Grom_Connections_AreValid()
        {
            var grom = new Grom();

            Assert.NotNull(grom.Connections.FirstOrDefault(c => c is MemoryMap));
            Assert.Equal(1, grom.Connections.Count());
        }

        [Fact]
        public void Grom_IsRomCompatible_IsTrue()
        {
            var grom = new Grom();

            Assert.True(grom.IsRomCompatible(null));
        }
    }
}
