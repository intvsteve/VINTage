// <copyright file="NativeMethods.WPF.cs" company="INTV Funhouse">
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
using System.Runtime.InteropServices;
using System.Text;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// P/Invoke to native functions in the Win32 API for features lacking in C#.
    /// </summary>
    internal static partial class NativeMethods
    {
        /// <summary>
        /// This special pseudo-window is used to broadcast a message to all windows in the system. USE WITH CAUTION!
        /// </summary>
        public static readonly IntPtr HWND_BROADCAST = (IntPtr)0xffff;

        /// <summary>
        /// Convert a path into a form without spaces.
        /// </summary>
        /// <param name="longName">The path to convert.</param>
        /// <returns>The converted path.</returns>
        public static string ToShortPathName(string longName)
        {
            uint bufferSize = 256;
            StringBuilder shortNameBuffer = new StringBuilder((int)bufferSize);

            uint result = GetShortPathName(longName, shortNameBuffer, bufferSize);
            System.Diagnostics.Debug.WriteLineIf(result == 0, "GetShortPathName failed.");
            return shortNameBuffer.ToString();
        }

        /// <summary>
        /// Retrieves the short path form of the specified path. Useful if an application cannot tolerate spaces in a path, or quoted strings.
        /// </summary>
        /// <param name="path">The path string.</param>
        /// <param name="shortPath">A pointer to a buffer to receive the null-terminated short form of the path. Passing IntPtr.Zero for this
        /// parameter and zero for shortPathLength will always return the required buffer size for a specified path.</param>
        /// <param name="shortPathLength">The size of the buffer that shortPath points to, in TCHARs. Set this parameter to zero if shortPath is set to IntPtr.Zero.</param>
        /// <returns>If the function succeeds, the return value is the length, in TCHARs, of the string that is copied to shortPath, not including the terminating null character.
        /// If the shortPath buffer is too small to contain the path, the return value is the size of the buffer, in TCHARs, that is required to hold the path and the terminating null character.
        /// If the function fails for any other reason, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint GetShortPathName(string path, StringBuilder shortPath, uint shortPathLength);

        /// <summary>
        /// Places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message.
        /// </summary>
        /// <param name="windowHandle">A handle to the window whose window procedure is to receive the message.</param>
        /// <param name="message">The message to be posted.</param>
        /// <param name="wordParameter">Additional message-specific information - Win32's WPARAM.</param>
        /// <param name="longParameter">Additional message-specific information - Win32's LPARAM.</param>
        /// <returns>If the function succeeds, the return value is <c>true</c>.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr windowHandle, uint message, IntPtr wordParameter, IntPtr longParameter);

        /// <summary>
        /// Defines a new window message that is guaranteed to be unique throughout the system. The message value can be used when sending or posting messages.
        /// </summary>
        /// <param name="message">The message to be registered.</param>
        /// <returns>If the message is successfully registered, the return value is a message identifier in the range 0xC000 through 0xFFFF.</returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint RegisterWindowMessage(string message);

        #region Shell Folder Functions

        /// <summary>
        /// Opens a Windows Explorer window with specified items in a particular folder selected.
        /// </summary>
        /// <param name="pidlFolder">A pointer to a fully qualified item ID list that specifies the folder.</param>
        /// <param name="cidl">A count of items in the selection array, apidl. If cidl is zero, then pidlFolder must point to a fully specified ITEMIDLIST describing a single item to select. This function opens the parent folder and selects that item.</param>
        /// <param name="apidl">A pointer to an array of PIDL structures, each of which is an item to select in the target folder referenced by pidlFolder.</param>
        /// <param name="dwFlags">The optional flags. Under Windows XP this parameter is ignored. In Windows Vista, the following flags are defined.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport("shell32.dll", ExactSpelling = true)]
        public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

        /// <summary>
        /// Returns the ITEMIDLIST structure associated with a specified file path.
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated Unicode string that contains the path. This string should be no more than MAX_PATH characters in length, including the terminating null character.</param>
        /// <returns>Returns a pointer to an ITEMIDLIST structure that corresponds to the path.</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr ILCreateFromPath(string pszPath);

        /// <summary>
        /// Releases resources held by a COM object.
        /// </summary>
        /// <param name="comObjs">An array of COM objects to release.</param>
        public static void ReleaseComObject(params object[] comObjs)
        {
            foreach (object obj in comObjs)
            {
                if ((obj != null) && Marshal.IsComObject(obj))
                {
                    Marshal.ReleaseComObject(obj);
                }
            }
        }

        // Adapted from Stack Overflow: http://stackoverflow.com/questions/10667012/getting-downloads-folder-in-c
        private static readonly string[] _knownFolderGuids = new string[]
        {
            "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
            "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
            "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
            "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
            "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
            "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
            "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
            "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
            "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
            "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
            "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
            "{352481E8-33BE-4251-BA85-6007CAEDCF9D}", // Internet Cache
            "{2B0F765D-C0E9-4171-908E-08A611B84FF6}", // Cookies
        };

        /// <summary>
        /// Flags for working with shell folders. See also: https://msdn.microsoft.com/en-us/library/windows/desktop/dd378447(v=vs.85).aspx
        /// </summary>
        [Flags]
        private enum KnownFolderFlags : uint
        {
            /// <summary>
            /// Simple PIDL.
            /// </summary>
            SimpleIDList              = 0x00000100,

            /// <summary>
            /// Gets the folder's default path independent of the current location of its parent. Requires DefaultPath also set.
            /// </summary>
            NotParentRelative         = 0x00000200,

            /// <summary>
            /// Gets the default path for a known folder.
            /// </summary>
            DefaultPath               = 0x00000400,

            /// <summary>
            /// Initializes the folder using its Desktop.ini settings.
            /// </summary>
            Init                      = 0x00000800,

            /// <summary>
            /// Gets the true system path for the folder, free of any aliased placeholders such as %USERPROFILE%, returned by SHGetKnownFolderIDList and IKnownFolder::GetIDList.
            /// </summary>
            NoAlias                   = 0x00001000,

            /// <summary>
            /// Stores the full path in the registry without using environment strings.
            /// </summary>
            DontUnexpand              = 0x00002000,

            /// <summary>
            /// Do not verify the folder's existence before attempting to retrieve the path or IDList.
            /// </summary>
            DontVerify                = 0x00004000,

            /// <summary>
            /// Forces the creation of the specified folder if that folder does not already exist.
            /// </summary>
            Create                    = 0x00008000,

            /// <summary>
            /// When running inside an app container, or when providing an app container token, this flag prevents redirection to app container folders. (Windows 7 and later)
            /// </summary>
            NoAppcontainerRedirection = 0x00010000,

            /// <summary>
            /// Return only aliased PIDLs. Do not use the file system path. (Windows 7 and later)
            /// </summary>
            AliasOnly                 = 0x80000000
        }

        /// <summary>
        /// Standard folders registered with the system. These folders are installed with Windows Vista
        /// and later operating systems, and a computer will have only folders appropriate to it
        /// installed.
        /// </summary>
        private enum KnownFolder
        {
            /// <summary>
            /// Contacts folder.
            /// </summary>
            Contacts,

            /// <summary>
            /// Desktop folder.
            /// </summary>
            Desktop,

            /// <summary>
            /// Documents folder.
            /// </summary>
            Documents,

            /// <summary>
            /// Downloads folder.
            /// </summary>
            Downloads,

            /// <summary>
            /// Favorites folder.
            /// </summary>
            Favorites,

            /// <summary>
            /// Links folder.
            /// </summary>
            Links,

            /// <summary>
            /// Music folder.
            /// </summary>
            Music,

            /// <summary>
            /// Pictures folder.
            /// </summary>
            Pictures,

            /// <summary>
            /// Saved games data folder.
            /// </summary>
            SavedGames,

            /// <summary>
            /// Saved searches folder.
            /// </summary>
            SavedSearches,

            /// <summary>
            /// Videos folder.
            /// </summary>
            Videos,

            /// <summary>
            /// Temporary Internet Files folder.
            /// </summary>
            InterentCache,
        }

        /// <summary>
        /// CLSID values for some of the known folders.
        /// </summary>
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb762494%28v=vs.85%29.aspx"/>
        private enum KnownFolderIdClsidValue
        {
            /// <summary>
            /// Desktop folder.
            /// </summary>
            Desktop = 0x0000,

            /// <summary>
            /// My Documents folder.
            /// </summary>
            Documents = 0x0005,

            /// <summary>
            /// My Music folder.
            /// </summary>
            Music = 0x000d,

            /// <summary>
            /// Temporary internet files.
            /// </summary>
            InterentCache = 0x0020,

            /// <summary>
            /// Internet cookies folder.
            /// </summary>
            CSIDL_COOKIES = 0x0021,

            /// <summary>
            /// Internet history folder.
            /// </summary>
            CSIDL_HISTORY = 0x0022,

            /// <summary>
            /// My Pictures folder.
            /// </summary>
            Pictures = 0x0027,
        }

        private static readonly Dictionary<KnownFolder, KnownFolderIdClsidValue> _knownFoldersToKnownClsids = new Dictionary<KnownFolder, KnownFolderIdClsidValue>()
        {
            { KnownFolder.Desktop, KnownFolderIdClsidValue.Desktop },
            { KnownFolder.Documents, KnownFolderIdClsidValue.Documents },
            { KnownFolder.Music, KnownFolderIdClsidValue.Music }
        };

        /// <summary>
        /// Gets the current path to the Downloads folder as currently configured. This does
        /// not require the folder to exist.
        /// </summary>
        /// <returns>The default path of the known folder.</returns>
        public static string GetDownloadsPath()
        {
            string downloadsPath = null;
            IntPtr outPath;
            int result = -1;
            try
            {
                result = SHGetKnownFolderPath(new Guid(_knownFolderGuids[(int)KnownFolder.Downloads]), (uint)KnownFolderFlags.DontVerify, IntPtr.Zero, out outPath);
                if (result >= 0)
                {
                    downloadsPath = Marshal.PtrToStringUni(outPath);
                }
            }
            catch (EntryPointNotFoundException)
            {
                // This exception arises in Windows xp. Though SHGetFolderPath is documented as a *WRAPPER* for SHGetKnownFolderPath, it turns out
                // that SHGetKnownFolderPath is *NOT AVAILABLE* in Windows xp. Since SHGetKnownFolderPath is the favored function, we'll catch this just
                // in case it happens in Windows xp and try again.
                // Actually, this probably never executes... Because there is no CLSID value for "Downloads". It was a folder dangling off MyDocuments, which
                // used a localized name most likely. Since the API requires a special CLSID, we'll go through the formalism of looking in the map, but
                // really, this doesn't do anything. :/
                var clsIdValue = KnownFolderIdClsidValue.Documents;
                if (_knownFoldersToKnownClsids.TryGetValue(KnownFolder.Downloads, out clsIdValue))
                {
                    try
                    {
                        var builder = new StringBuilder(260); // old-school max path
                        result = SHGetFolderPath(IntPtr.Zero, (int)clsIdValue, IntPtr.Zero, 0, builder);
                        if (result == 0)
                        {
                            // S_OK
                            downloadsPath = builder.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        // don't care - just return a null path, which is safe
                    }
                }
            }
            catch (Exception)
            {
                // don't care - just return a null path, which is safe
            }
            return downloadsPath;
        }

        // See https://msdn.microsoft.com/en-us/library/windows/desktop/bb762188(v=vs.85).aspx
        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

        // See https://msdn.microsoft.com/en-us/library/windows/desktop/bb762181%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, [Out] StringBuilder pszPath);

        #endregion //  Shell Folder Functions
    }
}
