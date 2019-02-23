// <copyright file="RomMetadataCredits.cs" company="INTV Funhouse">
// Copyright (c) 2016-2019 All Rights Reserved
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
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Class for extracting credits metadata from a .ROM-format file.
    /// </summary>
    public class RomMetadataCredits : RomMetadataBlock
    {
        private const int NumberOfCreditFlags = 8;

        /// <summary>
        /// A dictionary of predefined authors, as researched by Joe Zbiciak. This is transcribed from
        /// the 'authors' file included in the jzIntv source's doc/rom_fmt/.
        /// </summary>
        private static readonly Dictionary<byte, string> Authors = new Dictionary<byte, string>()
        {
            { 0x80, @"Alan Smith" },
            { 0x81, @"Andy Sells" },
            { 0x82, @"Barcay & Burt" },
            { 0x83, @"Bill Fisher" },
            { 0x84, @"Bill Goodrich" },
            { 0x85, @"Bob Newstadt" },
            { 0x86, @"Bob Whitehead" },
            { 0x87, @"Bob del Principe" },
            { 0x88, @"Brett Stutz" },
            { 0x89, @"Brian Dougherty" },
            { 0x8A, @"Carol Shaw" },
            { 0x8B, @"Chris Hawley" },
            { 0x8C, @"Chris Kingsley" },
            { 0x8D, @"Chris Markle" },
            { 0x8E, @"Connie Goldman" },
            { 0x8F, @"Cory Burton" },
            { 0x90, @"Crian Cummings" },
            { 0x91, @"Daisy Nguyen" },
            { 0x92, @"Dale Lynn" },
            { 0x93, @"Dan Bass" },
            { 0x94, @"Danny Brauer" },
            { 0x95, @"Dave Durran" },
            { 0x96, @"Dave James" },
            { 0x97, @"David Crane" },
            { 0x98, @"David Rolfe" },
            { 0x99, @"David Stifel" },
            { 0x9A, @"David Warhol" },
            { 0x9B, @"Dennis Clark" },
            { 0x9C, @"Diane Pershing" },
            { 0x9D, @"Don Daglow" },
            { 0x9E, @"Douglas A. Fults" },
            { 0x9F, @"Eddie Dombrower" },
            { 0xA0, @"Elain Xenos-Braswell" },
            { 0xA1, @"Eric Wels" },
            { 0xA2, @"Frank Evans" },
            { 0xA3, @"Fred Jones" },
            { 0xA4, @"Gary Johnson" },
            { 0xA5, @"Gary Kato" },
            { 0xA6, @"Gavin Claypool" },
            { 0xA7, @"Gene Smith" },
            { 0xA8, "George \"The Fat Man\" Sanger" },
            { 0xA9, @"Glyn Anderson" },
            { 0xAA, @"Gracie Lantz" },
            { 0xAB, @"Grahame Matthews" },
            { 0xAC, @"Greg Favor" },
            { 0xAD, @"Hal Finney" },
            { 0xAE, @"Jerry Moore" },
            { 0xAF, @"Ji-Wen Tsao" },
            { 0xB0, @"Joannie Gerber" },
            { 0xB1, @"Joe Jacobs" },
            { 0xB2, @"Joe [Ferreira] King" },
            { 0xB3, @"John Brooks" },
            { 0xB4, @"John Sohl" },
            { 0xB5, @"John Tomlinson" },
            { 0xB6, @"Joshua Jeffe" },
            { 0xB7, @"Judy Mason" },
            { 0xB8, @"Julie Hoshizaki" },
            { 0xB9, @"Kai Tran" },
            { 0xBA, @"Karen (Tanoyuye) McConathy" },
            { 0xBB, @"Karen Elliot" },
            { 0xBC, @"Karen Nugent" },
            { 0xBD, @"Karl Morris" },
            { 0xBE, @"Keith Robinson" },
            { 0xBF, @"Ken Smith" },
            { 0xC0, @"Keri Tombazian" },
            { 0xC1, @"Kevin Miller" },
            { 0xC2, @"Kimo Yap" },
            { 0xC3, @"Larry Zwick" },
            { 0xC4, @"Lori Sunahara" },
            { 0xC5, @"Mark Buchignani" },
            { 0xC6, @"Mark Buczek" },
            { 0xC7, @"Mark Kennedy" },
            { 0xC8, @"Mark Urbaniec" },
            { 0xC9, @"Marvin Mednick" },
            { 0xCA, @"Mayf Nutter" },
            { 0xCB, @"Michael Becker" },
            { 0xCC, @"Michelle Mock" },
            { 0xCD, @"Mike Breen" },
            { 0xCE, @"Mike Minkoff" },
            { 0xCF, @"Mike Winans" },
            { 0xD0, @"Minh Chou Tran" },
            { 0xD1, @"Mona Theiss" },
            { 0xD2, @"Monique Lujan-Bakerink" },
            { 0xD3, @"Pamela Dong" },
            { 0xD4, @"Pat Lewis" },
            { 0xD5, @"Pat Ransil" },
            { 0xD6, @"Patrick Schmitz" },
            { 0xD7, @"Patti Dworken" },
            { 0xD8, @"Patti Glick" },
            { 0xD9, @"Peggi Decarli" },
            { 0xDA, @"Peter Allen" },
            { 0xDB, @"Peter Bergman" },
            { 0xDC, @"Peter Farson" },
            { 0xDD, @"Peter Kaminski" },
            { 0xDE, @"Phil Austin" },
            { 0xDF, @"Phil Proctor" },
            { 0xE0, @"Ray Kaestner" },
            { 0xE1, @"Rich O'Keefe" },
            { 0xE2, @"Rick Koenig" },
            { 0xE3, @"Rick Levine" },
            { 0xE4, @"Rick Sinatra" },
            { 0xE5, @"Ron Surratt" },
            { 0xE6, @"Russ Haft" },
            { 0xE7, @"Russ Lieblich" },
            { 0xE8, @"Russ Ludwick" },
            { 0xE9, @"Sam Zalan" },
            { 0xEA, @"Scot Wegner" },
            { 0xEB, @"Scott Bishop" },
            { 0xEC, @"Scott Reynolds" },
            { 0xED, @"Scott Robitelle" },
            { 0xEE, @"Shal Farley" },
            { 0xEF, @"Shatao Lin" },
            { 0xF0, @"Simonot" },
            { 0xF1, @"Stephen Willey" },
            { 0xF2, @"Steve DeFrisco" },
            { 0xF3, @"Steve Ettinger" },
            { 0xF4, @"Steve Huston" },
            { 0xF5, @"Steve Montero" },
            { 0xF6, @"Steve Roney" },
            { 0xF7, @"Steve Sents" },
            { 0xF8, @"Tom Loughry" },
            { 0xF9, @"Tom Priestley" },
            { 0xFA, @"Tom Soulanille" },
            { 0xFB, @"Tony Pope" },
            { 0xFC, @"Vladimir Hrycenko" },
            { 0xFD, @"Wendell Brown" },
            { 0xFE, @"Wilfredo Aguilar" },
            ////{ 0xFF, @"Joe Zbiciak" }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.RomMetadataCredits"/> class.
        /// </summary>
        /// <param name="length">Length of the data in the metadata block.</param>
        public RomMetadataCredits(uint length)
            : base(length, RomMetadataIdTag.Credits)
        {
            _programming = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _graphics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _music = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _soundEffects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _voiceActing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _documentation = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _gameConceptDesign = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _boxOrOtherArtwork = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        #region Properties

        /// <summary>
        /// Gets the names of those credited with programming.
        /// </summary>
        public IEnumerable<string> Programming
        {
            get { return _programming; }
        }

        private HashSet<string> _programming;

        /// <summary>
        /// Gets the names of those credited for creating for art / graphics.
        /// </summary>
        public IEnumerable<string> Graphics
        {
            get { return _graphics; }
        }

        private HashSet<string> _graphics;

        /// <summary>
        /// Gets the names of those who created / programmed the music.
        /// </summary>
        public IEnumerable<string> Music
        {
            get { return _music; }
        }

        private HashSet<string> _music;

        /// <summary>
        /// Gets the names of those who developed sound effects.
        /// </summary>
        public IEnumerable<string> SoundEffects
        {
            get { return _soundEffects; }
        }

        private HashSet<string> _soundEffects;

        /// <summary>
        /// Gets the names of those who provided voice acting.
        /// </summary>
        public IEnumerable<string> VoiceActing
        {
            get { return _voiceActing; }
        }

        private HashSet<string> _voiceActing;

        /// <summary>
        /// Gets the names of those responsible for documentation.
        /// </summary>
        public IEnumerable<string> Documentation
        {
            get { return _documentation; }
        }

        private HashSet<string> _documentation;

        /// <summary>
        /// Gets the names of those who created the game concept or design.
        /// </summary>
        public IEnumerable<string> GameConceptDesign
        {
            get { return _gameConceptDesign; }
        }

        private HashSet<string> _gameConceptDesign;

        /// <summary>
        /// Gets the names of those responsible for box or other artwork.
        /// </summary>
        public IEnumerable<string> BoxOrOtherArtwork
        {
            get { return _boxOrOtherArtwork; }
        }

        private HashSet<string> _boxOrOtherArtwork;

        #endregion // Properties

        #region RomMetadataBlock

        /// <inheritdoc/>
        protected override uint DeserializePayload(INTV.Core.Utility.BinaryReader reader)
        {
            var bytesParsed = 0u;
            while (bytesParsed < Length)
            {
                bytesParsed += ParseCreditRecord(reader, bytesParsed);
            }
            return Length;
        }

        #endregion // RomMetadataBlock

        private uint ParseCreditRecord(INTV.Core.Utility.BinaryReader reader, uint runningTotal)
        {
            // The format of the Credits block consists of:
            // 1 byte bit-field describing which credits apply to the subsequent name
            // EITHER a 1-byte shorthand for the name (0x80-0xFF), or, a NULL-terminated UTF-8 string for the name.
            var bytesParsed = 0u;
            var creditFlags = (CreditFlags)reader.ReadByte();
            ++bytesParsed;
            ++runningTotal;

            var name = string.Empty;
            var character = reader.ReadByte();
            ++bytesParsed;
            ++runningTotal;

            if (!Authors.TryGetValue(character, out name))
            {
                // Discard the "stuffing" byte that indicates  UTF-8 or 0x01 should follow.
                if (character == 0x01)
                {
                    character = reader.ReadByte();
                    ++bytesParsed;
                    ++runningTotal;
                }

                var nameBuffer = new List<byte>();
                nameBuffer.Add(character);

                while ((character != 0) && (runningTotal < Length))
                {
                    character = reader.ReadByte();
                    nameBuffer.Add(character);
                    ++bytesParsed;
                    ++runningTotal;
                }

                name = System.Text.Encoding.UTF8.GetString(nameBuffer.ToArray(), 0, nameBuffer.Count).Trim('\0');
            }

            for (int i = 0; i < NumberOfCreditFlags; ++i)
            {
                var credit = (CreditFlags)(1 << i);
                if (creditFlags.HasFlag(credit))
                {
                    AddCredit(credit, name);
                }
            }
            return bytesParsed;
        }

        private void AddCredit(CreditFlags credit, string name)
        {
            switch (credit)
            {
                case CreditFlags.Programming:
                    _programming.Add(name);
                    break;
                case CreditFlags.Graphics:
                    _graphics.Add(name);
                    break;
                case CreditFlags.Music:
                    _music.Add(name);
                    break;
                case CreditFlags.SoundEffects:
                    _soundEffects.Add(name);
                    break;
                case CreditFlags.VoiceActing:
                    _voiceActing.Add(name);
                    break;
                case CreditFlags.Documentation:
                    _documentation.Add(name);
                    break;
                case CreditFlags.ConceptDesign:
                    _gameConceptDesign.Add(name);
                    break;
                case CreditFlags.BoxOrOtherArtwork:
                    _boxOrOtherArtwork.Add(name);
                    break;
            }
        }

        /// <summary>Flags to indicate specific contributions to a ROM.</summary>
        [Flags]
        private enum CreditFlags : byte
        {
            /// <summary>No credits.</summary>
            None = 0x00,

            /// <summary>Indicates programming contributions.</summary>
            Programming = 1 << 0,

            /// <summary>Indicates contributions to program art / graphics.</summary>
            Graphics = 1 << 1,

            /// <summary>Indicates contributions to music.</summary>
            Music = 1 << 2,

            /// <summary>Indicates contributions to sound effects.</summary>
            SoundEffects = 1 << 3,

            /// <summary>Indicates contributions to voice acting.</summary>
            VoiceActing = 1 << 4,

            /// <summary>Indicates documentation authorship.</summary>
            Documentation = 1 << 5,

            /// <summary>The program concept or design.</summary>
            ConceptDesign = 1 << 6,

            /// <summary>Indicates contributions to box, overlay, manual or other artwork.</summary>
            BoxOrOtherArtwork = 1 << 7
        }
    }
}
