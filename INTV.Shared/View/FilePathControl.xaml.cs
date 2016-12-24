// <copyright file="FilePathControl.xaml.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

using System.Windows;
using System.Windows.Controls;

namespace INTV.Shared.View
{
    /// <summary>
    /// Interaction logic for FilePathControl.xaml
    /// </summary>
    /// <remarks>This class is useful for displaying file paths. NOTE: This class is presently unused...</remarks>
    public partial class FilePathControl : TextBox
    {
        /// <summary>
        /// This attached property stores the base directory of a path.
        /// </summary>
        public static readonly DependencyProperty BaseDirectoryProperty = DependencyProperty.RegisterAttached("BaseDirectory", typeof(string), typeof(FilePathControl), new FrameworkPropertyMetadata());

        /// <summary>
        /// This attached property stores the full path.
        /// </summary>
        public static readonly DependencyProperty PathProperty = DependencyProperty.RegisterAttached("Path", typeof(string), typeof(FilePathControl), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        private string _baseDirectory;
        private string _path;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the FilePathControl class.
        /// </summary>
        public FilePathControl()
        {
            InitializeComponent();
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the base directory to use when displaying the path as a relative path.
        /// </summary>
        public string BaseDirectory
        {
            get { return _baseDirectory; }
            set { _baseDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the full file path.
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        #endregion // Properties
    }
}
