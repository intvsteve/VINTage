// <copyright file="ConvertToMultipleTests.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class ConvertToMultipleTests
    {
        [Fact]
        public void ConvertToMultiple_Convert_ProducesCodeCoverage()
        {
            Assert.Equal(new int[] { 65, 65, 65, 65 }, StringToCharsConverter.Instance.Convert("AAAA"));
        }

        private class StringToCharsConverter : ConvertToMultiple<StringToCharsConverter, string, int>
        {
            public override IEnumerable<int> Convert(string source)
            {
                foreach (var character in source)
                {
                    yield return (int)character;
                }
            }
        }
    }
}
