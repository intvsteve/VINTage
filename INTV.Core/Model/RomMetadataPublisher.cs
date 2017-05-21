// <copyright file="RomMetadataPublisher.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Class for publisher metadata in a .ROM-format ROM.
    /// </summary>
    public class RomMetadataPublisher : RomMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.RomMetadataPublisher"/> class.
        /// </summary>
        /// <param name="length">Length of the string in the metadata block.</param>
        public RomMetadataPublisher(uint length)
            : base(length, RomMetadataIdTag.Publisher)
        {
        }

        #region Properties

        /// <summary>
        /// Gets the publisher.
        /// </summary>
        public string Publisher { get; private set; }

        #endregion // Properties

        #region RomMetadataBlock

        /// <inheritdoc/>
        protected override uint DeserializePayload(INTV.Core.Utility.BinaryReader reader)
        {
            var publisherId = (PublisherId)reader.ReadByte();
            Publisher = PublisherIdToString(publisherId, (int)Length, reader);
            return Length;
        }

        #endregion // RomMetadataBlock

        private static string PublisherIdToString(PublisherId publisherId, int payloadLength, INTV.Core.Utility.BinaryReader reader)
        {
            var publisher = string.Empty;
            var shouldReadMore = false;
            switch (publisherId)
            {
                case PublisherId.MattelElectronics:
                    publisher = "Mattel Electronics";
                    break;
                case PublisherId.INTVCorp:
                    publisher = "INTV Corporation";
                    break;
                case PublisherId.Imagic:
                    publisher = "Imagic";
                    break;
                case PublisherId.Activision:
                    publisher = "Activision";
                    break;
                case PublisherId.Atarisoft:
                    publisher = "Atarisoft";
                    break;
                case PublisherId.Coleco:
                    publisher = "Coleco";
                    break;
                case PublisherId.CBS:
                    publisher = "CBS";
                    break;
                case PublisherId.ParkerBros:
                    publisher = "Parker Brothers";
                    break;

                case PublisherId.Other:
                default:
                    shouldReadMore = true;
                    break;
            }
            var remainingPayload = payloadLength - sizeof(PublisherId);
            if (remainingPayload > 0)
            {
                // PCLs only support UTF8... Spec says ASCII. Let's hope we don't run into anything *too* weird.
                var publisherData = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(remainingPayload), 0, remainingPayload).Trim('\0');
                if (shouldReadMore)
                {
                    publisher = publisherData;
                }
            }
            return publisher;
        }

        /// <summary>
        /// Pre-defined identifiers for game publisher.
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

#if false
            ZbiciakElectronics = 0x08,
            IntelligentVision = 0x09,
            Elektronite = 0x10,
            CollectorVision = 0x11,
            GoodDealGames = 0x12,
            IntellivisionRevolution = 0x13,
            BBWW = 0x14,
            Atari2600Land = 0x15,
#endif // false

            /// <summary>
            /// Other game publisher, name included in metadata.
            /// </summary>
            Other = 0xFF
        }
    }
}
