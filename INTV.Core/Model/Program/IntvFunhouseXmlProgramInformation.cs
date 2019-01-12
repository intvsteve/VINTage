// <copyright file="IntvFunhouseXmlProgramInformation.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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
using System.Globalization;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Resources;
using INTV.Core.Utility;

namespace INTV.Core.Restricted.Model.Program
{
    /// <summary>
    /// This class reflects the layout of the gameinfo table from intvfunhouse.com.
    /// </summary>
    /// <remarks>This class must be public in order for Xml.Serialization to work. Do not use it directly.</remarks>
    /// <remarks>Note that if the layout of the database table at intvfunhouse.com changes, this class must be
    /// updated to reflect the changes. Also, the data types in this table are suboptimal, using strings in many
    /// fields that should be other, more specific types.</remarks>
    public class IntvFunhouseXmlProgramInformation : ProgramInformation
    {
        /// <summary>
        /// Maximum length for a program name, based on INTV Funhouse database.
        /// </summary>
        public const int MaxProgramNameLength = 64;

        /// <summary>
        /// Maximum length for a vendor string, based on INTV Funhouse database.
        /// </summary>
        public const int MaxVendorNameLength = 32;

        /// <summary>
        /// Basic vendor information.
        /// </summary>
        /// <remarks>NOTE: This information is manually copied from the INTV Funhouse vendors database. In some cases, it may have diverged.</remarks>
        private static readonly Dictionary<string, VendorUrls> VendorInfo = new Dictionary<string, VendorUrls>(StringComparer.OrdinalIgnoreCase)
            {
                { "Activision", new VendorUrls("http://intellivisionlives.com/bluesky/games/credits/activision.shtml", "http://intellivisionlives.com/bluesky/games/") },
                { "Atarisoft", new VendorUrls("http://intellivisionlives.com/bluesky/games/credits/atarisoft.shtml", "http://intellivisionlives.com/bluesky/games/") },
                { "Coleco", new VendorUrls("http://intellivisionlives.com/bluesky/games/credits/colecoint.shtml", "http://intellivisionlives.com/bluesky/games/") },
                { "Dextell Ltd.", new VendorUrls(null, "http://intellivisionlives.com/bluesky/games/") },
                { "Imagic", new VendorUrls("http://intellivisionlives.com/bluesky/games/credits/imagic.shtml", "http://intellivisionlives.com/bluesky/games/") },
                { "Intellivision Inc.", new VendorUrls(null, "http://intellivisionlives.com/bluesky/games/") },
                { "CollectorVision", new VendorUrls("http://collectorvision.com/", "https://collectorvision.com/shop/intellivision") },
                { "Interphase", new VendorUrls("http://intellivisionlives.com/bluesky/games/credits/interphase.shtml", "http://intellivisionlives.com/bluesky/games/") },
                { "INTV Corporation", new VendorUrls("http://intellivisionlives.com/bluesky/games/credits/intv.shtml", "http://intellivisionlives.com/bluesky/games/") },
                { "Mattel Electronics", new VendorUrls("http://intellivisionlives.com/bluesky/games/", "http://intellivisionlives.com/bluesky/games/") },
                { "Parker Brothers", new VendorUrls("http://intellivisionlives.com/bluesky/games/credits/parkerbros.shtml", "http://intellivisionlives.com/bluesky/games/") },
                { "Sears", new VendorUrls(null, "http://intellivisionlives.com/bluesky/games/") },
                { "Sega", new VendorUrls("http://intellivisionlives.com/bluesky/games/credits/sega.shtml", "http://intellivisionlives.com/bluesky/games/") },
                { "Elektronite", new VendorUrls("http://elektronite.net", "http://elektronite.net/gallery") },
                { "Left Turn Only", new VendorUrls("http://www.leftturnonly.info/") },
                { "Intelligentvision", new VendorUrls("http://intellivision.us", "http://intellivision.us/intvgames") },
                { "INTV Funhouse", new VendorUrls("http://www.intvfunhouse.com/", "http://www.intvfunhouse.com/intvfunhouse.com/games/") },
                { "Intellivision Revolution", new VendorUrls("http://www.intellivisionrevolution.com/") },
                { "Blah Blah Woof Woof", new VendorUrls("http://www.blahblahwoofwoof.com/") },
                { "Homebrew, Inc.", new VendorUrls("http://fwgames.ca/", "http://fwgames.ca/") },
                { "2600 Connection", new VendorUrls("http://www.2600connection.com/") },
                { "Team Pixelboy", new VendorUrls("http://teampixelboy.com/", "http://teampixelboy.com/") },
            };

        #region Properties

        #region XML-Populated Properties

        // The raw features are public in so the XML serialization API can set them.
        #region Features

        /// <summary>
        /// Gets or sets the NTSC compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("ntsc")]
        public int NtscCompatibility
        {
            get { return _ntsc; }
            set { _ntsc = (int)((FeatureCompatibility)value).CoerceVideoStandardCompatibility(); }
        }
        private int _ntsc;

        /// <summary>
        /// Gets or sets the PAL compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("pal")]
        public int PalCompatibility
        {
            get { return _pal; }
            set { _pal = (int)((FeatureCompatibility)value).CoerceVideoStandardCompatibility(); }
        }
        private int _pal;

        /// <summary>
        /// Gets or sets the general ROM features.
        /// </summary>
        [System.Xml.Serialization.XmlElement("general_features")]
        public int GeneralFeatures { get; set; }

        /// <summary>
        /// Gets or sets the Model 1149 Keyboard Component feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("kc")]
        public int KeyboardComponentFeatures { get; set; }

        /// <summary>
        /// Gets or sets the Super Video Arcade compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("sva")]
        public int SuperVideoArcadeCompatibility { get; set; }

        /// <summary>
        /// Gets or sets the Intellivoice compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("ivoice")]
        public int IntellivoiceCompatibility { get; set; }

        /// <summary>
        /// Gets or sets the Intellivision II compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("intyii")]
        public int IntellivisionIICompatibility { get; set; }

        /// <summary>
        /// Gets or sets the ECS feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("ecs")]
        public int EcsFeatures { get; set; }

        /// <summary>
        /// Gets or sets the Tutorvision compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("tutor")]
        public int TutorVision { get; set; }

        /// <summary>
        /// Gets or sets the Intellicart feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("icart")]
        public int IntellicartFeatures { get; set; }

        /// <summary>
        /// Gets or sets the Cuttle Cart 3 feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("cc3")]
        public int CuttleCart3Features { get; set; }

        /// <summary>
        /// Gets or sets the JLP feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("jlp")]
        public int JLPFeatures { get; set; }

        /// <summary>
        /// Gets or sets a default name for JLP saved data sectors.
        /// </summary>
        /// <remarks>For legacy reasons, this is a string. It should be parsed as an integer value.</remarks>
        [System.Xml.Serialization.XmlElement("jlp_savegame")]
        public string JLPSaveData { get; set; }

        /// <summary>
        /// Gets or sets the LTO Flash! feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("lto_flash")]
        public int LtoFlashFeatures { get; set; }

        /// <summary>
        /// Gets or sets the Bee3 feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("bee3")]
        public int Bee3Features { get; set; }

        /// <summary>
        /// Gets or sets the Hive multicart feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("hive")]
        public int HiveFeatures { get; set; }

        #endregion // Features

        /// <summary>
        /// Gets or sets the INTV Funhouse program code.
        /// </summary>
        [System.Xml.Serialization.XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the program title.
        /// </summary>
        [System.Xml.Serialization.XmlElement("title")]
        public string ProgramTitle
        {
            get { return _title; }
            set { _title = value.DecodeHtmlString(); }
        }
        private string _title;

        /// <summary>
        /// Gets or sets the game publisher / vendor.
        /// </summary>
        [System.Xml.Serialization.XmlElement("vendor")]
        public string ProgramVendor { get; set; }

        /// <summary>
        /// Gets or sets the game's original publisher / vendor.
        /// </summary>
        [System.Xml.Serialization.XmlElement("orig_vendor")]
        public string OriginalProgramVendor { get; set; }

        /// <summary>
        /// Gets or sets the copyright year of the game.
        /// </summary>
        [System.Xml.Serialization.XmlElement("year")]
        public string YearString { get; set; }

        /// <summary>
        /// Gets or sets the ROM CRC string from the database.
        /// </summary>
        [System.Xml.Serialization.XmlElement("crc")]
        public string CrcString { get; set; }

        /// <summary>
        /// Gets or sets the CRC notes string from the database.
        /// </summary>
        [System.Xml.Serialization.XmlElement("crc_notes")]
        public string CrcNotesString { get; set; }

        /// <summary>
        /// Gets or sets the CRC incompatibility information from the database.
        /// </summary>
        [System.Xml.Serialization.XmlElement("crc_incompatibilities")]
        public string CrcIncompatibilitiesString { get; set; }

        /// <summary>
        /// Gets or sets the configuration files to use per ROM CRC.
        /// </summary>
        [System.Xml.Serialization.XmlElement("bin_cfg")]
        public string CfgFiles
        {
            get { return _binCfgs; }
            set { _binCfgs = value; }
        }
        private string _binCfgs;

        /// <summary>
        /// Gets or sets the external information (partial) URL for a program.
        /// </summary>
        [System.Xml.Serialization.XmlElement("externalinfo")]
        public string ExternalInfo { get; set; }

        /// <summary>
        /// Gets or sets the release date of the program. Expected to be in standard format YYYY-MM-DD or whatever else is supported by the standard parser.
        /// </summary>
        /// <remarks>NOTE: The value 0000-00-00 will throw!</remarks>
        [System.Xml.Serialization.XmlElement("release_date")]
        public System.DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the description of the program. This is often from a game catalog.
        /// </summary>
        [System.Xml.Serialization.XmlElement("description")]
        public string ProgramDescription { get; set; }

        /// <summary>
        /// Gets or sets raw information regarding the developers (programmers) of the program. String is vertical-bar-delimited (|).
        /// </summary>
        [System.Xml.Serialization.XmlElement("program")]
        public string ProgramDevelopers { get; set; }

        /// <summary>
        /// Gets or sets raw information regarding the concept (design) of the program. String is vertical-bar-delimited (|).
        /// </summary>
        [System.Xml.Serialization.XmlElement("concept")]
        public string ProgramConcept { get; set; }

        /// <summary>
        /// Gets or sets raw information regarding graphics in the program. String is vertical-bar-delimited (|).
        /// </summary>
        [System.Xml.Serialization.XmlElement("game_graphics")]
        public string ProgramGraphics { get; set; }

        /// <summary>
        /// Gets or sets raw information regarding sound effects used in the program. String is vertical-bar-delimited (|).
        /// </summary>
        [System.Xml.Serialization.XmlElement("soundfx")]
        public string ProgramSoundEffects { get; set; }

        /// <summary>
        /// Gets or sets raw information regarding music used in the program. String is vertical-bar-delimited (|).
        /// </summary>
        [System.Xml.Serialization.XmlElement("music")]
        public string ProgramMusic { get; set; }

        /// <summary>
        /// Gets or sets raw information regarding voice talent used in the program. String is vertical-bar-delimited (|).
        /// </summary>
        [System.Xml.Serialization.XmlElement("voices")]
        public string ProgramVoices { get; set; }

        /// <summary>
        /// Gets or sets raw information regarding program documentation. String is vertical-bar-delimited (|).
        /// </summary>
        [System.Xml.Serialization.XmlElement("game_docs")]
        public string ProgramDocumentation { get; set; }

        /// <summary>
        /// Gets or sets information regarding program packaging artwork. String is vertical-bar-delimited (|).
        /// </summary>
        [System.Xml.Serialization.XmlElement("box_art")]
        public string BoxArt { get; set; }

        /// <summary>
        /// Gets or sets the additional information for a program. String is new-line-delimited.
        /// </summary>
        [System.Xml.Serialization.XmlElement("other")]
        public string OtherInfo { get; set; }

        #endregion // XML-Populated Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override ProgramInformationOrigin DataOrigin
        {
            get { return ProgramInformationOrigin.Embedded; }
        }

        /// <inheritdoc />
        public override string Title
        {
            get { return ProgramTitle.DecodeHtmlString(); }
            set { }
        }

        /// <inheritdoc />
        public override string Vendor
        {
            get
            {
                var vendor = ProgramVendor;
                if (!string.IsNullOrEmpty(OriginalProgramVendor))
                {
                    vendor = string.Format(CultureInfo.InvariantCulture, "{0}", OriginalProgramVendor);
                }
                return vendor;
            }

            set
            {
            }
        }

        /// <inheritdoc />
        public override string Year
        {
            get
            {
                string yearString = YearString.Trim();
                var copyrightParts = YearString.Split(' ');
                if (copyrightParts.Length >= 1)
                {
                    yearString = copyrightParts[0].Trim();
                }
                int year;
                if (int.TryParse(YearString, out year))
                {
                    yearString = year.ToString();
                }
                return yearString.Trim();
            }

            set
            {
                YearString = value;
            }
        }

        /// <inheritdoc />
        public override ProgramFeatures Features
        {
            get
            {
                if (_features == null)
                {
                    _features = new ProgramFeatures();
                    _features.Ntsc = (FeatureCompatibility)NtscCompatibility & FeatureCompatibilityHelpers.ValidFeaturesMask;
                    _features.Pal = (FeatureCompatibility)PalCompatibility & FeatureCompatibilityHelpers.ValidFeaturesMask;
                    _features.GeneralFeatures = (GeneralFeatures)GeneralFeatures & Core.Model.Program.GeneralFeaturesHelpers.ValidFeaturesMask;
                    _features.KeyboardComponent = (KeyboardComponentFeatures)KeyboardComponentFeatures & Core.Model.Program.KeyboardComponentFeaturesHelpers.ValidFeaturesMask;
                    _features.SuperVideoArcade = (FeatureCompatibility)SuperVideoArcadeCompatibility & FeatureCompatibilityHelpers.ValidFeaturesMask;
                    _features.Intellivoice = (FeatureCompatibility)IntellivoiceCompatibility & FeatureCompatibilityHelpers.ValidFeaturesMask;
                    _features.IntellivisionII = (FeatureCompatibility)IntellivisionIICompatibility & FeatureCompatibilityHelpers.ValidFeaturesMask;
                    _features.Ecs = (EcsFeatures)EcsFeatures & Core.Model.Program.EcsFeaturesHelpers.ValidFeaturesMask;
                    _features.Tutorvision = (FeatureCompatibility)TutorVision & FeatureCompatibilityHelpers.ValidFeaturesMask;
                    _features.Intellicart = (IntellicartCC3Features)IntellicartFeatures & IntellicartCC3FeaturesHelpers.ValidFeaturesMask;
                    _features.CuttleCart3 = (CuttleCart3Features)CuttleCart3Features & Core.Model.Program.CuttleCart3FeaturesHelpers.ValidFeaturesMask;
                    _features.Jlp = (JlpFeatures)JLPFeatures & JlpFeaturesHelpers.ValidFeaturesMask;
                    _features.LtoFlash = (LtoFlashFeatures)LtoFlashFeatures & Core.Model.Program.LtoFlashFeaturesHelpers.ValidFeaturesMask;
                    _features.Bee3 = (Bee3Features)Bee3Features & Core.Model.Program.Bee3FeaturesHelpers.ValidFeaturesMask;
                    _features.Hive = (HiveFeatures)HiveFeatures & Core.Model.Program.HiveFeaturesHelpers.ValidFeaturesMask;

                    if (!string.IsNullOrEmpty(JLPSaveData))
                    {
                        int jlpRawFlashSectors;
                        if (int.TryParse(JLPSaveData, NumberStyles.Integer, CultureInfo.InvariantCulture, out jlpRawFlashSectors))
                        {
                            try
                            {
                                var jlpFlashSectors = Convert.ToUInt16(jlpRawFlashSectors);
                                if (jlpFlashSectors > 0)
                                {
                                    if (jlpRawFlashSectors <= JlpFeaturesHelpers.MaxTheoreticalJlpFlashSectorUsage)
                                    {
                                        _features.JlpFlashMinimumSaveSectors = jlpFlashSectors;
                                    }
                                }
                            }
                            catch (OverflowException)
                            {
                            }
                        }
                    }
                }
                return _features;
            }

            set
            {
                _features = value;
            }
        }
        private ProgramFeatures _features;

        /// <inheritdoc />
        public override string ShortName
        {
            get { return null; }
            set { }
        }

        /// <inheritdoc />
        public override IEnumerable<CrcData> Crcs
        {
            get
            {
                if (_crc.Count == 0)
                {
                    var crcs = CrcString.Split(',').Where(s => s.Length >= 10).Select(crc => uint.Parse(crc.Substring(2), NumberStyles.HexNumber)).ToArray();
                    var crcNotes = CrcNotesString.Split(',');
                    var crcIncompatibilities = CrcIncompatibilitiesString.Split(',');
                    var crcBinCfgs = CfgFiles.Split(',');
                    if ((crcBinCfgs.Length != 1) && (crcBinCfgs.Length != crcs.Length))
                    {
                        throw new InvalidOperationException();
                    }
                    var firstValue = int.Parse(crcBinCfgs.First());
                    var crcBinCfgNumbers = Enumerable.Repeat(firstValue, crcs.Length).ToArray();
                    if (crcBinCfgs.Length > 1)
                    {
                        for (int i = 0; i < crcBinCfgs.Length; ++i)
                        {
                            crcBinCfgNumbers[i] = int.Parse(crcBinCfgs[i]);
                        }
                    }
                    for (int i = 0; i < crcs.Length; ++i)
                    {
                        var note = (i < crcNotes.Length) ? crcNotes[i] : string.Empty;
                        var incompatibilities = IncompatibilityFlags.None;
                        if (i < crcIncompatibilities.Length)
                        {
                            int incompatibilitiesBits;
                            if (int.TryParse(crcIncompatibilities[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out incompatibilitiesBits))
                            {
                                incompatibilities = (IncompatibilityFlags)incompatibilitiesBits;
                            }
                        }
                        _crc.Add(new CrcData(crcs[i], note, incompatibilities, crcBinCfgNumbers[i]));
                    }
                }
                return _crc;
            }
        }
        private List<CrcData> _crc = new List<CrcData>();

        #endregion // IProgramInformation

        #region IProgramMetadata

        /// <inheritdoc />
        public override IEnumerable<string> LongNames
        {
            get { yield return Title; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> ShortNames
        {
            get { yield return Title.EnforceNameLength(RomInfoIndexHelpers.MaxShortNameLength, restrictToGromCharacters: false); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Descriptions
        {
            get
            {
                if (!string.IsNullOrEmpty(ProgramDescription))
                {
                    yield return ProgramDescription;
                }
            }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Publishers
        {
            get
            {
                yield return ProgramVendor;
                if (!string.IsNullOrEmpty(OriginalProgramVendor))
                {
                    yield return OriginalProgramVendor;
                }
            }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Programmers
        {
            get { return SplitMultipleEntryString(ProgramDevelopers, removeEmptyEntries: true, separator: "|"); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Designers
        {
            get { return SplitMultipleEntryString(ProgramConcept, removeEmptyEntries: true, separator: "|"); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Graphics
        {
            get { return SplitMultipleEntryString(ProgramGraphics, removeEmptyEntries: true, separator: "|"); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Music
        {
            get { return SplitMultipleEntryString(ProgramMusic, removeEmptyEntries: true, separator: "|"); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> SoundEffects
        {
            get { return SplitMultipleEntryString(ProgramSoundEffects, removeEmptyEntries: true, separator: "|"); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Voices
        {
            get { return SplitMultipleEntryString(ProgramVoices, removeEmptyEntries: true, separator: "|"); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Documentation
        {
            get { return SplitMultipleEntryString(ProgramDocumentation, removeEmptyEntries: true, separator: "|"); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Artwork
        {
            get { return SplitMultipleEntryString(BoxArt, removeEmptyEntries: true, separator: "|"); }
        }

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { yield return new MetadataDateTime(new DateTimeOffset(ReleaseDate), MetadataDateTimeFlags.Year | MetadataDateTimeFlags.Month | MetadataDateTimeFlags.Day); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Licenses
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> ContactInformation
        {
            get { return GetContactInformation(ProgramVendor, OriginalProgramVendor, ExternalInfo); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Versions
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> BuildDates
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> AdditionalInformation
        {
            get { return GetAdditionalInformation(OtherInfo, ProgramVendor, OriginalProgramVendor); }
        }

        #endregion // IProgramMetadata

        #endregion // Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            throw new InvalidOperationException();
        }

        #endregion // IProgramInformation

        /// <inheritdoc />
        public override string ToString()
        {
            return Title;
        }

        private static IEnumerable<string> GetContactInformation(string programVendor, string originalProgramVendor, string externalInfo)
        {
            var contactInformation = new List<string>();
            var externalInfoUrls = SplitMultipleEntryString(externalInfo, removeEmptyEntries: false);
            var useOriginalProgramVendorBaseUrl = true;
            foreach (var externalInfoUrl in externalInfoUrls)
            {
                if (!string.IsNullOrEmpty(externalInfoUrl))
                {
                    // EXTREMELY simplistic checking here. We're not going the full Uri inspection route.
                    if (externalInfoUrl.StartsWith("http"))
                    {
                        contactInformation.Add(externalInfoUrl);
                    }
                    else
                    {
                        var vendorBaseUrl = GetContactInfoBaseUrl(programVendor, originalProgramVendor, useOriginalProgramVendorBaseUrl);
                        if (!string.IsNullOrEmpty(vendorBaseUrl))
                        {
                            var contactInfoUrl = vendorBaseUrl;
                            var needsSlash = !vendorBaseUrl.EndsWith("/") && !externalInfoUrl.StartsWith("/");
                            if (needsSlash)
                            {
                                contactInfoUrl += "/";
                            }
                            contactInfoUrl += externalInfoUrl;
                            contactInformation.Add(contactInfoUrl);
                        }
                    }
                }
                useOriginalProgramVendorBaseUrl = false;
            }
            return contactInformation;
        }

        private static string GetContactInfoBaseUrl(string programVendor, string originalProgramVendor, bool useOriginalProgramVendor)
        {
            var vendor = useOriginalProgramVendor && !string.IsNullOrEmpty(originalProgramVendor) ? originalProgramVendor : programVendor;
            string contactUrlBase = null;
            if (!string.IsNullOrEmpty(vendor))
            {
                VendorUrls urls;
                if (VendorInfo.TryGetValue(vendor, out urls))
                {
                    contactUrlBase = urls.GameInfoBaseUrl;
                }
            }
            return contactUrlBase;
        }

        private static IEnumerable<string> GetAdditionalInformation(string otherInfo, string programVendor, string originalProgramVendor)
        {
            var additionalInformation = new List<string>(SplitMultipleEntryString(otherInfo, removeEmptyEntries: true));
            var vendors = new[] { originalProgramVendor, programVendor };
            foreach (var vendor in vendors)
            {
                if (!string.IsNullOrEmpty(vendor))
                {
                    VendorUrls urls;
                    if (VendorInfo.TryGetValue(vendor, out urls))
                    {
                        var vendorWebsite = string.Format(CultureInfo.CurrentCulture, Strings.AdditionalVendorInfoWebsite_Format, vendor, urls.WebsiteUrl);
                        additionalInformation.Add(vendorWebsite);
                    }
                }
            }
            return additionalInformation;
        }

        /// <summary>
        /// Splits the given string into multiple entries.
        /// </summary>
        /// <param name="rawData">The raw string value to split.</param>
        /// <param name="removeEmptyEntries">If <c>true</c>, exclude empty strings from the returned values.</param>
        /// <param name="separator">Specifies separators to use when splitting the string. If none are specified, newline (and combinations of it and carriage return) are used.</param>
        /// <returns>An enumerable of the values in <paramref name="rawData"/>, which are also HTML decoded, with HTML tags stripped.</returns>
        private static IEnumerable<string> SplitMultipleEntryString(string rawData, bool removeEmptyEntries, params string[] separator)
        {
            var values = Enumerable.Empty<string>();
            if (!string.IsNullOrEmpty(rawData))
            {
                if (!separator.Any())
                {
                    separator  = new[] { "\n", "\r", "\r\n", "\n\r" };
                }
                values = rawData.Split(separator, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None).Select(s => s.DecodeHtmlString().Trim());
            }
            return values;
        }

        /// <summary>
        /// Helper class for vendor URL information.
        /// </summary>
        private class VendorUrls : Tuple<string, string>
        {
            /// <summary>
            /// Initializes an instance of <see cref="VendorUrls"/>.
            /// </summary>
            /// <param name="websiteUrl">URL to a vendor's main web page.</param>
            internal VendorUrls(string websiteUrl)
                : this(websiteUrl, null)
            {
            }

            /// <summary>
            /// Initializes an instance of <see cref="VendorUrls"/>.
            /// </summary>
            /// <param name="websiteUrl">URL to a vendor's main web page.</param>
            /// <param name="gameInfoBaseUrl">URL to a vendor's root game information page.</param>
            internal VendorUrls(string websiteUrl, string gameInfoBaseUrl)
                : base(websiteUrl, gameInfoBaseUrl)
            {
            }

            /// <summary>
            /// Gets the URL for a vendor's main website page.
            /// </summary>
            internal string WebsiteUrl
            {
                get { return Item1; }
            }

            /// <summary>
            /// Gets the base URL to use for game information.
            /// </summary>
            internal string GameInfoBaseUrl
            {
                get { return Item2; }
            }
        }
    }
}
