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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Utility;
using Xunit;

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
        public void WeakKeyDictionary_AddDisposableEntriesAndGarbageCollect_EnsureDisposedEntriesAreRemoved()
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
        public void WeakKeyDictionary_AddObjectKeyValuePair_TryGetValueSucceeds()
        {
            var dictionary = new WeakKeyDictionary<object, object>();
            object key = "Why, it's a Key!";
            object value = new object();

            dictionary.Add(key, value);

            object valueOut;
            Assert.True(dictionary.TryGetValue("Why, it's a Key!", out valueOut));
            Assert.True(object.ReferenceEquals(value, valueOut));
        }

        [Fact]
        public void WeakKeyDictionary_AddKeyValue_TryGetValueSucceeds()
        {
            const int Value = 1;
            var key = new DisposableTestObject("key");
            var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();

            dictionary.Add(key, Value);

            int value;
            Assert.True(dictionary.TryGetValue(key, out value));
            Assert.Equal(Value, value);
        }

        [Fact]
        public void WeakKeyDictionary_AddKeyValuePair_TryGetValueSucceeds()
        {
            const int Value = 2;
            var key = new DisposableTestObject("key");
            var keeperArounder = new List<KeyValuePair<DisposableTestObject, int>>();
            var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();

            var entry = new KeyValuePair<DisposableTestObject, int>(key, Value);
            keeperArounder.Add(entry);
            dictionary.Add(entry);

            int value;
            Assert.True(dictionary.TryGetValue(key, out value));
            Assert.Equal(Value, value);
        }

        [Fact]
        public void WeakKeyDictionary_AddEntry_ContainsKeyAsObjectSucceeds()
        {
            using (var key = new DisposableTestObject("Shemp"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.True(dictionary.Contains((object)key));
            }
        }

        [Fact]
        public void WeakKeyDictionary_AddEntry_ContainsKeyValuePairSucceeds()
        {
            using (var key = new DisposableTestObject("Larry"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.True(dictionary.Contains(new KeyValuePair<DisposableTestObject, int>(key, FortyTwo)));
            }
        }

        [Fact]
        public void WeakKeyDictionary_AddEntry_ContainsKeyValuePairFails()
        {
            using (var key = new DisposableTestObject("Curly"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.False(dictionary.Contains(new KeyValuePair<DisposableTestObject, int>(key, -3)));
            }
        }

        [Fact]
        public void WeakKeyDictionary_AddEntry_TryGetValueSucceeds()
        {
            using (var key = new DisposableTestObject("Moe"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.True(dictionary.ContainsKey(key));

                int value;
                Assert.True(dictionary.TryGetValue(key, out value));
                Assert.Equal(FortyTwo, value);
            }
        }

        [Fact]
        public void WeakKeyDictionary_AddDuplicateEntry_DuplicateNotAdded()
        {
            using (var key = new DisposableTestObject("dontDupMeBro"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.True(dictionary.ContainsKey(key));
                Assert.False(dictionary.AddEntry(key, -1));
            }
        }

        [Fact]
        public void WeakKeyDictionary_AddDuplicateEntry_EntryUpdated()
        {
            using (var key = new DisposableTestObject("dontDupMeBro"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.False(dictionary.AddEntry(key, FortyTwo + 1));
                Assert.True(dictionary.ContainsKey(key));
                Assert.Equal(FortyTwo + 1, dictionary.GetEntry(key));
            }
        }

        [Fact]
        public void WeakKeyDictionary_RemoveEntry_EntryRemoved()
        {
            using (var key = new DisposableTestObject("Remove Me!"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.True(dictionary.RemoveEntry(key));
                Assert.False(dictionary.ContainsKey(key));
            }
        }

        [Fact]
        public void WeakKeyDictionary_RemoveKeyAsObject_EntryRemoved()
        {
            using (var key = new DisposableTestObject("Remove Me, too!"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                dictionary.Remove((object)key);
                Assert.False(dictionary.ContainsKey(key));
            }
        }

        [Fact]
        public void WeakKeyDictionary_RemoveKey_EntryRemoved()
        {
            using (var key = new DisposableTestObject("That's it, I'm outta here!"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.True(dictionary.Remove(key));
                Assert.False(dictionary.ContainsKey(key));
            }
        }

        [Fact]
        public void WeakKeyDictionary_RemoveKeyValuePair_EntryRemoved()
        {
            using (var key = new DisposableTestObject("That's it, we're outta here!"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.True(dictionary.Remove(new KeyValuePair<DisposableTestObject, int>(key, FortyTwo)));
                Assert.False(dictionary.ContainsKey(key));
            }
        }

        [Fact]
        public void WeakKeyDictionary_RemoveKeyValuePair_EntryNotRemoved()
        {
            using (var key = new DisposableTestObject("Just kidding!"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.False(dictionary.Remove(new KeyValuePair<DisposableTestObject, int>(key, 54)));
                Assert.True(dictionary.ContainsKey(key));
            }
        }

        [Fact]
        public void WeakKeyDictionary_TryGetRemovedValue_ValueNotRetrieved()
        {
            using (var key = new DisposableTestObject("What? Where'd that one go?"))
            {
                const int FortyTwo = 42;
                var dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
                Assert.True(dictionary.AddEntry(key, FortyTwo));
                Assert.True(dictionary.RemoveEntry(key));

                int value;
                Assert.False(dictionary.TryGetValue(key, out value));
            }
        }

        [Fact]
        public void WeakKeyDictionary_SetUsingItem_AddsItem()
        {
            var dictionary = new WeakKeyDictionary<DisposableTestObject, string>();
            var key = new DisposableTestObject("howdy");
            const string Value = "doody";
            dictionary[key] = Value;

            string value;
            Assert.True(dictionary.TryGetValue(key, out value));
        }

        [Fact]
        public void WeakKeyDictionary_GetUsingItem_GetsItem()
        {
            var dictionary = new WeakKeyDictionary<DisposableTestObject, string>();
            var key = new DisposableTestObject("howdy, pardner");
            const string Value = "how's it goin'";
            Assert.True(dictionary.AddEntry(key, Value));

            var value = dictionary[key];
            Assert.Equal(Value, value);
        }

        [Fact]
        public void WeakKeyDictionary_SetUsingItemUsingObject_AddsItem()
        {
            IDictionary dictionary = new WeakKeyDictionary<object, object>();
            object key = new DisposableTestObject("howdy");
            object value = "doody";
            dictionary[key] = value;

            Assert.Equal(value, dictionary[key]);
        }

        [Fact]
        public void WeakKeyDictionary_GetUsingItemUsingObject_GetsItem()
        {
            var dictionary = new WeakKeyDictionary<object, object>();
            var key = new DisposableTestObject("howdy, pardner");
            object value = "how's it goin'";
            Assert.True(dictionary.AddEntry(key, value));

            var valueOut = dictionary[key];
            Assert.Equal(value, valueOut);
        }

        [Fact]
        public void WeakKeyDictionary_AddKeysAndValues_EnsureKeys()
        {
            var weakKeyDictionary = new WeakKeyDictionary<string, int>();
            IDictionary dictionary = weakKeyDictionary; // for code coverage
            var keyNames = new[] { "a", "b", "seeya later, alligator!" };

            for (int i = 0; i < keyNames.Length; ++i)
            {
                weakKeyDictionary.Add(keyNames[i], i + 1);
            }

            var keys = weakKeyDictionary.Keys;
            Assert.Equal(keyNames.Length,  keys.Count);
            Assert.Equal(keyNames.Length,  dictionary.Keys.Count);
            var missingKeys = keyNames.Except(keys);
            Assert.False(missingKeys.Any());
        }

        [Fact]
        public void WeakKeyDictionary_AddKeysAndValues_EnsureValues()
        {
            var weakKeyDictionary = new WeakKeyDictionary<DisposableTestObject, int>();
            IDictionary dictionary = weakKeyDictionary; // for code coverage
            var values = new[] { -1, 48, 62, 88, -32 };

            for (int i = 0; i < values.Length; ++i)
            {
                weakKeyDictionary.Add(new DisposableTestObject(values[i].ToString()), values[i]);
            }

            var dictionaryValues = weakKeyDictionary.Values;
            Assert.Equal(values.Length,  dictionaryValues.Count);
            Assert.Equal(values.Length,  dictionary.Values.Count);
            var missingValues = values.Except(dictionaryValues);
            Assert.False(missingValues.Any());
        }

        [Fact]
        public void WeakKeyDictionaryWithData_Clear_EnsureEmpty()
        {
            const int NumValuesToAdd = 100;
            var keeperArounder = new List<string>();
            var dictionary = new WeakKeyDictionary<string, int>();
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                var key = i.ToString();
                keeperArounder.Add(key);
                dictionary.Add(key, i);
            }
            Assert.Equal(NumValuesToAdd, dictionary.Count);

            dictionary.Clear();

            Assert.Equal(0, dictionary.Count);
        }

        [Fact]
        public void WeakKeyDictionaryAsICollectionWithData_Clear_EnsureEmpty()
        {
            const int NumValuesToAdd = 100;
            ICollection<KeyValuePair<string, int>> dictionary = new WeakKeyDictionary<string, int>();
            var keeperArounder = new List<KeyValuePair<string, int>>();
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                var entry = new KeyValuePair<string, int>(i.ToString(), i);
                keeperArounder.Add(entry);
                dictionary.Add(entry);
            }
            Assert.Equal(NumValuesToAdd, dictionary.Count);

            dictionary.Clear();

            Assert.Equal(0, dictionary.Count);
        }

        [Fact]
        public void WeakKeyDictionaryWithData_ResetIDictionaryEnumerator_ThrowsNotSupportedException()
        {
            IDictionary dictionary = new WeakKeyDictionary<DisposableTestObject, int>();
            var values = new[] { -1, 48, 62, 88, -32 };

            for (int i = 0; i < values.Length; ++i)
            {
                dictionary.Add(new DisposableTestObject(values[i].ToString()), values[i]);
            }

            var dictionaryEnumerator = dictionary.GetEnumerator();
            Assert.Throws<NotSupportedException>(() => dictionaryEnumerator.Reset());
        }

        [Fact]
        public void WeakKeyDictionaryWithData_EnumerateUsingIEnuberable_ValidateContents()
        {
            var weakKeyDictionary = new WeakKeyDictionary<DisposableTestObject, int>();
            var keeperArounder = new List<DisposableTestObject>();
            var values = new[] { 69, 2009, 68, 97, 99, 2002, 2004 };

            for (int i = 0; i < values.Length; ++i)
            {
                var key = new DisposableTestObject(values[i].ToString());
                keeperArounder.Add(key);
                weakKeyDictionary.Add(key, values[i]);
            }

            IEnumerable dictionary = weakKeyDictionary;
            var index = 0;
            foreach (KeyValuePair<DisposableTestObject, int> entry in dictionary)
            {
                Assert.Equal(values[index].ToString(), entry.Key.Name);
                Assert.Equal(values[index], entry.Value);
                ++index;
            }
        }

        [Fact]
        public void WeakKeyDictionaryWithData_EnumerateUsingIDictionaryEnumerator_ValidateContents()
        {
            IDictionary dictionary = new WeakKeyDictionary<string, int>();
            var values = new[] { -1, 48, 62, 88, -32 };

            for (int i = 0; i < values.Length; ++i)
            {
                dictionary.Add(values[i].ToString(), values[i]);
            }

            var dictionaryEnumerator = dictionary.GetEnumerator();
            while (dictionaryEnumerator.MoveNext())
            {
                var current = dictionaryEnumerator.Current;
                var entry = dictionaryEnumerator.Entry;
                Assert.Equal(current, entry);
                var key = dictionaryEnumerator.Key;
                var value = dictionaryEnumerator.Value;
                var entryKey = entry.Key as string;
                var entryValue = (int)entry.Value;
                Assert.Equal(key, entryKey);
                Assert.Equal(value, entryValue);
            }
            var disposable = dictionaryEnumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                dictionaryEnumerator = null;
                disposable = null;
            }
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToNullKeyValuePairArray_ThrowsArgumentNullException()
        {
            const int NumValuesToAdd = 10;
            var keeperArounder = new List<KeyValuePair<string, int>>();
            ICollection<KeyValuePair<string, int>> dictionary = new WeakKeyDictionary<string, int>();
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                var entry = new KeyValuePair<string, int>(i.ToString(), i);
                keeperArounder.Add(entry);
                dictionary.Add(entry);
            }
            Assert.Equal(NumValuesToAdd, dictionary.Count);

            Assert.Throws<ArgumentNullException>(() => dictionary.CopyTo(null, 0));
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToNullArray_ThrowsArgumentNullException()
        {
            const int NumValuesToAdd = 10;
            var keeperArounder = new List<KeyValuePair<string, int>>();
            var weakKeyDictionary = new WeakKeyDictionary<string, int>();
            ICollection dictionary = weakKeyDictionary;
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                var entry = new KeyValuePair<string, int>(i.ToString(), i);
                keeperArounder.Add(entry);
                weakKeyDictionary.Add(entry);
            }
            Assert.Equal(NumValuesToAdd, weakKeyDictionary.Count);

            Assert.Throws<ArgumentNullException>(() => dictionary.CopyTo(null, 0));
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToKeyValuePairArrayWithNegativeIndex_ThrowsArgumentOutOfRangeException()
        {
            const int NumValuesToAdd = 6;
            ICollection<KeyValuePair<string, int>> dictionary = new WeakKeyDictionary<string, int>();
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                dictionary.Add(new KeyValuePair<string, int>(i.ToString(), i));
            }
            Assert.Equal(NumValuesToAdd, dictionary.Count);

            var destination = new KeyValuePair<string, int>[2];
            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.CopyTo(destination, -1));
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToArrayWithNegativeIndex_ThrowsArgumentOutOfRangeException()
        {
            const int NumValuesToAdd = 6;
            var weakKeyDictionary = new WeakKeyDictionary<string, int>();
            ICollection dictionary = weakKeyDictionary;
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                weakKeyDictionary.Add(new KeyValuePair<string, int>(i.ToString(), i));
            }
            Assert.Equal(NumValuesToAdd, weakKeyDictionary.Count);

            var destination = new KeyValuePair<string, int>[2];
            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.CopyTo(destination, -1));
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToKeyValuePairArrayWithIndexResultingInWritePastEndOfArray_ThrowsArgumentException()
        {
            const int NumValuesToAdd = 80;
            var keeperArounder = new List<KeyValuePair<string, int>>();
            ICollection<KeyValuePair<string, int>> dictionary = new WeakKeyDictionary<string, int>();
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                var data = new KeyValuePair<string, int>(i.ToString(), i);
                keeperArounder.Add(data);
                dictionary.Add(data);
            }
            Assert.True(NumValuesToAdd >= dictionary.Count);

            var destination = new KeyValuePair<string, int>[50];
            Assert.Throws<ArgumentException>(() => dictionary.CopyTo(destination, 48));
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToArrayWithInvalidRank_ThrowsArgumentException()
        {
            const int NumValuesToAdd = 2;
            var weakKeyDictionary = new WeakKeyDictionary<string, int>();
            ICollection dictionary = weakKeyDictionary;
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                weakKeyDictionary.Add(new KeyValuePair<string, int>(i.ToString(), i));
            }
            Assert.Equal(NumValuesToAdd, weakKeyDictionary.Count);

            var destination = new KeyValuePair<string, int>[2, 2];
            Assert.Throws<ArgumentException>(() => dictionary.CopyTo(destination, 0));
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToArrayWithInvalidArrayType_ThrowsArgumentException()
        {
            const int NumValuesToAdd = 3;
            var weakKeyDictionary = new WeakKeyDictionary<string, int>();
            ICollection dictionary = weakKeyDictionary;
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                weakKeyDictionary.Add(new KeyValuePair<string, int>(i.ToString(), i));
            }
            Assert.Equal(NumValuesToAdd, weakKeyDictionary.Count);

            var destination = new string[6];
            Assert.Throws<ArgumentException>(() => dictionary.CopyTo(destination, 0));
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToArrayWithIndexResultingInWritePastEndOfArray_ThrowsArgumentException()
        {
            const int NumValuesToAdd = 6;
            var keeperArounder = new List<KeyValuePair<string, int>>();
            var weakKeyDictionary = new WeakKeyDictionary<string, int>();
            ICollection dictionary = weakKeyDictionary;
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                var data = new KeyValuePair<string, int>(i.ToString(), i);
                keeperArounder.Add(data);
                weakKeyDictionary.Add(data);
            }
            Assert.Equal(NumValuesToAdd, weakKeyDictionary.Count);

            var destination = new KeyValuePair<string, int>[50];
            Assert.Throws<ArgumentException>(() => dictionary.CopyTo(destination, 45));
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToBeginningOfSufficientlyLargeKeyValuePairArray_Succeeds()
        {
            const int NumValuesToAdd = 4;
            ICollection<KeyValuePair<string, int>> dictionary = new WeakKeyDictionary<string, int>();
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                dictionary.Add(new KeyValuePair<string, int>(i.ToString(), i));
            }
            Assert.Equal(NumValuesToAdd, dictionary.Count);

            var destination = new KeyValuePair<string, int>[50];
            const int StartIndex = 0;
            dictionary.CopyTo(destination, StartIndex);
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToBeginningOfSufficientlyLargeArray_Succeeds()
        {
            const int NumValuesToAdd = 4;
            var weakKeyDictionary = new WeakKeyDictionary<string, int>();
            ICollection dictionary = weakKeyDictionary;
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                weakKeyDictionary.Add(new KeyValuePair<string, int>(i.ToString(), i));
            }
            Assert.Equal(NumValuesToAdd, weakKeyDictionary.Count);

            var destination = new KeyValuePair<string, int>[50];
            const int StartIndex = 0;
            dictionary.CopyTo(destination, StartIndex);
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToMiddleOfSufficientlyLargeKeyValuePairArray_Succeeds()
        {
            const int NumValuesToAdd = 12;
            var keeperArounder = new List<KeyValuePair<string, int>>();
            ICollection<KeyValuePair<string, int>> dictionary = new WeakKeyDictionary<string, int>();
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                var entry = new KeyValuePair<string, int>(i.ToString(), i);
                keeperArounder.Add(entry);
                dictionary.Add(entry);
            }
            Assert.Equal(NumValuesToAdd, dictionary.Count);

            var destination = new KeyValuePair<string, int>[50];
            const int StartIndex = 10;
            dictionary.CopyTo(destination, StartIndex);

            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                Assert.Equal(i.ToString(), destination[i + StartIndex].Key);
                Assert.Equal(i, destination[i + StartIndex].Value);
            }
        }

        [Fact]
        public void WeakKeyDictionaryWithData_CopyToEndOfSufficientlyLargeKeyValuePairArray_Succeeds()
        {
            const int NumValuesToAdd = 20;
            var keeperArounder = new List<KeyValuePair<string, int>>();
            ICollection<KeyValuePair<string, int>> dictionary = new WeakKeyDictionary<string, int>();
            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                var entry = new KeyValuePair<string, int>(i.ToString(), i);
                keeperArounder.Add(entry);
                dictionary.Add(entry);
            }
            Assert.Equal(NumValuesToAdd, dictionary.Count);

            var destination = new KeyValuePair<string, int>[50];
            const int StartIndex = 30;
            dictionary.CopyTo(destination, StartIndex);

            for (int i = 0; i < NumValuesToAdd; ++i)
            {
                Assert.Equal(i.ToString(), destination[i + StartIndex].Key);
                Assert.Equal(i, destination[i + StartIndex].Value);
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
