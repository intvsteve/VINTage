﻿// <copyright file="ProgramInformationOrigin.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// These values are used to describe the origin of a specific IProgramInformation instance.
    /// </summary>
    public enum ProgramInformationOrigin
    {
        /// <summary>
        /// Data's origin is unknown. Typically used to indicate an error condition or a newly
        /// discovered item that will be stored in the user-defined area.
        /// </summary>
        None,

        /// <summary>
        /// Data was found in the database embedded in INTV.Core.
        /// </summary>
        Embedded,

        /// <summary>
        /// Data was found in a drop-in database update file.
        /// </summary>
        UpdateFragment,

        /// <summary>
        /// Data was found in the user's local additional programs list.
        /// </summary>
        UserDefined,

        /// <summary>
        /// Data was provided via email submission, then exposed via INTV Funhouse.
        /// </summary>
        UserEmail,

        /// <summary>
        /// Data was found from the INTV Funhouse web site / web service.
        /// </summary>
        IntvFunhouse,

        /// <summary>
        /// Data was adapted from the jzIntv source or SDK (e.g. intvname - which may mean ROM metadata).
        /// </summary>
        JzIntv,

        /// <summary>
        /// Data was adapted from information supplied by LTO.
        /// </summary>
        Lto,

        /// <summary>
        /// Data was adapted from information supplied by Elektronite.
        /// </summary>
        Elektronite,

        /// <summary>
        /// Data was adapted from information supplied by IntelligentVision.
        /// </summary>
        Intelligentvision,

        /// <summary>
        /// Data was adapted from information supplied by CollectorVision.
        /// </summary>
        CollectorVision,

        /// <summary>
        /// Data was adapted from information at the Intellivision Lives (Blue Sky Rangers / Intellivision Productions) website.
        /// </summary>
        IntellivisionProductions,

        /// <summary>
        /// Data was adapted from a game catalog.
        /// </summary>
        GameCatalog,

        /// <summary>
        /// Data was collected from a metadata block in a LUIGI-format ROM.
        /// </summary>
        LuigiMetadataBlock,

        /// <summary>
        /// Data was collected from a metadata block in a .ROM-format ROM.
        /// </summary>
        RomMetadataBlock,

        /// <summary>
        /// Data was parsed from a .cfg file associated with a .BIN-format ROM.
        /// </summary>
        CfgVarMetadataBlock,
    }
}
