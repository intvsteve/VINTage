// <copyright file="OSVersion.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Abstraction of operating system version.
    /// </summary>
    public partial class OSVersion
    {
        /// <summary>
        /// The current operating system version.
        /// </summary>
        public static readonly OSVersion Current = new OSVersion();

        private OSVersion()
        {
            _version = Initialize();
        }

        /// <summary>
        /// Initializes a new version of the OSVersion type.
        /// </summary>
        /// <param name="major">Major version of the operating system.</param>
        /// <param name="minor">Minor version of the operating system.</param>
        /// <param name="patch">Patch / build version of the operating system.</param>
        public OSVersion(int major, int minor, int patch)
        {
            _version = new System.Version(major, minor, patch);
        }

        /// <summary>
        /// Gets the OSVersion as a <see cref="Version"/>.
        /// </summary>
        public Version Version
        {
            get { return _version; }
        }
        private Version _version;

        /// <summary>
        /// Gets the major version.
        /// </summary>
        public int Major
        {
            get { return Version.Major; }
        }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        public int Minor
        {
            get { return Version.Minor; }
        }

        /// <summary>
        /// Gets the patch / build version.
        /// </summary>
        public int Patch
        {
            get { return Version.Build; }
        }

        #region Operators

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The left hand side.</param>
        /// <param name="rhs">The right hand side.</param>
        /// <returns><c>true</c> if all three elements in the version are equal.</returns>
        /// <remarks>Comparison is done using underlying <see cref="Version"/> structure.</remarks>
        public static bool operator ==(OSVersion lhs, OSVersion rhs)
        {
            Version lv = object.ReferenceEquals(lhs, null) ? null : lhs.Version;
            Version rv = object.ReferenceEquals(rhs, null) ? null : rhs.Version;
            return lv == rv;
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="lhs">The left hand side.</param>
        /// <param name="rhs">The right hand side.</param>
        /// <returns><c>true</c> if all three elements in the version are not equal.</returns>
        /// <remarks>Comparison is done using underlying <see cref="Version"/> structure.</remarks>
        public static bool operator !=(OSVersion lhs, OSVersion rhs)
        {
            Version lv = (lhs == null) ? null : lhs.Version;
            Version rv = (rhs == null) ? null : rhs.Version;
            return lv != rv;
        }

        /// <summary>
        /// Determines if the version <paramref name="lhs"/> is newer (greater) than the one in <paramref name="rhs"/>.
        /// </summary>
        /// <param name="lhs">The left hand side.</param>
        /// <param name="rhs">The right hand side.</param>
        /// <returns><c>true</c> if the version in <paramref name="lhs"/> is considered greater than that in <paramref name="rhs"/>.</returns>
        /// <remarks>Comparison is done using underlying <see cref="Version"/> structure.</remarks>
        public static bool operator >(OSVersion lhs, OSVersion rhs)
        {
            Version lv = (lhs == null) ? null : lhs.Version;
            Version rv = (rhs == null) ? null : rhs.Version;
            return lv > rv;
        }

        /// <summary>
        /// Determines if the version <paramref name="lhs"/> is older (less) than the one in <paramref name="rhs"/>.
        /// </summary>
        /// <param name="lhs">The left hand side.</param>
        /// <param name="rhs">The right hand side.</param>
        /// <returns><c>true</c> if the version in <paramref name="lhs"/> is considered less than that in <paramref name="rhs"/>.</returns>
        /// <remarks>Comparison is done using underlying <see cref="Version"/> structure.</remarks>
        public static bool operator <(OSVersion lhs, OSVersion rhs)
        {
            Version lv = (lhs == null) ? null : lhs.Version;
            Version rv = (rhs == null) ? null : rhs.Version;
            return lv < rv;
        }

        /// <summary>
        /// Determines if the version <paramref name="lhs"/> is newer (greater) than or equal to the one in <paramref name="rhs"/>.
        /// </summary>
        /// <param name="lhs">The left hand side.</param>
        /// <param name="rhs">The right hand side.</param>
        /// <returns><c>true</c> if the version in <paramref name="lhs"/> is considered greater than or equal to that in <paramref name="rhs"/>.</returns>
        /// <remarks>Comparison is done using underlying <see cref="Version"/> structure.</remarks>
        public static bool operator >=(OSVersion lhs, OSVersion rhs)
        {
            Version lv = (lhs == null) ? null : lhs.Version;
            Version rv = (rhs == null) ? null : rhs.Version;
            return lv >= rv;
        }

        /// <summary>
        /// Determines if the version <paramref name="lhs"/> is older (less) than or equal to the one in <paramref name="rhs"/>.
        /// </summary>
        /// <param name="lhs">The left hand side.</param>
        /// <param name="rhs">The right hand side.</param>
        /// <returns><c>true</c> if the version in <paramref name="lhs"/> is considered less than or equal to that in <paramref name="rhs"/>.</returns>
        /// <remarks>Comparison is done using underlying <see cref="Version"/> structure.</remarks>
        public static bool operator <=(OSVersion lhs, OSVersion rhs)
        {
            Version lv = (lhs == null) ? null : lhs.Version;
            Version rv = (rhs == null) ? null : rhs.Version;
            return lv <= rv;
        }

        #endregion // Operators

        #region object Overrides

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Version.Equals(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
        }

        #endregion // object Overrides
    }
}
