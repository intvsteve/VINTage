// <copyright file="JlpHardwareVersionHelpersTests.cs" company="INTV Funhouse">
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
using System.Resources;
using INTV.Core.Model;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class JlpHardwareVersionHelpersTests
    {
        public static IEnumerable<object[]> JlpHardwareVersionDisplayStrings
        {
            get
            {
                var values = Enum.GetValues(typeof(JlpHardwareVersion)).Cast<JlpHardwareVersion>();
                foreach (var value in values)
                {
                    var resourceName = "INTV.Core.Resources.Strings";
                    var assembly = typeof(JlpHardwareVersion).Assembly;
                    var resourceManager = new ResourceManager(resourceName, assembly);
                    var resourceKey = "JlpHardwareVersion_" + value.ToString().Replace("Jlp", string.Empty);
                    var expectedDisplayValue = resourceManager.GetString(resourceKey);
                    yield return new object[] { value, expectedDisplayValue };
                }
            }
        }

        [Theory]
        [MemberData("JlpHardwareVersionDisplayStrings")]
        public void JlpHardwareVersion_ToDisplayString_ProvidesCorrectDisplayString(JlpHardwareVersion jlpHardwareVersion, string expectedDisplayValue)
        {
            var actualDisplayValue = jlpHardwareVersion.ToDisplayString();

            Assert.Equal(expectedDisplayValue, actualDisplayValue);
        }

        [Fact]
        public void JlpHardwareVersionThatIsBogus_ToDisplayString_ReturnsNone()
        {
            var bogusJlpVersion = (JlpHardwareVersion)12345;

            Assert.Equal(INTV.Core.Resources.Strings.JlpHardwareVersion_None, bogusJlpVersion.ToDisplayString());
        }

        [Theory]
        [MemberData("JlpHardwareVersionDisplayStrings")]
        public void JlpHardwareVersion_FromDisplayString_ProvidesCorrectValue(JlpHardwareVersion expectedJlpHardwareVersion, string displayString)
        {
            var jlpHardwareVersion = JlpHardwareVersionHelpers.FromDisplayString(displayString);

            Assert.Equal(expectedJlpHardwareVersion, jlpHardwareVersion);
        }

        [Fact]
        public void JlpHardwareVersion_FromNullDisplayString_ProvidesHardwareversionNone()
        {
            var jlpHardwareVersion = JlpHardwareVersionHelpers.FromDisplayString(null);

            Assert.Equal(JlpHardwareVersion.None, jlpHardwareVersion);
        }

        [Fact]
        public void JlpHardwareVersion_FromBogusDisplayString_ProvidesHardwareversionNone()
        {
            var jlpHardwareVersion = JlpHardwareVersionHelpers.FromDisplayString("~!)(!~ Derp");

            Assert.Equal(JlpHardwareVersion.None, jlpHardwareVersion);
        }

/*
        private static string GetResourceString(this Type type, string resourceName, string key)
        {
            var value = "!!MISSING STRING!!";
            try
            {
                var assembly = type.Assembly;
                resourceName = assembly.GetName().Name + ".Resources." + resourceName;
                var resourceManager = new System.Resources.ResourceManager(resourceName, type.Assembly);
                value = resourceManager.GetString(key);
            }
            catch (InvalidOperationException)
            {
                value = "!!RESOURCE '" + resourceName + "." + key + " ' IS NOT A STRING!!";
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                value = "!!NO RESOURCES FOUND FOR '" + resourceName + "." + key + "'!!";
            }
            catch (System.Resources.MissingSatelliteAssemblyException)
            {
                value = "!!NO SATELLITE RESOURCES FOUND FOR '" + resourceName + "." + key + "'!!";
            }
            return value;
        }
 * */
    }
}
