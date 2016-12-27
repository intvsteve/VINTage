// <copyright file="UserSpecifiedProgramInformation.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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
        private bool _isDirty;
        private string _title;
        private string _vendor;
        private string _year;
        private ProgramFeatures _features;
        private Dictionary<uint, KeyValuePair<string, IncompatibilityFlags>> _crcs = new Dictionary<uint, KeyValuePair<string, IncompatibilityFlags>>();
        private bool _initialized;
        private ProgramInformationOrigin _origin;

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
            _initialized = true;
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
            if (userSpecified != null)
            {
                _crcs = new Dictionary<uint, KeyValuePair<string, IncompatibilityFlags>>(userSpecified._crcs);
            }
            else
            {
                _crcs = programInformation.Crcs.ToDictionary(c => c.Crc, d => new KeyValuePair<string, IncompatibilityFlags>(d.Description, d.Incompatibilities));
            }
            _initialized = true;
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

        #region IProgramInformation Properties

        /// <inheritdoc />
        public override ProgramInformationOrigin DataOrigin
        {
            get { return _origin; }
        }

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

        /// <inheritdoc />
        public override string Vendor
        {
            get { return _vendor; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Vendor", value, ref _vendor, MarkDirty); }
        }

        /// <inheritdoc />
        public override string Year
        {
            get { return _year; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Year", value, ref _year, MarkDirty); }
        }

        /// <inheritdoc />
        public override ProgramFeatures Features
        {
            get { return _features; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Features", value, ref _features, MarkDirty); }
        }

        /// <inheritdoc />
        public override IEnumerable<CrcData> Crcs
        {
            get { return _crcs.Select(crc => new CrcData(crc.Key, crc.Value)); }
        }

        #endregion // IProgramInformation Properties

        /// <summary>
        /// Gets a value indicating whether the information has been modified or not.
        /// </summary>
        public bool IsModified
        {
            get { return _isDirty; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, "IsModified", value, ref _isDirty); }
        }

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

        private void MarkDirty<T>(string modifiedPropertyName, T newValue)
        {
            IsModified = _initialized;
        }
    }
}
