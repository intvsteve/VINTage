============================================================================
 LTO Flash! -- User Interface Software for Windows
============================================================================

Welcome to LTO Flash! -- the most advanced Intellivision (R) Cartridge!

Compatibility
-------------

The LTO Flash! User Interface Software for Windows is available
for Windows xp, or Windows Vista and later.

Installation
------------

Unzip the file you've downloaded and run the setup.exe program to
install the desktop application software.

NOTE: When unzipping, it is recommended you right click and choose
      Properties and, if the 'Unblock' button is present, click that.
      Doing so will allow the installer to run correctly.

Using the LTO Flash! User Interface Software
--------------------------------------------

The LTO Flash! User Interface Software presents two primary work areas:
the ROM List, and the Menu Layout Editor.

The ROM List
------------

The ROM List, which is on the left side of the main application window,
provides a simple way to list the unique ROMs on your system. It displays
various information about each ROM, including:

  Name
  Vendor
  Year
  Features
  Location on disk

You can choose to show or hide the details (Vendor, Year, and Features)
in the application settings for the ROM List.

There are several ways to add ROMs to the ROM List:
  a) Use Explorer to drag and drop files or folders into the ROM List
  b) Use the 'Add Files' or 'Add Folders' button in the ribbon to browse
     to files or folders to add
  c) Use the Add Files... or Add Folders... menu command in the main menu

If you add a folder, that folder, and all the folders it contains, will be
searched for ROMs.

When new ROMs are added, you may opt to have them also automatically added
to the menu layout.

To delete ROMs from the list, simply select one or more items in the list
and press the 'delete' key.

NOTE: The ROM List simply records your ROMs by location on disk. If you
      delete ROMs from the list, the entry is simply removed from the list.
      No files will be deleted on your computer.

The Menu Layout
---------------

The Menu Layout editor, which is on the right portion of the main application
window, is where you specify the layout of the files for your LTO Flash!

To add a ROM to your LTO Flash! menu, simply drag it from the ROM List
and drop it into the desired location in the Menu Layout.

You can create directories (folders) using the 'New Directory' button.
Other operations are also in the application ribbon, as well as via the
context (right click) menu.

At the very top of the Menu Layout Editor, you will see a list of the number
of items at the "root" of the menu, status icons indicating whether a
LTO Flash! device is connected, and an Intellivision console icon, which
indicates whether your LTO Flash! is also plugged into a powered-on
Intellivision Master Component. When your LTO Flash! is plugged into
your computer, you may also see an icon indicating that the menu layout
on your computer is different than the layout on your LTO Flash!

In addition to arranging your ROMs as you see fit, the Menu Layout Editor
provides the following:

  Long Name
  Short Name
  Icon color
  Manual

Each item on your LTO Flash! can have two names - a 'Long Name' and a
'Short Name'. When viewing your ROMs on an Intellivision console, the menu
browser will display the 'Short Name' using the standard Intellivision font.
When selected, the 'Long Name' will be shown, using a custom font if
necessary.

The 'Long Name' for a menu entry may be up to 60 characters long, while the
'Short Name' is limited to 18 characters.

You can also select a color for each item in your menu. In this manner, you
may choose to color-code your ROMs to help you identify them more quickly.

If you wish to do so, you can also associate a manual with your menu entry.
You will then be able to read the manual on your Intellivision console.
Manuals may be no larger than one Megabyte in size.

To rearrange items in the menu, simply drag and drop them.

The Ribbon
----------

If you have used more recent versions of Microsoft Office or other Windows
programs, you are already familiar with the "Ribbon". Just under the title
bar of the application's main window, you will find "tabs" titled "Home"
and "LTO Flash!" ... and perhaps others!

The Menu
--------
In the upper left corner, you will find the menu button, sometimes called the
"jewel". This menu provides you access to application settings and some of
the more advanced features for your LTO Flash!

The Home Tab
------------

You will spend most of your time using the features in the "Home" tab. Here
you see buttons to "Play", "Send to LTO Flash!", and other features.
Many of these commands are only available when your LTO Flash! device is
connected to your computer. The Intellivision console icon indicates whether
your LTO Flash! is also plugged into a powered-on Intellivision Master
Component. More detailed information about each action you can take appears
when you move the mouse over an element in the user interface and wait.

The LTO Flash! Tab
------------------

In the LTO Flash! tab, you will find more advanced features for using your
device, such as applying firmware updates and configuration features.

The Status Bar
--------------

Along the bottom of the window, you will also notice a status area, which
includes an icon indicating whether a LTO Flash! device is connected and
other information.

Communicating with your LTO Flash! Hardware
-------------------------------------------

The LTO Flash! User Interface Software communicates with your LTO Flash!
hardware when you plug it into a USB port. Using the desktop software,
you can configure various features provided by the LTO Flash! hardware, as
well as load your game ROMs and manuals onto the device, or run a program
immediately without copying it to the device. This latter mode is quite
useful if you are developing a game and wish to test it immediately on
actual Intellivision hardware.

By default, when you plug your LTO Flash! hardware into your computer, the
application will connect to the device.

You may also choose to manually connect to, or disconnect from, your
LTO Flash! These options are available in the Home tab. They are named:
  Find Devices
  Disconnect

The Find Devices button will scan for devices attached to your system,
or you can explicitly choose a specific connection.

To cease communications with an attached LTO Flash! device, click the
Disconnect button. This stops communication with the device.

Loading ROMs onto your LTO Flash!
---------------------------------

After you have configured the menu layout to your satisfaction and connected
LTO Flash! to your computer, you're ready to populate the file system on
the hardware and play your games!

Click the 'Send to LTO Flash!' button in the "Home" tab to put the contents
of your Menu Layout onto your LTO Flash! You may also choose to preview
changes, which will give a simple visual indication of items that will be
added, removed, or updated.

Getting ROMs from your LTO Flash!
---------------------------------

If you install this software on another computer and do not wish to recreate
your menu and ROM List, the best way to save time is to simply get the menu
layout from your LTO Flash! hardware to your computer. Click the
'Get from LTO Flash!' button to do this. Note that this will replace
the current menu layout on your system! You can find this function in the
LTO Flash! tab.

LTO Flash! Feature Configuration
--------------------------------

When your LTO Flash! is connected to your computer, you can configure
Intellivision II and ECS compatibility features as well as several other
options.

You can find these settings in the LTO Flash! tab of the Ribbon.

Some ROMs, most notoriously those from Coleco, do not work on the
Intellivision II console. However, the LTO Flash! provides a fix for this
problem. By default, this feature is enabled. You can choose to disable
this feature if you so desire. There are three settings:

  Intellivision II Compatibility Modes
  ------------------------------------
  Never:    Intellivision II compatibility fix not enabled
  Per Game: Intellivision II compatibility enabled for known incompatible ROMs
  Always:   Intellivision II compatibility fix always enabled

Some ROMs, such as the Atarisoft titles, are known to be incompatible with
the Entertainment Computer System (ECS). The LTO Flash! can provide a fix
for these programs as well, allowing you to enjoy them without the need to
disconnect your ECS. By default, LTO Flash! will only enable the ECS ROMs
when running games that are known to require or be enhanced by the ECS.
There are four settings:

  ECS Compatibility Modes
  -----------------------
  Never:      The ECS ROMs are always disabled; ECS games may not run
  ECS Games:  The ECS ROMs are enabled for ROMs known to require the ECS
  Compatible: The ECS ROMs are disabled only when a known incompatible ROM runs
  Always:     The ECS ROMs are always enabled; incompatible titles will not run

When you turn on your Intellivision console with the LTO Flash! cartridge
inserted, you are greeted with the title screen. You can configure how
this behaves as well:

  Show Title Screen Modes
  -----------------------
  Never:    Never show the title screen
  Power On: Show title screen on power up only
  Always:   Always show the title screen

LTO Flash! also will remember the last directory you had navigated to when
choosing a ROM to run, so that resetting to choose another game from the
same location does not require navigating the menu hierarchy. This can be
configured as follows:

  Save Menu Position Modes
  ------------------------
  Never:               Always start at the root menu
  Only During Session: Only while power is on - reset to root after power off
  Always:              Always remember the last menu, even after power off

NOTE: If you update the contents of your LTO Flash! the menu position will
      always be reset to the root location.

And what kind of Intellivision peripheral would the LTO Flash! be if it
didn't use the beloved 'key click' sound? Enable this feature to have
this sound effect play when navigating.

Finally, you can have your LTO Flash! perform file system maintenance while
it is in menu navigation mode. This allows the file system to consolidate
free space and more efficiently use the flash memory on your device.

Device Backup
-------------

The LTO Flash! User Interface Software offers a feature to make a
backup copy of the contents of your device. You may wish to do this from
time to time if you so desire, or prior to reformatting your device. You
can perform a device backup from the menu:

  Menu Button>>LTO Flash!>>Advanced>>Backup File System

When you make a backup, the contents of your device will be saved in the
LTO Flash directory using a subdirectory based on your device serial number
and the date and time of the backup:

  /Users/[your user name]/LTO Flash/Device_[your device serial number]/
  BackupData/[YYYY-MM-DD-HH-MM-SS-NNN]

When the backup completes, you will be informed of the location of the
backup. Note that ROMs stored in this backup are in the LUIGI ROM format.

Device Restore
--------------

If you have made any backups of your LTO Flash! contents, and later
modify or reformat the device, you can restore the previous files and
menu layout by performing a device restore operation from the menu:

  Menu Button>>LTO Flash!>>Advanced>>Restore File System...

When you choose this option, you will be reminded that the current contents
of the attached device will be lost. If you proceed, you will be prompted
to select one of the preexisting backups made for your device.

Clear ROMs Cache
----------------

When the software prepares files for deployment to your LTO Flash! it
stores the file in a separate location, with the intent of avoiding
the work of recreating the file for subsequent updates. However, it is
possible that, due to bugs in the application, an encoding error may
have occurred. Choosing this command simply deletes these files, so
the next time you update the contents of your LTO Flash! they will be
recreated. If you find yourself using this command, there may be some
other problem in the software that must be fixed.

Reformatting the LTO Flash!
---------------------------

If you decide you wish to remove all contents from your LTO Flash!, you may
quickly do so using the Reformat File System command. You will find this
command in the Menu Button>>LTO Flash!>>Advanced menu.

Firmware Updates
----------------

From time to time, bug fixes or new features may become available for your
LTO Flash! You can apply firmware updates, or restore your device to the
factory default firmware, using the options in the Firmware submenu, found
in Menu>>LTO Flash! or by clicking the Update Firmware... button found
on the LTO Flash! tab in the ribbon.

Further Information
-------------------

Not all features are documented in this readme file! Feel free to explore the
software - it won't hurt you!

Where are the Files?
--------------------

As you use the LTO Flash! User Interface Software, it will store data in
a directory named 'LTO Flash' in your 'Documents' directory.

Application preferences will be stored in a preferences file named:
  user.config

The preferences file is located in a rather incomprehensible location. To find
it, it's easiest to open a new Explorer window, and at the very top, type in:
  %LocalAppData%

Then, locate the INTVFunhouse subdirectory. Within this directory, you will
find the subdirectory containing the preferences file. It's unlikely you'll
need to directly access this file.

All of your ROMs, configuration files, manuals, and so forth remain in their
original locations on your computer, and will not be modified by the
application. Note, however, that if you add ROMs to the ROM List from a
CD-ROM, flash drive, network drive, or other removable media, a copy of the
ROM will be stored in the ROMs subdirectory inside the LTO Flash directory.
If you remove the flash drive or CD-ROM, this backup copy will be used.

TROUBLESHOOTING
---------------

This software is not perfect. Here are some troubleshooting suggestions:

  1. The application software crashes.
     If you are using the software and a crash occurs, most of the time you
     will encounter a dialog that reports information about the crash and
     have the opportunity to report the problem.
     Include this information, as well as what you were doing, and whether
     you are able to repeat the crash, in a report to:
       support@intvfunhouse.com

  2. It's also possible that the application will crash in an unexpected
     manner that produces a traditional Windows crash report. Gather what
     information you can and send it to support@intvfunhouse.com.
     These tend to be more difficult to reproduce.

OTHER
-----

Intellivision (R) is a trademark of Intellivision, Productions, Inc.
Left Turn Only, LLC and INTV Funhouse are not affiliated with
Intellivision Productions, Inc.
