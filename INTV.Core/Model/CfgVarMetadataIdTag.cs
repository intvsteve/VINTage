// <copyright file="CfgVarMetadataIdTag.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;

namespace INTV.Core.Model
{
    /// <summary>
    /// Metadata ID values for 'CFGVAR' entries supported in jzIntv.
    /// </summary>
    /// <remarks>The authoritative list of these is in the jzIntv source, in
    /// doc/utilities/as1600.txt.</remarks>
    public enum CfgVarMetadataIdTag : byte
    {
        /// <summary>An invalid or unknown tag.</summary>
        Invalid,

        /// <summary>The full name of the program.</summary>
        Name,

        /// <summary>Abbreviated name of the program. Eighteen characters or less is best.</summary>
        ShortName,

        /// <summary>Name of the program's author.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        Author,

        /// <summary>Name of the artist who provided in-program artwork.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        GameArt,

        /// <summary>Name of in-program music composers, arrangers.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        Music,

        /// <summary>Name of the sound effects artist.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        SoundEffects,

        /// <summary>Name of the voice actor.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        VoiceActing,

        /// <summary>Name of the documentation author.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        Documentation,

        /// <summary>Name of the artist that did box, manual, overlay artwork.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        BoxOrOtherArtwork,

        /// <summary>Creator of the program concept.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        ConceptDesign,

        /// <summary>Free form string (URL preferred) for more information.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        MoreInfo,

        /// <summary>Name of the program's publisher.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        Publisher,

        /// <summary>License the program's code is released under.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        License,

        /// <summary>Brief description of the program.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        Description,

        /// <summary>Date program was released.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        ReleaseDate,

        /// <summary>Synonym for <see cref="CfgVarMetadataIdTag.ReleaseDate"/>.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        Year,

        /// <summary>Date program was built.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        BuildDate,

        /// <summary>Free-form descriptive version string.</summary>
        /// <remarks>This may be defined multiple times.</remarks>
        Version,

        /// <summary>Compatibility value: Compatibility level with ECS.</summary>
        EcsCompatibility,

        /// <summary>Compatibility value: Compatibility level with Intellivoice.</summary>
        IntellivoiceCompatibility,

        /// <summary>Compatibility value: Compatibility with Intellivision II.</summary>
        IntellivisionIICompatibility,

        /// <summary>Compatibility value: Compatibility with Keyboard Component.</summary>
        KeyboardComponentCompatibility,

        /// <summary>Compatibility value: Compatibility with TutorVision/INTV88</summary>
        TutorvisionCompatibility,

        /// <summary>Remapped values that are similar to EcsCompatibility.</summary>
        /// <remarks>0 = same as EcsCompatibility = 1; 1 = same as EcsCompatibility = 3</remarks>
        Ecs, 

        /// <summary>Remapped values that are similar to IntellivoiceCompatibility.</summary>
        /// <remarks>0 = same as IntellivoiceCompatibility = 1; 1 = same as IntellivoiceCompatibility = 2</remarks>
        Voice,

        /// <summary>Remapped values that are similar to IntellivisionIICompatibility.</summary>
        /// <remarks>0 = same as IntellivisionIICompatibility = 0; 1 = same as IntellivisionIICompatibility = 1</remarks>
        IntellivisionII,

        /// <summary>Indicates whether to enable the LTO Flash mapper.</summary>
        /// <remarks>0 = disable LTO mapper; 1 = enable.</remarks>
        LtoFlashMapper,

        /// <summary>Enable JLP accelerators.</summary>
        JlpAccelerators,

        /// <summary>Synonym for JlpAccelerators.</summary>
        Jlp,

        /// <summary>Specify JLP flash storage capacity in 1.5K byte blocks.</summary>
        JlpFlash,
    }

    /// <summary>
    /// Helper methods for <see cref="CfgVarMetadataIdTag"/>.
    /// </summary>
    public static class CfgVarMetadataIdTagHelpers
    {
        private static readonly List<KeyValuePair<CfgVarMetadataIdTag, string>> CfgVarIdTagNames = new List<KeyValuePair<CfgVarMetadataIdTag, string>>()
        {
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Invalid, "<--!INVALID!-->"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Name, "name"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.ShortName, "short_name"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Author, "author"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.GameArt, "game_art_by"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Music, "music_by"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.SoundEffects, "sfx_by"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.VoiceActing, "voices_by"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Documentation, "docs_by"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.BoxOrOtherArtwork, "box_art_by"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.ConceptDesign, "concept_by"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.MoreInfo, "more_info_at"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Publisher, "publisher"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.License, "license"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Description, "desc"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Description, "description"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.ReleaseDate, "release_date"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Year, "year"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.BuildDate, "build_date"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Version, "version"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.EcsCompatibility, "ecs_compat"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.IntellivoiceCompatibility, "voice_compat"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.IntellivisionIICompatibility, "intv2_compat"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.KeyboardComponentCompatibility, "kc_compat"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.TutorvisionCompatibility, "tv_compat"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Ecs,  "ecs"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Voice, "voice"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.IntellivisionII, "intv2"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.LtoFlashMapper, "lto_mapper"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.JlpAccelerators, "jlp_accel"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Jlp, "jlp"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.JlpFlash, "jlpflash"),
            new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.JlpFlash, "jlp_flash"),
        };

        /// <summary>
        /// Gets the name used in a .cfg file for the given metadata ID tag.
        /// </summary>
        /// <param name="tag">The tag to get the name of.</param>
        /// <returns>The name used in a .cfg file for the tag.</returns>
        public static string ToCfgVarString(this CfgVarMetadataIdTag tag)
        {
#if DEBUG
            // Sanity check the CfgVar table and enum.
            System.Diagnostics.Debug.Assert(!CfgVarIdTagNames.Any(e => string.IsNullOrEmpty(e.Value)), "There's an empty CFGVAR name!");
#endif
            var name = CfgVarIdTagNames.FirstOrDefault(e => e.Key == tag).Value;
            return name;
        }

        /// <summary>
        /// Gets the ID tag enumerator for a given tag.
        /// </summary>
        /// <param name="name">The name to get an ID tag for.</param>
        /// <returns>The ID tag enumeration value, or <c>CfgVarMetadataIdTag.Invalid</c> if <paramref name="name"/>
        /// is not a valid CFGVAR name as defined in as1600.txt.</returns>
        public static CfgVarMetadataIdTag ToCfgVarMetadataIdTag(this string name)
        {
            var tag = CfgVarIdTagNames.FirstOrDefault(e => e.Value == name).Key;
            return tag;
        }

        /// <summary>
        /// Converts a CFGVAR ID tag to a tuple of ROM and other metadata identifiers.
        /// </summary>
        /// <param name="tag">The CFGVAR ID to convert.</param>
        /// <param name="feature">If <paramref name="tag"/> indicates a program feature, receives which type of feature.</param>
        /// <param name="cfgVarTag">If <paramref name="tag"/> indicates a value that does not have a corresponding ROM metadata ID tag, and is not a feature.</param>
        /// <returns>The ROM metadata identifier tag.</returns>
        public static RomMetadataIdTag ToRomMetadataIdTag(this CfgVarMetadataIdTag tag, out FeatureCategory feature, out CfgVarMetadataIdTag cfgVarTag)
        {
            cfgVarTag = CfgVarMetadataIdTag.Invalid;
            feature = FeatureCategory.None;
            var romTag = RomMetadataIdTag.Ignore;
            switch (tag)
            {
                case CfgVarMetadataIdTag.Name:
                    romTag = RomMetadataIdTag.Title;
                    break;
                case CfgVarMetadataIdTag.ShortName:
                    romTag = RomMetadataIdTag.ShortTitle;
                    break;
                case CfgVarMetadataIdTag.Author:
                    cfgVarTag = tag;
                    romTag = RomMetadataIdTag.Credits;
                    break;
                case CfgVarMetadataIdTag.GameArt:
                    cfgVarTag = tag;
                    romTag = RomMetadataIdTag.Credits;
                    break;
                case CfgVarMetadataIdTag.Music:
                    cfgVarTag = tag;
                    romTag = RomMetadataIdTag.Credits;
                    break;
                case CfgVarMetadataIdTag.SoundEffects:
                    cfgVarTag = tag;
                    romTag = RomMetadataIdTag.Credits;
                    break;
                case CfgVarMetadataIdTag.VoiceActing:
                    cfgVarTag = tag;
                    romTag = RomMetadataIdTag.Credits;
                    break;
                case CfgVarMetadataIdTag.Documentation:
                    cfgVarTag = tag;
                    romTag = RomMetadataIdTag.Credits;
                    break;
                case CfgVarMetadataIdTag.BoxOrOtherArtwork:
                    cfgVarTag = tag;
                    romTag = RomMetadataIdTag.Credits;
                    break;
                case CfgVarMetadataIdTag.ConceptDesign:
                    cfgVarTag = tag;
                    romTag = RomMetadataIdTag.Credits;
                    break;
                case CfgVarMetadataIdTag.MoreInfo:
                    romTag = RomMetadataIdTag.UrlContactInfo;
                    break;
                case CfgVarMetadataIdTag.Publisher:
                    romTag = RomMetadataIdTag.Publisher;
                    break;
                case CfgVarMetadataIdTag.License:
                    romTag = RomMetadataIdTag.License;
                    break;
                case CfgVarMetadataIdTag.Description:
                    romTag = RomMetadataIdTag.Description;
                    break;
                case CfgVarMetadataIdTag.ReleaseDate:
                case CfgVarMetadataIdTag.Year:
                    romTag = RomMetadataIdTag.ReleaseDate;
                    break;
                case CfgVarMetadataIdTag.BuildDate:
                    romTag = RomMetadataIdTag.BuildDate;
                    break;
                case CfgVarMetadataIdTag.Version:
                    romTag = RomMetadataIdTag.Version;
                    break;
                case CfgVarMetadataIdTag.EcsCompatibility:
                    feature = FeatureCategory.Ecs;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.IntellivoiceCompatibility:
                    feature = FeatureCategory.Intellivoice;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.IntellivisionIICompatibility:
                    feature = FeatureCategory.IntellivisionII;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.KeyboardComponentCompatibility:
                    feature = FeatureCategory.KeyboardComponent;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.TutorvisionCompatibility:
                    feature = FeatureCategory.Tutorvision;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.Ecs:
                    feature = FeatureCategory.EcsLegacy;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.Voice:
                    feature = FeatureCategory.IntellivoiceLegacy;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.IntellivisionII:
                    feature = FeatureCategory.IntellivisionIILegacy;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.LtoFlashMapper:
                    feature = FeatureCategory.LtoFlash;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.JlpAccelerators:
                case CfgVarMetadataIdTag.Jlp:
                    feature = FeatureCategory.Jlp;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.JlpFlash:
                    feature = FeatureCategory.JlpFlashCapacity;
                    romTag = RomMetadataIdTag.Features;
                    break;
                case CfgVarMetadataIdTag.Invalid:
                    break;
                default:
                    throw new InvalidOperationException(Resources.Strings.CfgVarMetadata_ToRomError);
            }
            return romTag;
        }

        /// <summary>
        /// Converts a CFGVAR ID tag to a tuple of of LUIGI and other metadata identifiers.
        /// </summary>
        /// <param name="tag">The CFGVAR ID to convert.\.</param>
        /// <param name="feature">If <paramref name="tag"/> indicates a program feature, receives which type of feature.</param>
        /// <param name="cfgVarTag">If <paramref name="tag"/> indicates a value not directly part of LUIGI's metadata, nor its feature support.</param>
        /// <returns>The LUIGI metadata identifier tag.</returns>
        /// <remarks>All feature-related data is directly supported by the LUIGI format. Therefore, a CFG metadata tag
        /// that describes a feature will cause this function to return <see cref="LuigiMetadataIdTag.Miscellaneous"/>
        /// and set the specific feature in the <paramref name="feature"/> output. CFGVAR metadata that is not directly represented
        /// by the LUIGI format, nor its defined metadata tags, will cause this function to return <see cref="LuigiMetadataIdTag.Miscellaneous"/>
        /// and the original value of the <paramref name="tag"/> argument will be returned via <paramref name="cfgVarTag"/>.</remarks>
        public static LuigiMetadataIdTag ToLuigiMetadataIdTag(this CfgVarMetadataIdTag tag, out FeatureCategory feature, out CfgVarMetadataIdTag cfgVarTag)
        {
            cfgVarTag = CfgVarMetadataIdTag.Invalid;
            feature = FeatureCategory.None;
            var luigiTag = LuigiMetadataIdTag.Unknown;
            switch (tag)
            {
                case CfgVarMetadataIdTag.Name:
                    luigiTag = LuigiMetadataIdTag.Name;
                    break;
                case CfgVarMetadataIdTag.ShortName:
                    luigiTag = LuigiMetadataIdTag.ShortName;
                    break;
                case CfgVarMetadataIdTag.Author:
                    luigiTag = LuigiMetadataIdTag.Author;
                    break;
                case CfgVarMetadataIdTag.GameArt:
                    luigiTag = LuigiMetadataIdTag.Graphics;
                    break;
                case CfgVarMetadataIdTag.Music:
                    luigiTag = LuigiMetadataIdTag.Music;
                    break;
                case CfgVarMetadataIdTag.SoundEffects:
                    luigiTag = LuigiMetadataIdTag.SoundEffects;
                    break;
                case CfgVarMetadataIdTag.VoiceActing:
                    luigiTag = LuigiMetadataIdTag.VoiceActing;
                    break;
                case CfgVarMetadataIdTag.Documentation:
                    luigiTag = LuigiMetadataIdTag.Documentation;
                    break;
                case CfgVarMetadataIdTag.BoxOrOtherArtwork:
                    luigiTag = LuigiMetadataIdTag.BoxOrOtherArtwork;
                    break;
                case CfgVarMetadataIdTag.ConceptDesign:
                    luigiTag = LuigiMetadataIdTag.ConceptDesign;
                    break;
                case CfgVarMetadataIdTag.MoreInfo:
                    luigiTag = LuigiMetadataIdTag.UrlContactInfo;
                    break;
                case CfgVarMetadataIdTag.Publisher:
                    luigiTag = LuigiMetadataIdTag.Publisher;
                    break;
                case CfgVarMetadataIdTag.License:
                    luigiTag = LuigiMetadataIdTag.License;
                    break;
                case CfgVarMetadataIdTag.Description:
                    luigiTag = LuigiMetadataIdTag.Description;
                    break;
                case CfgVarMetadataIdTag.ReleaseDate:
                case CfgVarMetadataIdTag.Year:
                    luigiTag = LuigiMetadataIdTag.Date;
                    break;
                case CfgVarMetadataIdTag.BuildDate:
                    cfgVarTag = tag;
                    luigiTag = LuigiMetadataIdTag.Miscellaneous;
                    break;
                case CfgVarMetadataIdTag.Version:
                    cfgVarTag = tag;
                    luigiTag = LuigiMetadataIdTag.Miscellaneous;
                    break;
                case CfgVarMetadataIdTag.EcsCompatibility:
                    feature = FeatureCategory.Ecs;
                    break;
                case CfgVarMetadataIdTag.IntellivoiceCompatibility:
                    feature = FeatureCategory.Intellivoice;
                    break;
                case CfgVarMetadataIdTag.IntellivisionIICompatibility:
                    feature = FeatureCategory.IntellivisionII;
                    break;
                case CfgVarMetadataIdTag.KeyboardComponentCompatibility:
                    feature = FeatureCategory.KeyboardComponent;
                    break;
                case CfgVarMetadataIdTag.TutorvisionCompatibility:
                    feature = FeatureCategory.Tutorvision;
                    break;
                case CfgVarMetadataIdTag.Ecs:
                    feature = FeatureCategory.EcsLegacy;
                    break;
                case CfgVarMetadataIdTag.Voice:
                    feature = FeatureCategory.IntellivoiceLegacy;
                    luigiTag = LuigiMetadataIdTag.Miscellaneous;
                    break;
                case CfgVarMetadataIdTag.IntellivisionII:
                    feature = FeatureCategory.IntellivisionIILegacy;
                    luigiTag = LuigiMetadataIdTag.Miscellaneous;
                    break;
                case CfgVarMetadataIdTag.LtoFlashMapper:
                    feature = FeatureCategory.LtoFlash;
                    break;
                case CfgVarMetadataIdTag.JlpAccelerators:
                case CfgVarMetadataIdTag.Jlp:
                    feature = FeatureCategory.Jlp;
                    break;
                case CfgVarMetadataIdTag.JlpFlash:
                    feature = FeatureCategory.JlpFlashCapacity;
                    break;
                case CfgVarMetadataIdTag.Invalid:
                    break;
                default:
                    throw new InvalidOperationException(Resources.Strings.CfgVarMetadata_ToLuigiError);
            }
            return luigiTag;
        }
    }
}
