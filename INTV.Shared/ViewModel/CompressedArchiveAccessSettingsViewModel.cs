// <copyright file="CompressedArchiveAccessSettingsViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using INTV.Shared.ComponentModel;
using INTV.Shared.CompressedArchiveAccess;
using INTV.Shared.Model;

#if WIN
using SettingsPageVisualType = INTV.Shared.View.CompressedArchiveAccessSettings;
#elif MAC
#if __UNIFIED__
using OSColor = AppKit.NSColor;
#else
using OSColor = MonoMac.AppKit.NSColor;
#endif // __UNIFIED__
using Settings = INTV.Shared.Properties.Settings;
using SettingsPageVisualType = INTV.Shared.View.CompressedArchiveAccessSettingsPageController;
#elif GTK
using Settings = INTV.Shared.Properties.Settings;
using SettingsPageVisualType = INTV.Shared.View.CompressedArchiveAccessSettings;
#endif // WIN

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// High-level compressed archive configuration pseudo-option.
    /// </summary>
    public enum CompressedArchiveSelection
    {
        /// <summary>A value indicating that compressed archive access should be disabled.</summary>
        Disabled,

        /// <summary>A value indicating that all supported compressed archive formats should be supported.</summary>
        All,

        /// <summary>A value indicating that user-specified compressed archive formats should be supported.</summary>
        Custom
    }

    /// <summary>
    /// Implements the ViewModel for a settings page that is used to specify how the application can or will use
    /// compressed archives.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(ISettingsPage))]
    [LocalizedName(typeof(Resources.Strings), "CompressedArchiveSettingsPage_Title")]
    [Weight(0.85)]
    [Icon("Resources/Images/compressed_archive_32xLG.png")]
    public sealed class CompressedArchiveAccessSettingsViewModel : SettingsPageViewModel<SettingsPageVisualType>
    {
        #region UI Strings

        public static readonly string Disabled = Resources.Strings.CompressedArchiveAccess_Disabled;
        public static readonly string DisabledTip = Resources.Strings.CompressedArchiveAccess_Disabled_Tip;
        public static readonly string All = Resources.Strings.CompressedArchiveAccess_All;
        public static readonly string AllTip = Resources.Strings.CompressedArchiveAccess_All_Tip;
        public static readonly string Custom = Resources.Strings.CompressedArchiveAccess_Custom;
        public static readonly string CustomTip = Resources.Strings.CompressedArchiveAccess_Custom_Tip;
        public static readonly string IncludeNested = Resources.Strings.CompressedArchiveFormat_SearchNestedArchives;
        public static readonly string IncludeNestedTip = Resources.Strings.CompressedArchiveFormat_SearchNestedArchives_Tip;
        public static readonly string MaxSizeInMegabytesLabel = Resources.Strings.CompressedArchiveFormat_MaxSizeConsidered;
        public static readonly string MaxSizeInMegabytesTip = Resources.Strings.CompressedArchiveFormat_MaxSizeConsidered_Tip;

        #endregion // UI Strings

        private static readonly IDictionary<EnabledCompressedArchiveFormats, string> CompoundFormatNames = new Dictionary<EnabledCompressedArchiveFormats, string>()
        {
            { EnabledCompressedArchiveFormats.Tar | EnabledCompressedArchiveFormats.GZip, Resources.Strings.CompressedArchiveFormat_TarGzip_DisplayName },
            { EnabledCompressedArchiveFormats.Tar | EnabledCompressedArchiveFormats.BZip2, Resources.Strings.CompressedArchiveFormat_TarBzip2_DisplayName },
        };

        /// <summary>
        /// Initializes the ViewModel using data from persistent settings.
        /// </summary>
        public CompressedArchiveAccessSettingsViewModel()
        {
            EnabledFormats = Properties.Settings.Default.EnabledArchiveFormats;
            if (EnabledFormats == EnabledCompressedArchiveFormats.None)
            {
                Mode = CompressedArchiveSelection.Disabled;
            }
            else if (EnabledFormats == EnabledCompressedArchiveFormats.All)
            {
                Mode = CompressedArchiveSelection.All;
            }
            else
            {
                Mode = CompressedArchiveSelection.Custom;
            }

            _maxCompressedArchiveSize = Properties.Settings.Default.MaxArchiveSizeMB;
            _enableNestedArchives = Properties.Settings.Default.SearchNestedArchives;

            // kind of roundabout, but flexible...
            var availableFormats = EnabledCompressedArchiveFormats.All.ToCompressedArchiveFormats().Select(f => new[] { f }.FromCompressedArchiveFormats());
            CompressedArchiveFormats = new ObservableCollection<CompressedArchiveFormatViewModel>(availableFormats.Select(f => new CompressedArchiveFormatViewModel(this, f, EnabledFormats.HasFlag(f))));
            UpdateCompoundArchiveFormatsText();
        }

        /// <summary>
        /// Gets or sets the high-level compressed archive configuration mode.
        /// </summary>
        public CompressedArchiveSelection Mode
        {
            get { return _mode; }
            set { this.AssignAndUpdateProperty("Mode", value, ref _mode, (p, v) => UpdateMode(v)); }
        }
        private CompressedArchiveSelection _mode;

        /// <summary>
        /// Gets or sets the maximum supported compressed archive size (in Megabytes).
        /// </summary>
        public int MaxCompressedArchiveSize
        {
            get { return _maxCompressedArchiveSize; }
            set { this.AssignAndUpdateProperty("MaxCompressedArchiveSize", value, ref _maxCompressedArchiveSize, (p, v) => UpdateMaxArchiveSize(v)); }
        }
        private int _maxCompressedArchiveSize;

        /// <summary>
        /// Gets or sets a value indicating whether support for accessing archive files contained within other archive files is supported.
        /// </summary>
        public bool EnableNestedArchives
        {
            get { return _enableNestedArchives; }
            set { this.AssignAndUpdateProperty("EnableNestedArchives", value, ref _enableNestedArchives, (p, v) => UpdateEnableNestedArchives(v)); }
        }
        private bool _enableNestedArchives;

        /// <summary>
        /// Gets the compressed archive formats that the user can individually determine support for within the application.
        /// </summary>
        public ObservableCollection<CompressedArchiveFormatViewModel> CompressedArchiveFormats { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not custom mode selection is available to the user interface.
        /// </summary>
        public bool EnableCustomModeSelection
        {
            get { return Mode == CompressedArchiveSelection.Custom; }
        }

        /// <summary>
        /// Gets a value indicating whether or not other compressed-archive-related options should be enabled for modifications in the user interface.
        /// </summary>
        public bool EnableOtherOptions
        {
            get { return Mode != CompressedArchiveSelection.Disabled; }
        }

        /// <summary>
        /// Gets a string value that describes any compound formats that may be supported.
        /// </summary>
        public string CompoundFormats { get; private set; }

        private EnabledCompressedArchiveFormats EnabledFormats { get; set; }

        /// <summary>
        /// Updates the settings to indicate which compressed archive formats should be enabled in the application.
        /// </summary>
        internal void UpdateEnabledFormats()
        {
            UpdateEnabledFormats(CompressedArchiveFormats.Aggregate(EnabledCompressedArchiveFormats.None, (x, f) => f.Enabled ? x | f.Format : x));
        }

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
        }

        private void UpdateMode(CompressedArchiveSelection newMode)
        {
            var newFormats = EnabledFormats;
            if (newMode == CompressedArchiveSelection.Disabled)
            {
                newFormats = EnabledCompressedArchiveFormats.None;
            }
            else if (newMode == CompressedArchiveSelection.All)
            {
                newFormats = EnabledCompressedArchiveFormats.All;
            }
            UpdateEnabledFormats(newFormats);
        }

        private void UpdateEnabledFormats(EnabledCompressedArchiveFormats newFormats)
        {
            EnabledFormats = newFormats;
            Properties.Settings.Default.EnabledArchiveFormats = EnabledFormats;
            if (EnabledFormats.ToCompressedArchiveFormats().Any())
            {
                ICompressedArchiveAccessExtensions.EnableCompressedArchiveAccess();
            }
            else
            {
                ICompressedArchiveAccessExtensions.DisableCompressedArchiveAccess();
            }
            EnabledFormats.UpdateAvailableCompressedArchiveFormats();
            this.RaisePropertyChanged("EnableCustomModeSelection");
            this.RaisePropertyChanged("EnableOtherOptions");
            UpdateCompoundArchiveFormatsText();
        }

        private void UpdateMaxArchiveSize(int newMaxSize)
        {
            Properties.Settings.Default.MaxArchiveSizeMB = newMaxSize;
            ICompressedArchiveAccessExtensions.SetMaxAllowedArchiveSizeInMegabytes(newMaxSize);
        }

        private void UpdateEnableNestedArchives(bool enableNestedArchives)
        {
            Properties.Settings.Default.SearchNestedArchives = enableNestedArchives;
            if (enableNestedArchives)
            {
                ICompressedArchiveAccessExtensions.EnableNestedArchiveAccess();
            }
            else
            {
                ICompressedArchiveAccessExtensions.DisableNestedArchiveAccess();
            }
        }

        private void UpdateCompoundArchiveFormatsText()
        {
            var compoundFormats = new Dictionary<EnabledCompressedArchiveFormats, List<string>>();
            foreach (var compoundFormatFileExtension in CompressedArchiveFormatExtensions.CompoundArchiveFormats)
            {
                var fileExtension = compoundFormatFileExtension.Key;
                var format = compoundFormatFileExtension.Value.FromCompressedArchiveFormats(onlyIncludeAvailableFormats: false);
                List<string> fileExtensions;
                if (compoundFormats.TryGetValue(format, out fileExtensions))
                {
                    fileExtensions.Add(fileExtension);
                }
                else
                {
                    compoundFormats[format] = new List<string>() { fileExtension };
                }
            }

            var compoundFormatsInUse = new List<string>(Enumerable.Repeat(string.Empty, compoundFormats.Count + 1));
            var enabledFormats = Mode == CompressedArchiveSelection.All ? EnabledCompressedArchiveFormats.All.ToCompressedArchiveFormats().FromCompressedArchiveFormats() : EnabledFormats;
            var insertIndex = 1;
            foreach (var compoundFormat in compoundFormats)
            {
                if (enabledFormats.HasFlag(compoundFormat.Key))
                {
                    string displayName;
                    if (CompoundFormatNames.TryGetValue(compoundFormat.Key, out displayName))
                    {
                        var fileExtensions = string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ", compoundFormat.Value.Select(x => "*" + x));
                        compoundFormatsInUse[insertIndex++] = string.Format(CultureInfo.CurrentCulture, "  {0} ({1})", displayName, fileExtensions);
                    }
                }
            }

            if (compoundFormatsInUse.Any(f => !string.IsNullOrEmpty(f)))
            {
                compoundFormatsInUse[0] = Resources.Strings.CompressedArchiveFormat_CompoundFormatsAvailable;
            }
            CompoundFormats = string.Join("\n", compoundFormatsInUse);
            this.RaisePropertyChanged("CompoundFormats");
        }
    }
}
