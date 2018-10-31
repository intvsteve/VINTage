﻿// <copyright file="AlternateRomTests.cs" company="INTV Funhouse">
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
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class AlternateRomTests
    {
        private const string TestBinPath = "/Resources/tagalong.bin";
        private const string TestCfgPath = "/Resources/tagalong.cfg";

        private const uint TestBinCrc = 0xECBA3AF7;
        private const uint TestCfgCrc = 0x06B5EA3E;

        [Fact]
        public void AlternateRom_CreateWithNullArguments_ThrowsArgumentNullException()
        {
            AlternateRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);

            Assert.Throws<ArgumentNullException>(() => new AlternateRom(null, null, null));
        }

        [Fact]
        public void AlternateRom_CreateWithNullAlternateBinPath_ThrowsArgumentNullException()
        {
            var storageAccess = AlternateRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);

            var rom = Rom.Create(TestBinPath, TestCfgPath);
            Assert.NotNull(rom);
            var alternateCfgPath = "/Resources/TestWithNullBinPath/tagalong.cfg";
            storageAccess.CreateCopyOfResource(TestCfgPath, alternateCfgPath);

            Assert.Throws<ArgumentNullException>(() => new AlternateRom(null, alternateCfgPath, rom));
        }

        [Fact]
        public void AlternateRom_CreateWithNullAlternateCfgPath_CreatesAlternateWithNullCfgPath()
        {
            var storageAccess = AlternateRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);

            var rom = Rom.Create(TestBinPath, TestCfgPath);
            Assert.NotNull(rom);
            var alternateBinPath = "/Resources/TestWithNullCfgPath/tagalong.bin";
            storageAccess.CreateCopyOfResource(TestBinPath, alternateBinPath);
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
            var storageAccess = AlternateRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);

            var alternateBinPath = "/Resources/TestWithNullOriginalROM/tagalong.bin";
            storageAccess.CreateCopyOfResource(TestBinPath, alternateBinPath);
            var alternateCfgPath = "/Resources/TestWithNullOriginalROM/tagalong.cfg";
            storageAccess.CreateCopyOfResource(TestCfgPath, alternateCfgPath);
            var alternate = new AlternateRom(alternateBinPath, alternateCfgPath, null);

            Assert.NotNull(alternate);
            Assert.True(alternate.IsValid);
            Assert.Null(alternate.Original);
            Assert.NotNull(alternate.Alternate);
            Assert.Equal(alternateBinPath, alternate.RomPath);
            Assert.Equal(alternateCfgPath, alternate.ConfigPath);
            Assert.Equal(TestBinCrc, alternate.Crc);
            Assert.Equal(TestCfgCrc, alternate.CfgCrc);
            Assert.True(alternate.Validate());
        }

        [Fact]
        public void AlternateRom_CreateWithNonExistentPaths_ThrowsNullReferenceException()
        {
            var storageAccess = AlternateRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var alternateBinPath = "/Resources/TestWithNonexistentPaths/tagalong.bin";
            var alternateCfgPath = "/Resources/TestWithNonexistentPaths/tagalong.cfg";

            Assert.Throws<NullReferenceException>(() => new AlternateRom(alternateBinPath, alternateCfgPath, null));
        }

        [Fact]
        public void AlternateRom_CreateWithNonExistentCfgPath_ThrowsNullReferenceException()
        {
            var storageAccess = AlternateRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var alternateBinPath = "/Resources/TestWithMissingCfgPath/tagalong.bin";
            storageAccess.CreateCopyOfResource(TestBinPath, alternateBinPath);
            var alternateCfgPath = "/Resources/TestWithMissingCfgPath/tagalong.cfg";

            Assert.Throws<NullReferenceException>(() => new AlternateRom(alternateBinPath, alternateCfgPath, null));
        }

        [Fact]
        public void AlternateRom_ChangeAlternateCfgPath_RefreshCfgCrcChangesCrc()
        {
            var storageAccess = AlternateRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var alternateBinPath = "/Resources/TestWithCfgAtAlternateLocationThatIsChanging/tagalong.bin";
            storageAccess.CreateCopyOfResource(TestBinPath, alternateBinPath);
            var alternateCfgPath = "/Resources/TestWithCfgAtAlternateLocationThatIsChanging/tagalong.cfg";
            storageAccess.CreateCopyOfResource(TestCfgPath, alternateCfgPath);
            var alternate = new AlternateRom(alternateBinPath, alternateCfgPath, null);
            Assert.NotNull(alternate);
            Assert.True(alternate.IsValid);
            Assert.Equal(TestCfgCrc, alternate.CfgCrc);

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
            Assert.NotEqual(TestCfgCrc, cfgCrc);
            Assert.Equal(cfgCrc, alternate.CfgCrc);
            Assert.Equal(cfgCrc, alternate.Alternate.CfgCrc);
            Assert.Equal("Weener Weener Cheekeen Deeener", alternate.GetBinFileMetadata().LongNames.First());
        }

        [Fact]
        public void AlternateRom_ChangeAlternateRom_RefreshCrcChangesCrc()
        {
            var storageAccess = AlternateRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var rom = Rom.Create(TestBinPath, TestCfgPath);
            var alternateBinPath = "/Resources/TestWithRomAtAlternateLocationThatIsChagning/tagalong.bin";
            storageAccess.CreateCopyOfResource(TestBinPath, alternateBinPath);
            var alternate = new AlternateRom(alternateBinPath, null, rom);
            Assert.NotNull(alternate);
            Assert.True(alternate.IsValid);
            Assert.Equal(0u, alternate.CfgCrc);
            Assert.Equal(TestBinCrc, alternate.Crc);

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
            Assert.NotEqual(TestBinCrc, crc);
            Assert.Equal(alternate.Crc, alternate.Alternate.Crc);
            Assert.NotEqual(alternate.Original.Crc, alternate.Crc);
            Assert.NotEqual(alternate.Original.Crc, alternate.Alternate.Crc);
        }

        private class AlternateRomTestStorageAccess : CachedResourceStorageAccess<AlternateRomTestStorageAccess>
        {
        }
    }
}