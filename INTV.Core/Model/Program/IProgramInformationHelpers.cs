// <copyright file="IProgramInformationHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
using System.Linq;
using INTV.Core.Restricted.Model.Program;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Provides some useful functions for working with IProgramInformation.
    /// </summary>
    public static class IProgramInformationHelpers
    {
        /// <summary>
        /// Add a new CRC value to the program information.
        /// </summary>
        /// <param name="programInformation">The program information to which a new CRC is to be added.</param>
        /// <param name="newCrc">The new CRC by which the program may be identified.</param>
        /// <returns><c>true</c> if the new Crc value was added, <c>false</c> if the Crc was already present.</returns>
        /// <remarks>No description or incompatibility flags will be assigned. It is assumed the ROM is compatible with all Intellivision hardware
        /// and needs no special identification.</remarks>
        public static bool AddCrc(this IProgramInformation programInformation, uint newCrc)
        {
            return programInformation.AddCrc(newCrc, string.Empty);
        }

        /// <summary>
        /// Add a new CRC value to the program information.
        /// </summary>
        /// <param name="programInformation">The program information to which a new CRC is to be added.</param>
        /// <param name="newCrc">The new CRC by which the program may be identified.</param>
        /// <param name="crcDescription">A brief (one or two word) description, if applicable.</param>
        /// <returns><c>true</c> if the new Crc value was added, <c>false</c> if the Crc was already present.</returns>
        /// <remarks>No incompatibility flags will be assigned. It is assumed the ROM is compatible with all Intellivision hardware
        /// and needs no special identification.</remarks>
        public static bool AddCrc(this IProgramInformation programInformation, uint newCrc, string crcDescription)
        {
            return programInformation.AddCrc(newCrc, crcDescription, IncompatibilityFlags.None);
        }

        /// <summary>
        /// Modify an existing CRC value in the program information.
        /// </summary>
        /// <param name="programInformation">The program information whose CRC data is to be modified.</param>
        /// <param name="crc">The CRC whose description is to be modified.</param>
        /// <param name="newCrcDescription">The new description of the CRC. If <c>null</c> the description is left unchanged.</param>
        /// <param name="newIncompatibilityFlags">The new incompatibility flags to assign.</param>
        /// <returns><c>true</c> if the new settings were applied, <c>false</c> if the CRC was not found.</returns>
        public static bool ModifyCrc(this IProgramInformation programInformation, uint crc, string newCrcDescription, IncompatibilityFlags newIncompatibilityFlags)
        {
            CrcData crcData = programInformation.Crcs.FirstOrDefault(x => x.Crc == crc);
            bool modifiedCrc = crcData != null;
            if (modifiedCrc)
            {
                if (newCrcDescription != null)
                {
                    crcData.Description = newCrcDescription;
                }
                crcData.Incompatibilities = newIncompatibilityFlags;
            }
            return modifiedCrc;
        }

        /// <summary>
        /// Get the name of the program that includes any ROM variant information for a specific checksum.
        /// </summary>
        /// <param name="programInformation">The program information whose name is to be computed for the given CRC.</param>
        /// <param name="crc">The variant for which to get a full name.</param>
        /// <returns>The full name of the program.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if no matching CRC is found.</exception>
        public static string GetNameForCrc(this IProgramInformation programInformation, uint crc)
        {
            var name = programInformation.Title;
            var crcData = programInformation.Crcs.FirstOrDefault(crcEntry => crcEntry.Crc == crc);
            if (crcData != null)
            {
                var variantName = crcData.Description;
                if (!string.IsNullOrEmpty(variantName))
                {
                    name = string.Format("{0} ({1})", name, variantName);
                }
                return name;
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Merges the given IProgramInformation data to form a new, combined version of the information.
        /// </summary>
        /// <param name="programInformation">The "primary" source of information.</param>
        /// <param name="fieldsToMerge">Identifies which fields to merge.</param>
        /// <param name="otherSources">The other information sources to merge, and how to merge them. See Remarks.</param>
        /// <returns>The merged program information.</returns>
        /// <remarks>Each of the additional information sources in the merge will be combined with the primary. The newly
        /// merged information is initialized using the data from <paramref name="programInformation"/>. The
        /// other sources should be ordered such that the first entry is the "most important" and the final entry the
        /// "least important". Each entry also describes which fields it is allowed to override in the merge.
        /// This is to offer a means of conflict resolution in situations arising from multiple sources containing
        /// the same information and attempting to override the default. For example, consider the case in which
        /// <paramref name="programInformation"/> and two entries are provided via <paramref name="otherSources"/>,
        /// all three of which define a value for <see cref="IProgramInformation.Title"/>. Here are some ways in which
        /// this can be configured:
        /// <code>
        /// fieldsToMerge = ProgramInformationMergeFieldsFlags.None;
        /// </code>
        /// In this case, the information in <paramref name="programInformation"/> will not be changed. A copy of the
        /// data within it will be returned. Note, however, that the specific implementation of <see cref="IProgramInformation"/>
        /// used to deliver the resulting data may be different than the original!
        /// Now, consider this scenario:
        /// <code>
        /// fieldsToMerge = ProgramInformationMergeFieldsFlags.Title;
        /// otherSources[0].Item2 = ProgramInformationMergeFieldsFlags.Title;
        /// otherSources[1].Item2 = ProgramInformationMergeFieldsFlags.Title;
        /// </code>
        /// In this case, each entry allows 'Title' to be set. Because <paramref name="programInformation"/> is
        /// treated as the highest priority, its value will be retained unless it is not set (in this case, a <c>null</c>
        /// or empty string). So, if programInformation.Title has no value, but otherSources[0].Item1.Title does, then
        /// the final result contains otherSources[0].Item1.Title, regardless of the value of otherSources[1].Item1.Title.
        /// By carefully considering these flags, you can have a reasonably rich order-of-precedence defined for establishing
        /// the data in the merged result.
        /// </remarks>
        /// <exception cref="System.ArgumentException">Thrown if unrecognized flags are provided in <paramref name="fieldsToMerge"/> or via <paramref name="otherSources"/>.</exception>
        public static IProgramInformation Merge(this IProgramInformation programInformation, ProgramInformationMergeFieldsFlags fieldsToMerge, params Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>[] otherSources)
        {
            // Grr.... PCLs (at least in .NET 4.0) don't support Enum.GetValues()
            var flagsToProcess = new[] { ProgramInformationMergeFieldsFlags.Title, ProgramInformationMergeFieldsFlags.Vendor, ProgramInformationMergeFieldsFlags.Year, ProgramInformationMergeFieldsFlags.Features, ProgramInformationMergeFieldsFlags.ShortName, ProgramInformationMergeFieldsFlags.Crcs };

            var unknownFlagsForMerge = otherSources.Select(s => s.Item2).Concat(new[] { fieldsToMerge }).Except(flagsToProcess).Except(new[] { ProgramInformationMergeFieldsFlags.None, ProgramInformationMergeFieldsFlags.All });
            if (unknownFlagsForMerge.Any())
            {
                var unknownFlags = unknownFlagsForMerge.Aggregate((all, flag) => all | flag);
                throw new ArgumentException(string.Format(Resources.Strings.ProgramInformation_InvalidFieldFormat, unknownFlags));
            }

            var mergedProgramInformation = new UserSpecifiedProgramInformation(programInformation);
            var mergedPriorityFlags = fieldsToMerge; // accumulates if a "higher priority" info has already claimed a field
            foreach (var otherSource in otherSources)
            {
                var otherInformation = otherSource.Item1;
                var updateFlags = otherSource.Item2;
                foreach (var flag in flagsToProcess)
                {
                    if (updateFlags.HasFlag(flag))
                    {
                        switch (flag)
                        {
                            case ProgramInformationMergeFieldsFlags.Title:
                                if (string.IsNullOrEmpty(mergedProgramInformation.Title) || !mergedPriorityFlags.HasFlag(flag))
                                {
                                    mergedProgramInformation.Title = otherInformation.Title;
                                    if (!string.IsNullOrEmpty(mergedProgramInformation.Title))
                                    {
                                        mergedPriorityFlags |= flag;
                                    }
                                }
                                break;
                            case ProgramInformationMergeFieldsFlags.Vendor:
                                if (string.IsNullOrEmpty(mergedProgramInformation.Vendor) || !mergedPriorityFlags.HasFlag(flag))
                                {
                                    mergedProgramInformation.Vendor = otherInformation.Vendor;
                                    if (!string.IsNullOrEmpty(mergedProgramInformation.Vendor))
                                    {
                                        mergedPriorityFlags |= flag;
                                    }
                                }
                                break;
                            case ProgramInformationMergeFieldsFlags.Year:
                                if (string.IsNullOrEmpty(mergedProgramInformation.Year) || !mergedPriorityFlags.HasFlag(flag))
                                {
                                    mergedProgramInformation.Year = otherInformation.Year;
                                    if (!string.IsNullOrEmpty(mergedProgramInformation.Year))
                                    {
                                        mergedPriorityFlags |= flag;
                                    }
                                }
                                break;
                            case ProgramInformationMergeFieldsFlags.Features:
                                // This runs the risk of combining conflicting flags...
                                mergedProgramInformation.Features = ProgramFeatures.Combine(mergedProgramInformation.Features, otherInformation.Features);
                                mergedPriorityFlags |= flag;
                                break;
                            case ProgramInformationMergeFieldsFlags.ShortName:
                                if (string.IsNullOrEmpty(mergedProgramInformation.ShortName) || !mergedPriorityFlags.HasFlag(flag))
                                {
                                    mergedProgramInformation.ShortName = otherInformation.ShortName;
                                    if (!string.IsNullOrEmpty(mergedProgramInformation.ShortName))
                                    {
                                        mergedPriorityFlags |= flag;
                                    }
                                }
                                break;
                            case ProgramInformationMergeFieldsFlags.Crcs:
                                var crcsToMerge = otherInformation.Crcs.Where(o => !mergedProgramInformation.Crcs.Any(m => m.Crc == o.Crc));
                                foreach (var crc in crcsToMerge)
                                {
                                    mergedProgramInformation.AddCrc(crc.Crc, crc.Description, crc.Incompatibilities);
                                    mergedPriorityFlags |= flag;
                                }
                                break;
                        }
                    }
                }
            }
            return mergedProgramInformation;
        }

        /// <summary>
        /// Gets the INTV Funhouse database code for a program if it is available.
        /// </summary>
        /// <param name="programInformation">An instance of <see cref="IProgramInformation"/>.</param>
        /// <returns>The INTV Funhouse database code; values of <c>null</c> or empty string are to be considered invalid.</returns>
        internal static string GetDatabaseCode(this IProgramInformation programInformation)
        {
            string code = null;
            var intvFunhouseInformation = programInformation as IntvFunhouseXmlProgramInformation;
            if (intvFunhouseInformation != null)
            {
                code = intvFunhouseInformation.Code.Trim();
            }
            if (string.IsNullOrEmpty(code))
            {
                var unmergedInformation = programInformation as UnmergedProgramInformation;
                if (unmergedInformation != null)
                {
                    code = unmergedInformation.Code.Trim();
                }
            }
            return code;
        }
    }
}
