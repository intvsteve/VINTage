// <copyright file="MetadataString.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace INTV.Core.Model
{
    /// <summary>
    /// This class is intended as a drop-in replacement for strings that may be vulnerable to encountering invalid characters.
    /// </summary>
    /// <remarks>
    /// <para>The specific use cases meant to be handled are binary data streams that may contain malformed, or
    /// disallowed characters which will ultimately be stored in XML. This issue was first here:
    /// https://github.com/intvsteve/VINTage/issues/257 in which, due to raw binary data being deserialized in a LUIGI ROM
    /// (due to other issues), the resulting bad ROM name was saved to both ROM list and Menu Layout XML files. The subsequent
    /// launch of the application was unable to load these files due to the serialization of invalid XML string characters.
    /// (There is debate about whether this should be considered a bug in the .NET XmlSerializer implementation.)</para>
    /// <para>Regardless, this type can be used as the backing property to replace an existing string that is vulnerable to
    /// such situations. Simply add a property of this type, mark it with an XElementAttribute name of the property it's backing,
    /// and then mark the original property with the XmlIgnore tag.</para>
    /// </remarks>
    /// <example>
    /// This example illustrates the original version of a class, followed by the modified version of the class using this type.
    /// The ROM's Name property is subject to this problem, as its value can be retrieved via intvname or by parsing data from
    /// a ROM file. Parsing errors, malformed or malicious content could cause invalid characters to appear in 'Name'.
    /// <code>
    /// class RomInfo
    /// {
    ///     public string Name { get; set; }
    ///
    ///     public string FilePath { get; set; }
    /// }
    /// </code>
    /// The modified class uses MetadataString to inoculate against bad characters in 'Name'. Note that because XmlSerializer
    /// must be able to access the properties, they must all remain public.
    /// <code>
    /// class RomInfo
    /// {
    ///     public RomInfo()
    ///     {
    ///         XmlName = new MetadataString();
    ///     }
    ///
    ///     [XmlElement("Name")]
    ///     public MetadataString XmlName { get; set; }
    ///
    ///     [XmlIgnore]
    ///     public string Name
    ///     {
    ///         get { return XmlName.Text; }
    ///         set { XmlName.Text = value; }
    ///     }
    ///
    ///     public string FilePath { get; set; }
    /// }
    /// </code>
    /// </example>
    public class MetadataString
    {
        /// <summary>
        ///  Gets or sets a value indicating whether the <see cref="XmlText"/> content has been escaped as BinHex text.
        /// </summary>
        [XmlAttribute]
        public bool Escaped { get; set; }

        /// <summary>
        /// Gets or sets the text that will be saved during XML serialization.
        /// </summary>
        [XmlText]
        public string XmlText { get; set; }

        /// <summary>
        /// Gets or sets the text that may need to be stored as a BinHex representation when serialized to XML.
        /// </summary>
        /// <remarks>When being set, the value is inspected to determine if BinHex representation is necessary.
        /// When being read, if the <see cref="XmlText"/> has been escaped as BinHex, it will be un-escaped.</remarks>
        [XmlIgnore]
        public string Text
        {
            get
            {
                var text = XmlText;
                if (Escaped)
                {
                    var bytes = new List<byte>();
                    var xmlReaderSettings = new XmlReaderSettings()
                    {
                        ConformanceLevel = ConformanceLevel.Fragment
                    };
                    using (var reader = XmlReader.Create(new StringReader(XmlText), xmlReaderSettings))
                    {
                        reader.MoveToContent();

                        var byteBuffer = new byte[128];
                        var bytesRead = reader.ReadContentAsBinHex(byteBuffer, 0, byteBuffer.Length);
                        while (bytesRead > 0)
                        {
                            bytes.AddRange(byteBuffer.Take(bytesRead));
                            bytesRead = reader.ReadContentAsBinHex(byteBuffer, 0, byteBuffer.Length);
                        }
                    }
                    text = Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
                }
                return text;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    XmlText = value;
                    Escaped = false;
                }
                else
                {
                    try
                    {
                        XmlText = XmlConvert.VerifyXmlChars(value);
                        Escaped = false;
                    }
                    catch (XmlException)
                    {
                        var xmlWriterSettings = new XmlWriterSettings()
                        {
                            ConformanceLevel = ConformanceLevel.Fragment
                        };
                        var stringBuilder = new StringBuilder();
                        using (var writer = XmlWriter.Create(stringBuilder, xmlWriterSettings))
                        {
                            var bytes = Encoding.UTF8.GetBytes(value);
                            writer.WriteBinHex(bytes, 0, bytes.Length);
                        }
                        XmlText = stringBuilder.ToString();
                        Escaped = true;
                    }
                }
            }
        }
    }
}
