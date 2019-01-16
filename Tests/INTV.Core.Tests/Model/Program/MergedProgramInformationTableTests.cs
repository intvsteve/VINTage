// <copyright file="MergedProgramInformationTableTests.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class MergedProgramInformationTableTests
    {
        [Fact]
        public void MergedProgramInformationTable_NeverMergedWithAnotherTable_HasNoPrograms()
        {
            var mergedInformationTable = new MergedProgramInformationTable();

            Assert.False(mergedInformationTable.Programs.Any());
        }

        [Fact]
        public void MergedProgramInformationTable_MergedWithEmptyTable_HasNoPrograms()
        {
            var mergedInformationTable = new MergedProgramInformationTable();

            var emptyTable = new TestProgramInformationTable();
            mergedInformationTable.MergeTable(emptyTable);

            Assert.False(mergedInformationTable.Programs.Any());
        }

        [Fact]
        public void MergedProgramInformationTable_MergedWithNonEmptyTable_HasPrograms()
        {
            var mergedInformationTable = new MergedProgramInformationTable();

            var testProgramInformation = new TestProgramInformation() { Title = "Marty McTesterson", Features = ProgramFeatures.GetUnrecognizedRomFeatures() };
            testProgramInformation.AddCrcs(1);
            var table = new TestProgramInformationTable();
            table.AddEntries(testProgramInformation);
            mergedInformationTable.MergeTable(table);

            Assert.True(mergedInformationTable.Programs.Any());
        }

        [Fact]
        public void MergedProgramInformationTable_FindRomWithProgramIdentifier_FindsInformationAsExpected()
        {
            var mergedInformationTable = new MergedProgramInformationTable();
            var testProgramIdentifier = new ProgramIdentifier(0x123u, 0x456u);
            var testProgramInformation = new TestProgramInformation() { Title = "Buffy Buckingham", Features = ProgramFeatures.GetUnrecognizedRomFeatures() };
            testProgramInformation.AddCrcs(3);
            testProgramInformation.AddCrc(testProgramIdentifier.DataCrc, "Version 0", IncompatibilityFlags.Tutorvision, testProgramIdentifier.OtherData);
            var table = new TestProgramInformationTable();
            table.AddEntries(testProgramInformation);
            mergedInformationTable.MergeTable(table);

            var foundInformation = mergedInformationTable.FindProgram(testProgramIdentifier);

            Assert.NotNull(foundInformation);
        }

        private class TestProgramInformationTable : IProgramInformationTable
        {
            private Dictionary<ProgramIdentifier, IProgramInformation> _entries = new Dictionary<ProgramIdentifier, IProgramInformation>();

            public IEnumerable<IProgramInformation> Programs
            {
                get { return _entries.Values; }
            }

            public IProgramInformation FindProgram(uint crc)
            {
                var information = _entries.FirstOrDefault(e => e.Key.DataCrc == crc).Value;
                return information;
            }

            public IProgramInformation FindProgram(ProgramIdentifier programIdentifier)
            {
                var information = _entries.FirstOrDefault(e => e.Key == programIdentifier).Value;
                return information;
            }

            internal void AddEntries(TestProgramInformation information)
            {
                foreach (var crcData in information.Crcs.Cast<CrcDataAdvanced>())
                {
                    var programIdentifier = new ProgramIdentifier(crcData.Crc, crcData.CfgCrc);
                    _entries[programIdentifier] = information;
                }
            }
        }

        private class TestProgramInformation : ProgramInformation
        {
            /// <inheritdoc />
            public override ProgramInformationOrigin DataOrigin
            {
                get { return ProgramInformationOrigin.None; }
            }

            /// <inheritdoc />
            public override string Title { get; set; }

            /// <inheritdoc />
            public override string Year { get; set; }

            /// <inheritdoc />
            public override ProgramFeatures Features { get; set; }

            /// <inheritdoc />
            public override string ShortName { get; set; }

            /// <inheritdoc />
            public override IEnumerable<CrcData> Crcs
            {
                get { return _crcs.Values; }
            }
            private Dictionary<uint, CrcDataAdvanced> _crcs = new Dictionary<uint, CrcDataAdvanced>();

            /// <inheritdoc />
            public override IEnumerable<string> LongNames
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> ShortNames
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Descriptions
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Publishers
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Programmers
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Designers
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Graphics
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Music
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> SoundEffects
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Voices
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Documentation
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Artwork
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<MetadataDateTime> ReleaseDates
            {
                get { return Enumerable.Empty<MetadataDateTime>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Licenses
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> ContactInformation
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Versions
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<MetadataDateTime> BuildDates
            {
                get { return Enumerable.Empty<MetadataDateTime>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> AdditionalInformation
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
            {
                return AddCrc(newCrc, crcDescription, incompatibilities, 0u);
            }

            public bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities, uint cfgCrc)
            {
                var added = !_crcs.ContainsKey(newCrc);
                if (added)
                {
                    _crcs[newCrc] = new CrcDataAdvanced(newCrc, crcDescription, incompatibilities, 0, cfgCrc);
                }
                return added;
            }

            public void AddCrcs(int numberOfCrcsToAdd)
            {
                var crc = 0x1000u;
                var cfgCrc = 0x2000u;
                _crcs.Clear();
                for (var i = 0; i < numberOfCrcsToAdd; ++i)
                {
                    var crcName = "Version " + i;
                    AddCrc(crc + (uint)i, crcName, IncompatibilityFlags.None, cfgCrc + (uint)i);
                }
            }
        }

        private class CrcDataAdvanced : CrcData
        {
            public CrcDataAdvanced(uint crc, string description, IncompatibilityFlags incompatibilities, int binConfigTemplate, uint cfgCrc)
                : base(crc, description, incompatibilities, binConfigTemplate)
            {
                CfgCrc = cfgCrc;
            }

            public uint CfgCrc { get; set; }
        }
    }
}
