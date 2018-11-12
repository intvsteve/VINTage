// <copyright file="IRomHelpersSupport.cs" company="INTV Funhouse">
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
using System.Threading;
using INTV.Core.Model;

namespace INTV.TestHelpers.Core.Utility
{
    public static class IRomHelpersSupport
    {
        private static readonly Lazy<Dictionary<string, RomInfoData>> RomInfosLazy = new Lazy<Dictionary<string, RomInfoData>>(Initialize);

        private static Dictionary<string, RomInfoData> RomInfos
        {
            get { return RomInfosLazy.Value; }
        }

        /// <summary>
        /// Creates an enumerable containing ROM info strings.
        /// </summary>
        /// <param name="name">The name of the program.</param>
        /// <param name="copyright">The copyright date of the program.</param>
        /// <param name="shortName">The short name of the program.</param>
        /// <returns>An enumerable of strings containing the data.</returns>
        public static IEnumerable<string> CreateRomInfoStrings(string name = null, string copyright = null, string shortName = null)
        {
            var infoStrings = new List<string>();
            if (!string.IsNullOrEmpty(shortName))
            {
                infoStrings.Add(shortName);
                infoStrings.AddRange(Enumerable.Repeat(string.Empty, (int)RomInfoIndex.ShortName));
            }
            if (!string.IsNullOrEmpty(copyright))
            {
                if (infoStrings.Count >= (int)RomInfoIndex.Copyright + 1)
                {
                    infoStrings[(int)(RomInfoIndex.NumEntries - RomInfoIndex.Copyright - 1)] = copyright;
                }
                else
                {
                    infoStrings.Add(copyright);
                    infoStrings.AddRange(Enumerable.Repeat(string.Empty, (int)RomInfoIndex.Copyright));
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                if (infoStrings.Count >= (int)RomInfoIndex.Name + 1)
                {
                    infoStrings[(int)(RomInfoIndex.NumEntries - RomInfoIndex.Name - 1)] = name;
                }
                else
                {
                    infoStrings.Add(name);
                    infoStrings.AddRange(Enumerable.Repeat(string.Empty, (int)RomInfoIndex.Name));
                }
            }
            infoStrings.Reverse();
            return infoStrings;
        }

        /// <summary>
        /// Stores ROM information that will be removed from the cache when the returned <see cref="IDisposable"/> object is disposed.
        /// </summary>
        /// <param name="romPath">The path of the ROM in an <see cref="IStorageAccess"/>.</param>
        /// <param name="name">The name of the program.</param>
        /// <param name="copyright">The copyright date of the program.</param>
        /// <param name="shortName">The short name of the program.</param>
        /// <returns>An <see cref="IDisposable"/> that will remove the ROM information when disposed.</returns>
        public static IDisposable AddSelfCleaningRomInfo(string romPath, string name = null, string copyright = null, string shortName = null)
        {
            var infoStrings = CreateRomInfoStrings(name, copyright, shortName);
            return new SelfRemovingRomInfo(romPath, infoStrings);
        }

        /// <summary>
        ///  Stores ROM information.
        /// </summary>
        /// <param name="romPath">The path of the ROM in an <see cref="IStorageAccess"/>.</param>
        /// <param name="romInfo">The ROM information.</param>
        /// <returns><c>true</c> if the ROM information was newly added; false if existing information was replaced.</returns>
        public static bool AddRomInfo(string romPath, IEnumerable<string> romInfo)
        {
            lock (RomInfos)
            {
                var added = !string.IsNullOrEmpty(romPath);
                if (added)
                {
                    RomInfoData info;
                    added = !RomInfos.TryGetValue(romPath, out info);
                    if (added)
                    {
                        info = new RomInfoData(romInfo);
                    }
                    info.Using();
                    RomInfos[romPath] = info;
                }
                return added;
            }
        }

        /// <summary>
        /// Removes ROM information.
        /// </summary>
        /// <param name="romPath">The path of the ROM in an <see cref="IStorageAccess"/>.</param>
        /// <returns><c>true</c> if the information was removed; false if not (implying it is still in use).</returns>
        public static bool RemoveRomInfo(string romPath)
        {
            lock (RomInfos)
            {
                var removed = !string.IsNullOrEmpty(romPath);
                if (removed)
                {
                    RomInfoData info;
                    if (RomInfos.TryGetValue(romPath, out info))
                    {
                        removed = info.DoneUsing();
                        if (removed)
                        {
                            RomInfos.Remove(romPath);
                        }
                    }
                }
                return removed;
            }
        }

        private static Dictionary<string, RomInfoData> Initialize()
        {
            var romInfos = new Dictionary<string, RomInfoData>();
            IRomHelpers.InitializeCallbacks(GetRomInfo);
            return romInfos;
        }

        private static IEnumerable<string> GetRomInfo(IRom rom)
        {
            lock (RomInfos)
            {
                var info = Enumerable.Empty<string>();
                if (!string.IsNullOrEmpty(rom.RomPath))
                {
                    RomInfoData romInfo;
                    if (RomInfos.TryGetValue(rom.RomPath, out romInfo))
                    {
                        info = romInfo.Info;
                    }
                }
                return info;
            }
        }

        private class RomInfoData
        {
            private int _refCount;

            public RomInfoData(IEnumerable<string> info)
            {
                Info = info;
                _refCount = 0;
            }

            public IEnumerable<string> Info { get; private set; }

            public void Using()
            {
                Interlocked.Increment(ref _refCount);
            }

            public bool DoneUsing()
            {
                return Interlocked.Decrement(ref _refCount) == 0;
            }
        }

        private class SelfRemovingRomInfo : IDisposable
        {
            public SelfRemovingRomInfo(string romPath, IEnumerable<string> romInfo)
            {
                RomPath = romPath;
                AddRomInfo(romPath, romInfo);
            }

            private string RomPath { get; set; }

            public void Dispose()
            {
                RemoveRomInfo(RomPath);
            }
        }
    }
}
