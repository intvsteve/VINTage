// <copyright file="PresentationOrder.cs" company="INTV Funhouse">
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
using System.Linq;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Describes the order in which the LFS presents the entries in a directory.
    /// </summary>
    public class PresentationOrder : INTV.Core.Utility.ByteSerializer, IEnumerable<ushort>
    {
        #region Constants

        /// <summary>
        /// The flat size in bytes.
        /// </summary>
        public const int FlatSizeInBytes = FileSystemConstants.MaxItemCount * sizeof(ushort);

        #endregion // Constants

        private List<ushort> _presentationOrder;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a PresentationOrder.
        /// </summary>
        public PresentationOrder()
        {
            _presentationOrder = new List<ushort>(Enumerable.Repeat(ushort.MaxValue, FileSystemConstants.MaxItemCount));
        }

        /// <summary>
        /// Initializes a new instance of a PresentationOrder from an enumerable of IFileInfo objects.
        /// </summary>
        /// <param name="fileInfos">The files to place in the presentation order.</param>
        public PresentationOrder(IEnumerable<ILfsFileInfo> fileInfos)
            : this()
        {
            if (fileInfos != null)
            {
                int i = 0;
                foreach (var fileInfo in fileInfos)
                {
                    _presentationOrder[i] = fileInfo.GlobalFileNumber;
                    ++i;
                }
            }
            Validate();
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the number of valid entries in the PresentationOrder.
        /// </summary>
        public int ValidEntryCount { get; private set; }

        /// <summary>
        /// Gets or sets the element at the given index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
        /// <remarks>The index must be in the range [0-FileSystemConstants.MaxItemCount).</remarks>
        public ushort this[int index]
        {
            get
            {
                return _presentationOrder[index];
            }

            set
            {
                _presentationOrder[index] = value;
                Validate();
            }
        }

        #region ByteSerializer Properties

        /// <inheritdoc />
        public override int SerializeByteCount
        {
            get { return FlatSizeInBytes; }
        }

        /// <inheritdoc />
        public override int DeserializeByteCount
        {
            get { return FlatSizeInBytes; }
        }

        #endregion // ByteSerializer Properties

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of a PresentationOrder by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a PresentationOrder.</returns>
        public static PresentationOrder Inflate(System.IO.Stream stream)
        {
            return Inflate<PresentationOrder>(stream);
        }

        /// <summary>
        /// Creates a new instance of a PresentationOrder by inflating it from a Stream.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a PresentationOrder.</returns>
        public static PresentationOrder Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            return Inflate<PresentationOrder>(reader);
        }

        #region Operators

        /// <summary>
        /// Determines equality of two PresentationOrder objects.
        /// </summary>
        /// <param name="lhs">Left-hand side of equality operator.</param>
        /// <param name="rhs">Right-hand side of equality operator.</param>
        /// <returns><c>true</c> if the two objects are equal.</returns>
        public static bool operator ==(PresentationOrder lhs, PresentationOrder rhs)
        {
            bool areEqual = object.ReferenceEquals(lhs, rhs);
            if (!areEqual && !object.ReferenceEquals(lhs, null) && !object.ReferenceEquals(rhs, null))
            {
                areEqual = (lhs._presentationOrder.Count == rhs._presentationOrder.Count) && (lhs.ValidEntryCount == rhs.ValidEntryCount);
                for (int i = 0; areEqual && (i < FileSystemConstants.MaxItemCount); ++i)
                {
                    areEqual = lhs._presentationOrder[i] == rhs._presentationOrder[i];
                }
            }
            return areEqual;
        }

        /// <summary>
        /// Determines inequality of two PresentationOrder objects.
        /// </summary>
        /// <param name="lhs">Left-hand side of inequality operator.</param>
        /// <param name="rhs">Right-hand side of inequality operator.</param>
        /// <returns><c>true</c> if the two objects are not equal.</returns>
        public static bool operator !=(PresentationOrder lhs, PresentationOrder rhs)
        {
            return !(rhs == lhs);
        }

        #endregion // Operators

        /// <summary>
        /// Adds an entry to the presentation order at the first available location.
        /// </summary>
        /// <param name="item">The entry to add to the presentation order.</param>
        public void Add(ushort item)
        {
            var insertLocation = _presentationOrder.IndexOf(ushort.MaxValue);
            if (insertLocation < 0)
            {
                throw new InvalidOperationException();
            }
            _presentationOrder[insertLocation] = item;
            Validate();
        }

        #region ByteSerializer

        /// <inheritdoc />
        public override int Serialize(INTV.Core.Utility.BinaryWriter writer)
        {
            if (_presentationOrder.Count != FileSystemConstants.MaxItemCount)
            {
                throw new System.InvalidOperationException();
            }
            foreach (var entry in _presentationOrder)
            {
                writer.Write(entry);
            }
            return SerializeByteCount;
        }

        /// <inheritdoc />
        public override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            for (int i = 0; i < FileSystemConstants.MaxItemCount; ++i)
            {
                _presentationOrder[i] = reader.ReadUInt16();
            }
            Validate();
            return DeserializeByteCount;
        }

        #endregion // ByteSerializer

        #region IEnumerable<ushort>

        /// <inheritdoc />
        public IEnumerator<ushort> GetEnumerator()
        {
            return _presentationOrder.GetEnumerator();
        }

        #endregion // IEnumerable<ushort>

        #region IEnumerable

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_presentationOrder).GetEnumerator();
        }

        #endregion // IEnumerable

        #region object Overrides

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            bool areEqual = obj is PresentationOrder;
            if (areEqual)
            {
                areEqual = this == (PresentationOrder)obj;
            }
            return areEqual;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var element in _presentationOrder)
            {
                hash ^= element.GetHashCode();
            }
            return hash;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "PresentationOrder {ValidEntries:" + ValidEntryCount + "}";
        }

        #endregion // object Overrides

        private void Validate()
        {
            if (_presentationOrder.Count != FileSystemConstants.MaxItemCount)
            {
                throw new System.InvalidOperationException();
            }
            int validEntries = 0;
            foreach (var globalFileIndex in _presentationOrder)
            {
                if (globalFileIndex < FileSystemConstants.GlobalFileTableSize)
                {
                    ++validEntries;
                }
            }
            ValidEntryCount = validEntries;
        }
    }
}
