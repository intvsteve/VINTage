// <copyright file="WeakKeyDictionaryTests.cs" company="INTV Funhouse">
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
using System.Text;
using System.Threading.Tasks;
using INTV.Core.Utility;
using Xunit;
using Xunit.Abstractions;

namespace INTV.Core.Tests.Utility
{
    public class WeakKeyDictionaryTests
    {
        [Fact]
        public void VerifyIsNotSynchronized()
        {
            var dictionary = new WeakKeyDictionary<DisposableTestObject, string>();
            Assert.False(dictionary.IsSynchronized);
        }

        [Fact]
        public void ValidateSyncRoot()
        {
            var dictionary = new WeakKeyDictionary<DisposableTestObject, string>();
            Assert.NotNull(dictionary.SyncRoot);
        }

        [Fact]
        public void VerifyIsNotFixedSize()
        {
            var dictionary = new WeakKeyDictionary<DisposableTestObject, string>();
            Assert.False(dictionary.IsFixedSize);
        }

        [Fact]
        public void VerifyIsNotReadOnly()
        {
            var dictionary = new WeakKeyDictionary<DisposableTestObject, string>();
            Assert.False(dictionary.IsReadOnly);
        }

        [Fact]
        public void WeakKeyDictionary_AddDisposableEntries_EnsureDisposedEntriesAreRemoved()
        {
            var strings = new[] { "First", "second" };
            var dictionary = new WeakKeyDictionary<DisposableTestObject, string>();
            foreach (var entry in strings)
            {
                Assert.True(AddShortLivedObjectToDictionary(dictionary, entry));
            }
            Assert.Equal(2, dictionary.Count);

            GC.Collect();
            Assert.Equal(0, dictionary.LiveCount);
        }

        [Fact]
        public void WeakKeyDictionary_AddEntry_TryGetValueSucceeds()
        {
            using (var entry = new DisposableTestObject("dontDupMeBro"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(entry, FortyTwo));
                Assert.True(dictionary.ContainsKey(entry));

                int value;
                Assert.True(dictionary.TryGetValue(entry, out value));
                Assert.Equal(FortyTwo, value);
            }
        }

        [Fact]
        public void WeakKeyDictionary_AddDuplicateEntry_DuplicateNotAdded()
        {
            using (var entry = new DisposableTestObject("dontDupMeBro"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(entry, FortyTwo));
                Assert.True(dictionary.ContainsKey(entry));
                Assert.False(dictionary.AddEntry(entry, -1));
            }
        }

        [Fact]
        public void WeakKeyDictionary_AddDuplicateEntry_EntryUpdated()
        {
            using (var entry = new DisposableTestObject("dontDupMeBro"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(entry, FortyTwo));
                Assert.False(dictionary.AddEntry(entry, FortyTwo + 1));
                Assert.True(dictionary.ContainsKey(entry));
                Assert.Equal(FortyTwo + 1, dictionary.GetEntry(entry));
            }
        }

        [Fact]
        public void WeakKeyDictionary_RemoveEntry_EntryRemoved()
        {
            using (var entry = new DisposableTestObject("Remove Me!"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(entry, FortyTwo));
                Assert.True(dictionary.RemoveEntry(entry));
                Assert.False(dictionary.ContainsKey(entry));
            }
        }

        [Fact]
        public void WeakKeyDictionary_TryGetRemovedValue_ValueNotRetrieved()
        {
            using (var entry = new DisposableTestObject("What? Where'd that one go?"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(entry, FortyTwo));
                Assert.True(dictionary.RemoveEntry(entry));

                int value;
                Assert.False(dictionary.TryGetValue(entry, out value));
            }
        }

        private bool AddShortLivedObjectToDictionary(WeakKeyDictionary<DisposableTestObject, string> dictionary, string name)
        {
            return dictionary.AddEntry(new DisposableTestObject(name), name);
        }

        private class DisposableTestObject : IDisposable
        {
            private bool _disposed;
            private bool _calledDispose;

            public DisposableTestObject(string name)
            {
                Name = name;
            }

            ~DisposableTestObject()
            {
                Dispose(false);
            }

            public string Name { get; private set; }

            #region IDisposable

            /// <inheritdoc />
            public void Dispose()
            {
                if (!_disposed)
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
            }

            #endregion // IDisposable

            private void Dispose(bool disposing)
            {
                _disposed = true;
                _calledDispose = disposing;
            }
        }
    }
}
