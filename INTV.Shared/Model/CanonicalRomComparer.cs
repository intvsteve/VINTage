// <copyright file="CanonicalRomComparer.cs" company="INTV Funhouse">
// Copyright (c) 2015-2019 All Rights Reserved
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.Shared.Utility;

namespace INTV.Shared.Model
{
    // UNDONE This comparer isn't in use. It has performance issues when used in
    // large ROM sets because of circumstances that can cause repeated regeneration
    // of LUIGI files to do the canonical compare. Also, with the base class being
    // in the INTV.Core assembly, this has the disadvantage of needing features not
    // available in INTV.Core. It could be plugged into INTV.Core at run-time, but
    // the performance concerns have precluded its adoption.

    /// <summary>
    /// Implements a comparer based on the canonical ROM format. This requires only that the two ROM binaries are equal.
    /// There may be differences in their features (e.g. compatibility with other hardware may differ). This can result in
    /// equivalencies even when two copies of the the same .bin have been paired with two DIFFERENT .cfg files. For example,
    /// if The Dreadnaught Factor's .bin has been patched to work on PAL systems with a special .cfg in one case, but not
    /// patched in the other, this comparison would report equality, even though the two resulting LUIGI (or, in fact, ROM)
    /// files behave very differently in hardware. (One works on PAL systems, the other does not.)
    /// </summary>
    public class CanonicalRomComparer : RomComparer
    {
        /// <summary>
        /// The default comparer instance.
        /// </summary>
        public static readonly CanonicalRomComparer Default = new CanonicalRomComparer();

        private const RomFormat CanonicalRomFormat = RomFormat.Luigi;
        private static readonly RomComparer BaseComparer = new RomComparerStrictCrcOnly();

        #region Constructor

        /// <summary>
        /// Initializes a new instance of CanonicalRomComparer.
        /// </summary>
        public CanonicalRomComparer()
        {
            TemporaryRoms = new Dictionary<StorageLocation, IRom>();
            TemporaryCanonicalRoms = new Dictionary<StorageLocation, IRom>();
        }

        #endregion // Constructor

        private Dictionary<StorageLocation, IRom> TemporaryRoms { get; set; } // key is original ROM location, value is temporary ROM
        private Dictionary<StorageLocation, IRom> TemporaryCanonicalRoms { get; set; } // key is original ROM location, value is temporary canonical ROM

        /// <summary>
        /// Clears any cached data used for comparing ROMs. This is an attempt at improving performance.
        /// </summary>
        public static void ClearCache()
        {
            Default.ClearTemporaryItems();
        }

        #region RomComparer

        /// <inheritdoc />
        public override int Compare(IRom x, IProgramInformation programInformationX, IRom y, IProgramInformation programInformationY)
        {
            var result = BaseComparer.Compare(x, programInformationX, y, programInformationY);
            if (result != 0)
            {
                result = CanonicalCompare(x, y, false);
            }
            return result;
        }

        #endregion // RomComparer

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            ClearTemporaryItems();
        }

        #endregion // IDisposable

        protected int CanonicalCompare(IRom rom, IRom otherRom, bool includeFeatures)
        {
            // Always do the compare based on canonical form. Perhaps we could optimize for cases when 'rom' and 'otherRom'
            // are both already the same format... This could save considerable time in cases such as adding ROMs from a CD or
            // thumb drive, in which case temporary local copies must be made, in addition to possible temporary LUIGI copies.
            var result = -1;
            try
            {
                IRom canonicalReferenceRom = rom;
                var createdCanonicalReferenceRom = false;
                if ((rom.Format != CanonicalRomFormat) && !TemporaryCanonicalRoms.TryGetValue(rom.RomPath, out canonicalReferenceRom))
                {
                    // rom isn't in canonical format, and we haven't already cached a canonical form of it -- so create one
                    var isTemporary = false;
                    createdCanonicalReferenceRom = CanonicalRomNeedsUpdate(rom, out canonicalReferenceRom, out isTemporary); // will return existing if possible
                    if ((canonicalReferenceRom == null) || isTemporary)
                    {
                        canonicalReferenceRom = GetCanonicalRomForCompare(rom, isTemporary);
                        if (isTemporary)
                        {
                            TemporaryCanonicalRoms[rom.RomPath] = canonicalReferenceRom;
                        }
                    }
                }

                IRom canonicalOtherRom = otherRom;
                if ((otherRom.Format != CanonicalRomFormat) && !TemporaryCanonicalRoms.TryGetValue(otherRom.RomPath, out canonicalOtherRom))
                {
                    // otherRom isn't in canonical format, and we haven't already cached a canonical form of it -- so create one
                    var isTemporary = false;
                    CanonicalRomNeedsUpdate(otherRom, out canonicalOtherRom, out isTemporary);
                    isTemporary |= createdCanonicalReferenceRom;
                    if ((canonicalOtherRom == null) || isTemporary)
                    {
                        canonicalOtherRom = GetCanonicalRomForCompare(otherRom, isTemporary);
                        if (isTemporary)
                        {
                            TemporaryCanonicalRoms[otherRom.RomPath] = canonicalOtherRom;
                        }
                    }
                }

                result = includeFeatures ? RomComparerStrict.Default.Compare(canonicalReferenceRom, canonicalOtherRom) : RomComparerStrictCrcOnly.Default.Compare(canonicalReferenceRom, canonicalOtherRom);
                if ((result != 0) && (canonicalReferenceRom != null) && (canonicalOtherRom != null))
                {
                    // ROMs are not considered equal based on the more simplistic, fast check. Do the full canonical check.
                    var doCrc = true;
                    if (includeFeatures)
                    {
                        var referenceFeatures = (ulong)Rom.GetLuigiHeader(canonicalReferenceRom).Features;
                        var otherFeatures = (ulong)Rom.GetLuigiHeader(canonicalOtherRom).Features;
                        var featureDifferences = (referenceFeatures ^ otherFeatures) & ~(ulong)LuigiFeatureFlags.FeatureFlagsExplicitlySet;
                        if (featureDifferences == 0)
                        {
                            referenceFeatures = (ulong)Rom.GetLuigiHeader(canonicalReferenceRom).Features2;
                            otherFeatures = (ulong)Rom.GetLuigiHeader(canonicalOtherRom).Features2;
                            featureDifferences = referenceFeatures ^ otherFeatures;
                        }
                        doCrc = featureDifferences == 0;
                        if (!doCrc)
                        {
                            result = -1;
                        }
                    }
                    if (doCrc)
                    {
                        // always skip including features in the CRC because of the 'explicitly set' bit.
                        var referenceCrc = INTV.Core.Utility.Crc32.OfFile(canonicalReferenceRom.RomPath, Rom.GetComparisonIgnoreRanges(canonicalReferenceRom, true));
                        var otherCrc = INTV.Core.Utility.Crc32.OfFile(canonicalOtherRom.RomPath, Rom.GetComparisonIgnoreRanges(canonicalOtherRom, true));
                        result = (int)referenceCrc - (int)otherCrc;
                    }
                }
            }
            catch (System.InvalidOperationException)
            {
            }
            return result;
        }

        private IRom GetCanonicalRomForCompare(IRom rom, bool useTemporaryForCompare)
        {
            return GetLuigRomForCompare(rom, useTemporaryForCompare);
        }

        private bool CanonicalRomNeedsUpdate(IRom rom, out IRom canonicalRom, out bool treatAsTemporary)
        {
            return LuigiNeedsUpdate(rom, out canonicalRom, out treatAsTemporary);
        }

        private IRom GetLuigRomForCompare(IRom rom, bool temporaryCopy)
        {
            string temporaryRomPath = null;
            IRom tempRom = null;
            if (temporaryCopy && !TemporaryRoms.TryGetValue(rom.RomPath, out tempRom))
            {
                temporaryRomPath = rom.RomPath.Path.EnsureUniqueFileName();
            }
            if (tempRom == null)
            {
                tempRom = (temporaryCopy || rom.RomPath.Path.IsPathOnRemovableDevice()) ? rom.CopyToLocalRomsDirectory(temporaryRomPath) : rom;
            }
            if (temporaryCopy)
            {
                TemporaryRoms[rom.RomPath] = tempRom;
            }
            var luigiPath = tempRom.RomPath.ChangeExtension(RomFormat.Luigi.FileExtension());
            var jzIntvConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.JzIntv.Model.Configuration>();
            var converterApp = jzIntvConfiguration.GetConverterApps(tempRom, RomFormat.Luigi).First(); // Convert to LUIGI
            var result = INTV.Shared.Utility.RunExternalProgram.Call(converterApp.Item1, "\"" + tempRom.RomPath + "\"", RomListConfiguration.Instance.RomsDirectory);
            if ((result != 0) || !luigiPath.Exists())
            {
                var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.LuigiConversionError_Format, rom.RomPath, result);
                throw new System.InvalidOperationException(message);
            }
            return INTV.Core.Model.Rom.Create(luigiPath, StorageLocation.InvalidLocation);
        }

        private bool LuigiNeedsUpdate(IRom rom, out IRom luigiRom, out bool treatAsTemporary)
        {
            luigiRom = null;
            treatAsTemporary = false;
            System.Diagnostics.Debug.Assert(rom.Format != RomFormat.Luigi, "Only non-LUIGI format ROMs should be checked!");
            var romPath = rom.RomPath;
            var luigiPath = romPath.ChangeExtension(RomFormat.Luigi.FileExtension());
            if (romPath.Path.IsPathOnRemovableDevice())
            {
                // look in app's local ROMs directory for a LUIGI file
                var localRomsDirectory = RomListConfiguration.Instance.RomsDirectory;
                luigiPath = luigiPath.AlterContainingLocation(localRomsDirectory);
                treatAsTemporary = luigiPath.Exists();
                if (treatAsTemporary)
                {
                    luigiPath = luigiPath.EnsureUnique();
                }
            }
            var needsUpdate = !luigiPath.Exists();
            if (!needsUpdate)
            {
                var luigiHeader = LuigiFileHeader.GetHeader(luigiPath);
                if (luigiHeader != null)
                {
                    needsUpdate = (rom.Format != luigiHeader.OriginalRomFormat) || (rom.Crc != luigiHeader.OriginalRomCrc32) || (rom.CfgCrc != luigiHeader.OriginalCfgCrc32);
                }
                else
                {
                    needsUpdate = true;
                }
            }
            return needsUpdate;
        }

        private void ClearTemporaryItems()
        {
            foreach (var temp in TemporaryRoms)
            {
                var tempRom = temp.Value;
                if (tempRom.RomPath.Exists())
                {
                    tempRom.RomPath.Path.ClearReadOnlyAttribute();
                    FileUtilities.DeleteFile(tempRom.RomPath.Path, false, 4);
                }
                if (tempRom.ConfigPath.Exists())
                {
                    tempRom.ConfigPath.Path.ClearReadOnlyAttribute();
                    FileUtilities.DeleteFile(tempRom.ConfigPath.Path, false, 4);
                }
            }
            TemporaryRoms.Clear();
            foreach (var temp in TemporaryCanonicalRoms)
            {
                var tempRom = temp.Value;
                if (tempRom.RomPath.Exists())
                {
                    tempRom.RomPath.Path.ClearReadOnlyAttribute();
                    FileUtilities.DeleteFile(tempRom.RomPath.Path, false, 4);
                }
                if (tempRom.ConfigPath.Exists())
                {
                    tempRom.ConfigPath.Path.ClearReadOnlyAttribute();
                    FileUtilities.DeleteFile(tempRom.ConfigPath.Path, false, 4);
                }
            }
            TemporaryCanonicalRoms.Clear();
        }
    }
}
