// <copyright file="UnmergedProgramInformationTable.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Data from jzIntv and Joe Zbiciak's ROM lists (and other locations).
    /// </summary>
    internal class UnmergedProgramInformationTable : ProgramInformationTable
    {
        private static readonly string Mattel = "Mattel Electronics";
        private static readonly string Activision = "Activision";
        private static readonly string JoeZbiciak = "Joe Zbiciak";

        private static readonly UnmergedProgram[] UnmergedPrograms =
        {
            new UnmergedProgram("exec", "Master Component EXECutive ROM", Mattel, "1978", new[] { 0xCBCE86F7u, 0xEA552A22u, 0xC72E969Au }, new[] { string.Empty, "Sears", "Intellivision II" }, GeneralFeatures.SystemRom),
            new UnmergedProgram("grom", "Master Component Graphics ROM", Mattel, "1978", new[] { 0x683A4158u, }, new[] { string.Empty }, GeneralFeatures.SystemRom),
            new UnmergedProgram("kc", "Keyboard Component 6502 EXEC", Mattel, "1980", new[] { 0x43792FA8u }, new[] { string.Empty }, GeneralFeatures.SystemRom, KeyboardComponentFeatures.Requires),
            new UnmergedProgram("voic", "Intellivoice ROM", Mattel, "1981", new[] { 0xDE7579Du }, new[] { string.Empty }, GeneralFeatures.SystemRom, FeatureCompatibility.Requires),
            new UnmergedProgram("ecs", "ECS BASIC ROM", Mattel, "1982", new[] { 0xEA790A06u, 0x77E856A3u, 0x5C5BA736u, 0x82299A41u }, new[] { string.Empty, string.Empty, string.Empty, string.Empty }, GeneralFeatures.SystemRom, EcsFeatures.Requires),
            new UnmergedProgram("ecsp", "ECS PATCH ROM", Mattel, "1982", new[] { 0x31327440u, 0x06441924u }, new[] { string.Empty, string.Empty }, GeneralFeatures.SystemRom, EcsFeatures.Requires),
            new UnmergedProgram("ecsm", "ECS ROM", Mattel, "1982", new[] { 0x82299A41u, 0x5C5BA736u }, new[] { string.Empty, string.Empty }, GeneralFeatures.SystemRom, EcsFeatures.Requires), // First CRC is from rom_merge of ECS.BIN with ECSPATCH.BIN ("merged" ROM). Second was found during LUI testing.
            new UnmergedProgram("icrt", "Intellicart! Menu", JoeZbiciak, "2000", new[] { 0x6DE037B6u }, new[] { string.Empty }, GeneralFeatures.SystemRom, IntellicartCC3Features.Requires),
            new UnmergedProgram("cc3", "CuttleCart3 Menu", JoeZbiciak, "2007", new[] { 0xB2A488E3u }, new[] { string.Empty }, GeneralFeatures.SystemRom, CuttleCart3Features.Requires),
            new UnmergedProgram("m201", "MTE-201 Test Cartridge", Mattel, "1981", new[] { 0xBD8A6569u, 0xC82E4D84u }, new[] { string.Empty, string.Empty }, new[] { 8, 8 }),
            new UnmergedProgram("mshm", "Astrosmash / Meteor", Mattel, "1981", new[] { 0x00BE8BBAu, 0xB9BB15EBu }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("dndc", "Adventure", Mattel, "1982", new[] { 0x11C3BCFAu, 0x36675697u }, new[] { "Advanced Dungeon & Dragons prototype", "Advanced Dungeon & Dragons prototype" }),
            new UnmergedProgram("air", "Air Strike", Mattel, "1982", new[] { 0x2C668249u, 0x677E0C65u }, new[] { "Prototype", "Prototype" }),
            ////new UnmergedProgram("blow", "Blowout", Mattel, "1982", new[] { ,  }, new[] { "Unreleased", "Unreleased" }),
            new UnmergedProgram("scad", "Space Cadet", Mattel, "1982", new[] { 0xF8EF3E5Au, 0x44F676A6u }, new[] { "Unreleased", "Unreleased" }),
            new UnmergedProgram("czyc", "Crazy Clones", Mattel, "1981", new[] { 0xE1EE408Fu, 0x6E2A2FF8u }, new[] { "Prototype", "Prototype" }),
            new UnmergedProgram("78a", "Mattel Demonstration Cartridge", Mattel, "1978", new[] { 0x0487675Cu, 0xDC9CA967u }, new[] { "Revised", "Revised" }),
            new UnmergedProgram("1982", "Mattel Demonstration Cartridge", Mattel, "1982", new[] { 0xD27E560Au, 0x20FCDC42u }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("1983", "Mattel Demonstration Cartridge", "1983", Mattel, new[] { 0xD5F038B6u, 0x16E92DFBu }, new[] { string.Empty, string.Empty }, new[] { 1, 1 }),
            new UnmergedProgram("estr", "Easter Eggs.A", Mattel, "1981", new[] { 0x20ACE89Du, 0x0EC7FE32u }, new[] { "Prototype", "Prototype" }),
            new UnmergedProgram("hypl", "Hypnotic Lights", Mattel, "1981", new[] { 0xA3147630u, 0x399E04ABu }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("gfg", "Go For The Gold", Mattel, "1983", new[] { 0x98A9D6A5u, 0xDBCA82C5u, 0x65EAF88Du }, new[] { "Unreleased", "Incomplete", "Incomplete" }, GeneralFeatures.PageFlipping),
            new UnmergedProgram("grid", "Grid Shock", Mattel, "1982", new[] { 0x291AC826u, 0x8C44929Au }, new[] { "Prototype", "Prototype" }),
            new UnmergedProgram("hlws", "Halloween Street", Mattel, "1981", new[] { 0x4830F720u, 0x7A9DC4CAu }, new[] { "Prototype", "Prototype" }),
            new UnmergedProgram("hhat", "Hard Hat", Mattel, "1982", new[] { 0xB6A3D4DEu, 0x7F5C7366u }, new[] { "Unreleased", "Unreleased" }),
            ////new UnmergedProgram("mlb2", "Baseball 2", Mattel, "1982", new[] { 0x0u, 0x0u }, new[] { "Prototype", "Prototype" }, new[] { 3, 3 }),
            new UnmergedProgram("qst", "Quest", Mattel, "1983", new[] { 0xA13F143Du, 0x97BB35B6u, 0x526C1A4Bu }, new[] { "Unfinished", "Unfinished", "Unfinished" }, GeneralFeatures.PageFlipping, FeatureCompatibility.Requires) { CrcCfgs = new[] { 1, 1, 1 } },
            new UnmergedProgram("hf3d", "Hover Force 3-D", Mattel, "1984", new[] { 0xB021ED7Bu, 0xEC2DD27Au }, new[] { "Unfinished", "Unfinished" }, new[] { 2, 2 }),
            new UnmergedProgram("mino", "Minotaur", Mattel, "1981", new[] { 0xBD731E3Cu, 0x9FA78BDAu }, new[] { "Treasure of Tarmin prototype", "Treasure of Tarmin prototype" }),
            new UnmergedProgram("mino", "Minotaur", Mattel, "1982", new[] { 0x2F9C93FCu, 0xEAF15CECu, 0x5A4CE519u }, new[] { "Treasure of Tarmin prototype", "Treasure of Tarmin prototype", "Rocks" }),
            new UnmergedProgram("snth", "Santa's Helper", Mattel, "1983", new[] { 0xE221808Cu, 0x2194B23Fu }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("shtl", "Space Shuttle", Mattel, "1983", new[] { 0xE98B9163u, 0x95335132u }, new[] { "?", "?" }, FeatureCompatibility.Requires) { CrcCfgs = new[] { 1, 1 } },
            new UnmergedProgram("smst", "Super Masters", Mattel, "1982", new[] { 0xBAB638F2u, 0x50238137u }, new[] { "Prototype", "Prototype" }),
            new UnmergedProgram("ssoc", "Super Soccer", Mattel, "1983", new[] { 0x51B82EB7u, 0x4027799Du }, new[] { "Unfinished", "Unfinished" }, EcsFeatures.Requires),
            new UnmergedProgram("klrs", "Killer Songs", string.Empty, "1981", new[] { 0x40F64CBCu, 0x48B4DD00u }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("raid", "River Raid", Activision, "1982", new[] { 0x95466AD3u, 0x9A3708B1u }, new[] { "Prototype", "Prototype" }),
            new UnmergedProgram("lol", "League of Light", Activision, "1983", new[] { 0x91ABEBACu, 0x67A45962u, 0xB4287B95u, 0x9649B06Du, 0x88C7E4AFu, 0xADCFE1AAu }, new[] { string.Empty, string.Empty, "Rocks", "Rocks", string.Empty, string.Empty }),
            new UnmergedProgram("rbtr", "Robot Rubble", Activision, "1983", new[] { 0x7473916Du, 0x478BC10Bu, 0xA5E28783u, 0x8478A56Du, 0x243B0812u, 0xD559A5E2u }, new[] { "Prototype 1", "Prototype 1", "Prototype 2", "Prototype 2", "Prototype 3", "Prototype 3" }),
            new UnmergedProgram("ddog", "Deadly Dogs!", "INTV Corporation", "1987", new[] { 0xCDC14ED8u, 0x07C5F45Du }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("grvy", "Groovy!", JoeZbiciak, "1999", new[] { 0x5F3728C6u, 0xB7F8FDA1u }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("spnw", "Spinning World", JoeZbiciak, "1999", new[] { 0xAB7B7CC2u, 0xF3B2EA49u }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("pong", "Pong", JoeZbiciak, "2000", new[] { 0xCFDB778Eu, 0x4FF2E846u }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("mzdm", "Maze Demo", JoeZbiciak, "2001", new[] { 0xF98AA25Fu, 0x49872090u }, new[] { string.Empty, string.Empty }),
            new UnmergedProgram("cmst", "C.M. Sound Test", "Carl Mueller, Jr.", "1999", new[] { 0x33AFB1FAu, 0x314780BBu }, new[] { string.Empty, string.Empty }, new[] { 3, 3 }),
            new UnmergedProgram("cstl", "Castle", "Arnauld Chevallier", "2002", new[] { 0xB84C7E9Eu, 0x71594AB3u }, new[] { "Philly Classic Trailer", "Philly Classic Trailer" }),
            new UnmergedProgram("upng", "Ultimate Pong", "Intellivision Revolution", "2016", new[] { 0xA8226426u, 0x3B37438Du }, new[] { "v1", "v1" }, EcsFeatures.Incompatible),
            new UnmergedProgram("upng", "Ultimate Pong", "Intellivision Revolution", "2016", new[] { 0x9FAC74A0u, 0x8590788Au, 0x2C020099u, 0xD868176Fu, 0x1716CC79u, 0x7F76E3CAu, 0xFDFEB70Au, 0x88AC9B98u, 0x83317B00u, 0xA4BBCD2Du }, new[] { "v2", "v2", "v3", "v3", "v4", "v4", "v5", "v5", "v6", "v6" }, EcsFeatures.Enhances),
            new UnmergedProgram("smf", "Super Mine-Field", "CollectorVision", "2016", new[] { 0x4A823F45u, 0xC1F16FA6u, 0x44F88BB2u, 0x0A6D052Du, 0x42B58218u, 0x3F746DF9u, 0x19542F38u, 0x4170603Fu }, new[] { string.Empty, string.Empty, "v1", "v1", "v2", "v2", "v3", "v3" }),
            new UnmergedProgram("spgs", "Super Pro Gosub", "Intellivision Revolution", "2016", new[] { 0x64ACF739u, 0xFC7C9B9Bu, 0x73B69904u, 0x7555CAD6u, 0x2611F934u, 0xECFFF425u }, new[] { "proto v5", "proto v5", "proto v6", "proto v6", "proto v6.1", "proto v6.1" }, EcsFeatures.Incompatible),
            ////new UnmergedProgram("isv1", "IntyBASIC Showcase Volume 1", "Intellivision Revolution", "2016", new[] { ,  }, new[] { string.Empty, string.Empty }),
            ////new UnmergedProgram("db", "Desert Bus", "Freewheeling Games", "2016", new[] { ,  }, new[] { string.Empty, string.Empty}),
        };

        private static readonly UnmergedProgramInformationTable TheInstance = new UnmergedProgramInformationTable();
        private static readonly IEnumerable<IProgramInformation> ThePrograms = UnmergedPrograms.Select(x => x.ToUnmergedProgramInformation()).Cast<IProgramInformation>();

        private UnmergedProgramInformationTable()
        {
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static UnmergedProgramInformationTable Instance
        {
            get { return TheInstance; }
        }

        /// <inheritdoc />
        public override IEnumerable<IProgramInformation> Programs
        {
            get { return ThePrograms; }
        }

        private struct UnmergedProgram
        {
            public string Code;
            public uint[] CrcData;
            public string[] CrcDescriptions;
            public int[] CrcCfgs;
            public string Year;
            public string Title;
            public ProgramFeatures Features;
            public string Vendor;

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat(0, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="crcCfgs">The default .cfg file to use if one is not provided.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, int[] crcCfgs)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = crcCfgs;
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="intellivoice">Intellivoice compatibility.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, FeatureCompatibility intellivoice)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat(0, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
                Features.Intellivoice = intellivoice;
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="ecs">ECS feature requirements.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, EcsFeatures ecs)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat(0, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
                Features.Ecs = ecs;
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="generalFeatures">Generic program features.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, GeneralFeatures generalFeatures)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat((generalFeatures == GeneralFeatures.SystemRom) ? -1 : 0, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
                Features.GeneralFeatures = generalFeatures;
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="generalFeatures">Generic program features.</param>
            /// <param name="keyboardComponent">Keyboard component features.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, GeneralFeatures generalFeatures, KeyboardComponentFeatures keyboardComponent)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat(-1, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
                Features.GeneralFeatures = generalFeatures;
                Features.KeyboardComponent = keyboardComponent;
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="generalFeatures">Generic program features.</param>
            /// <param name="intellivoice">Intellivoice compatibility.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, GeneralFeatures generalFeatures, FeatureCompatibility intellivoice)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat((generalFeatures == GeneralFeatures.SystemRom) ? -1 : 0, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
                Features.GeneralFeatures = generalFeatures;
                Features.Intellivoice = intellivoice;
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="generalFeatures">Generic program features.</param>
            /// <param name="ecs">ECS feature requirements.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, GeneralFeatures generalFeatures, EcsFeatures ecs)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat(-1, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
                Features.GeneralFeatures = generalFeatures;
                Features.Ecs = ecs;
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="generalFeatures">Generic program features.</param>
            /// <param name="intellicart">Intellicart features.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, GeneralFeatures generalFeatures, IntellicartCC3Features intellicart)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat(-1, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
                Features.GeneralFeatures = generalFeatures;
                Features.Intellicart = intellicart;
            }

            /// <summary>
            /// Creates a new instance if the UnmergedProgram structure.
            /// </summary>
            /// <param name="code">INTV Funhouse code for the ROM.</param>
            /// <param name="title">The title of the program (typically title screen title).</param>
            /// <param name="vendor">Vendor who wrote the program.</param>
            /// <param name="year">The year the program was copyrighted (or released).</param>
            /// <param name="crcData">The CRC values of known versions of the program.</param>
            /// <param name="crcDescriptions">The descriptions of the known versions of the program for each CRC.</param>
            /// <param name="generalFeatures">Generic program features.</param>
            /// <param name="cuttleCart3">Cuttle Cart 3 features.</param>
            internal UnmergedProgram(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, GeneralFeatures generalFeatures, CuttleCart3Features cuttleCart3)
            {
                Code = code;
                CrcData = crcData;
                CrcDescriptions = crcDescriptions;
                CrcCfgs = Enumerable.Repeat(-1, crcData.Count()).ToArray();
                Year = year;
                Title = title;
                Vendor = vendor;
                Features = new ProgramFeatures();
                Features.GeneralFeatures = generalFeatures;
                Features.CuttleCart3 = cuttleCart3;
            }

            /// <summary>
            /// Creates an instance of UnmergedProgramInformation using data in the raw UnmergedProgram.
            /// </summary>
            /// <returns>A new instance of UnmergedProgramInformation.</returns>
            internal UnmergedProgramInformation ToUnmergedProgramInformation()
            {
                return new UnmergedProgramInformation(Code, Title, Vendor, Year, CrcData, CrcDescriptions, CrcCfgs, Features);
            }
        }
    }
}
