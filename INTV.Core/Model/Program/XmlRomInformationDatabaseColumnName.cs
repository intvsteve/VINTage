// <copyright file="XmlRomInformationDatabaseColumnName.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// The supported column name values for a <see cref="XmlRomInformationDatabaseColumn"/> represented as an enum.
    /// </summary>
    public enum XmlRomInformationDatabaseColumnName
    {
        /// <summary>
        /// Sentinel value representing an invalid column name.
        /// </summary>
        Invalid,

        /// <summary>
        /// Name of a column containing an unsigned 32-bit integer value representing the ROM's CRC.
        /// </summary>
        crc,

        /// <summary>
        /// Name of column containing an unsigned 32-bit integer value representing the ROM's canonical .cfg CRC32 or other half of LUIGI hash.
        /// If a column this this name has a value of zero and is a BIN format ROM, see the bin_cfg field to check for canonical CFG file.
        /// </summary>
        crc_2,

        /// <summary>
        /// Name of a column containing  the gameinfo database code, which is a string of of length [1-4].
        /// </summary>
        code,

        /// <summary>
        /// Name of a column containing the name of the ROM. If the column's value is not empty, it overrides program title defined in the gameinfo database.
        /// As a practical matter, this name should be limited to 60 characters.
        /// </summary>
        title,

        /// <summary>
        /// Name of a column containing the short name for a ROM variant. If the column's value is not empty, it overrides the short name in the gameinfo database.
        /// As a practical matter, this name should be limited to 18 characters.
        /// </summary>
        name,

        /// <summary>
        /// Name of a column containing that indicates variation number of game with same code value. This should be treated as a signed byte.
        /// </summary>
        variant,

        /// <summary>
        /// Name of a column containing the platform of the ROM. This should always be Intellivision.
        /// </summary>
        platform, // enum('Intellivision')

        /// <summary>
        /// Name of a column containing the format of the given ROM data. This should always be one of <see cref="RomFormat"/>
        /// </summary>
        format, // enum('BIN+CFG','ROM','LUIGI','')

        /// <summary>
        /// Name of a column containing  the program's role on the given platform.
        /// </summary>
        /// <remarks>This should be exposed via <see cref="GeneralFeatures"/>.</remarks>
        type, // enum('BIOS','Program') NOT NULL DEFAULT 'Program' COMMENT '',

        /// <summary>
        /// Name of a column containing origin of the data in the RomInfo entry. Note that more than one value can be specified.
        /// </summary>
        origin, // set('INTV Funhouse','Intellivision Lives','manual entry','e-mail','intvname','ROM','CFG','LUIGI','Catalog','other') NOT NULL COMMENT '',

        /// <summary>
        /// Name of a column containing short description of the program, often from a game catalog.
        /// </summary>
        description,

        /// <summary>
        /// Name of a column containing when the program became generally available; format YYYY-MM-DD.
        /// </summary>
        release_date,

        /// <summary>
        /// Name of a column containing origin of the information for the entry in the database. This is a URL (e.g. external info). If specified,
        /// overrides the value in the gameinfo database.
        /// </summary>
        source,

        /// <summary>
        /// Name of a column containing NTSC compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        ntsc,

        /// <summary>
        /// Name of a column containing PAL compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        pal,

        /// <summary>
        /// Name of a column containing <see cref="GeneralFeatures"/>; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        general_features,

        /// <summary>
        /// Name of a column containing <see cref="KeyboardComponentFeatures"/>; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        kc,

        /// <summary>
        /// Name of a column containing Sears Super Video Arcade compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        sva,

        /// <summary>
        /// Name of a column containing Intellivoice compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        ivoice,

        /// <summary>
        /// Name of a column containing Intellivision II compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        intyii,

        /// <summary>
        /// Name of a column containing <see cref="EcsFeatures"/>; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        ecs,

        /// <summary>
        /// Name of a column containing TutorVision compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        tutor,

        /// <summary>
        /// Name of a column containing <see cref="IntellicartCC3Features"/>; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        icart,

        /// <summary>
        /// Name of a column containing <see cref="CuttleCart3Features"/>; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        cc3,

        /// <summary>
        /// Name of a column containing the non-save-data portion of <see cref="JlpFeatures"/>; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// <remarks>Also see the value of the jlp_savegame value to form a complete value for <see cref="JlpFeatures"/>.</remarks>
        /// </summary>
        jlp,

        /// <summary>
        /// Name of a column containing JLP save data sectors; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// If non-negative, the low 10 bits describe the number of JLP flash data sectors the program uses.
        /// </summary>
        jlp_savegame,

        /// <summary>
        /// Name of a column containing <see cref="LtoFlashFeatures"/> and compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        /// <remarks>Also see the value of the jlp_savegame value to form a complete value for <see cref="LtoFlashFeatures"/>.</remarks>
        lto_flash,

        /// <summary>
        /// Name of a column containing <see cref="Bee3Features"/> and compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        /// <remarks>A description of any type of flash or memory card storage is not currently available.</remarks>
        bee3,

        /// <summary>
        /// Name of a column containing <see cref="HiveFeatures"/> and compatibility; non-negative indicates override gameinfo. This should be treated as a 16-bit value.
        /// </summary>
        /// <remarks>A description of any type of flash or memory card storage is not currently available.</remarks>
        hive,

        /// <summary>
        /// Name of a column containing program authors, separated by '|'.
        /// </summary>
        program,

        /// <summary>
        /// Name of a column containing program concept / design creators, separated by '|'.
        /// </summary>
        concept,

        /// <summary>
        /// Name of a column containing program graphics artists, separated by '|'.
        /// </summary>
        game_graphics,

        /// <summary>
        /// Name of a column containing program sound effects creators, separated by '|'.
        /// </summary>
        soundfx,

        /// <summary>
        /// Name of a column containing program music composers, separated by '|'.
        /// </summary>
        music,

        /// <summary>
        /// Name of a column containing voice actors, separated by '|'.
        /// </summary>
        voices,

        /// <summary>
        /// Name of a column containing program documentation authors, separated by '|'.
        /// </summary>
        game_docs,

        /// <summary>
        /// Name of a column containing packaging artists, separated by '|'.
        /// </summary>
        box_art,

        /// <summary>
        /// Name of a column containing other contributors, e.g. technical advisors, Q.A. team, separated by '|'.
        /// </summary>
        other,

        /// <summary>
        /// Name of a column containing canonical CFG file, or contents of CFG.
        /// </summary>
        /// <remarks>If the entire string value parses as a non-negative integer, cross-reference with canonical cfg_files database.</remarks>
        bin_cfg,

        /// <summary>
        /// Name of a column containing the preferred box variant image if the value is a non-negative integer. Treat as a signed 8-bit integer.
        /// </summary>
        box_variant,

        /// <summary>
        /// Name of a column containing the preferred screen shot image if the value is a non-negative integer. Treat as a signed 8-bit integer.
        /// </summary>
        screenshot,

        /// <summary>
        /// Name of a column containing a link to an animated preview of the program.
        /// </summary>
        preview,

        /// <summary>
        /// Name of a column containing a ROM download / purchase URL.
        /// </summary>
        get_rom,
    }

    /// <summary>
    /// Helper methods for <see cref="XmlRomInformationDatabaseColumnName"/>.
    /// </summary>
    public static class RomInformationDatabaseColumnNameHelpers
    {
        /// <summary>
        /// Converts a string to a <see cref="XmlRomInformationDatabaseColumnName"/>
        /// </summary>
        /// <param name="romInfoDatdabaseColumnName">The string to convert to a <see cref="XmlRomInformationDatabaseColumnName"/>.</param>
        /// <returns>A <see cref="XmlRomInformationDatabaseColumnName"/> value, <see cref="XmlRomInformationDatabaseColumnName.Invalid"/> if <paramref name="romInfoDatdabaseColumnName"/>
        /// is not a valid string representation of a <see cref="XmlRomInformationDatabaseColumnName"/>.</returns>
        public static XmlRomInformationDatabaseColumnName ToRomInfoDatabaseColumnName(this string romInfoDatdabaseColumnName)
        {
            var column = XmlRomInformationDatabaseColumnName.Invalid;
            if (string.IsNullOrEmpty(romInfoDatdabaseColumnName) || !Enum.TryParse<XmlRomInformationDatabaseColumnName>(romInfoDatdabaseColumnName, out column))
            {
                column = XmlRomInformationDatabaseColumnName.Invalid;
            }
            return column;
        }
    }
}
