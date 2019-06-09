// <copyright file="STADiscoverer.cs" company="INTV Funhouse">
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
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace INTV.TestHelpers.Shared.Xunit.Sdk
{
    /// <summary>
    /// Provides common implementation for STA discoverers.
    /// </summary>
    public abstract class STADiscoverer : IXunitTestCaseDiscoverer
    {
        /// <summary>
        /// Gets the wrapped discoverer to use.
        /// </summary>
        protected abstract IXunitTestCaseDiscoverer WrappedDiscoverer { get; }

        /// <inheritdoc/>
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            return WrappedDiscoverer.Discover(discoveryOptions, testMethod, factAttribute).Select(t => WrapTestCase(t, factAttribute));
        }

        private static STATestCase WrapTestCase(IXunitTestCase testCase, IAttributeInfo factAttribute)
        {
            var usePackUri = factAttribute.GetNamedArgument<bool>(STATestCase.UsePackUriPropertyName);
            var usePackUriApplication = factAttribute.GetNamedArgument<bool>(STATestCase.UsePackUriApplicationPropertyName);
            var staTestCase = new STATestCase(testCase)
            {
                UsePackUri = usePackUri,
                UsePackUriApplication = usePackUriApplication,
            };
            return staTestCase;
        }
    }
}
