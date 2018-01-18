// <copyright file="CfgFileMetadataProgramInformation.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This class implements IProgramInformation based on data parsed from a .cfg file.
    /// </summary>
    public class CfgFileMetadataProgramInformation : ProgramInformation
    {
        private string _title;
        private string _vendor;
        private string _year;
        private CrcData _crc;
        private ProgramFeatures _features;

        #region Constructors

        public CfgFileMetadataProgramInformation(IRom rom)
        {
            _features = ProgramFeatures.DefaultFeatures.Clone();
            Metadata = Enumerable.Empty<CfgVarMetadataBlock>();
            var binFormatRom = Rom.AsSpecificRomType<BinFormatRom>(rom);
            if (binFormatRom != null)
            {
                Metadata = binFormatRom.Metadata;
                var stringMetaData = Metadata.FirstOrDefault(m => m.Type == CfgVarMetadataIdTag.Name) as CfgVarMetadataString;
                if ((stringMetaData != null) && !string.IsNullOrEmpty(stringMetaData.StringValue))
                {
                    _title = stringMetaData.StringValue;
                }
                stringMetaData = Metadata.FirstOrDefault(m => m.Type == CfgVarMetadataIdTag.ShortName) as CfgVarMetadataString;
                if ((stringMetaData != null) && !string.IsNullOrEmpty(stringMetaData.StringValue))
                {
                    ShortName = stringMetaData.StringValue;
                }
                var date = Metadata.OfType<CfgVarMetadataDate>().FirstOrDefault(d => d.Type == CfgVarMetadataIdTag.ReleaseDate);
                if ((date != null) && date.Date.Flags.HasFlag(MetadataDateTimeFlags.Year))
                {
                    _year = date.Date.Date.Year.ToString();
                }
                var vendor = Metadata.FirstOrDefault(m => m.Type == CfgVarMetadataIdTag.Publisher) as CfgVarMetadataString;
                if (vendor != null)
                {
                    _vendor = vendor.StringValue;
                }

                // NOTE: If these are specified multiple times, the 'last one wins' rule will be in effect.
                // That's technically OK, since the behavior is unspecified. From as1600.txt:
                // 
                // Lines marked with a "*" can be repeated.  For example, if a game has multiple
                // authors, you can list them all with their own author variable.  Likewise, if
                // a program was released multiple times, you can give a list of release dates.
                // 
                // For other values, repeating a variable does not have a well defined meaning.
                // Typically (but not always), the first instance takes precedence.
                // 
                // So in the case of specifying features multiple times, this code will result in
                // 'last one wins'.
                foreach (var feature in Metadata.OfType<CfgVarMetadataFeatureCompatibility>())
                {
                    switch (feature.Type)
                    {
                        case CfgVarMetadataIdTag.Ecs:
                        case CfgVarMetadataIdTag.EcsCompatibility:
                            _features.Ecs = (EcsFeatures)feature.Compatibility;
                            break;
                        case CfgVarMetadataIdTag.Voice:
                        case CfgVarMetadataIdTag.IntellivoiceCompatibility:
                            _features.Intellivoice = feature.Compatibility;
                            break;
                        case CfgVarMetadataIdTag.IntellivisionII:
                        case CfgVarMetadataIdTag.IntellivisionIICompatibility:
                            _features.IntellivisionII = feature.Compatibility;
                            break;
                        case CfgVarMetadataIdTag.KeyboardComponentCompatibility:
                            _features.KeyboardComponent = (KeyboardComponentFeatures)feature.Compatibility;
                            break;
                        case CfgVarMetadataIdTag.TutorvisionCompatibility:
                            _features.Tutorvision = feature.Compatibility;
                            break;
                        case CfgVarMetadataIdTag.JlpAccelerators:
                        case CfgVarMetadataIdTag.Jlp:
                            _features.Jlp = (JlpFeatures)feature.Compatibility;
                            if (_features.Jlp != JlpFeatures.Incompatible)
                            {
                                _features.JlpHardwareVersion = JlpHardwareVersion.Jlp03; // Assume minimal hardware version needed
                                if (_features.Jlp > JlpFeatures.Tolerates)
                                {
                                    // Flash storage is indicated, so check for it.
                                    var flashStorage = Metadata.FirstOrDefault(m => m.Type == CfgVarMetadataIdTag.JlpFlash) as CfgVarMetadataInteger;
                                    if ((flashStorage != null) && (flashStorage.IntegerValue > 0))
                                    {
                                        // TODO: Min value checking here?
                                        _features.JlpFlashMinimumSaveSectors = (ushort)flashStorage.IntegerValue;
                                        _features.Jlp |= JlpFeatures.SaveDataRequired;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                var ltoMapper = Metadata.FirstOrDefault(m => m.Type == CfgVarMetadataIdTag.LtoFlashMapper) as CfgVarMetadataBoolean;
                if (ltoMapper != null)
                {
                    _features.LtoFlash = LtoFlashFeatures.Requires | LtoFlashFeatures.LtoFlashMemoryMapped;
                }
                _crc = new CrcData(binFormatRom.Crc, string.Empty, _features.ToIncompatibilityFlags());
            }
        }

        #endregion // Constructors

        #region Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override ProgramInformationOrigin DataOrigin
        {
            get { return ProgramInformationOrigin.CfgVarMetadataBlock; }
        }

        /// <inheritdoc />
        public override string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <inheritdoc />
        public override string Vendor
        {
            get { return _vendor; }
            set { _vendor = value; }
        }

        /// <inheritdoc />
        public override string Year
        {
            get { return _year; }
            set { _year = value; }
        }

        /// <inheritdoc />
        public override ProgramFeatures Features
        {
            get { return _features; }
            set { _features = value; }
        }

        /// <inheritdoc />
        public override IEnumerable<CrcData> Crcs
        {
            get { yield return _crc; }
        }

        #endregion IProgramInformation

        /// <summary>
        /// Gets the authors.
        /// </summary>
        public IEnumerable<string> Authors
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.Author).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the graphics artists.
        /// </summary>
        public IEnumerable<string> Graphics
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.GameArt).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the music credits.
        /// </summary>
        public IEnumerable<string> Music
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.Music).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the sound effects credits.
        /// </summary>
        public IEnumerable<string> SoundEffects
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.SoundEffects).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the voice acting credits.
        /// </summary>
        public IEnumerable<string> Voices
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.VoiceActing).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the documentation credits.
        /// </summary>
        public IEnumerable<string> Documentation
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.Documentation).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the artwork credits for boxes, et. al.
        /// </summary>
        public IEnumerable<string> Artwork
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.BoxOrOtherArtwork).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the program concept credits.
        /// </summary>
        public IEnumerable<string> Concept
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.ConceptDesign).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the 'more info' values.
        /// </summary>
        public IEnumerable<string> MoreInfo
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.MoreInfo).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the publishers.
        /// </summary>
        public IEnumerable<string> Publishers
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.Publisher).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the release licenses.
        /// </summary>
        public IEnumerable<string> Licenses
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.License).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        public IEnumerable<string> Descriptions
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.Description).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets the release dates.
        /// </summary>
        public IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { return Metadata.OfType<CfgVarMetadataDate>().Where(m => m.Type == CfgVarMetadataIdTag.ReleaseDate).Select(m => m.Date); }
        }

        /// <summary>
        /// Gets the build dates.
        /// </summary>
        public IEnumerable<MetadataDateTime> BuildDates
        {
            get { return Metadata.OfType<CfgVarMetadataDate>().Where(m => m.Type == CfgVarMetadataIdTag.BuildDate).Select(m => m.Date); }
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        public IEnumerable<string> Versions
        {
            get { return Metadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.Version).Select(m => m.StringValue); }
        }

        /// <summary>
        /// Gets all the metadata in its originally parsed form.
        /// </summary>
        internal IEnumerable<CfgVarMetadataBlock> Metadata { get; private set; }

        #endregion Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            throw new System.NotImplementedException();
        }

        #endregion // IProgramInformation
    }
}
