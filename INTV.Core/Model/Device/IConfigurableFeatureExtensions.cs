// <copyright file="IConfigurableFeatureExtensions.cs" company="INTV Funhouse">
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

using System;
using System.Globalization;

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Extension methods for <see cref="IConfigurableFeature"/>.
    /// </summary>
    public static class IConfigurableFeatureExtensions
    {
        /// <summary>
        /// Verifies that the given configurable feature is not read-only.
        /// </summary>
        /// <param name="configurableFeature">The configurable feature to verify.</param>
        /// <remarks>Methods that just throw expose a code coverage bug (at least in Visual Studio 2012) that leaves one bogus 'uncovered' block.</remarks>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void VerifyWriteAccess(this IConfigurableFeature configurableFeature)
        {
            if (configurableFeature is IReadOnlyConfigurableFeature)
            {
                ReportInvalidAccess(configurableFeature.DisplayName);
            }
        }

        /// <summary>
        /// Verifies that the given configurable feature is not read-only.
        /// </summary>
        /// <typeparam name="T">The data type of the configurable feature.</typeparam>
        /// <param name="configurableFeature">The configurable feature to verify.</param>
        /// <remarks>Methods that just throw expose a code coverage bug (at least in Visual Studio 2012) that leaves one bogus 'uncovered' block.</remarks>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void VerifyWriteAccess<T>(this IConfigurableFeature<T> configurableFeature)
        {
            if ((configurableFeature is IReadOnlyConfigurableFeature) || (configurableFeature is IReadOnlyConfigurableFeature<T>))
            {
                ReportInvalidAccess(configurableFeature.DisplayName);
            }
        }

        private static void ReportInvalidAccess(string name)
        {
            var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.ConfigurableFeature_InvalidWriteOperationMessageFormat, name);
            throw new InvalidOperationException(message);
        }
    }
}
