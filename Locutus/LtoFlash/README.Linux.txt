============================================================================
 LTO Flash! -- User Interface Software for Linux
============================================================================

Welcome to LTO Flash! -- the most advanced Intellivision (R) Cartridge!

Compatibility
-------------

The LTO Flash! User Interface Software for Linux has been tested with
the following distros:
  Ubuntu 16.04.2

Installation
------------

Unzip the the file you've downloaded if not done automatically by
your web browser.

TBD


**TO RUN THE APP: TBD**

  Additional Required Software
  ----------------------------

  The LTO Flash! User Interface Software may require additional software
  to be installed in order to function with your LTO Flash! hardware.

    1. MONO Runtime - required to run the application
    2. FTDI Drivers - may be required to communicate with your LTO Flash!
       though the need to do this has not yet been encountered

  1. MONO Runtime
  ---------------
  The LTO Flash! User Interface Software for Linux is implemented using
  C#, and requires the MONO runtime to be installed. When you launch the
  application, if the MONO runtime is not installed, you will be prompted
  to install it. Simply follow the prompts.

  If you prefer, you can download the current MONO framework here:
    http://www.mono-project.com/download/ TBD

  2. Gtk-sharp

  3. ???? TBD

  ?. FTDI Drivers TBD
  ---------------
  The LTO Flash! hardware may require the Virtual Communication Port drivers
  from FTDI. You can download these drivers directly from FTDI, or install
  the appropriate version included in the ZIP file.

  The general FTDI Virtual COM Port (VCP) driver page is:
    http://www.ftdichip.com/Drivers/VCP.htm

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
  a) Use Finder to drag and drop files or folders into the ROM List
  b) Use the 'Add Files' or 'Add Folders' button in the toolbar to browse
     to files or folders to add
  c) Use the Add Files... or Add Folders... menu command in the File menu

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

You can create directories (folders) using the 'New Directory' command in
the Edit menu, or by clicking the New Directory button (+) in the
Menu Layout Editor, as well as via the context (right click) menu.

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
LTO Flash! These options are available in the Tools menu. In the Tools menu,
you will find the following options:
  Find LTO Flash! Devices
  Connect to Device on Port
  Disconnect from LTO Flash!

The Find LTO Flash! Devices command will attempt to determine whether any
LTO Flash! devices are attached to your computer.

The Connect To Device on Port menu will list the ports in your system that
may be connected to a LTO Flash!. You would most likely use this if you
previously disconnected your device and wish to reconnect. You will notice
that the LTO Flash! manifests as a standard serial port on your system.

To cease communications with an attached LTO Flash! device, choose the
Disconnect from LTO Flash! command. This stops communication with the
device and makes it safer to unplug it from your system.

  !!!! NOTE NOTE NOTE !!!!
  ------------------------
  We recommend you disconnect from your LTO Flash! by choosing:
    Tools>>Disconnect from LTO Flash!

Loading ROMs onto your LTO Flash!
---------------------------------

After you have configured the menu layout to your satisfaction and connected
LTO Flash! to your computer, you're ready to populate the file system on
the hardware and play your games!

Click the Send to LTO Flash! button on the toolbar, or choose the menu
item in the Tools menu to put the contents of your Menu Layout onto your
LTO Flash! You may also choose to preview changes, which will give a
simple visual indication of items that will be added, removed, or updated.

Getting ROMs from your LTO Flash!
---------------------------------

If you install this software on another computer and do not wish to recreate
your menu and ROM List, the best way to save time is to simply get the menu
layout from your LTO Flash! hardware to your computer. You will find the
command to do this in the Tools menu. Note that this will replace the
current menu layout on your system!

LTO Flash! Feature Configuration
--------------------------------

When your LTO Flash! is connected to your computer, you can configure
Intellivision II and ECS compatibility features as well as several other
options.

You can observe and change these settings when you have a device attached
to your computer by choosing the Device Information command from the
Tools menu.

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

Device Information
------------------

You can observe various information about your LTO Flash! device by selecting
  Tools>>Device Information

in the menu. Here you will find various information about your LTO Flash!
device, including serial number, device settings, firmware versions,
low-level file system information, and flash lifetime details.

Device Backup
-------------

The LTO Flash! User Interface Software offers a feature to make a
backup copy of the contents of your device. You may wish to do this from
time to time if you so desire, or prior to reformatting your device. You
can perform a device backup from the menu:

  Tools>>LTO Flash!>>Advanced>>Backup File System

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

  Tools>>LTO Flash!>>Advanced>>Restore File System...

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
command in the Tools>>LTO Flash!>>Advanced menu.

Firmware Updates
----------------

From time to time, bug fixes or new features may become available for your
LTO Flash! You can apply firmware updates, or restore your device to the
factory default firmware, using the options in the Firmware submenu, found
in Tools>>LTO Flash! or by clicking the Update Firmware... button found
in the Device Information dialog's Firmware page.

Further Information
-------------------

Not all features are documented in this readme file! Feel free to explore the
software - it won't hurt you!

Where are the Files?
--------------------

As you use the LTO Flash! User Interface Software, it will store data in
a directory named 'LTO Flash' in your 'Documents' directory.

Application preferences will be stored in a preferences file named:
  com.intvfunhouse.ltoflash.plist
 
The preferences file is located in the Library/Preferences directory under
your home directory.

All of your ROMs, configuration files, manuals, and so forth remain in their
original locations on your computer, and will not be modified by the
application. Note, however, that if you add ROMs to the ROM List from a
CD-ROM, flash drive, network drive, or other removable media, a copy of the
ROM will be stored in the ROMs subdirectory inside the LTO Flash directory.
If you remove the flash drive or CD-ROM, this backup copy will be used.

TROUBLESHOOTING
---------------

As noted above, due to some unfortunate behaviors in the FTDI Virtual
Communication Port (VCP) drivers, the following problems may occur:

  1. Computer unexpectedly restarts (crashes).
     CAUSE: Unplugging LTO Flash!
     REMEDY: This problem is much more likely to occur when unplugging your
             LTO Flash! while it is connected to the application software.
             Remember to choose Tools>>Disconnect or use the keyboard
             shortcut Cmd+D to disconnect before unplugging your device.

  2. Computer does not recognize your LTO Flash!
     CAUSE: There are at least three known reasons for this to happen.
       a) The software is not configured to automatically connect to devices
          when detected, nor to prompt for connection.
       b) The application software, it must be admitted with great sadness,
          is imperfect. Perhaps it is not detecting devices correctly.
       c) The FTDI VCP serial port driver itself is not functioning correctly.
       d) A bad USB cable
       e) A damaged device.
     REMEDY: The remedy depends on the problem, but troubleshooting this
             problem is simple enough.
               a) Attempt to connect to the device directly using the menu.
                  You will find a list of potential connections in the
                  following menu:
                    Tools>>Connect To Device on Port
               b) If no possible connections can be found in the menu, check
                  your cable, and unplug and reattach your LTO Flash! In rare
                  cases, it may help to wait a few minutes between attempts.
                  While debugging this scenario, for unknown reasons the driver
                  responds with a 'device busy' error until some internal
                  timeout lapses. Normal operation then resumes.
               c) If, after checking connections, you still cannot connect
                  to your device, quit and restart the application software.
               d) If you have restarted your computer and there are no
                  messages showing that the USB Serial Driver is
                  starting or stopping, try a different USB cable. Your
                  LTO Flash! arrives with a high-quality USB cable. If you
                  are using a different cable, try the original. If that
                  fails, try another known good cable. Also try other USB
                  ports if possible.
               e) If all else fails, it's possible that you've found a bug
                  in the application software, a hardware compatibility
                  problem, or have a defective device.

  3. The application software crashes.
     If you are using the software and a crash occurs, most of the time you
     will encounter a dialog that reports information about the crash and
     have the opportunity to report the problem.
     Include this information, as well as what you were doing, and whether
     you are able to repeat the crash, in a report to:
       support@intvfunhouse.com

 4.  It's also possible that the application will crash in an unexpected
     manner that produces a traditional Apple crash report.
     Again, gather what information you can and send it to:
       support@intvfunhouse.com
     These crashes tend to be more difficult to reproduce.

OTHER
-----

Intellivision (R) is a trademark of Intellivision, Productions, Inc.
Left Turn Only, LLC and INTV Funhouse are not affiliated with
Intellivision Productions, Inc.
