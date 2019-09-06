// <copyright file="Settings.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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

using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using INTV.Shared.Converter;
using INTV.Shared.Model;
using INTV.Shared.Utility;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [DataContract(Name = ContractName, Namespace = ContractNamespace)]
    internal sealed partial class Settings
    {
        private const string ContractName = "VINTageSharedSettings";
        private const string ContractNamespace = "https://www.intvfunhouse.com";

        private const string EnableMenuIconsSettingName = "EnableMenuIcons";

        private const int DefaultRomListNameColWidth = 192;
        private const int DefaultRomListVendorColWidth = 128;
        private const int DefaultRomListYearColWidth = 48;
        private const int DefaultRomListFeaturesColWidth = 72;
        private const int DefaultRomListPathColWidth = 160;
        ////private const int DefaultRomListManualPathColWidth = ;

        /// <inheritdoc/>
        public override object this[string key]
        {
            get
            {
                switch (key)
                {
                    case WindowPositionSettingName:
                        return WindowPosition;
                    case WindowSizeSettingName:
                        return WindowSize;
                    case RomListSearchDirectoriesSettingName:
                        return RomListSearchDirectories;
                    default:
                        return base[key];
                }
            }
            set
            {
                switch (key)
                {
                    case WindowPositionSettingName:
                        WindowPosition = (Gdk.Point)value;
                        break;
                    case WindowSizeSettingName:
                        WindowSize = (Gdk.Size)value;
                        break;
                    case RomListSearchDirectoriesSettingName:
                        RomListSearchDirectories = (SearchDirectories)value;
                        break;
                    default:
                        base[key] = value;
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public override double Weight { get; } = 0.1; // see INTV.Shared.Model.RomListConfiguration

        /// <summary>
        /// Gets or sets the setting that stores previous window position.
        /// </summary>
        public Gdk.Point WindowPosition
        {
            get { return GetSetting<Gdk.Point>(WindowPositionSettingName); }
            set { SetSetting(WindowPositionSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the setting that stores previous window size.
        /// </summary>
        public Gdk.Size WindowSize
        {
            get { return GetSetting<Gdk.Size>(WindowSizeSettingName); }
            set { SetSetting(WindowSizeSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the setting storing previous window state.
        /// </summary>
        public Gdk.WindowState WindowState
        {
            get { return (Gdk.WindowState)GetSetting<Gdk.WindowState>(WindowStateSettingName); }
            set { SetSetting(WindowStateSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable drawing menu icons.
        /// </summary>
        /// <value><c>true</c> if enable menu icons; otherwise, <c>false</c>.</value>
        /// <remarks>Even when (in testing) the following was changed, it did not seem to
        /// make a difference -- i.e. menu icons were not displayed:
        /// .../desktop/gnome/interface/menus_have_icons: true</remarks>
        public bool EnableMenuIcons
        {
            get { return GetSetting<bool>(EnableMenuIconsSettingName); }
            set { SetSetting(EnableMenuIconsSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the rom list 'Title' column.
        /// </summary>
        public int RomListNameColWidth
        {
            get { return GetSetting<int>(RomListNameColWidthSettingName); }
            set { SetSetting(RomListNameColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the rom list 'Vendor' column.
        /// </summary>
        public int RomListVendorColWidth
        {
            get { return GetSetting<int>(RomListVendorColWidthSettingName); }
            set { SetSetting(RomListVendorColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the rom list 'Year' column.
        /// </summary>
        public int RomListYearColWidth
        {
            get { return GetSetting<int>(RomListYearColWidthSettingName); }
            set { SetSetting(RomListYearColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the rom list 'Features' column.
        /// </summary>
        public int RomListFeaturesColWidth
        {
            get { return GetSetting<int>(RomListFeaturesColWidthSettingName); }
            set { SetSetting(RomListFeaturesColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the rom list 'Path' column.
        /// </summary>
        public int RomListPathColWidth
        {
            get { return GetSetting<int>(RomListPathColWidthSettingName); }
            set { SetSetting(RomListPathColWidthSettingName, value); }
        }

////        /// <summary>
////        /// Gets or sets the width of the rom list 'Manual' column.
////        /// </summary>
////        public int RomListManualPathColWidth
////        {
////            get { return GetSetting<int>(RomListManualPathColWidthSettingName); }
////            set { SetSetting(RomListManualPathColWidthSettingName, value); }
////        }

        /// <inheritdoc/>
        protected override void InitializeFromSettingsFile()
        {
            InitializeFromSettingsFile<SettingsDto>();
        }

        /// <inheritdoc/>
        protected override void AddCustomTypeConverters()
        {
            base.AddCustomTypeConverters();
            if (!TypeConverterRegistrar.TryRegisterAttribute<Gdk.Point, GdkPointConverter>())
            {
                ApplicationLogger.RecordDebugTraceMessage($"Failed to register type converter for: {typeof(Gdk.Point)}");
            }
            if (!TypeConverterRegistrar.TryRegisterAttribute<Gdk.Size, GdkSizeConverter>())
            {
                ApplicationLogger.RecordDebugTraceMessage($"Failed to register type converter for: {typeof(Gdk.Size)}");
            }
            AddCustomTypeConverter(typeof(Gdk.WindowState), TypeDescriptor.GetConverter(typeof(Gdk.WindowState)));
            AddCustomTypeConverter(typeof(Gdk.Point), TypeDescriptor.GetConverter(typeof(Gdk.Point)));
            AddCustomTypeConverter(typeof(Gdk.Size), TypeDescriptor.GetConverter(typeof(Gdk.Size)));
            AddCustomTypeConverter(typeof(SearchDirectories), TypeDescriptor.GetConverter(typeof(SearchDirectories)));
        }

        /// <summary>
        /// GTK-specific initialization.
        /// </summary>
        partial void OSInitializeDefaults()
        {
            AddSetting(WindowPositionSettingName, new Gdk.Point(80, 80), isApplicationSetting: true);
            AddSetting(WindowSizeSettingName, new Gdk.Size(1024, 768), isApplicationSetting: true);
            AddSetting(WindowStateSettingName, (Gdk.WindowState)0, isApplicationSetting: true);

            var defaultShowMenuIconSetting = false;
            //try
            //{
            //    defaultShowMenuIconSetting = (bool)Client.Get("/desktop/gnome/interface/menus_have_icons");
            //}
            //catch (GConf.NoSuchKeyException)
            //{
            //}
            AddSetting(EnableMenuIconsSettingName, defaultShowMenuIconSetting, isApplicationSetting: true);

            AddSetting(RomListNameColWidthSettingName, DefaultRomListNameColWidth, isApplicationSetting: true);
            AddSetting(RomListVendorColWidthSettingName, DefaultRomListVendorColWidth, isApplicationSetting: true);
            AddSetting(RomListYearColWidthSettingName, DefaultRomListYearColWidth, isApplicationSetting: true);
            AddSetting(RomListFeaturesColWidthSettingName, DefaultRomListFeaturesColWidth, isApplicationSetting: true);
            AddSetting(RomListPathColWidthSettingName, DefaultRomListPathColWidth, isApplicationSetting: true);
            ////AddSetting(RomListManualPathColWidthSettingName, DefaultRomListManualPathColWidth, isApplicationSetting: true);
        }

        [DataContract(Name = ContractName, Namespace = ContractNamespace)]
        private sealed class SettingsDto : IExtensibleDataObject
        {
            public ExtensionDataObject ExtensionData
            {
                get { return _extensibleDataObject; }
                set { _extensibleDataObject = value; }
            }
            private ExtensionDataObject _extensibleDataObject;

            [DataMember]
            public bool RomListValidateAtStartup { get; set; }

            [DataMember]
            public bool RomListSearchForRomsAtStartup { get; set; }

            [DataMember]
            public SearchDirectories RomListSearchDirectories { get; set; }

            [DataMember]
            public bool ShowRomDetails { get; set; }

            [DataMember]
            public bool DisplayRomFileNameForTitle { get; set; }

            [DataMember]
            public bool CheckForAppUpdatesAtLaunch { get; set; }

            [DataMember]
            public bool ShowDetailedErrors { get; set; }

            [DataMember(EmitDefaultValue = false)]
            public int MaxGZipEntriesSearch { get; set; }
        }
    }
}
