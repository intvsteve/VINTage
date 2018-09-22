// <copyright file="EcsTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class EcsTests
    {
        [Fact]
        public void Ecs_Connections_AreValid()
        {
            var ecs = new Ecs();

            Assert.NotNull(ecs.Connections.FirstOrDefault(c => c is MemoryMap));
            Assert.NotNull(ecs.Connections.FirstOrDefault(c => c is CartridgePort));
        }

        [Fact]
        public void Ecs_IRomCompatibleWithNull_ThrowsNullReferenceException()
        {
            var ecs = new Ecs();

            Assert.Throws<NullReferenceException>(() => ecs.IsRomCompatible(null));
        }

        [Fact]
        public void Ecs_IRomCompatibleWithEcsRom_IsTrue()
        {
            var ecs = new Ecs();
            var programInformationTable = ProgramInformationTable.Initialize(Enumerable.Empty<ProgramInformationTableDescriptor>().ToArray());
            var mindStrikeProgramId = new ProgramIdentifier(0x9D57498F);
            var mindStrikeInfo = programInformationTable.FindProgram(mindStrikeProgramId);
            Assert.NotNull(mindStrikeInfo);
            var mindStrikeDescription = new ProgramDescription(mindStrikeProgramId.DataCrc, null, mindStrikeInfo);

            Assert.True(ecs.IsRomCompatible(mindStrikeDescription));
        }
    }
}
