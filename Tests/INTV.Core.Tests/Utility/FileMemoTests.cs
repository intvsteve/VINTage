// <copyright file="FileMemoTests.cs" company="INTV Funhouse">
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
using System.IO;
using System.Threading;
using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class FileMemoTests
    {
        [Fact]
        public void FileMemo_CheckMemoOfNonexistentFile_ReportsNoMemo()
        {
            var memos = new TestFileMemo();

            int memo = -2;
            Assert.False(memos.CheckMemo("~/nonexistent_file.txt", out memo));
            Assert.Equal(TestFileMemo.InitialMemoValue, memo);
        }

        [Fact]
        public void FileMemo_AddMemoOfValidFile_MemoAdded()
        {
            var testFileName = "~/test-file.dat";
            var file = CreateTestFile(testFileName, 123, 21);
            var memos = new TestFileMemo();

            var memo = TestFileMemo.ComputeMemoValue(file);
            Assert.True(memos.AddMemo(testFileName, memo));
            file.Dispose();
            file = null;
        }

        [Fact]
        public void FileMemo_AddMemoOfNonexistentFile_MemoNotAdded()
        {
            var testFileName = "~/not-a-valid-test-file.dat";
            var memos = new TestFileMemo();

            Assert.False(memos.AddMemo(testFileName, 42));
        }

        [Fact]
        public void FileMemo_AddMemoOfFileThatIsThenDeletedAndAddedAgain_MemoAddedThenRemoved()
        {
            var testFileName = "~/delete-this-test-file.dat";
            int memo = 42; // just use a bogus memo
            var memos = new TestFileMemo();

            using (StreamUtilities.OpenFileStream(testFileName, memos.Storage))
            {
                Assert.True(memos.AddMemo(testFileName, memo));
            }

            Assert.False(memos.AddMemo(testFileName, memo));
        }

        [Fact]
        public void FileMemo_AddValidMemoThenAddInvalidMemorForSamePath_MemoIsRemoved()
        {
            var testFileName = "~/add-valid-then-add-invalid-dat";
            int memo = 42; // just use a bogus memo
            var memos = new TestFileMemo();

            using (StreamUtilities.OpenFileStream(testFileName, memos.Storage))
            {
                Assert.True(memos.AddMemo(testFileName, memo));
                Assert.False(memos.AddMemo(testFileName, TestFileMemo.InitialMemoValue));
            }
        }

        [Fact]
        public void FileMemo_CheckMemoOfAddedFile_GetsCorrectMemo()
        {
            var testFileName = "~/add-me-check-me.dat";
            var file = CreateTestFile(testFileName, 123, 21);
            var memo = TestFileMemo.ComputeMemoValue(file);
            var memos = new TestFileMemo();

            Assert.True(memos.AddMemo(testFileName, memo));

            var storedMemo = TestFileMemo.InitialMemoValue;
            Assert.True(memos.CheckMemo(testFileName, out storedMemo));
            Assert.Equal(memo, storedMemo);
            file.Dispose();
            file = null;
        }

        [Fact]
        public void FileMemo_CheckMemoOfAddedFileAfterLastWriteTimeChanges_MemoNotFound()
        {
            var testFileName = "~/add-me-check-me-change-write-time.dat";
            var file = CreateTestFile(testFileName, 64, 12);
            var memo = TestFileMemo.ComputeMemoValue(file);
            var memos = new TestFileMemo();
            Assert.True(memos.AddMemo(testFileName, memo));

            var originalLastWriteTime = TestStorageAccess.GetLastWriteTimeUtc(testFileName);
            Thread.Sleep(1); // ugh... but tests can run fast enough to fail w/ equal timestamps
            var newLastWriteTime = DateTime.UtcNow;
            Assert.NotEqual(originalLastWriteTime, newLastWriteTime);
            TestStorageAccess.SetLastWriteTimeUtc(testFileName, newLastWriteTime);

            var returnedMemo = memo;
            Assert.False(memos.CheckMemo(testFileName, out returnedMemo));
            Assert.Equal(TestFileMemo.InitialMemoValue, returnedMemo);
            file.Dispose();
            file = null;
        }

        [Fact]
        public void FileMemo_CheckMemoOfAddedFileAfterSizeChanges_MemoNotFound()
        {
            var testFileName = "~/add-me-check-me-change-file-size.dat";
            var file = CreateTestFile(testFileName, 64, 12);
            var memo = TestFileMemo.ComputeMemoValue(file);
            var memos = new TestFileMemo();
            Assert.True(memos.AddMemo(testFileName, memo));

            var originalLastWriteTime = TestStorageAccess.GetLastWriteTimeUtc(testFileName);
            Thread.Sleep(1); // ugh... but tests can run fast enough to fail w/ equal timestamps
            var originalSize = StreamUtilities.FileSize(testFileName, memos.Storage);
            file.Seek(0, SeekOrigin.End);
            file.WriteByte(0xFF);
            Assert.NotEqual(originalSize, StreamUtilities.FileSize(testFileName, memos.Storage));
            TestStorageAccess.SetLastWriteTimeUtc(testFileName, originalLastWriteTime);
            Assert.Equal(originalLastWriteTime, TestStorageAccess.GetLastWriteTimeUtc(testFileName));

            var returnedMemo = memo;
            Assert.False(memos.CheckMemo(testFileName, out returnedMemo));
            Assert.Equal(TestFileMemo.InitialMemoValue, returnedMemo);
            file.Dispose();
            file = null;
        }

        [Fact]
        public void FileMemo_CheckMemoOfAddedFileAfterSizeAndLastWriteTimeChange_MemoNotFound()
        {
            var testFileName = "~/add-me-check-me-write-more-data-and-change-time.dat";
            var file = CreateTestFile(testFileName, 64, 12);
            var memo = TestFileMemo.ComputeMemoValue(file);
            var memos = new TestFileMemo();
            Assert.True(memos.AddMemo(testFileName, memo));

            var originalLastWriteTime = StreamUtilities.LastFileWriteTimeUtc(testFileName, memos.Storage);
            Thread.Sleep(1); // ugh... but tests can run fast enough to fail w/ equal timestamps
            var originalSize = StreamUtilities.FileSize(testFileName, memos.Storage);
            file.Seek(0, SeekOrigin.End);
            file.WriteByte(0xFF);
            Assert.NotEqual(originalSize, StreamUtilities.FileSize(testFileName, memos.Storage));
            Assert.NotEqual(originalLastWriteTime, StreamUtilities.LastFileWriteTimeUtc(testFileName, memos.Storage));

            var returnedMemo = memo;
            Assert.False(memos.CheckMemo(testFileName, out returnedMemo));
            Assert.Equal(TestFileMemo.InitialMemoValue, returnedMemo);
            file.Dispose();
            file = null;
        }

        [Fact]
        public void FileMemo_CheckMemoOfAddedFileAfterRenaming_MemoNotFound()
        {
            var testFileName = "~/add-me-check-me-rename-me.dat";
            var file = CreateTestFile(testFileName, 64, 12);
            var memo = TestFileMemo.ComputeMemoValue(file);
            var memos = new TestFileMemo();
            Assert.True(memos.AddMemo(testFileName, memo));
            var returnedMemo = memo;
            Assert.True(memos.CheckMemo(testFileName, out returnedMemo));

            var newTestFileName = "~/add-me-check-me-rename-me.dat~";
            TestStorageAccess.Rename(testFileName, newTestFileName);

            Assert.False(memos.CheckMemo(testFileName, out returnedMemo));
            Assert.Equal(TestFileMemo.InitialMemoValue, returnedMemo);
            file.Dispose();
            file = null;
        }

        [Fact]
        public void FileMemo_CheckAddMemoOfInvalidFile_DoesNotAddMemo()
        {
            var testFileName = "~/invisible-file.dat";
            var memos = new TestFileMemo();

            int memo = 99;
            Assert.False(memos.CheckAddMemo(testFileName, null, out memo));
            Assert.Equal(TestFileMemo.InitialMemoValue, memo);
            Assert.True(memos.CalledGetMemo);
        }

        [Fact]
        public void FileMemo_CheckAddMemoOfValidFile_AddsMemo()
        {
            var testFileName = "~/check-and-add-me.dat";
            var file = CreateTestFile(testFileName, 97, 5);
            var expectedMemo = TestFileMemo.ComputeMemoValue(file);
            Assert.NotEqual(TestFileMemo.InitialMemoValue, expectedMemo);
            var memos = new TestFileMemo();

            int memo = 1;
            Assert.True(memos.CheckAddMemo(testFileName, null, out memo));
            Assert.True(memos.CalledGetMemo);
            Assert.Equal(expectedMemo, memo);
            file.Dispose();
            file = null;
        }

        [Fact]
        public void FileMemo_CheckAndAddMemoAfterChangingFile_UpdatesMemo()
        {
            var testFileName = "~/check-add-change.dat";
            var file = CreateTestFile(testFileName, 69, 5);
            var expectedInitialMemo = TestFileMemo.ComputeMemoValue(file);
            Assert.NotEqual(TestFileMemo.InitialMemoValue, expectedInitialMemo);
            var memos = new TestFileMemo();

            int memo = 1;
            Assert.True(memos.CheckAddMemo(testFileName, null, out memo));
            Assert.True(memos.CalledGetMemo);
            Assert.Equal(expectedInitialMemo, memo);

            file.Seek(0, SeekOrigin.End);
            file.WriteByte(0x69);

            var updatedMemo = memo;
            Assert.True(memos.CheckAddMemo(testFileName, null, out updatedMemo));
            Assert.NotEqual(expectedInitialMemo, updatedMemo);
            file.Dispose();
            file = null;
        }

        [Fact]
        public void FileMemo_CheckAndAddMemoAfterRenamingFileThenBack_RemovesThenAddsMemo()
        {
            var testFileName = "~/check-add-change.dat";
            var file = CreateTestFile(testFileName, 99, 10);
            var expectedMemo = TestFileMemo.ComputeMemoValue(file);
            Assert.NotEqual(TestFileMemo.InitialMemoValue, expectedMemo);
            var memos = new TestFileMemo();

            int memo = 1;
            Assert.True(memos.CheckAddMemo(testFileName, null, out memo));
            Assert.True(memos.CalledGetMemo);
            Assert.Equal(expectedMemo, memo);

            var newTestFileName = "~/check-add-change.dat~";
            TestStorageAccess.Rename(testFileName, newTestFileName);

            var updatedMemo = memo;
            Assert.False(memos.CheckAddMemo(testFileName, null, out updatedMemo));
            Assert.NotEqual(expectedMemo, updatedMemo);
            Assert.Equal(TestFileMemo.InitialMemoValue, updatedMemo);

            TestStorageAccess.Rename(newTestFileName, testFileName);
            Assert.True(memos.CheckAddMemo(testFileName, null, out updatedMemo));
            Assert.Equal(expectedMemo, updatedMemo);

            file.Dispose();
            file = null;
        }

        /// <summary>
        /// Populates a fake test file with some data.
        /// </summary>
        /// <param name="location">A name (fake location) for a fake test file.</param>
        /// <param name="numBytes">Number of bytes to put into the fake file.</param>
        /// <param name="divisorForData">A divisor to use for pushing data into the file.</param>
        /// <returns>The populated stream (fake file).</returns>
        /// <remarks>The bytes written to the file follow this formula: (i mod divisorForData) AND 0xFF
        /// It is assumed that <paramref name="divisorForData"/> is greater than zero, and small enough to put
        /// interesting values into the fake file.</remarks>
        private static Stream CreateTestFile(string location, int numBytes, int divisorForData)
        {
            var stream = TestStorageAccess.OpenOrCreate(location, numBytes);
            for (int i = 0; i < numBytes; ++i)
            {
                var byteToWrite = (byte)((i % divisorForData) & 0xFF);
                stream.WriteByte(byteToWrite);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private class TestFileMemo : FileMemo<int>
        {
            public const int InitialMemoValue = 0;

            public TestFileMemo()
                : base(new TestStorageAccess())
            {
            }

            public bool CalledGetMemo { get; private set; }

            public IStorageAccess Storage
            {
                get { return StorageAccess; }
            }

            public static int ComputeMemoValue(Stream stream)
            {
                var memoValue = InitialMemoValue;
                if (stream != null)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var byteValue = stream.ReadByte();
                    while (byteValue >= 0)
                    {
                        memoValue = CombineHashCodes(memoValue, byteValue.GetHashCode());
                        byteValue = stream.ReadByte();
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return memoValue;
            }

            protected override int DefaultMemoValue
            {
                get { return InitialMemoValue; }
            }

            protected override int GetMemo(string filePath, object data)
            {
                CalledGetMemo = true;
                var stream = StreamUtilities.OpenFileStream(filePath, StorageAccess);
                var memoValue = ComputeMemoValue(stream);
                return memoValue;
            }

            protected override bool IsValidMemo(int memo)
            {
                return memo != DefaultMemoValue;
            }

            private static int CombineHashCodes(int h1, int h2)
            {
                return ((h1 << 5) + h1) ^ h2;
            }
        }
    }
}
