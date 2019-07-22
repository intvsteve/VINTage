// <copyright file="Settings.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

// USE_CHANGESET_ENGINE is disabled because it causes this:
// GConf-WARNING **: : You can't use a GConfEngine that has an active GConfClient wrapper object. Use GConfClient API instead.
////#define USE_CHANGESET_ENGINE

using System.Linq;
using INTV.Shared.Model;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    internal sealed partial class Settings
    {
        public const string EnableMenuIconsSettingName = "EnableMenuIcons";

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
        /// <remarks>Register custom converters for point and size.
        /// It does not seem that the bindings to GConf support schemas?
        /// Haven't learned how these work, nor if the tools teased back in 2006
        /// at the repo at GitHub ever panned out. Since these are few and far
        /// between, just hand-roll things as needed.</remarks>
        protected override void AddCustomTypeConverters()
        {
            base.AddCustomTypeConverters();
            AddCustomTypeConverter(typeof(Gdk.Point), GetPoint, SetPoint);
            AddCustomTypeConverter(typeof(Gdk.Size), GetSize, SetSize);
            AddCustomTypeConverter(typeof(SearchDirectories), GetSearchDirectories, SetSearchDirectories);
        }

        private object GetPoint(string key)
        {
            int x = (int)GetSetting(key, "X");
            int y = (int)GetSetting(key, "Y");
            var point = new Gdk.Point(x, y);
            return point;
        }

        private void SetPoint(string key, object value)
        {
            var point = (Gdk.Point)value;
#if USE_CHANGESET_ENGINE
            using (var changes = new GConf.ChangeSet())
            {
                changes.Set(GetAbsoluteKey(key, "X"), point.X);
                changes.Set(GetAbsoluteKey(key, "Y"), point.Y);
                GConf.Engine.Default.CommitChangeSet(changes, false);
            }
#else
            StoreSetting(key, point.X, "X");
            StoreSetting(key, point.Y, "Y");
#endif // USE_CHANGESET_ENGINE
        }

        private object GetSize(string key)
        {
            int width = (int)GetSetting(key, "Width");
            int height = (int)GetSetting(key, "Height");
            var size = new Gdk.Size(width, height);
            return size;
        }

        private void SetSize(string key, object value)
        {
            var size = (Gdk.Size)value;
#if USE_CHANGESET_ENGINE
            using (var changes = new GConf.ChangeSet())
            {
                changes.Set(GetAbsoluteKey(key, "Width"), size.Width);
                changes.Set(GetAbsoluteKey(key, "Height"), size.Height);
                GConf.Engine.Default.CommitChangeSet(changes, false);
            }
#else
            StoreSetting(key, size.Width, "Width");
            StoreSetting(key, size.Height, "Height");
#endif // USE_CHANGESET_ENGINE
        }

        private object GetSearchDirectories(string key)
        {
            // TODO: Actually implement this! Supposedly, array of strings is supported by GConf...
            var directories = GetSetting(key) as string[];
            var searchDirectories = new SearchDirectories(directories == null ? Enumerable.Empty<string>() : directories);
            return searchDirectories;
        }

        private void SetSearchDirectories(string key, object value)
        {
            // TODO: Actually implement this! Supposedly, array of strings is supported by GConf...
            var searchDirectories = value as SearchDirectories;
            var directories = searchDirectories == null ? searchDirectories.ToArray() : new string[] { };
            StoreSetting(key, directories);
        }

        /// <summary>
        /// GTK-specific initialization.
        /// </summary>
        partial void OSInitializeDefaults()
        {
            AddSetting(WindowPositionSettingName, new Gdk.Point(80, 80), isApplicationSetting: true);
            AddSetting(WindowSizeSettingName, new Gdk.Size(1024, 768), isApplicationSetting: true);
            AddSetting(WindowStateSettingName, 0, isApplicationSetting: true);

            var defaultShowMenuIconSetting = false;
            try
            {
                defaultShowMenuIconSetting = (bool)Client.Get("/desktop/gnome/interface/menus_have_icons");
            }
            catch (GConf.NoSuchKeyException)
            {
            }
            AddSetting(EnableMenuIconsSettingName, defaultShowMenuIconSetting);

            AddSetting(RomListNameColWidthSettingName, DefaultRomListNameColWidth);
            AddSetting(RomListVendorColWidthSettingName, DefaultRomListVendorColWidth);
            AddSetting(RomListYearColWidthSettingName, DefaultRomListYearColWidth);
            AddSetting(RomListFeaturesColWidthSettingName, DefaultRomListFeaturesColWidth);
            AddSetting(RomListPathColWidthSettingName, DefaultRomListPathColWidth);
            ////AddSetting(RomListManualPathColWidthSettingName, DefaultRomListManualPathColWidth);
        }
    }
}
