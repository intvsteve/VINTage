// <copyright file="LuigiDataHunkBlock.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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
    /// A block containing a chunk of data destined for Locutus' external RAM.
    /// </summary>
    /// <remarks>This block is not parsed.</remarks>
    public class LuigiDataHunkBlock : LuigiDataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.LuigiDataHunkBlock"/> class.
        /// </summary>
        internal LuigiDataHunkBlock()
            : base(LuigiDataBlockType.DataHunk)
        {
        }
    }
}
