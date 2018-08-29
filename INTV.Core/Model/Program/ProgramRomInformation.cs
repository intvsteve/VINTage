// <copyright file="ProgramRomInformation.cs" company="INTV Funhouse">
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
using System.Text;

namespace INTV.Core.Model.Program
{
    public class ProgramRomInformation : IProgramRomInformation
    {
        #region Properties

        #region IProgramRomInformation

        /// <inheritdoc />
        public string Title
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public string Vendor
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public string Year
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public string LongName
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public string ShortName
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public string VariantName
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public RomFormat Format
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public ProgramIdentifier Id
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public ProgramFeatures Features
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public IProgramMetadata Metadata
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        #endregion // IProgramRomInformation

        #endregion // Properties
    }
}
