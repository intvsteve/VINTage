// <copyright file="CfgVarMetadataInteger.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// A simple class for integer metadata from a .CFG file.
    /// </summary>
    public class CfgVarMetadataInteger : CfgVarMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.CfgVarMetadataInteger"/> class.
        /// </summary>
        /// <param name="type">he specific kind of Boolean metadata.</param>
        public CfgVarMetadataInteger(CfgVarMetadataIdTag type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets or sets the integer value.
        /// </summary>
        public int IntegerValue { get; protected set; }

        protected override void Parse(string payload)
        {
            int value;
            if (int.TryParse(GetCleanPayloadString(payload), out value))
            {
                IntegerValue = value;
            }
        }
    }
}
