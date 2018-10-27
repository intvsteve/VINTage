// <copyright file="RomMetadataPublisherTests.cs" company="INTV Funhouse">
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

using INTV.Core.Model;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomMetadataPublisherTests
    {
        [Theory]
        [InlineData((byte)PublisherId.MattelElectronics, "Mattel Electronics")]
        [InlineData((byte)PublisherId.INTVCorp, "INTV Corporation")]
        [InlineData((byte)PublisherId.Imagic, "Imagic")]
        [InlineData((byte)PublisherId.Activision, "Activision")]
        [InlineData((byte)PublisherId.Atarisoft, "Atarisoft")]
        [InlineData((byte)PublisherId.Coleco, "Coleco")]
        [InlineData((byte)PublisherId.CBS, "CBS")]
        [InlineData((byte)PublisherId.ParkerBros, "Parker Brothers")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in BinaryWriter' works correctly.")]
        public void RomMetadataPublisher_DeserializePayloadUsingPublisherId_ProducesCorrectPublisherName(byte publisherId, string expectedPublisherName)
        {
            var publisherMetadata = new RomMetadataPublisher(1);

            using (var stream = new System.IO.MemoryStream())
            {
                stream.WriteByte(publisherId);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                using (var reader = new BinaryReader(stream))
                {
                    publisherMetadata.Deserialize(reader);
                }
            }

            Assert.Equal(expectedPublisherName, publisherMetadata.Publisher);
        }

        /// <summary>
        /// Note that this is a duplicate of the private enum defined in <see cref="RomMetadataPublisher"/>.
        /// </summary>
        private enum PublisherId : byte
        {
            /// <summary>
            /// Published by Mattel Electronics.
            /// </summary>
            MattelElectronics = 0x00,

            /// <summary>
            /// published by INTV Corporation.
            /// </summary>
            INTVCorp = 0x01,

            /// <summary>
            /// Published by Imagic.
            /// </summary>
            Imagic = 0x02,

            /// <summary>
            /// Published by Activision.
            /// </summary>
            Activision = 0x03,

            /// <summary>
            /// Published by Atarisoft.
            /// </summary>
            Atarisoft = 0x04,

            /// <summary>
            /// Published by Coleco.
            /// </summary>
            Coleco = 0x05,

            /// <summary>
            /// Published by CBS Electronics.
            /// </summary>
            CBS = 0x06,

            /// <summary>
            /// Published by Parker Brothers.
            /// </summary>
            ParkerBros = 0x07,
        }
    }
}
