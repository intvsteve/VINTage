// <copyright file="MetadataStringTests.cs" company="INTV Funhouse">
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

using System.IO;
using System.Xml.Serialization;
using INTV.Core.Model;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class MetadataStringTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void MetadataString_SetToNullOrEmpty_DoesNotThrow(string nullOrEmpty)
        {
            var metadataString = new MetadataString();

            metadataString.Text = nullOrEmpty;

            Assert.True(string.IsNullOrEmpty(metadataString.Text));
            Assert.False(metadataString.Escaped);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\"")]
        [InlineData("\"\"")]
        [InlineData("\"\"\"")]
        [InlineData(" \t\" a\"b ")]
        [InlineData("\" a\"b \"")]
        [InlineData("Kröte")]
        [InlineData(" \" Kröte \" ")]
        [InlineData("Kr\"öte")]
        [InlineData("\"\"Kröte")]
        [InlineData("\n\"\"Kröte\t\n \" ")]
        [InlineData("\"子豚\"")]
        [InlineData("子\"豚")]
        [InlineData("子豚")]
        [InlineData("\"ハウディドゥーディー\"")]
        [InlineData("ハウディドゥー\"ディー")]
        [InlineData("ハウディドゥー\"ディー\" ")]
        [InlineData("ハウディドゥーディー")]
        public void MetadataString_ShouldNotNeedEscaping_IsNotEscaped(string stringThatDoesNotNeedToBeEscaped)
        {
            var metadataString = new MetadataString();

            metadataString.Text = stringThatDoesNotNeedToBeEscaped;

            Assert.False(metadataString.Escaped);
            Assert.Equal(stringThatDoesNotNeedToBeEscaped, metadataString.XmlText);
            Assert.Equal(stringThatDoesNotNeedToBeEscaped, metadataString.Text);
        }

        public static string EvilString
        {
            get
            {
                var evilBytes = new byte[321];
                for (var i = 0; i < evilBytes.Length; ++i)
                {
                    evilBytes[i] = (byte)(i % 256);
                }
                var evilString = System.Text.Encoding.UTF8.GetString(evilBytes);
                return evilString;
            }
        }

        [Fact]
        public void MetadataString_AssignEvilString_IsEscapedAndUnescapesCorrectly()
        {
            var metadataString = new MetadataString();

            metadataString.Text = EvilString;

            Assert.True(metadataString.Escaped);
            Assert.Equal(EvilString, metadataString.Text);
        }

        #region XmlSerializer Tests

        /// <summary>
        /// Ensure that the MetadataString behaves the way you'd expect for a decent XML string.
        /// </summary>
        [Fact]
        public void MetadataString_XmlSerializeDeserializeWithGoodString_SerializesAndDeserializesCorrectly()
        {
            var originalTestClassInstance = new TestClassForXmlSerialization()
            {
                IsReallyCool = true,
                Name = "Wie geht es Ihnen? <Mir geht's doch gut.> They're \"GRRREEEAAAATT!!\"\tdude."
            };

            var xmlString = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(TestClassForXmlSerialization));
                serializer.Serialize(stringWriter, originalTestClassInstance);
                xmlString = stringWriter.ToString();
            }

            TestClassForXmlSerialization deserializedInstance = null;
            using (var stringReader = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(TestClassForXmlSerialization));
                deserializedInstance = serializer.Deserialize(stringReader) as TestClassForXmlSerialization;
            }

            Assert.Equal(originalTestClassInstance.IsReallyCool, deserializedInstance.IsReallyCool);
            Assert.Equal(originalTestClassInstance.Name, deserializedInstance.Name);
        }

        /// <summary>
        /// Ensure that when a class uses the MetadataString type with a really evil string that it
        /// can be serialized to, and deserialized from XML as expected, resuscitating the evil.
        /// </summary>
        [Fact]
        public void MetadataString_XmlSerializeDeserializeWithEvilString_SerializesAndDeserializesCorrectly()
        {
            var originalTestClassInstance = new TestClassForXmlSerialization()
            {
                IsReallyCool = true,
                Name = EvilString
            };

            var xmlString = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(TestClassForXmlSerialization));
                serializer.Serialize(stringWriter, originalTestClassInstance);
                xmlString = stringWriter.ToString();
            }

            TestClassForXmlSerialization deserializedInstance = null;
            using (var stringReader = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(TestClassForXmlSerialization));
                deserializedInstance = serializer.Deserialize(stringReader) as TestClassForXmlSerialization;
            }

            Assert.Equal(originalTestClassInstance.IsReallyCool, deserializedInstance.IsReallyCool);
            Assert.Equal(originalTestClassInstance.Name, deserializedInstance.Name);
        }

        /// <summary>
        /// Ensure that an old instance of the object serialized as XML can be parsed into the new
        /// version of the object and retain the data as expected.
        /// </summary>
        [Fact]
        public void MetadataString_SerializeOldClassDeserializeNewClassUsingGoodString_ProducesEquivalentObject()
        {
            var oldObject = new OldTestClassForXmlSerialization()
            {
                IsReallyCool = true,
                Name = "Well isn't <that> special!"
            };

            var xmlString = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(OldTestClassForXmlSerialization));
                serializer.Serialize(stringWriter, oldObject);
                xmlString = stringWriter.ToString();
            }

            TestClassForXmlSerialization newObject = null;
            using (var stringReader = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(TestClassForXmlSerialization));
                newObject = serializer.Deserialize(stringReader) as TestClassForXmlSerialization;
            }

            Assert.Equal(oldObject.IsReallyCool, newObject.IsReallyCool);
            Assert.Equal(oldObject.Name, newObject.Name);
        }

        /// <summary>
        /// Ensure that for all practical intents and purposes, if a well-behaved string is in an
        /// XML string created by the new implementation that the old implementation can use it.
        /// </summary>
        [Fact]
        public void MetadataString_SerializeNewClassDeserializeOldClassUsingGoodString_ProducesEquivalentObject()
        {
            var newObject = new TestClassForXmlSerialization()
            {
                IsReallyCool = true,
                Name = "Well isn't <that> special!"
            };

            var xmlString = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(TestClassForXmlSerialization));
                serializer.Serialize(stringWriter, newObject);
                xmlString = stringWriter.ToString();
            }

            OldTestClassForXmlSerialization oldObject = null;
            using (var stringReader = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(OldTestClassForXmlSerialization));
                oldObject = serializer.Deserialize(stringReader) as OldTestClassForXmlSerialization;
            }

            Assert.Equal(newObject.IsReallyCool, oldObject.IsReallyCool);
            Assert.Equal(newObject.Name, oldObject.Name);
        }

        /// <summary>
        /// Ensure that if an old version of the deserialization is used with the output of the newer
        /// serialization that it will not crash, and that it will use the BinHex string.
        /// </summary>
        [Fact]
        public void MetadataString_SerializeNewObjectWithEvilStringDeserializeOldClass_ProducesOldObjectWithBinHexString()
        {
            var newObject = new TestClassForXmlSerialization()
            {
                IsReallyCool = true,
                Name = EvilString
            };

            var xmlString = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(TestClassForXmlSerialization));
                serializer.Serialize(stringWriter, newObject);
                xmlString = stringWriter.ToString();
            }

            OldTestClassForXmlSerialization oldObject = null;
            using (var stringReader = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(OldTestClassForXmlSerialization));
                oldObject = serializer.Deserialize(stringReader) as OldTestClassForXmlSerialization;
            }

            Assert.Equal(newObject.IsReallyCool, oldObject.IsReallyCool);
            Assert.Equal(newObject.XmlString.XmlText, oldObject.Name);
        }

        /// <summary>
        /// Ensure that the old behavior is what we expect.
        /// </summary>
        [Fact]
        public void MetadataString_SerializeOldObjectWithEvilString_ThrowsInvalidOperationOnDeserialize()
        {
            var oldEvilObject = new OldTestClassForXmlSerialization()
            {
                IsReallyCool = true,
                Name = EvilString
            };

            var xmlString = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(OldTestClassForXmlSerialization));
                serializer.Serialize(stringWriter, oldEvilObject);
                xmlString = stringWriter.ToString();
            }

            using (var stringReader = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(OldTestClassForXmlSerialization));
                Assert.Throws<System.InvalidOperationException>(() => serializer.Deserialize(stringReader));
            }
        }

        #endregion // XmlSerializer Usage Tests

        /// <summary>
        /// Simple class to emulate application of the new MetadataString class to supersede a
        /// current property on a type such that it "invisibly" replaces the existing property
        /// from the point of view of the XML serialization process. This class replaces the
        /// original 'Name' property on <see cref="OldTestClassForXmlSerialization"/> to use
        /// the new <see cref="MetadataString"/> object as a sort of backing field whose XML
        /// serialization is safe in spite of illegal characters arriving from an external
        /// source. The existing references to the Name property can operate as before.
        /// However, when saved to XML, the object will instead use the safe string, which will
        /// use BinHex format if the string contains anything toxic.
        /// </summary>
        public class TestClassForXmlSerialization
        {
            public TestClassForXmlSerialization()
            {
                XmlString = new MetadataString();
            }

            public bool IsReallyCool { get; set; }

            [XmlIgnore]
            public string Name
            {
                get { return XmlString.Text; }
                set { XmlString.Text = value; }
            }

            [XmlElement("Name")]
            public MetadataString XmlString { get; set; }
        }

        /// <summary>
        /// This class emulates an older version of <see cref="TestClassForXmlSerialization"/> that
        /// does not have the protections provided by <see cref="MetadataString"/>. It will freely
        /// convert between the old and new version - with the caveat that writing an 'evil' string
        /// to this class, while it will serialize, will fail to deserialize due to invalid characters.
        /// If the old class is deserialized from XML created by the new version, it will not know
        /// to un-bin-hex and convert back to a string, so the BinHex string is used.
        /// </summary>
        [XmlRoot("TestClassForXmlSerialization")]
        public class OldTestClassForXmlSerialization
        {
            public bool IsReallyCool { get; set; }

            public string Name { get; set; }
        }
    }
}
