// <copyright file="JzIntvSettingsPageController.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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
using INTV.Shared.Utility;
using INTV.JzIntv.Model;
using INTV.Shared.ComponentModel;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.JzIntvUI.Model;
using INTV.JzIntvUI.ViewModel;
using INTV.Shared.View;
using INTV.JzIntvUI.Commands;

namespace INTV.JzIntvUI.View
{
    public partial class JzIntvSettingsPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public JzIntvSettingsPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public JzIntvSettingsPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public JzIntvSettingsPageController()
            : base("JzIntvSettingsPage", NSBundle.MainBundle)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the view as a strongly typed value.
        /// </summary>
        public new JzIntvSettingsPage View
        {
            get { return (JzIntvSettingsPage)base.View; }
        }

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        #endregion IFakeDependencyObject

        /// <summary>
        /// Gets or sets the selected resolution.
        /// </summary>
        [INTV.Shared.Utility.OSExport("SelectedResolution")]
        public NSNumber SelectedResolution
        {
            get
            {
                if (!_initializedDisplayResolutions)
                {
                    var selectedResolutionViewModel = ViewModel.SelectedDisplayResolutionViewModel;
                    var selectedResolution = selectedResolutionViewModel == null ? JzIntvSettingsPageViewModel.DefaultResolution : selectedResolutionViewModel.Resolution;
                    var selection = ViewModel.AvailableDisplayResolutions.First(b => b.Resolution == selectedResolution);
                    var selectionIndex = ViewModel.AvailableDisplayResolutions.IndexOf(selection);
                    if (selectionIndex < DisplayResolutionsArrayController.ArrangedObjects().Length)
                    {
                        _selectedResolution = selectionIndex;
                    }
                }
                return _selectedResolution;
            }

            set
            {
                _selectedResolution = value;
                ViewModel.SelectedDisplayResolutionViewModel = ViewModel.AvailableDisplayResolutions[_selectedResolution.Int32Value];
            }
        }
        private NSNumber _selectedResolution;
        private bool _initializedDisplayResolutions;

        /// <summary>
        /// Gets or sets the selected display mode.
        /// </summary>
        [INTV.Shared.Utility.OSExport("SelectedMode")]
        public NSNumber SelectedMode
        {
            get
            {
                if (!_initializeDisplayModes)
                {
                    var selectedModeViewModel = ViewModel.SelectedDisplayModeViewModel;
                    var selectedMode = selectedModeViewModel == null ? JzIntvSettingsPageViewModel.DefaultMode : selectedModeViewModel.DisplayMode;
                    selectedModeViewModel = ViewModel.AvailableDisplayModes.First(b => b.DisplayMode == selectedMode);
                    var selectionIndex = ViewModel.AvailableDisplayModes.IndexOf(selectedModeViewModel);
                    if (selectionIndex < JzIntvDisplayModesController.ArrangedObjects().Length)
                    {
                        _selectedMode = selectionIndex;
                    }
                }
                return _selectedMode;
            }

            set
            {
                _selectedMode = value;
                ViewModel.SelectedDisplayModeViewModel = ViewModel.AvailableDisplayModes[_selectedMode.Int32Value];
            }
        }
        private NSNumber _selectedMode;
        private bool _initializeDisplayModes;

        [OSExport(JzIntvSettingsPageViewModel.EnableIntellivoicePropertyName)]
        private int EnableIntellivoice
        {
            get { return (int)ViewModel.EnableIntellivoice; }
            set { ViewModel.EnableIntellivoice = (EnableFeature)value; }
        }

        [OSExport(JzIntvSettingsPageViewModel.EnableEcsPropertyName)]
        private int EnableEcs
        {
            get { return (int)ViewModel.EnableEcs; }
            set { ViewModel.EnableEcs = (EnableFeature)value; }
        }

        [OSExport(JzIntvSettingsPageViewModel.EnableJlpPropertyName)]
        private int EnableJlp
        {
            get { return (int)ViewModel.EnableJlp; }
            set { ViewModel.EnableJlp = (EnableFeature)value; }
        }

        [OSExport(JzIntvSettingsPageViewModel.InitialKeyboardMapSettingName)]
        private int InitialKeyboardMap
        {
            get { return (int)ViewModel.InitialKeyboardMap; }
            set { ViewModel.InitialKeyboardMap = (INTV.JzIntv.Model.KeyboardMap)value; }
        }

        [OSExport("EnableAdditionalArguments")]
        private bool EnableAdditionalArguments
        {
            get { return ViewModel.CommandLineMode == CommandLineMode.AutomaticWithAdditionalArguments; }
        }

        [OSExport("EnableCustomArguments")]
        private bool EnableCustomArguments
        {
            get { return ViewModel.CommandLineMode == CommandLineMode.Custom; }
        }

        [OSExport(JzIntvSettingsPageViewModel.StatusPropertyName)]
        private string Status
        {
            get { return ViewModel.Status; }
        }

        [OSExport(JzIntvSettingsPageViewModel.ConfigurationStatusColorPropertyName)]
        private NSColor ConfigurationStatusColor
        {
            get { return ViewModel.ConfigurationStatusColor; }
        }

        [OSExport("ShowEnableMouse")]
        private bool ShowEnableMouse
        {
            get { return false; }
        }

        private static SelectedTab LastSelectedTab { get; set; }

        private JzIntvSettingsPageViewModel ViewModel { get { return DataContext as JzIntvSettingsPageViewModel; } } 

        #endregion // Properties

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object GetValue(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            JzIntv.Model.Emulator.Instances();
            DisplayResolutionsArrayController.SynchronizeCollection(ViewModel.AvailableDisplayResolutions);
            _initializedDisplayResolutions = true;
            var selectedResolutionViewModel = ViewModel.SelectedDisplayResolutionViewModel;
            var selectedResolution = selectedResolutionViewModel == null ? JzIntvSettingsPageViewModel.DefaultResolution : selectedResolutionViewModel.Resolution;
            selectedResolutionViewModel = ViewModel.AvailableDisplayResolutions.First(r => r.Resolution == selectedResolution);
            if (selectedResolutionViewModel != null)
            {
                var selectedIndex = ViewModel.AvailableDisplayResolutions.IndexOf(selectedResolutionViewModel);
                if (selectedIndex >= 0)
                {
                    DisplayResolutionsArrayController.SelectionIndex = selectedIndex;
                }
            }

            JzIntvDisplayModesController.SynchronizeCollection(ViewModel.AvailableDisplayModes);
            _initializeDisplayModes = true;
            var selectedModeViewModel = ViewModel.SelectedDisplayModeViewModel;
            var selectedMode = selectedModeViewModel == null ? JzIntvSettingsPageViewModel.DefaultMode : selectedModeViewModel.DisplayMode;
            selectedModeViewModel = ViewModel.AvailableDisplayModes.First(m => m.DisplayMode == selectedMode);
            if (selectedModeViewModel != null)
            {
                var selectedIndex = ViewModel.AvailableDisplayModes.IndexOf(selectedModeViewModel);
                if (selectedIndex >= 0)
                {
                    JzIntvDisplayModesController.SelectionIndex = selectedIndex;
                }
            }

            if (LastSelectedTab == SelectedTab.None)
            {
                LastSelectedTab = ConfigurationCommandGroup.AreRequiredEmulatorPathsValid(false) ? SelectedTab.General : SelectedTab.Paths;
            }

            var tab = View.FindChild<NSTabView>();
            if (LastSelectedTab > SelectedTab.None)
            {
                tab.SelectAt((int)LastSelectedTab - 1);
            }

            InitializeAlwaysUpdateDelegates(tab);
            InitializeCommandLineModeRadioButtons(tab);

            tab.DidSelect += OnTabSelected;
            View.Controller = this;
            HandleViewModelPropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(JzIntvSettingsPageViewModel.EnableIntellivoicePropertyName));
            HandleViewModelPropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(JzIntvSettingsPageViewModel.EnableEcsPropertyName));
            ViewModel.PropertyChanged += HandleViewModelPropertyChanged;
        }

        private void InitializeAlwaysUpdateDelegates(NSTabView tab)
        {
            var alwaysUpdateDelegate = new AlwaysUpdateTextChanges();

            var tabsToVisit = new[] { SelectedTab.Input.ToString(), SelectedTab.Advanced.ToString() };
            foreach (var tabItem in tab.Items)
            {
                var identifer = tabItem.Identifier as NSString;
                if ((identifer != null) && tabsToVisit.Contains(identifer))
                {
                    foreach (var textField in tabItem.View.Subviews.OfType<NSTextField>().Where(t => t.Identifier.StartsWith("UseCommitDelegate")))
                    {
                        textField.Delegate = alwaysUpdateDelegate;
                    }
                }
            }
        }

        private void InitializeCommandLineModeRadioButtons(NSTabView tab)
        {
            var state = (int)ViewModel.CommandLineMode;
            var advancedTab = tab.Items.First(t => SelectedTab.Advanced.ToString().Equals((NSString)t.Identifier));
            foreach (var radioButton in advancedTab.View.Subviews.OfType<NSButton>().Where(b => b.Identifier.StartsWith("CommandLine")))
            {
                radioButton.State = radioButton.Tag == state ? NSCellStateValue.On : NSCellStateValue.Off;
            }
        }


        private void HandleViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case JzIntvSettingsPageViewModel.EnableEcsPropertyName:
                case JzIntvSettingsPageViewModel.EnableIntellivoicePropertyName:
                case JzIntvSettingsPageViewModel.StatusPropertyName:
                case JzIntvSettingsPageViewModel.ConfigurationStatusColorPropertyName:
                    this.RaiseChangeValueForKey(e.PropertyName);
                    break;
                case "CommandLineMode":
                    this.RaiseChangeValueForKey("EnableAdditionalArguments");
                    this.RaiseChangeValueForKey("EnableCustomArguments");
                    break;
            }
        }

        private void OnTabSelected(object sender, NSTabViewItemEventArgs e)
        {
            var tabIdentifier = e.Item.Identifier as NSString;
            SelectedTab selectedTab;
            if (!System.Enum.TryParse<SelectedTab>(tabIdentifier, out selectedTab))
            {
                selectedTab = SelectedTab.Paths;
            }
            LastSelectedTab = selectedTab;
        }

        partial void CommandLineOptionSelected(NSObject sender)
        {
            var button = sender as NSButton;
            System.Diagnostics.Debug.WriteLine("Clicked on " + button.Identifier);
            ViewModel.CommandLineMode = (CommandLineMode)button.Tag;
        }

        partial void ClearConfigurationPath(NSObject sender)
        {
            var control = sender as NSControl;
            if (control != null)
            {
                ICommand command = null;
                var whichFile = (EmulatorFile)control.Tag;
                switch (whichFile)
                {
                    case EmulatorFile.JzIntv:
                        command = ConfigurationCommandGroup.ResetJzIntvCommand;
                        break;
                    case EmulatorFile.Exec:
                        command = ConfigurationCommandGroup.ResetExecCommand;
                        break;
                    case EmulatorFile.Grom:
                        command = ConfigurationCommandGroup.ResetGromCommand;
                        break;
                    case EmulatorFile.Ecs:
                        command = ConfigurationCommandGroup.ResetEcsCommand;
                        break;
                    case EmulatorFile.KeyboardConfig:
                        command = ConfigurationCommandGroup.ClearKeyboardConfigFileCommand;
                        break;
                    case EmulatorFile.Cgc0Config:
                        command = ConfigurationCommandGroup.ClearClassicGameController0ConfigFileCommand;
                        break;
                    case EmulatorFile.Cgc1Config:
                        command = ConfigurationCommandGroup.ClearClassicGameController1ConfigFileCommand;
                        break;
                    default:
                        OSMessageBox.Show("Unrecognized file: " + whichFile + "(" + control.Tag + ")", "Reset File: Missing Implementation");
                        break;
                }
                if (command != null)
                {
                    command.Execute(ViewModel);
                }
            }
        }

        partial void SetConfigurationPath(NSObject sender)
        {
            var control = sender as NSControl;
            if (control != null)
            {
                ICommand command = null;
                var whichFile = (EmulatorFile)control.Tag;
                switch (whichFile)
                {
                    case EmulatorFile.JzIntv:
                        command = ConfigurationCommandGroup.LocateJzIntvCommand;
                        break;
                    case EmulatorFile.Exec:
                        command = ConfigurationCommandGroup.LocateExecCommand;
                        break;
                    case EmulatorFile.Grom:
                        command = ConfigurationCommandGroup.LocateGromCommand;
                        break;
                    case EmulatorFile.Ecs:
                        command = ConfigurationCommandGroup.LocateEcsCommand;
                        break;
                    case EmulatorFile.KeyboardConfig:
                        command = ConfigurationCommandGroup.SelectKeyboardConfigFileCommand;
                        break;
                    case EmulatorFile.Cgc0Config:
                        command = ConfigurationCommandGroup.SelectClassicGameController0ConfigFileCommand;
                        break;
                    case EmulatorFile.Cgc1Config:
                        command = ConfigurationCommandGroup.SelectClassicGameController1ConfigFileCommand;
                        break;
                    default:
                        OSMessageBox.Show("Unrecognized file: " + whichFile + "(" + control.Tag + ")", "Set File: Missing Implementation");
                        break;
                }
                if (command != null)
                {
                    command.Execute(ViewModel);
                }
            }
        }

        partial void ResetResolutionToDefault(NSObject sender)
        {
            var selectedResolution = JzIntvSettingsPageViewModel.DefaultResolution;
            var selection = ViewModel.AvailableDisplayResolutions.First(b => b.Resolution == selectedResolution);
            if (selection != null)
            {
                var selectedIndex = ViewModel.AvailableDisplayResolutions.IndexOf(selection);
                if (selectedIndex >= 0)
                {
                    SelectedResolution = selectedIndex;
                    DisplayResolutionsArrayController.SelectionIndex = selectedIndex;
                    this.RaiseChangeValueForKey("SelectedResolution");
                }
            }
        }

        private enum SelectedTab
        {
            None = 0,

            General,

            Paths,

            Display,

            Input,

            Advanced
        }

        private enum TextFieldId
        {
            None = 0,

            Joystick0,

            Joystick1,

            Joystick2,

            Joystick3,

            AdditionalCommandLine,

            CustomCommandLine
        }

        [Register("AlwaysUpdateTextChanges")]
        private class AlwaysUpdateTextChanges : NSTextFieldDelegate
        {
            #region Constructors

            public AlwaysUpdateTextChanges()
            {
                Initialize();
            }

            /// <summary>
            /// Called when created from unmanaged code.
            /// </summary>
            /// <param name="handle">Native pointer to NSTextFieldDelegate.</param>
            public AlwaysUpdateTextChanges(System.IntPtr handle)
                : base(handle)
            {
                Initialize();
            }

            /// <summary>
            /// Called when created directly from a XIB file.
            /// </summary>
            /// <param name="coder">Used to deserialize from a XIB.</param>
            [Export("initWithCoder:")]
            public AlwaysUpdateTextChanges(NSCoder coder)
                : base(coder)
            {
                Initialize();
            }

            /// <summary>Shared initialization code.</summary>
            void Initialize()
            {
            }

            #endregion // Constructors

            /// <inheritdoc/>
            public override void Changed(NSNotification notification)
            {
                var textField = notification.Object as NSTextField;
                var newText = textField.StringValue;
                if (string.IsNullOrWhiteSpace(newText))
                {
                    newText = null;
                }
                var whichTextField = (TextFieldId)textField.Tag;
                switch (whichTextField)
                {
                    case TextFieldId.Joystick0:
                        Properties.Settings.Default.Joystick0Config = newText;
                        break;
                    case TextFieldId.Joystick1:
                        Properties.Settings.Default.Joystick1Config = newText;
                        break;
                    case TextFieldId.Joystick2:
                        Properties.Settings.Default.Joystick2Config = newText;
                        break;
                    case TextFieldId.Joystick3:
                        Properties.Settings.Default.Joystick3Config = newText;
                        break;
                    case TextFieldId.AdditionalCommandLine:
                        Properties.Settings.Default.AdditionalCommandLineArguments = newText;
                        break;
                    case TextFieldId.CustomCommandLine:
                        Properties.Settings.Default.CustomCommandLine = newText;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
