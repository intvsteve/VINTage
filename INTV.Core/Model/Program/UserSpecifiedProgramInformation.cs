// <copyright file="UserSpecifiedProgramInformation.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using INTV.Core.ComponentModel;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This class provides an implementation of IProgramInformation that allows users to create
    /// custom-defined ProgramInformation. This can happen as new ROM variants of old program are
    /// found, new program are created, or default settings from other sources are overridden.
    /// </summary>
    public class UserSpecifiedProgramInformation : ProgramInformation, INotifyPropertyChanged
    {
        private bool _initialized;

        #region Constructors

        /// <summary>
        /// Creates a new instance of the UserSpecifiedProgramInformation class.
        /// </summary>
        /// <param name="crc">The CRC of the program's ROM file.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public UserSpecifiedProgramInformation(uint crc)
            : this(crc, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of the UserSpecifiedProgramInformation class.
        /// </summary>
        /// <param name="crc">The CRC of the program's ROM file.</param>
        /// <param name="title">The title of the program.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public UserSpecifiedProgramInformation(uint crc, string title)
            : this(crc, title, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of the UserSpecifiedProgramInformation class.
        /// </summary>
        /// <param name="crc">The CRC of the program's ROM file.</param>
        /// <param name="title">The title of the program.</param>
        /// <param name="year">Copyright date of the program.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public UserSpecifiedProgramInformation(uint crc, string title, string year)
            : this(crc, title, year, ProgramFeatures.GetUnrecognizedRomFeatures())
        {
        }

        /// <summary>
        /// Creates a new instance of the UserSpecifiedProgramInformation class.
        /// </summary>
        /// <param name="crc">The CRC of the program's ROM file.</param>
        /// <param name="title">The title of the program.</param>
        /// <param name="year">Copyright date of the program.</param>
        /// <param name="features">The features of the program.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public UserSpecifiedProgramInformation(uint crc, string title, string year, ProgramFeatures features)
            : this(crc, title, year, features, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of the UserSpecifiedProgramInformation class.
        /// </summary>
        /// <param name="crc">The CRC of the program's ROM file.</param>
        /// <param name="title">The title of the program.</param>
        /// <param name="year">Copyright date of the program.</param>
        /// <param name="features">The features of the program.</param>
        /// <param name="crcDescription">Description of the variant of the program identified by its CRC.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public UserSpecifiedProgramInformation(uint crc, string title, string year, ProgramFeatures features, string crcDescription)
            : this(crc, title, year, features, crcDescription, IncompatibilityFlags.None)
        {
        }

        /// <summary>
        /// Creates a new instance of the UserSpecifiedProgramInformation class.
        /// </summary>
        /// <param name="crc">The CRC of the program's ROM file.</param>
        /// <param name="title">The title of the program.</param>
        /// <param name="year">Copyright date of the program.</param>
        /// <param name="features">The features of the program.</param>
        /// <param name="crcDescription">Description of the variant of the program identified by its CRC.</param>
        /// <param name="incompatibilities">Describes known hardware incompatibilities associated with the ROM.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public UserSpecifiedProgramInformation(uint crc, string title, string year, ProgramFeatures features, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            if (crc == 0)
            {
                throw new ArgumentException(Resources.Strings.UserSpecifiedProgramInformation_CreatedWithInvalidCRCMessage);
            }
            _title = string.IsNullOrEmpty(title) ? UnknownProgramTitle : title;
            _year = year;
            _features = features;
            _crcs[crc] = new KeyValuePair<string, IncompatibilityFlags>(crcDescription, incompatibilities);
            _origin = ProgramInformationOrigin.UserDefined;
            FinishInitialization();
        }

        /// <summary>
        /// Creates a new instance of the UserSpecifiedProgramInformation class from another IProgramInformation object.
        /// </summary>
        /// <param name="programInformation">The source program information.</param>
        /// <exception cref="ArgumentException">Thrown if programInformation is null.</exception>
        public UserSpecifiedProgramInformation(IProgramInformation programInformation)
        {
            if (programInformation == null)
            {
                throw new ArgumentException(Resources.Strings.UserSpecifiedProgramInformation_CreatedWithInvalidSource);
            }
            SourceInformation = programInformation;
            var userSpecified = programInformation as UserSpecifiedProgramInformation;
            _title = programInformation.Title;
            _vendor = programInformation.Vendor;
            _year = programInformation.Year;
            _features = programInformation.Features;
            _origin = programInformation.DataOrigin;
            ShortName = programInformation.ShortName;
            if (userSpecified != null)
            {
                _crcs = new Dictionary<uint, KeyValuePair<string, IncompatibilityFlags>>(userSpecified._crcs);
            }
            else
            {
                _crcs = programInformation.Crcs.ToDictionary(c => c.Crc, d => new KeyValuePair<string, IncompatibilityFlags>(d.Description, d.Incompatibilities));
            }
            FinishInitialization();
        }

        private UserSpecifiedProgramInformation()
        {
        }

        #endregion // Constructors

        #region INotifyPropertyChanged Events

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged Events

        #region Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override ProgramInformationOrigin DataOrigin
        {
            get { return _origin; }
        }
        private ProgramInformationOrigin _origin;

        /// <inheritdoc />
        public override string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (!string.IsNullOrEmpty(value) && (string.IsNullOrEmpty(_title) || ((_title == UnknownProgramTitle) && (value != UnknownProgramTitle))))
                {
                    this.AssignAndUpdateProperty(PropertyChanged, "Title", value, ref _title, MarkDirty);
                }
            }
        }
        private string _title;

        /// <inheritdoc />
        public override string Vendor
        {
            get { return _vendor; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Vendor", value, ref _vendor, MarkDirty); }
        }
        private string _vendor;

        /// <inheritdoc />
        public override string Year
        {
            get { return _year; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Year", value, ref _year, MarkDirty); }
        }
        private string _year;

        /// <inheritdoc />
        public override ProgramFeatures Features
        {
            get { return _features; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Features", value, ref _features, MarkDirty); }
        }
        private ProgramFeatures _features;

        /// <inheritdoc />
        public override IEnumerable<CrcData> Crcs
        {
            get { return _crcs.Select(crc => new CrcData(crc.Key, crc.Value)); }
        }
        private Dictionary<uint, KeyValuePair<string, IncompatibilityFlags>> _crcs = new Dictionary<uint, KeyValuePair<string, IncompatibilityFlags>>();

        #endregion // IProgramInformation

        #region IProgramMetadata

        /// <inheritdoc />
        public override IEnumerable<string> LongNames
        {
            get { return _longNames; }
        }
        private HashSet<string> _longNames;

        /// <inheritdoc />
        public override IEnumerable<string> ShortNames
        {
            get { return _shortNames; }
        }
        private HashSet<string> _shortNames;

        /// <inheritdoc />
        public override IEnumerable<string> Descriptions
        {
            get { return _descriptions; }
        }
        private HashSet<string> _descriptions;

        /// <inheritdoc />
        public override IEnumerable<string> Publishers
        {
            get { return _publishers; }
        }
        private HashSet<string> _publishers;

        /// <inheritdoc />
        public override IEnumerable<string> Programmers
        {
            get { return _programmers; }
        }
        private HashSet<string> _programmers;

        /// <inheritdoc />
        public override IEnumerable<string> Designers
        {
            get { return _designers; }
        }
        private HashSet<string> _designers;

        /// <inheritdoc />
        public override IEnumerable<string> Graphics
        {
            get { return _graphics; }
        }
        private HashSet<string> _graphics;

        /// <inheritdoc />
        public override IEnumerable<string> Music
        {
            get { return _music; }
        }
        private HashSet<string> _music;

        /// <inheritdoc />
        public override IEnumerable<string> SoundEffects
        {
            get { return _soundEffects; }
        }
        private HashSet<string> _soundEffects;

        /// <inheritdoc />
        public override IEnumerable<string> Voices
        {
            get { return _voices; }
        }
        private HashSet<string> _voices;

        /// <inheritdoc />
        public override IEnumerable<string> Documentation
        {
            get { return _documentation; }
        }
        private HashSet<string> _documentation;

        /// <inheritdoc />
        public override IEnumerable<string> Artwork
        {
            get { return _artwork; }
        }
        private HashSet<string> _artwork;

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { return _releaseDates; }
        }
        private SortedSet<MetadataDateTime> _releaseDates;

        /// <inheritdoc />
        public override IEnumerable<string> Licenses
        {
            get { return _licenses; }
        }
        private HashSet<string> _licenses;

        /// <inheritdoc />
        public override IEnumerable<string> ContactInformation
        {
            get { return _contactInformation; }
        }
        private HashSet<string> _contactInformation;

        /// <inheritdoc />
        public override IEnumerable<string> Versions
        {
            get { return _versions; }
        }
        private HashSet<string> _versions;

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> BuildDates
        {
            get { return _buildDates; }
        }
        private SortedSet<MetadataDateTime> _buildDates;

        /// <inheritdoc />
        public override IEnumerable<string> AdditionalInformation
        {
            get { return GetAdditionalInformationString(); }
        }

        private Dictionary<string, string> _additionalInformation;

        #endregion // IProgramMetadata

        /// <summary>
        /// Gets a value indicating whether the information has been modified or not.
        /// </summary>
        public bool IsModified
        {
            get { return _isDirty; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, "IsModified", value, ref _isDirty); }
        }
        private bool _isDirty;

        private IProgramInformation SourceInformation { get; set; }

        #endregion // Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            if (newCrc == 0)
            {
                throw new ArgumentOutOfRangeException(Resources.Strings.UserSpecifiedProgramInformation_AddInvalidCRCMessage);
            }
            KeyValuePair<string, IncompatibilityFlags> crcInfo;
            bool replacedEntry = _crcs.TryGetValue(newCrc, out crcInfo);
            _crcs[newCrc] = new KeyValuePair<string, IncompatibilityFlags>(crcDescription, incompatibilities);
            return replacedEntry;
        }

        #endregion // IProgramInformation

        /// <summary>
        /// Adds a long name to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="longName">The long name to add.</param>
        /// <returns><c>true</c> if the long name was added, <c>false</c> if the value was already in the long names list.</returns>
        public bool AddLongName(string longName)
        {
            var added = _longNames.Add(longName);
            return added;
        }

        /// <summary>
        /// Adds a short name to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="shortName">The short name to add.</param>
        /// <returns><c>true</c> if the short name was added, <c>false</c> if the value was already in the short names list.</returns>
        public bool AddShortName(string shortName)
        {
            var added = _shortNames.Add(shortName);
            return added;
        }

        /// <summary>
        /// Adds a description to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="description">The description to add.</param>
        /// <returns><c>true</c> if the description was added, <c>false</c> if the value was already in the description list.</returns>
        public bool AddDescription(string description)
        {
            var added = _descriptions.Add(description);
            return added;
        }

        /// <summary>
        /// Adds a publisher credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="publisher">The publisher credit to add.</param>
        /// <returns><c>true</c> if the publisher credit was added, <c>false</c> if the value was already in the publisher credits list.</returns>
        public bool AddPublisher(string publisher)
        {
            var added = _publishers.Add(publisher);
            return added;
        }

        /// <summary>
        /// Adds a programmer credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="programmer">The programmer credit to add.</param>
        /// <returns><c>true</c> if the programmer credit was added, <c>false</c> if the value was already in the programmer credits list.</returns>
        public bool AddProgrammer(string programmer)
        {
            var added = _programmers.Add(programmer);
            return added;
        }

        /// <summary>
        /// Adds a designer credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="designer">The designer credit to add.</param>
        /// <returns><c>true</c> if the designer credit was added, <c>false</c> if the value was already in the designer credits list.</returns>
        public bool AddDesigner(string designer)
        {
            var added = _designers.Add(designer);
            return added;
        }

        /// <summary>
        /// Adds a graphics credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="graphics">The graphics credit to add.</param>
        /// <returns><c>true</c> if the graphics credit was added, <c>false</c> if the value was already in the graphics credits list.</returns>
        public bool AddGraphics(string graphics)
        {
            var added = _graphics.Add(graphics);
            return added;
        }

        /// <summary>
        /// Adds a music credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="music">The music credit to add.</param>
        /// <returns><c>true</c> if the music credit was added, <c>false</c> if the value was already in the music credits list.</returns>
        public bool AddMusic(string music)
        {
            var added = _music.Add(music);
            return added;
        }

        /// <summary>
        /// Adds a sound effects credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="soundEffects">The sound effects credit to add.</param>
        /// <returns><c>true</c> if the sound effects credit was added, <c>false</c> if the value was already in the sound effects credits list.</returns>
        public bool AddSoundEffects(string soundEffects)
        {
            var added = _soundEffects.Add(soundEffects);
            return added;
        }

        /// <summary>
        /// Adds a voice credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="voice">The voice credit to add.</param>
        /// <returns><c>true</c> if the voice credit was added, <c>false</c> if the value was already in the voice credits list.</returns>
        public bool AddVoice(string voice)
        {
            var added = _voices.Add(voice);
            return added;
        }

        /// <summary>
        /// Adds a documentation credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="documentation">The documentation credit to add.</param>
        /// <returns><c>true</c> if the documentation credit was added, <c>false</c> if the value was already in the documentation credits list.</returns>
        public bool AddDocumentation(string documentation)
        {
            var added = _documentation.Add(documentation);
            return added;
        }

        /// <summary>
        /// Adds an artwork credit to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="artwork">The artwork credit to add.</param>
        /// <returns><c>true</c> if the artwork credit was added, <c>false</c> if the value was already in the artwork credits list.</returns>
        public bool AddArtwork(string artwork)
        {
            var added = _artwork.Add(artwork);
            return added;
        }

        /// <summary>
        /// Adds a release date to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="releaseDate">The release date to add.</param>
        /// <returns><c>true</c> if the release date was added, <c>false</c> if the value was already in the release dates list.</returns>
        public bool AddReleaseDate(MetadataDateTime releaseDate)
        {
            var added = _releaseDates.Add(releaseDate);
            return added;
        }

        /// <summary>
        /// Adds a license to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="license">The license to add.</param>
        /// <returns><c>true</c> if the license was added, <c>false</c> if the value was already in the licenses list.</returns>
        public bool AddLicense(string license)
        {
            var added = _licenses.Add(license);
            return added;
        }

        /// <summary>
        /// Adds contact information to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="contactInformation">The contact information to add.</param>
        /// <returns><c>true</c> if the contact information was added, <c>false</c> if the value was already in the contact information list.</returns>
        public bool AddContactInformation(string contactInformation)
        {
            var added = _contactInformation.Add(contactInformation);
            return added;
        }

        /// <summary>
        /// Adds a version to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="version">The version to add.</param>
        /// <returns><c>true</c> if the version was added, <c>false</c> if the value was already in the versions list.</returns>
        public bool AddVersion(string version)
        {
            var added = _versions.Add(version);
            return added;
        }

        /// <summary>
        /// Adds a build date to the metadata. Duplicate values are rejected.
        /// </summary>
        /// <param name="buildDate">The build date to add.</param>
        /// <returns><c>true</c> if the build date was added, <c>false</c> if the value was already in the build dates list.</returns>
        public bool AddBuildDate(MetadataDateTime buildDate)
        {
            var added = _buildDates.Add(buildDate);
            return added;
        }

        /// <summary>
        /// Provide more supplemental information about the program to the metadata.
        /// </summary>
        /// <param name="key">An identifier for the additional information.</param>
        /// <param name="value">The additional information.</param>
        /// <remarks>If data is already present for the identifier described by <paramref name="key"/>, then <paramref name="value"/> is appended to the existing data.</remarks>
        public void AddAdditionalInformation(string key, string value)
        {
            string existingInfo;
            if (_additionalInformation.TryGetValue(key, out existingInfo))
            {
                existingInfo += ", " + value;
            }
            else
            {
                existingInfo = value;
            }
            _additionalInformation[key] = existingInfo;
        }

        private void FinishInitialization()
        {
            var metadata = SourceInformation as IProgramMetadata;
            if (metadata != null)
            {
                _longNames = new HashSet<string>(metadata.LongNames, StringComparer.OrdinalIgnoreCase);
                _shortNames = new HashSet<string>(metadata.ShortNames, StringComparer.OrdinalIgnoreCase);
                _descriptions = new HashSet<string>(metadata.Descriptions, StringComparer.OrdinalIgnoreCase);
                _publishers = new HashSet<string>(metadata.Publishers, StringComparer.OrdinalIgnoreCase);
                _programmers = new HashSet<string>(metadata.Programmers, StringComparer.OrdinalIgnoreCase);
                _designers = new HashSet<string>(metadata.Designers, StringComparer.OrdinalIgnoreCase);
                _graphics = new HashSet<string>(metadata.Graphics, StringComparer.OrdinalIgnoreCase);
                _music = new HashSet<string>(metadata.Music, StringComparer.OrdinalIgnoreCase);
                _soundEffects = new HashSet<string>(metadata.SoundEffects, StringComparer.OrdinalIgnoreCase);
                _voices = new HashSet<string>(metadata.Voices, StringComparer.OrdinalIgnoreCase);
                _documentation = new HashSet<string>(metadata.Documentation, StringComparer.OrdinalIgnoreCase);
                _artwork = new HashSet<string>(metadata.Artwork, StringComparer.OrdinalIgnoreCase);
                _releaseDates = new SortedSet<MetadataDateTime>(metadata.ReleaseDates);
                _licenses = new HashSet<string>(metadata.Licenses, StringComparer.OrdinalIgnoreCase);
                _contactInformation = new HashSet<string>(metadata.ContactInformation, StringComparer.OrdinalIgnoreCase);
                _versions = new HashSet<string>(metadata.Versions, StringComparer.OrdinalIgnoreCase);
                _buildDates = new SortedSet<MetadataDateTime>(metadata.BuildDates);
                _additionalInformation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var additionalInformationIndex = 0;
                foreach (var additionalInformation in metadata.AdditionalInformation)
                {
                    _additionalInformation[additionalInformationIndex.ToString(CultureInfo.InvariantCulture)] = additionalInformation;
                    ++additionalInformationIndex;
                }
                if (string.IsNullOrEmpty(ShortName))
                {
                    if (!string.IsNullOrEmpty(_shortNames.FirstOrDefault()))
                    {
                        ShortName = _shortNames.First();
                    }
                }
            }
            else
            {
                _longNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _shortNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _descriptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _publishers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _programmers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _designers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _graphics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _music = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _soundEffects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _voices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _documentation = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _artwork = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _releaseDates = new SortedSet<MetadataDateTime>();
                _licenses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _contactInformation = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _versions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _buildDates = new SortedSet<MetadataDateTime>();
                _additionalInformation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                if (!string.IsNullOrEmpty(_title))
                {
                    _longNames.Add(_title);
                }

                if (!string.IsNullOrEmpty(ShortName))
                {
                    _shortNames.Add(ShortName);
                }

                if (!string.IsNullOrEmpty(_vendor))
                {
                    AddPublisher(_vendor);
                }

                if (!string.IsNullOrEmpty(_year))
                {
                    int releaseYear;
                    if (int.TryParse(_year, out releaseYear))
                    {
                        var dateTime = new DateTime(releaseYear, MetadataDateTime.DefaultMonth, MetadataDateTime.DefaultDay);
                        var releaseDate = new MetadataDateTime(dateTime, MetadataDateTimeFlags.Year);
                        AddReleaseDate(releaseDate);
                    }
                }
            }

            _initialized = true;
        }

        private void MarkDirty<T>(string modifiedPropertyName, T newValue)
        {
            IsModified = _initialized;
        }

        private IEnumerable<string> GetAdditionalInformationString()
        {
            var additionalInfos = _additionalInformation.Select(i => string.Format(CultureInfo.CurrentCulture, "{0}: {1}", i.Key, i.Value));
            return additionalInfos;
        }
    }
}
