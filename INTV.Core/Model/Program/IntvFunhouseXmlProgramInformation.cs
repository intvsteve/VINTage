// <copyright file="IntvFunhouseXmlProgramInformation.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using INTV.Core.Model.Program;

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

        private string _code;
        private string _title;
        private string _vendor;
        private string _orig_vendor;
        private string _year;
        private int _ntsc;
        private int _pal;
        private int _generalFeatures;
        private int _keyboardComponent;
        private int _sva;
        private int _intellivoice;
        private int _intyii;
        private int _ecs;
        private int _tutorvision;
        private int _icart;
        private int _cc3;
        private int _jlp;
        private string _jlp_savedata;
        private int _ltoFlash;
        private int _bee3;
        private int _hive;
        private string _crcString;
        private string _crcNotesString;
        private string _crcIncompatibilitiesString;
        private List<CrcData> _crc = new List<CrcData>();
        private ProgramFeatures _features;
        private string _externalInfo;

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
            set { _ntsc = value; }
        }

        /// <summary>
        /// Gets or sets the PAL compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("pal")]
        public int PalCompatibility
        {
            get { return _pal; }
            set { _pal = value; }
        }

        /// <summary>
        /// Gets or sets the general ROM features.
        /// </summary>
        [System.Xml.Serialization.XmlElement("general_features")]
        public int GeneralFeatures
        {
            get { return _generalFeatures; }
            set { _generalFeatures = value; }
        }

        /// <summary>
        /// Gets or sets the Model 1149 Keyboard Component feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("kc")]
        public int KeyboardComponentFeatures
        {
            get { return _keyboardComponent; }
            set { _keyboardComponent = value; }
        }

        /// <summary>
        /// Gets or sets the Super Video Arcade compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("sva")]
        public int SuperVideoArcadeCompatibility
        {
            get { return _sva; }
            set { _sva = value; }
        }

        /// <summary>
        /// Gets or sets the Intellivoice compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("ivoice")]
        public int IntellivoiceCompatibility
        {
            get { return _intellivoice; }
            set { _intellivoice = value; }
        }

        /// <summary>
        /// Gets or sets the Intellivision II compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("intyii")]
        public int IntellivisionIICompatibility
        {
            get { return _intyii; }
            set { _intyii = value; }
        }

        /// <summary>
        /// Gets or sets the ECS feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("ecs")]
        public int EcsFeatures
        {
            get { return _ecs; }
            set { _ecs = value; }
        }

        /// <summary>
        /// Gets or sets the Tutorvision compatibility bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("tutor")]
        public int TutorVision
        {
            get { return _tutorvision; }
            set { _tutorvision = value; }
        }

        /// <summary>
        /// Gets or sets the Intellicart feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("icart")]
        public int IntellicartFeatures
        {
            get { return _icart; }
            set { _icart = value; }
        }

        /// <summary>
        /// Gets or sets the Cuttle Cart 3 feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("cc3")]
        public int CuttleCart3Features
        {
            get { return _cc3; }
            set { _cc3 = value; }
        }

        /// <summary>
        /// Gets or sets the JLP feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("jlp")]
        public int JLPFeatures
        {
            get { return _jlp; }
            set { _jlp = value; }
        }

        /// <summary>
        /// Gets or sets a default name for JLP saved data files. Is this irrelevant?
        /// </summary>
        [System.Xml.Serialization.XmlElement("jlp_savegame")]
        public string JLPSaveData
        {
            get { return _jlp_savedata; }
            set { _jlp_savedata = value; }
        }

        /// <summary>
        /// Gets or sets the LTO Flash! feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("lto_flash")]
        public int LtoFlashFeatures
        {
            get { return _ltoFlash; }
            set { _ltoFlash = value; }
        }

        /// <summary>
        /// Gets or sets the Bee3 feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("bee3")]
        public int Bee3Features
        {
            get { return _bee3; }
            set { _bee3 = value; }
        }

        /// <summary>
        /// Gets or sets the Hive multicart feature bits.
        /// </summary>
        [System.Xml.Serialization.XmlElement("hive")]
        public int HiveFeatures
        {
            get { return _hive; }
            set { _hive = value; }
        }

        #endregion // Features

        /// <summary>
        /// Gets or sets the INTV Funhouse program code.
        /// </summary>
        [System.Xml.Serialization.XmlElement("code")]
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// Gets or sets the program title.
        /// </summary>
        [System.Xml.Serialization.XmlElement("title")]
        public string ProgramTitle
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the game publisher / vendor.
        /// </summary>
        [System.Xml.Serialization.XmlElement("vendor")]
        public string ProgramVendor
        {
            get { return _vendor; }
            set { _vendor = value; }
        }

        /// <summary>
        /// Gets or sets the game's original publisher / vendor.
        /// </summary>
        [System.Xml.Serialization.XmlElement("orig_vendor")]
        public string OriginalProgramVendor
        {
            get { return _orig_vendor; }
            set { _orig_vendor = value; }
        }

        /// <summary>
        /// Gets or sets the copyright year of the game.
        /// </summary>
        [System.Xml.Serialization.XmlElement("year")]
        public string YearString
        {
            get { return _year; }
            set { _year = value; }
        }

        /// <summary>
        /// Gets or sets the ROM CRC string from the database.
        /// </summary>
        [System.Xml.Serialization.XmlElement("crc")]
        public string CrcString
        {
            get { return _crcString; }
            set { _crcString = value; }
        }

        /// <summary>
        /// Gets or sets the CRC notes string from the database.
        /// </summary>
        [System.Xml.Serialization.XmlElement("crc_notes")]
        public string CrcNotesString
        {
            get { return _crcNotesString; }
            set { _crcNotesString = value; }
        }

        /// <summary>
        /// Gets or sets the CRC incompatibility information from the database.
        /// </summary>
        [System.Xml.Serialization.XmlElement("crc_incompatibilities")]
        public string CrcIncompatibilitiesString
        {
            get { return _crcIncompatibilitiesString; }
            set { _crcIncompatibilitiesString = value; }
        }

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
        /// Gets or sets the external info (partial) URL for a program.
        /// </summary>
        [System.Xml.Serialization.XmlElement("externalinfo")]
        public string ExternalInfo
        {
            get { return _externalInfo; }
            set { _externalInfo = value; }
        }

        #endregion // XML-Populated Properties

        #region IProgramInformation

        #region Properties

        /// <inheritdoc />
        public override ProgramInformationOrigin DataOrigin
        {
            get { return ProgramInformationOrigin.Embedded; }
        }

        /// <inheritdoc />
        public override string Title
        {
            get { return ProgramInformationTable.StringDecoder(ProgramTitle); }
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
                    vendor = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", OriginalProgramVendor);
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
                string yearString = _year.Trim();
                var copyrightParts = _year.Split(' ');
                if (copyrightParts.Length >= 1)
                {
                    yearString = copyrightParts[0].Trim();
                }
                int year;
                if (int.TryParse(_year, out year))
                {
                    yearString = year.ToString();
                }
                return yearString.Trim();
            }

            set
            {
                _year = value;
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
                }
                return _features;
            }

            set
            {
                _features = value;
            }
        }

        /// <inheritdoc />
        public override IEnumerable<CrcData> Crcs
        {
            get
            {
                if (_crc.Count == 0)
                {
                    var crcs = CrcString.Split(',').Where(s => s.Length >= 10).Select(crc => uint.Parse(crc.Substring(2), System.Globalization.NumberStyles.HexNumber)).ToArray();
                    var crcNotes = CrcNotesString.Split(',');
                    var crcIncompatibilities = CrcIncompatibilitiesString.Split(',');
                    var crcBinCfgs = CfgFiles.Split(',');
                    if ((crcBinCfgs.Length != 1) && (crcBinCfgs.Length != crcs.Length))
                    {
                        throw new System.InvalidOperationException();
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
                        var incompatibilities = (i < CrcIncompatibilitiesString.Length) ? (IncompatibilityFlags)uint.Parse(crcIncompatibilities[i], System.Globalization.NumberStyles.HexNumber) : IncompatibilityFlags.None;
                        _crc.Add(new CrcData(crcs[i], note, incompatibilities, crcBinCfgNumbers[i]));
                    }
                }
                return _crc;
            }
        }

        #endregion // Properties

        /// <inheritdoc />
        public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            throw new System.NotImplementedException("INTV Funhouse XML program info AddCrc not implemented.");
        }

        #endregion // IProgramInformation

        /// <inheritdoc />
        public override string ToString()
        {
            return Title;
        }
    }
}
