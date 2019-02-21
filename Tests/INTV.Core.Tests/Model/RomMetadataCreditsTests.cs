// <copyright file="RomMetadataCreditsTests.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomMetadataCreditsTests
    {
        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Uses the behavior of 'LeaveOpen' in 'BinaryReader' -- works correctly.")]
        public void RomMetadataCredits_ContainsEvilBytes_ProducesExpectedUnicodeString()
        {
            using (var metadataCreditsStream = new System.IO.MemoryStream())
            {
                var bytes = new byte[255];
                metadataCreditsStream.WriteByte(1); // Programmer credit
                for (var i = 1; i <= 255; ++i)
                {
                    metadataCreditsStream.WriteByte((byte)i);
                    bytes[i - 1] = (byte)i;
                }
                metadataCreditsStream.Seek(0, System.IO.SeekOrigin.Begin);
                using (var reader = new INTV.Core.Utility.BinaryReader(metadataCreditsStream))
                {
                    var streamLength = metadataCreditsStream.Length;
                    var credits = new RomMetadataCredits((uint)streamLength);
                    var bytesDecoded = credits.Deserialize(reader);

                    // The first byte is 0x01 -- which is stuffed in credits parsing, so "stuff" it here.
                    var expectedString = System.Text.Encoding.UTF8.GetString(bytes, 1, 254);
                    Assert.Equal(streamLength, bytesDecoded);
                    Assert.Equal(expectedString, credits.Programming.First());
                }
            }
        }
    }
}
