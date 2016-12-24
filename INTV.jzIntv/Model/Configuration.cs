// <copyright file="Configuration.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using INTV.Core.ComponentModel;
using INTV.Core.Model;
using INTV.Core.Model.Device;

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// This class maintains configuration settings to pass to the jzIntv emulator.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IConfiguration))]
    [System.ComponentModel.Composition.ExportMetadata("FeatureName", FeatureName)]
    public class Configuration : ModelBase, IConfiguration
    {
        /// <summary>
        /// Name of the feature, for use with MEF.
        /// </summary>
        public const string FeatureName = "jzIntv";

        private const string ToolsDirectoryName = "tools";

        // UNDONE Consolidate this and the one in INTV.Shared. Perhaps put into INTV.Core?
#if WIN || WIN_XP
        private static readonly string ProgramSuffix = ".exe";
#else
        private static readonly string ProgramSuffix = string.Empty;
#endif

        private static Configuration _instance;
        private Dictionary<ProgramFile, string> _programPaths = new Dictionary<ProgramFile, string>();
        private DisplayMode _mode;
        private DisplayResolution _resolution;
        private bool _intellivoice;
        private bool _ecs;

        #region Constructors

        private Configuration()
        {
            _programPaths = new Dictionary<ProgramFile, string>();
            var toolsPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ToolsDirectoryName);
            ToolsDirectory = toolsPath;
            IRomHelpers.AddConfigurationEntry(IRomHelpers.ToolsDirectoryKey, ToolsDirectory + System.IO.Path.DirectorySeparatorChar);
            ProgramFile[] toolApps =
                {
                    INTV.JzIntv.Model.ProgramFile.Bin2Rom,
                    INTV.JzIntv.Model.ProgramFile.Rom2Bin,
                    INTV.JzIntv.Model.ProgramFile.Bin2Luigi,
                    INTV.JzIntv.Model.ProgramFile.Rom2Luigi,
                    INTV.JzIntv.Model.ProgramFile.Luigi2Bin,
                    INTV.JzIntv.Model.ProgramFile.IntvName,
                };
            foreach (var toolApp in toolApps)
            {
                SetProgramPath(toolApp, toolsPath);
            }
            _instance = this;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the single instance of the configuration object.
        /// </summary>
        internal static Configuration Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the directory in which tools are located.
        /// </summary>
        public string ToolsDirectory { get; private set; }

        /// <summary>
        /// Gets or sets the display mode to use.
        /// </summary>
        public DisplayMode DisplayMode
        {
            get { return _mode; }
            set { AssignAndUpdateProperty("DisplayMode", value, ref _mode); }
        }

        /// <summary>
        /// Gets or sets the resolution to use.
        /// </summary>
        public DisplayResolution Resolution
        {
            get { return _resolution; }
            set { AssignAndUpdateProperty("Resolution", value, ref _resolution); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable Intellivoice emulation.
        /// </summary>
        public bool EnableIntellivoice
        {
            get { return _intellivoice; }
            set { AssignAndUpdateProperty("EnableIntellivoice", value, ref _intellivoice); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable ECS emulation.
        /// </summary>
        public bool EnableEcs
        {
            get { return _ecs; }
            set { AssignAndUpdateProperty("EnableECS", value, ref _ecs); }
        }

        #region IConfiguration

        /// <inheritdoc />
        public object Settings
        {
            get { return null; }
        }

        /// <inheritdoc />
        public IEnumerable<IPeripheral> ConnectedPeripheralsHistory
        {
            get { return Enumerable.Empty<IPeripheral>(); } // this does not make sense for jzIntv
        }

        #endregion // IConfiguration

        #endregion // Properties

        /// <summary>
        /// Sets the directory that should contain the specified program.
        /// </summary>
        /// <param name="program">The program whose directory is being specified.</param>
        /// <param name="directoryContainingProgram">The directory that contains the program.</param>
        public void SetProgramPath(ProgramFile program, string directoryContainingProgram)
        {
            _programPaths[program] = directoryContainingProgram;
        }

        /// <summary>
        /// Gets the absolute path to the specified program.
        /// </summary>
        /// <param name="program">The program whose absolute path is being retrieved.</param>
        /// <returns>The absolute path of the program.</returns>
        /// <remarks>This method is not symmetric with SetProgramPath -- it returns the absolute path to the requested program in full.</remarks>
        public string GetProgramPath(ProgramFile program)
        {
            string programPath;
            if (_programPaths.TryGetValue(program, out programPath))
            {
                programPath = System.IO.Path.Combine(programPath, program.ProgramName()) + ProgramSuffix;
                IRomHelpers.AddConfigurationEntry(program.ToString(), programPath); // ensure it's registered w/ INTV.Core
            }
            return programPath;
        }

        /// <summary>
        /// Gets the application(s) to use to convert a ROM file from one format to another.
        /// </summary>
        /// <param name="rom">The ROM file to be converted.</param>
        /// <param name="targetFormat">The target format to convert the ROM to.</param>
        /// <returns>An enumerable containing the fully qualified paths to the converter programs needed to perform the conversion, as well as
        /// the output file format produced by the converter.</returns>
        /// <remarks>The converter programs are returned in the order they must execute.</remarks>
        public IEnumerable<Tuple<string, RomFormat>> GetConverterApps(IRom rom, RomFormat targetFormat)
        {
            var converterPrograms = GetConverterPrograms(rom.Format, targetFormat);
            var converterApps = converterPrograms.Select(p => new Tuple<string, RomFormat>(GetProgramPath(p), p.OutputFormat()));
            return converterApps;
        }

        private static IEnumerable<ProgramFile> GetConverterPrograms(RomFormat sourceFormat, RomFormat targetFormat)
        {
            var convertPrograms = new List<ProgramFile>();

            switch (sourceFormat)
            {
                case RomFormat.Bin:
                    switch (targetFormat)
                    {
                        case RomFormat.Bin:
                            break;
                        case RomFormat.Intellicart:
                        case RomFormat.CuttleCart3:
                        case RomFormat.CuttleCart3Advanced:
                            convertPrograms.Add(ProgramFile.Bin2Rom);
                            break;
                        case RomFormat.Luigi:
                            convertPrograms.Add(ProgramFile.Bin2Luigi);
                            break;
                    }
                    break;
                case RomFormat.Intellicart:
                case RomFormat.CuttleCart3:
                case RomFormat.CuttleCart3Advanced:
                    switch (targetFormat)
                    {
                        case RomFormat.Bin:
                            convertPrograms.Add(ProgramFile.Rom2Bin);
                            break;
                        case RomFormat.Intellicart:
                        case RomFormat.CuttleCart3:
                        case RomFormat.CuttleCart3Advanced:
                            break;
                        case RomFormat.Luigi:
                            convertPrograms.Add(ProgramFile.Rom2Luigi);
                            break;
                    }
                    break;
                case RomFormat.Luigi:
                    switch (targetFormat)
                    {
                        case RomFormat.Bin:
                            convertPrograms.Add(ProgramFile.Luigi2Bin);
                            break;
                        case RomFormat.Intellicart:
                        case RomFormat.CuttleCart3:
                        case RomFormat.CuttleCart3Advanced:
                            convertPrograms.Add(ProgramFile.Luigi2Bin);
                            convertPrograms.Add(ProgramFile.Bin2Rom);
                            break;
                        case RomFormat.Luigi:
                            break;
                    }
                    break;
            }

            return convertPrograms;
        }
    }
}
