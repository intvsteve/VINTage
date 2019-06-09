// <copyright file="TestCultureInfo.cs" company="INTV Funhouse">
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
using System.Threading;

namespace INTV.TestHelpers.Shared.Utility
{
    /// <summary>
    /// Helper class to change the UI and optionally format culture for a planned duration.
    /// </summary>
    /// <remarks>When disposed, the original thread's culture settings are restored to the values when initially called.</remarks>
    public sealed class TestCultureInfo : IDisposable
    {
        private Thread _thread;
        private CultureInfo _originalUICulture;
        private CultureInfo _originalFormatCulture;

        /// <summary>
        /// Change the UI and format culture of the calling thread to the given <paramref name="uiCulture"/>.
        /// </summary>
        /// <param name="uiCulture">The name of the culture to use.</param>
        public TestCultureInfo(string uiCulture)
            : this(uiCulture, uiCulture)
        {
        }

        /// <summary>
        /// Change the UI and format cultures of the calling thread to the given values.
        /// </summary>
        /// <param name="uiCulture">The name of the UI culture to use (resources, et. al.).</param>
        /// <param name="formatCulture">The name of the UI culture to use for formatted data display.</param>
        public TestCultureInfo(string uiCulture, string formatCulture)
        {
            _thread = Thread.CurrentThread;
            _originalUICulture = _thread.CurrentUICulture;
            _originalFormatCulture = _thread.CurrentCulture;
            if (!string.IsNullOrEmpty(uiCulture))
            {
                _thread.CurrentUICulture = CultureInfo.GetCultureInfo(uiCulture);
            }
            if (!string.IsNullOrEmpty(formatCulture))
            {
                _thread.CurrentUICulture = CultureInfo.GetCultureInfo(formatCulture);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if ((_thread != null) && _thread.IsAlive)
            {
                _thread.CurrentUICulture = _originalUICulture;
                _thread.CurrentCulture = _originalFormatCulture;
            }
            _thread = null;
        }
    }
}
