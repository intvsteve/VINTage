// <copyright file="BinaryWriter.cs" company="INTV Funhouse">
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

using System.Text;

namespace INTV.Core.Utility
{
    /// <summary>
    /// Because we're not using .NET 4.5, emulate its behavior w/ the new constructor option that
    /// keeps the underlying stream open upon dispose.
    /// </summary>
    public class BinaryWriter : System.IO.BinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the BinaryWriter class based on the specified stream and using UTF-8 encoding and that leaves
        /// the given stream open upon disposal
        /// </summary>
        /// <param name="stream">The output stream.</param>
        public BinaryWriter(System.IO.Stream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BinaryWriter class based on the specified stream and character encoding that leaves
        /// the stream open.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public BinaryWriter(System.IO.Stream stream, Encoding encoding)
            : this(stream, encoding, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BinaryWriter class based on the specified stream and character encoding, and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after the BinaryWriter object is disposed; otherwise, <c>false</c>.</param>
        public BinaryWriter(System.IO.Stream stream, Encoding encoding, bool leaveOpen)
            : base(stream, encoding)
        {
            LeaveOpen = leaveOpen;
        }

        private bool LeaveOpen { get; set; }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!LeaveOpen)
            {
                base.Dispose(disposing);
            }
        }
    }
}
