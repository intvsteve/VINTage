// <copyright file="ProgramInformationTableDescriptor.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    public struct ProgramInformationTableDescriptor
    {
        private string _path;
        private Func<string, IProgramInformationTable> _factory;

        /// <summary>
        /// Initializes a new instance of ProgramInformationTableDescriptor.
        /// </summary>
        /// <param name="path">Absolute path to a program information table.</param>
        /// <param name="factory">The factory function for the table.</param>
        public ProgramInformationTableDescriptor(string path, Func<string, IProgramInformationTable> factory)
        {
            _path = path;
            _factory = factory;
        }

        /// <summary>
        /// Gets the full path to the information table on the local file system.
        /// </summary>
        public string FilePath
        {
            get { return _path; }
        }

        /// <summary>
        /// Gets the factory function for an information table.
        /// </summary>
        public Func<string, IProgramInformationTable> Factory
        {
            get { return _factory; }
        }
    }
}
