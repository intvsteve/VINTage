﻿// <copyright file="XmlRomTests.cs" company="INTV Funhouse">
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
    public class XmlRomTests
    {
        [Fact]
        public void XmlRom_CreateUninitializedRom_IsNotValid()
        {
            var rom = new XmlRom();

            Assert.False(rom.IsValid);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRom_ResolvedRomIsNull()
        {
            var rom = new XmlRom();

            Assert.Null(rom.ResolvedRom);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRom_RomFormatIsNone()
        {
            var rom = new XmlRom();

            Assert.Equal(RomFormat.None, rom.Format);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRom_RomPathIsNull()
        {
            var rom = new XmlRom();

            Assert.Null(rom.RomPath);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRom_ConfigPathIsNull()
        {
            var rom = new XmlRom();

            Assert.Null(rom.ConfigPath);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRom_CrcIsZero()
        {
            var rom = new XmlRom();

            Assert.Equal(0u, rom.Crc);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRom_CfgCrcIsZero()
        {
            var rom = new XmlRom();

            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRomAndRefreshCrc_CrcRemainsZeroAndUnchanged()
        {
            var rom = new XmlRom();

            var changed = true;
            var crc = rom.RefreshCrc(out changed);

            Assert.False(changed);
            Assert.Equal(0u, crc);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRomAndRefreshCfgCrc_CfgCrcRemainsZeroAndUnchanged()
        {
            var rom = new XmlRom();

            var changed = true;
            var cfgCrc = rom.RefreshCfgCrc(out changed);

            Assert.False(changed);
            Assert.Equal(0u, cfgCrc);
        }

        [Fact]
        public void XmlRom_CreateUninitializedRomAnd_ValidateThrowsArgumentNullException()
        {
            XmlFormatRomTestStorageAccess.Initialize();
            var rom = new XmlRom();

            Assert.Throws<ArgumentNullException>(() => rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToNullRomPath_ThrowsArgumentNullException()
        {
            XmlFormatRomTestStorageAccess.Initialize();
            var rom = new XmlRom();

            Assert.Throws<ArgumentNullException>(() => rom.UpdateRomPath(null));
        }

        [Fact]
        public void XmlRom_SetToNonexistentRomPath_RemainsInvalid()
        {
            XmlFormatRomTestStorageAccess.Initialize();
            var rom = new XmlRom();

            rom.UpdateRomPath("/oopsie" + TestRomResources.TestRomPath);

            Assert.False(rom.IsValid);
            Assert.Null(rom.ResolvedRom);
        }

        [Fact]
        public void XmlRom_SetToNullCfgPath_CfgCrcRemainsZero()
        {
            XmlFormatRomTestStorageAccess.Initialize();
            var rom = new XmlRom();

            rom.UpdateConfigPath(null);

            Assert.Equal(null, rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void XmlRom_SetToNonexistentCfgPath_CfgPathChanges()
        {
            XmlFormatRomTestStorageAccess.Initialize();
            var rom = new XmlRom();

            var invalidCfgPath = "/ooops" + TestRomResources.TestCfgPath;
            rom.UpdateConfigPath(invalidCfgPath);

            Assert.Equal(invalidCfgPath, rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void XmlRom_SetToValidRomPath_BecomesValid()
        {
            var romPath = XmlFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomPath).First();
            var rom = new XmlRom();

            rom.UpdateRomPath(romPath);

            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Rom, rom.Format);
            Assert.Equal(romPath, rom.RomPath);
            Assert.Equal(null, rom.ConfigPath);
            Assert.Equal(TestRomResources.TestRomCrc, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToValidLuigiPath_BecomesValid()
        {
            var romPath = XmlFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = new XmlRom();

            rom.UpdateRomPath(romPath);

            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Luigi, rom.Format);
            Assert.Equal(romPath, rom.RomPath);
            Assert.Equal(null, rom.ConfigPath);
            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc); // LUIGI should return original .BIN ROM's CRC
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc); // LUIGI should return original .CFG file's CRC
            Assert.True(rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToValidRomPathChangeToAnotherValidPath_RemainsValid()
        {
            var paths = XmlFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomPath, TestRomResources.TestRomMetadataPath);
            var rom = new XmlRom();
            rom.UpdateRomPath(paths[0]);
            Assert.True(rom.IsValid);
            Assert.Equal(TestRomResources.TestRomCrc, rom.Crc);
            Assert.Equal(paths[0], rom.RomPath);

            rom.UpdateRomPath(paths[1]);

            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Rom, rom.Format);
            Assert.Equal(paths[1], rom.RomPath);
            Assert.Equal(null, rom.ConfigPath);
            Assert.Equal(TestRomResources.TestRomMetadataCrc, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToValidRomPathChangeToInvalidPath_BecomesInvalid()
        {
            var romPath = XmlFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomPath).First();
            var rom = new XmlRom();
            rom.UpdateRomPath(romPath);
            Assert.True(rom.IsValid);
            Assert.Equal(TestRomResources.TestRomCrc, rom.Crc);
            Assert.Equal(romPath, rom.RomPath);

            var invalidPath = "/bad" + romPath;
            rom.UpdateRomPath(invalidPath);

            Assert.False(rom.IsValid);
            Assert.Null(rom.ResolvedRom);
            Assert.Equal(RomFormat.None, rom.Format);
            Assert.Equal(invalidPath, rom.RomPath);
            Assert.Equal(null, rom.ConfigPath);
            Assert.Equal(0u, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
            Assert.False(rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToValidRomPathChangeToValidBinPathWithDifferentFormat_RemainsValid()
        {
            var paths = XmlFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomPath, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = new XmlRom();
            rom.UpdateRomPath(paths[0]);
            Assert.True(rom.IsValid);
            Assert.Equal(TestRomResources.TestRomCrc, rom.Crc);
            Assert.Equal(paths[0], rom.RomPath);
            Assert.Equal(RomFormat.Rom, rom.Format);

            rom.UpdateRomPath(paths[1]);
            rom.UpdateConfigPath(paths[2]);

            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.Equal(paths[1], rom.RomPath);
            Assert.Equal(paths[2], rom.ConfigPath);
            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToValidBinPathChangeCfgPath_BinCrcUnchangedAndCfgCrcUpdatesImmediately()
        {
            var paths = XmlFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath, TestRomResources.TestCfgMetadataPath);
            var rom = new XmlRom();
            rom.UpdateRomPath(paths[0]);
            rom.UpdateConfigPath(paths[1]);
            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.Equal(paths[0], rom.RomPath);
            Assert.Equal(paths[1], rom.ConfigPath);
            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            rom.UpdateConfigPath(paths[2]);
            var crcChanged = true;
            var crc = rom.RefreshCrc(out crcChanged);
            var cfgCrcChanged = true;
            var cfgCrc = rom.RefreshCfgCrc(out cfgCrcChanged);

            Assert.False(crcChanged);
            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestBinCrc, crc);
            Assert.False(cfgCrcChanged);
            Assert.Equal(TestRomResources.TestMetadataCfgCrc, cfgCrc);
            Assert.Equal(TestRomResources.TestMetadataCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void XmlRom_SetToValidBinThenModifyBin_RefreshCrcChanges()
        {
            IReadOnlyList<string> paths;
            var storageAccess = XmlFormatRomTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var testBinToModifyPath = paths[0];

            var rom = new XmlRom();
            rom.UpdateRomPath(testBinToModifyPath);
            rom.UpdateConfigPath(paths[1]);
            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.Equal(testBinToModifyPath, rom.RomPath);
            Assert.Equal(paths[1], rom.ConfigPath);
            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            using (var binStream = storageAccess.Open(testBinToModifyPath))
            {
                binStream.Seek(0, System.IO.SeekOrigin.End);
                var dataToAppend = Enumerable.Repeat((byte)0xFF, 128).ToArray();
                binStream.Write(dataToAppend, 0, 128);
                binStream.Seek(0, System.IO.SeekOrigin.Begin);
            }

            var crcChanged = false;
            var crc = rom.RefreshCrc(out crcChanged);

            Assert.True(crcChanged);
            Assert.NotEqual(TestRomResources.TestBinCrc, crc);
        }

        [Fact]
        public void XmlRom_SetToValidBinThenModifyCfg_RefreshCfgCrcChanges()
        {
            IReadOnlyList<string> paths;
            var storageAccess = XmlFormatRomTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var testCfgToModifyPath = paths[1];

            var rom = new XmlRom();
            rom.UpdateRomPath(paths[0]);
            rom.UpdateConfigPath(testCfgToModifyPath);
            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.Equal(paths[0], rom.RomPath);
            Assert.Equal(testCfgToModifyPath, rom.ConfigPath);
            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            using (var cfgStream = storageAccess.Open(testCfgToModifyPath))
            {
                cfgStream.Seek(0, System.IO.SeekOrigin.End);
                var cfgToAppend = 
@"
[vars]
name = ""wut?""
";
                var cfgDataToAppend = System.Text.Encoding.UTF8.GetBytes(cfgToAppend);
                cfgStream.Write(cfgDataToAppend, 0, cfgDataToAppend.Length);
                cfgStream.Seek(0, System.IO.SeekOrigin.Begin);
            }

            var cfgCrcChanged = false;
            var cfgCrc = rom.RefreshCfgCrc(out cfgCrcChanged);

            Assert.True(cfgCrcChanged);
            Assert.NotEqual(TestRomResources.TestCfgCrc, cfgCrc);
            Assert.Equal("wut?", rom.GetBinFileMetadata().LongNames.First());
        }

        private class XmlFormatRomTestStorageAccess : CachedResourceStorageAccess<XmlFormatRomTestStorageAccess>
        {
        }
    }
}
