// <copyright file="AlternateRomTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class AlternateRomTests
    {
        [Fact]
        public void AlternateRom_CreateWithNullArguments_ThrowsArgumentNullException()
        {
            AlternateRomTestStorageAccess.Initialize(null);

            Assert.Throws<ArgumentNullException>(() => new AlternateRom(null, null, null));
        }

        [Fact]
        public void AlternateRom_CreateWithNullAlternateBinPath_ThrowsArgumentNullException()
        {
            IReadOnlyList<string> paths;
            var storageAccess = AlternateRomTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);

            var rom = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom);
            var alternateCfgPath = "/Resources/TestWithNullBinPath/tagalong.cfg";
            storageAccess.CreateCopyOfResource(TestRomResources.TestCfgPath, alternateCfgPath);

            Assert.Throws<ArgumentNullException>(() => new AlternateRom(null, alternateCfgPath, rom));
        }

        [Fact]
        public void AlternateRom_CreateWithNullAlternateCfgPath_CreatesAlternateWithNullCfgPath()
        {
            IReadOnlyList<string> paths;
            var storageAccess = AlternateRomTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);

            var rom = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom);
            var alternateBinPath = "/Resources/TestWithNullCfgPath/tagalong.bin";
            storageAccess.CreateCopyOfResource(TestRomResources.TestBinPath, alternateBinPath);
            var alternate = new AlternateRom(alternateBinPath, null, rom);

            Assert.NotNull(alternate);
            Assert.True(alternate.IsValid);
            Assert.NotNull(alternate.Original);
            Assert.NotNull(alternate.Alternate);
            Assert.NotEqual(rom.RomPath, alternate.RomPath);
            Assert.NotEqual(rom.ConfigPath, alternate.ConfigPath);
            Assert.Null(alternate.ConfigPath);
            Assert.Equal(rom.Format, alternate.Format);
            Assert.Equal(rom.Crc, alternate.Crc);
            Assert.Equal(rom.Crc, alternate.Crc);
            Assert.NotEqual(rom.CfgCrc, alternate.CfgCrc);
            Assert.Equal(0u, alternate.CfgCrc);
            Assert.True(alternate.Validate());
        }

        [Fact]
        public void AlternateRom_CreateWithNullOriginalRom_AlternateIsValid()
        {
            var paths = AlternateRomTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);

            var alternateBinPath = paths[0]; // "/Resources/TestWithNullOriginalROM/tagalong.bin";
            var alternateCfgPath = paths[1]; // "/Resources/TestWithNullOriginalROM/tagalong.cfg";
            var alternate = new AlternateRom(alternateBinPath, alternateCfgPath, null);

            Assert.NotNull(alternate);
            Assert.True(alternate.IsValid);
            Assert.Null(alternate.Original);
            Assert.NotNull(alternate.Alternate);
            Assert.Equal(alternateBinPath, alternate.RomPath);
            Assert.Equal(alternateCfgPath, alternate.ConfigPath);
            Assert.Equal(TestRomResources.TestBinCrc, alternate.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, alternate.CfgCrc);
            Assert.True(alternate.Validate());
        }

        [Fact]
        public void AlternateRom_CreateWithNonExistentPaths_ThrowsNullReferenceException()
        {
            AlternateRomTestStorageAccess.Initialize(null);
            var alternateBinPath = "/Resources/TestWithNonexistentPaths/tagalong.bin";
            var alternateCfgPath = "/Resources/TestWithNonexistentPaths/tagalong.cfg";

            Assert.Throws<NullReferenceException>(() => new AlternateRom(alternateBinPath, alternateCfgPath, null));
        }

        [Fact]
        public void AlternateRom_CreateWithNonExistentCfgPath_ThrowsNullReferenceException()
        {
            var alternateBinPath = AlternateRomTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestCfgPath).First();
            var alternateCfgPath = "/Resources/TestWithMissingCfgPath/tagalong.cfg";

            Assert.Throws<NullReferenceException>(() => new AlternateRom(alternateBinPath, alternateCfgPath, null));
        }

        [Fact]
        public void AlternateRom_ChangeAlternateCfgPath_RefreshCfgCrcChangesCrc()
        {
            IReadOnlyList<string> paths;
            var storageAccess = AlternateRomTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var alternateBinPath = paths[0];
            var alternateCfgPath = paths[1];
            var alternate = new AlternateRom(alternateBinPath, alternateCfgPath, null);
            Assert.NotNull(alternate);
            Assert.True(alternate.IsValid);
            Assert.Equal(TestRomResources.TestCfgCrc, alternate.CfgCrc);

            using (var cfgStream = storageAccess.Open(alternateCfgPath))
            {
                cfgStream.Seek(0, System.IO.SeekOrigin.End);
                var cfgToAppend =
@"
[vars]
name = ""Weener Weener Cheekeen Deeener""
";
                var cfgDataToAppend = System.Text.Encoding.UTF8.GetBytes(cfgToAppend);
                cfgStream.Write(cfgDataToAppend, 0, cfgDataToAppend.Length);
                cfgStream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            var cfgCrcChanged = false;
            var cfgCrc = alternate.RefreshCfgCrc(out cfgCrcChanged);

            Assert.True(cfgCrcChanged);
            Assert.NotEqual(TestRomResources.TestCfgCrc, cfgCrc);
            Assert.Equal(cfgCrc, alternate.CfgCrc);
            Assert.Equal(cfgCrc, alternate.Alternate.CfgCrc);
            Assert.Equal("Weener Weener Cheekeen Deeener", alternate.GetBinFileMetadata().LongNames.First());
        }

        [Fact]
        public void AlternateRom_ChangeAlternateRom_RefreshCrcChangesCrc()
        {
            IReadOnlyList<string> paths;
            var storageAccess = AlternateRomTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);
            var alternateBinPath = "/Resources/TestWithRomAtAlternateLocationThatIsChanging/tagalong.bin";
            storageAccess.CreateCopyOfResource(TestRomResources.TestBinPath, alternateBinPath);
            var alternate = new AlternateRom(alternateBinPath, null, rom);
            Assert.NotNull(alternate);
            Assert.True(alternate.IsValid);
            Assert.Equal(0u, alternate.CfgCrc);
            Assert.Equal(TestRomResources.TestBinCrc, alternate.Crc);

            using (var binStream = storageAccess.Open(alternateBinPath))
            {
                binStream.Seek(0, System.IO.SeekOrigin.End);
                var dataToAppend = Enumerable.Repeat((byte)0xFF, 128).ToArray();
                binStream.Write(dataToAppend, 0, 128);
                binStream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            var crcChanged = false;
            var crc = alternate.RefreshCrc(out crcChanged);

            Assert.True(crcChanged);
            Assert.Equal(crc, alternate.Crc);
            Assert.NotEqual(TestRomResources.TestBinCrc, crc);
            Assert.Equal(alternate.Crc, alternate.Alternate.Crc);
            Assert.NotEqual(alternate.Original.Crc, alternate.Crc);
            Assert.NotEqual(alternate.Original.Crc, alternate.Alternate.Crc);
        }

        private class AlternateRomTestStorageAccess : CachedResourceStorageAccess<AlternateRomTestStorageAccess>
        {
        }
    }
}
