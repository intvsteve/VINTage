// <copyright file="XmlRomTests.cs" company="INTV Funhouse">
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
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class XmlRomTests
    {
        private const string TestRomPath = "/Resources/tagalong.rom";
        private const string TestRomMetadataPath = "/Resources/tagalong_metadata.rom";
        private const string TestBinPath = "/Resources/tagalong.bin";
        private const string TestCfgPath = "/Resources/tagalong.cfg";
        private const string TestCfgMetadataPath = "/Resources/tagalong_metadata.cfg";
        private const string TestLuigiFromBinPath = "/Resources/tagalong.luigi";

        private const uint TestRomCrc = 0xFEF0BD41;
        private const uint TestRomMetadataCrc = 0xC03B6B9E;
        private const uint TestBinCrc = 0xECBA3AF7;
        private const uint TestCfgCrc = 0x06B5EA3E;
        private const uint TestMetadataCfgCrc = 0x68C3401C;

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
            XmlFormatRomTestStorageAccess.Initialize(null);
            var rom = new XmlRom();

            Assert.Throws<ArgumentNullException>(() => rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToNullRomPath_ThrowsArgumentNullException()
        {
            XmlFormatRomTestStorageAccess.Initialize(null);
            var rom = new XmlRom();

            Assert.Throws<ArgumentNullException>(() => rom.UpdateRomPath(null));
        }

        [Fact]
        public void XmlRom_SetToNonexistentRomPath_RemainsInvalid()
        {
            XmlFormatRomTestStorageAccess.Initialize(null);
            var rom = new XmlRom();

            rom.UpdateRomPath("/oops/" + TestRomPath);

            Assert.False(rom.IsValid);
            Assert.Null(rom.ResolvedRom);
        }

        [Fact]
        public void XmlRom_SetToNullCfgPath_CfgCrcRemainsZero()
        {
            XmlFormatRomTestStorageAccess.Initialize(null);
            var rom = new XmlRom();

            rom.UpdateConfigPath(null);

            Assert.Equal(null, rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void XmlRom_SetToNonexistentCfgPath_CfgPathChanges()
        {
            XmlFormatRomTestStorageAccess.Initialize(null);
            var rom = new XmlRom();

            var invalidCfgPath = "/ooops/" + TestCfgPath;
            rom.UpdateConfigPath(invalidCfgPath);

            Assert.Equal(invalidCfgPath, rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void XmlRom_SetToValidRomPath_BecomesValid()
        {
            XmlFormatRomTestStorageAccess.Initialize(TestRomPath);
            var rom = new XmlRom();

            rom.UpdateRomPath(TestRomPath);

            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Rom, rom.Format);
            Assert.Equal(TestRomPath, rom.RomPath);
            Assert.Equal(null, rom.ConfigPath);
            Assert.Equal(TestRomCrc, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToValidRomPathChangeToAnotherValidPath_RemainsValid()
        {
            XmlFormatRomTestStorageAccess.Initialize(TestRomPath, TestRomMetadataPath);
            var rom = new XmlRom();
            rom.UpdateRomPath(TestRomPath);
            Assert.True(rom.IsValid);
            Assert.Equal(TestRomCrc, rom.Crc);
            Assert.Equal(TestRomPath, rom.RomPath);

            rom.UpdateRomPath(TestRomMetadataPath);

            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Rom, rom.Format);
            Assert.Equal(TestRomMetadataPath, rom.RomPath);
            Assert.Equal(null, rom.ConfigPath);
            Assert.Equal(TestRomMetadataCrc, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToValidRomPathChangeToInvalidPath_BecomesInvalid()
        {
            XmlFormatRomTestStorageAccess.Initialize(TestRomPath);
            var rom = new XmlRom();
            rom.UpdateRomPath(TestRomPath);
            Assert.True(rom.IsValid);
            Assert.Equal(TestRomCrc, rom.Crc);
            Assert.Equal(TestRomPath, rom.RomPath);

            var invalidPath = "/bad/" + TestRomPath;
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
            XmlFormatRomTestStorageAccess.Initialize(TestRomPath, TestBinPath, TestCfgPath);
            var rom = new XmlRom();
            rom.UpdateRomPath(TestRomPath);
            Assert.True(rom.IsValid);
            Assert.Equal(TestRomCrc, rom.Crc);
            Assert.Equal(TestRomPath, rom.RomPath);
            Assert.Equal(RomFormat.Rom, rom.Format);

            rom.UpdateRomPath(TestBinPath);
            rom.UpdateConfigPath(TestCfgPath);

            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.Equal(TestBinPath, rom.RomPath);
            Assert.Equal(TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestBinCrc, rom.Crc);
            Assert.Equal(TestCfgCrc, rom.CfgCrc);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void XmlRom_SetToValidBinPathChangeCfgPath_BinCrcUnchangedAndCfgCrcUpdatesImmediately()
        {
            var storageAccess = XmlFormatRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath, TestCfgMetadataPath);
            var rom = new XmlRom();
            rom.UpdateRomPath(TestBinPath);
            rom.UpdateConfigPath(TestCfgPath);
            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.Equal(TestBinPath, rom.RomPath);
            Assert.Equal(TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestBinCrc, rom.Crc);
            Assert.Equal(TestCfgCrc, rom.CfgCrc);

            rom.UpdateConfigPath(TestCfgMetadataPath);
            var crcChanged = true;
            var crc = rom.RefreshCrc(out crcChanged);
            var cfgCrcChanged = true;
            var cfgCrc = rom.RefreshCfgCrc(out cfgCrcChanged);

            Assert.False(crcChanged);
            Assert.Equal(TestBinCrc, rom.Crc);
            Assert.Equal(TestBinCrc, crc);
            Assert.False(cfgCrcChanged);
            Assert.Equal(TestMetadataCfgCrc, cfgCrc);
            Assert.Equal(TestMetadataCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void XmlRom_SetToValidBinThenModifyBin_RefreshCrcChanges()
        {
            var storageAccess = XmlFormatRomTestStorageAccess.Initialize(TestCfgPath);
            var testBinToModifyPath = "/Resources/tagalong_hacked.bin";
            storageAccess.CreateCopyOfResource(TestBinPath, testBinToModifyPath);

            var rom = new XmlRom();
            rom.UpdateRomPath(testBinToModifyPath);
            rom.UpdateConfigPath(TestCfgPath);
            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.Equal(testBinToModifyPath, rom.RomPath);
            Assert.Equal(TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestBinCrc, rom.Crc);
            Assert.Equal(TestCfgCrc, rom.CfgCrc);

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
            Assert.NotEqual(TestBinCrc, crc);
        }

        [Fact]
        public void XmlRom_SetToValidBinThenModifyCfg_RefreshCfgCrcChanges()
        {
            var storageAccess = XmlFormatRomTestStorageAccess.Initialize(TestBinPath);
            var testCfgToModifyPath = "/Resources/tagalong_hacked.cfg";
            storageAccess.CreateCopyOfResource(TestCfgPath, testCfgToModifyPath);

            var rom = new XmlRom();
            rom.UpdateRomPath(TestBinPath);
            rom.UpdateConfigPath(testCfgToModifyPath);
            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.Equal(TestBinPath, rom.RomPath);
            Assert.Equal(testCfgToModifyPath, rom.ConfigPath);
            Assert.Equal(TestBinCrc, rom.Crc);
            Assert.Equal(TestCfgCrc, rom.CfgCrc);

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
            Assert.NotEqual(TestCfgCrc, cfgCrc);
            Assert.Equal("wut?", rom.GetBinFileMetadata().LongNames.First());
        }

        private class XmlFormatRomTestStorageAccess : CachedResourceStorageAccess<XmlFormatRomTestStorageAccess>
        {
        }
    }
}
