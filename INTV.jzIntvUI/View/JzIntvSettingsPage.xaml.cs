using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using INTV.JzIntv.Model;
using INTV.JzIntvUI.Commands;
using INTV.JzIntvUI.ViewModel;
using INTV.Shared.ComponentModel;

namespace INTV.JzIntvUI.View
{
    /// <summary>
    /// Interaction logic for JzIntvSettingsPage.xaml
    /// </summary>
    public partial class JzIntvSettingsPage : UserControl
    {
        #region Commands

        /// <summary>
        /// Command for drag-enter of a file.
        /// </summary>
        public static readonly RelayCommand DragFileEnterCommand = new RelayCommand(DragFileEnter);

        /// <summary>
        /// Command for drag-over of a file.
        /// </summary>
        public static readonly RelayCommand DragFileOverCommand = new RelayCommand(DragFileOver);

        /// <summary>
        /// Command for drag-drop of a file.
        /// </summary>
        public static readonly RelayCommand DragFileDropCommand = new RelayCommand(DragFileDrop);

        #endregion Commands

        #region Constructors

        /// <summary>
        /// Initialize a new instance of JzIntvSettingsPage.
        /// </summary>
        public JzIntvSettingsPage()
        {
            InitializeComponent();
        }

        #endregion // Constructors

        private static void DragFileEnter(object dragEventArgs)
        {
            var dragArgs = (DragEventArgs)dragEventArgs;
            if (!dragArgs.Handled)
            {
                AcceptFile(dragArgs);
            }
        }

        private static void DragFileOver(object dragEventArgs)
        {
            var dragArgs = (DragEventArgs)dragEventArgs;
            if (!dragArgs.Handled)
            {
                AcceptFile(dragArgs);
            }
        }

        private static void DragFileDrop(object dragEventArgs)
        {
            var dragArgs = (DragEventArgs)dragEventArgs;
            if (!dragArgs.Handled)
            {
                var whichFile = GetEmulatorFileType(dragArgs);
                var file = GetFile(dragArgs);
                if (AcceptFile(whichFile, file))
                {
                    var viewModel = ((System.Windows.FrameworkElement)dragArgs.Source).DataContext as JzIntvSettingsPageViewModel;
                    switch (whichFile)
                    {
                        case EmulatorFile.JzIntv:
                            viewModel.EmulatorPath = file;
                            break;
                        case EmulatorFile.Exec:
                            viewModel.ExecRomPath = file;
                            break;
                        case EmulatorFile.Grom:
                            viewModel.GromRomPath = file;
                            break;
                        case EmulatorFile.Ecs:
                            viewModel.EcsRomPath = file;
                            break;
                        case EmulatorFile.KeyboardConfig:
                            viewModel.DefaultKeyboardConfigPath = file;
                            break;
                        case EmulatorFile.Cgc0Config:
                            viewModel.JzIntvCgc0ConfigPath = file;
                            break;
                        case EmulatorFile.Cgc1Config:
                            viewModel.JzIntvCgc1ConfigPath = file;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static EmulatorFile GetEmulatorFileType(DragEventArgs dragArgs)
        {
            var emulatorFileType = (EmulatorFile)((FrameworkElement)dragArgs.Source).Tag;
            return emulatorFileType;
        }

        private static string GetFile(DragEventArgs dragArgs)
        {
            var data = dragArgs.Data as System.Windows.IDataObject;
            var file = (data.GetData(System.Windows.DataFormats.FileDrop) as System.Collections.Generic.IEnumerable<string>).First();
            return file;
        }

        private static bool AcceptFile(DragEventArgs dragArgs)
        {
            var accept = AcceptFile(GetEmulatorFileType(dragArgs), GetFile(dragArgs));
            if (!accept)
            {
                dragArgs.Effects = DragDropEffects.None;
            }
            return accept;
        }

        private static bool AcceptFile(EmulatorFile whichFile, string path)
        {
            var acceptFile = !string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path);
            if (acceptFile)
            {
                switch (whichFile)
                {
                    case EmulatorFile.JzIntv:
                        acceptFile = string.Compare(System.IO.Path.GetExtension(path), INTV.Shared.Utility.PathUtils.ProgramSuffix, true) == 0;
                        break;
                    case EmulatorFile.Ecs:
                    case EmulatorFile.Exec:
                    case EmulatorFile.Grom:
                        acceptFile = ConfigurationCommandGroup.IsRomPathValid(path);
                        break;
                    case EmulatorFile.KeyboardConfig:
                    case EmulatorFile.Cgc0Config:
                    case EmulatorFile.Cgc1Config:
                        break;
                    default:
                        acceptFile = false; // don't know what file this is for
                        break;
                }
            }
            return acceptFile;
        }
    }
}
