// <copyright file="FileSystemFile.cs" company="INTV Funhouse">
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

using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// This class assists in the process of uploading a file on the host PC's local file system to a
    /// Locutus device or downloading a file (fork) from Locutus.
    /// </summary>
    public class FileSystemFile : INTV.Core.Utility.ByteSerializer, System.IDisposable
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of FileSystemFile.
        /// </summary>
        /// <remarks>The file represented here is a temporary file on the host computer's file system.</remarks>
        public FileSystemFile()
            : this(System.IO.Path.GetTempFileName())
        {
            IsTempFile = true;
        }

        /// <summary>
        /// Creates a new instance of FileSystemFile.
        /// </summary>
        /// <param name="filePath">The fully qualified path to an existing file to represent.</param>
        public FileSystemFile(string filePath)
            : this(new System.IO.FileInfo(filePath))
        {
        }

        /// <summary>
        /// Creates a new instance of FileSystemFile.
        /// </summary>
        /// <param name="fileInfo">The file information of an existing file on the host computer's file system.</param>
        public FileSystemFile(System.IO.FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        ~FileSystemFile()
        {
            Dispose(false);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the file information of the wrapped file.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public System.IO.FileInfo FileInfo { get; protected set; }

        [System.Xml.Serialization.XmlIgnore]
        protected bool IsTempFile { get; set; }

        #region ByteSerializer Properties

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public override int SerializeByteCount
        {
            get { return (int)FileInfo.Length; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public override int DeserializeByteCount
        {
            get { return (int)FileInfo.Length; }
        }

        #endregion // ByteSerializer Properties

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of a FileSystemFile by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a temporary FileSystemFile.</returns>
        public static FileSystemFile Inflate(System.IO.Stream stream)
        {
            return Inflate<FileSystemFile>(stream);
        }

        /// <summary>
        /// Creates a new instance of a FileSystemFile by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a temporary FileSystemFile.</returns>
        public static FileSystemFile Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            return Inflate<FileSystemFile>(reader);
        }

        #region ByteSerializer

        /// <inheritdoc />
        public override int Serialize(Core.Utility.BinaryWriter writer)
        {
            using (var file = FileUtilities.OpenFileStream(FileInfo.FullName))
            {
                file.CopyTo(writer.BaseStream);
            }
            return (int)FileInfo.Length;
        }

        /// <inheritdoc />
        public override int Deserialize(Core.Utility.BinaryReader reader)
        {
            using (var file = new System.IO.FileStream(FileInfo.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Write))
            {
                reader.BaseStream.CopyTo(file);
            }
            return (int)FileInfo.Length;
        }

        #endregion // ByteSerializer

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                System.GC.SuppressFinalize(this);
            }
            if (IsTempFile)
            {
                try
                {
                    INTV.Shared.Utility.FileUtilities.DeleteFile(FileInfo.FullName, false, 2);
                }
                catch
                {
                }
                FileInfo = null;
            }
        }

        #endregion // IDisposable
    }
}
