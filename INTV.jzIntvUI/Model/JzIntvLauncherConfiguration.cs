// <copyright file="JzIntvLauncherConfiguration.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Core.Model.Device;

namespace INTV.JzIntvUI.Model
{
    [System.ComponentModel.Composition.Export(typeof(IConfiguration))]
    [System.ComponentModel.Composition.ExportMetadata("FeatureName", FeatureName)]
    public class JzIntvLauncherConfiguration : ModelBase, IConfiguration
    {
        /// <summary>
        /// Name of the feature, for use with MEF.
        /// </summary>
        public const string FeatureName = "jzIntvUI";

        #region Constructors

        private JzIntvLauncherConfiguration()
        {
            /*
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
            }*/
            _instance = this;
        }

        private static JzIntvLauncherConfiguration _instance;

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the single instance of the configuration object.
        /// </summary>
        internal static JzIntvLauncherConfiguration Instance
        {
            get { return _instance; }
        }

        #region IConfiguration

        /// <inheritdoc />
        public object Settings
        {
            get { return Properties.Settings.Default; }
        }

        /// <inheritdoc />
        public IEnumerable<IPeripheral> ConnectedPeripheralsHistory
        {
            get { return Enumerable.Empty<IPeripheral>(); } // does this make sense for jzIntv? E.g. if ECS enabled, include, or Intellivoice?
        }

        #endregion // IConfiguration

        #endregion // Properties
    }
}
