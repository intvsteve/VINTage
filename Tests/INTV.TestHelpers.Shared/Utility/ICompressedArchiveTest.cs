// <copyright file="ICompressedArchiveTest.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Threading;
using INTV.Shared.Utility;

namespace INTV.TestHelpers.Shared.Utility
{
    /// <summary>
    /// Test mixin interface for tests that work with <see cref="INTV.Shared.Utility.CompressedArchiveFormat"/>.
    /// </summary>
    public interface ICompressedArchiveTest
    {
    }

    /// <summary>
    /// The <see cref="ICompressedArchiveTest"/> mixin implementation.
    /// </summary>
    public static class CompressedArchiveFormatTestHelpers
    {
        private static int _fakeFormatOffset = 100;
        private static int _fakeImplementationOffset = 100;

        /// <summary>
        /// Gets a fake, unique compressed archive format value to use for tests.
        /// </summary>
        /// <param name="test">The test that wishes to use the mixin.</param>
        /// <returns>A unique fake compressed archive format value.</returns>
        public static CompressedArchiveFormat GetFakeCompressedArchiveFormatForTest(this ICompressedArchiveTest test)
        {
            var fakeFormat = CompressedArchiveFormat.BZip2 + Interlocked.Increment(ref _fakeFormatOffset);
            return fakeFormat;
        }

        /// <summary>
        /// Gets a fake, unique compressed archive implementation value to use for tests.
        /// </summary>
        /// <param name="test">The test that wishes to use the mixin.</param>
        /// <returns>A unique fake compressed archive implementation value.</returns>
        public static CompressedArchiveAccessImplementation GetFakeCompressedArchiveAccessImplementationForTest(this ICompressedArchiveTest test)
        {
            var fakeImplementation = CompressedArchiveAccessImplementation.Other + Interlocked.Increment(ref _fakeImplementationOffset);
            return fakeImplementation;
        }

        /// <summary>
        /// Creates fake <see cref="CompressedArchiveFormat"/> and <see cref="CompressedArchiveAccessImplementation"/> values and registers them for use.
        /// </summary>
        /// <param name="test">The test that wishes to use the mixin.</param>
        /// <param name="numberOfFileExtensionsAndImplementations">The number of file extensions and implementations to register.</param>
        /// <returns>The unique fake compressed archive format to use in a test, which has been registered for further use.</returns>
        public static CompressedArchiveFormat RegisterTestCompressedArchiveFormat(this ICompressedArchiveTest test, int numberOfFileExtensionsAndImplementations = 1)
        {
            var format = test.GetFakeCompressedArchiveFormatForTest();
            var implementations = new List<CompressedArchiveAccessImplementation>();
            var fileExtensions = new List<string>();

            for (var i = 0; i < numberOfFileExtensionsAndImplementations; ++i)
            {
                var implementation = test.GetFakeCompressedArchiveAccessImplementationForTest();
                implementations.Add(implementation);
                fileExtensions.Add(".fake" + implementation);
            }

            if (!format.RegisterCompressedArchiveFormat(fileExtensions, implementations))
            {
                throw new FailedToRegisterTestCompressedArchiveFormatException();
            }

            return format;
        }

        [Serializable]
        private class FailedToRegisterTestCompressedArchiveFormatException : InvalidOperationException
        {
            public FailedToRegisterTestCompressedArchiveFormatException()
            {
            }

            #region ISerializable

            /// <summary>
            /// Serialization constructor.
            /// </summary>
            /// <param name="info">Serialization descriptor.</param>
            /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
            protected FailedToRegisterTestCompressedArchiveFormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
                : base(info, context)
            {
            }

            #endregion // ISerializable
        }
    }
}
