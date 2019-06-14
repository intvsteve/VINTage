// <copyright file="ByteSerializer.cs" company="INTV Funhouse">
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// Simple class for serializing or deserializing a class using raw binary.
    /// </summary>
    public abstract class ByteSerializer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of ByteSerializer.
        /// </summary>
        public ByteSerializer()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the expected byte count of the serialized object.
        /// </summary>
        /// <remarks>If this value is less than or equal to zero, this indicates either that the object's
        /// serialized size cannot be known a priori, but must be determined by performing the serialization.</remarks>
        public abstract int SerializeByteCount { get; }

        /// <summary>
        /// Gets the expected byte count to deserialize the object, or the number of bytes deserialized.
        /// </summary>
        /// <remarks>If the value is less than or equal to zero, this indicates that the object has not
        /// yet been deserialized, and the size is not known a priori, but rather determined during deserialization.</remarks>
        public abstract int DeserializeByteCount { get; }

        /// <summary>
        /// Creates a new instance of a ByteSerializer-based type by inflating it from a Stream.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of the given type.</returns>
        public static T Inflate<T>(System.IO.Stream stream) where T : ByteSerializer, new()
        {
            T inflatedObject = default(T);
            using (var reader = new BinaryReader(stream))
            {
                inflatedObject = Inflate<T>(reader);
            }
            return inflatedObject;
        }

        /// <summary>
        /// Creates a new instance of a ByteSerializer-based type by inflating it from a BinaryReader.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="reader">The binary reader to use to deserialize the data for the object.</param>
        /// <returns>A new instance of the given type.</returns>
        public static T Inflate<T>(BinaryReader reader) where T : ByteSerializer, new()
        {
            T inflatedObject = new T();
            inflatedObject.Deserialize(reader);
            return inflatedObject;
        }

        /// <summary>
        /// Serializes the data in the object to a stream.
        /// </summary>
        /// <param name="stream">The stream into which to serialize the object.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>The number of bytes serialized into the stream.</returns>
        public int Serialize(System.IO.Stream stream, System.Text.Encoding encoding)
        {
            int numSerialized = 0;
            using (var writer = new BinaryWriter(stream, encoding))
            {
                numSerialized = Serialize(writer);
            }
            return numSerialized;
        }

        /// <summary>
        /// Serializes the data in the object using a binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to use to serialize the data.</param>
        /// <returns>The number of bytes serialized by the writer.</returns>
        public abstract int Serialize(BinaryWriter writer);

        /// <summary>
        /// Inflate the data in the binary reader into the object.
        /// </summary>
        /// <param name="reader">The reader to use to inflate the data.</param>
        /// <returns>The number of bytes deserialized by the reader.</returns>
        public abstract int Deserialize(BinaryReader reader);
    }
}
