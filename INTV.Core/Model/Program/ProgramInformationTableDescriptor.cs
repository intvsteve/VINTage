// <copyright file="ProgramInformationTableDescriptor.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    public sealed class ProgramInformationTableDescriptor
    {
        private readonly StorageLocation _location;
        private readonly Func<StorageLocation, IProgramInformationTable> _factory;

        /// <summary>
        /// Creates a new instance with default data (null location and factory).
        /// </summary>
        public ProgramInformationTableDescriptor()
            : this(StorageLocation.Null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of ProgramInformationTableDescriptor.
        /// </summary>
        /// <param name="location">Location of a program information table.</param>
        /// <param name="factory">The factory function for the table.</param>
        public ProgramInformationTableDescriptor(StorageLocation location, Func<StorageLocation, IProgramInformationTable> factory)
        {
            _location = location;
            _factory = factory;
        }

        /// <summary>
        /// Gets the location of the information table.
        /// </summary>
        public StorageLocation FilePath
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets the factory function for an information table.
        /// </summary>
        public Func<StorageLocation, IProgramInformationTable> Factory
        {
            get { return _factory; }
        }
    }
}
