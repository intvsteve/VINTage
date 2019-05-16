#############################################################################
# Project Version Tools                                                     #
# ------------------------------------------------------------------------- #
# This makefile defines rules to assist with updating version information   #
# used by various projects. Unfortunately, these tools do not update the    #
# InstallShield installer version information.                              #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# Do nothing for clean.
# ------------------------------------------------------------------------- #
ifneq ($(MAKECMDGOALS),clean)

# ------------------------------------------------------------------------- #
# This makefile includes version.mak, which determines the values of the
# following variables, either directly or indirectly:
#
#   VERSION                          : version - major.minor.build.revision
#   BUILD_REVISION                   : user-friendly build information
#   CUSTOM_BUILD                     : custom build string (usually empty)
#   COPYRIGHT_DATE                   : copyright date to use
#   VERSION_CS_CLASS_NAME            : name of a C# class to generate
#   VERSION_CS_CLASS_NAMESPACE       : namespace for the class
#   VERSION_CS_CLASS_OUTPUT_DIR      : directory in which to generate class
#   VERSION_CS                       : name of the file containing C# class
#   INFO_PLIST_PATH                  : Mac only - Info.plist file to generate
#   OLDEST_MACOS32_DEPLOYMENT_TARGET : Mac only - oldest OS target for 32-bit
#   OLDEST_MACOS64_DEPLOYMENT_TARGET : Mac only - oldest OS target for 64-bit
#
# These variables must be properly assigned for the tools to work correctly.
# ------------------------------------------------------------------------- #

include version.mak

# ------------------------------------------------------------------------- #
# Function: EmitVersionInfoClass                                            #
# ------------------------------------------------------------------------- #
# This function declares a C# class that will contain version information
# that is supplied to the function call. This function's arguments are:
#   $(1) : the name of the class (e.g. VERSION_CS_CLASS_NAME)
#   $(2) : the file and product copyright date
#   $(3) : the namespace in which to place the class
#   $(4) : the full version number
#   $(5) : the short version string
#   $(6) : the user-friendly build string
# ------------------------------------------------------------------------- #
define EmitVersionInfoClass
// <copyright file="$(1).cs" company="INTV Funhouse">
// Copyright (c) $(2) All Rights Reserved
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

namespace $(3)
{
    /// <summary>
    /// This class contains the various version strings to use in a VINTage project.
    /// It provides constants for use by assemblies in the various projects that
    /// desire to use them.
    /// </summary>
    /// <remarks>THIS CLASS IS GENERATED! DO NOT MANUALLY EDIT THIS FILE!</remarks>
    internal static class $(1)
    {
        /// <summary>
        /// The full version string.
        /// </summary>
        public const string FullVersionString = "$(4)";

        /// <summary>
        /// The short version string.
        /// </summary>
        public const string ShortVersionString = "$(5)";

        /// <summary>
        /// The build version string.
        /// </summary>
        public const string BuildVersionString = "$(6)";

        /// <summary>
        /// The current copyright year string.
        /// </summary>
        public const string CurrentCopyrightYear = "$(2)";
    }
}
endef

# ------------------------------------------------------------------------- #
# VERSION_CS_CONTENT contains the generated class. Because versions of GNU
# make prior to 4.0 do not support the 'file' function, export the contents
# to an environment variable, which is then consumed by update_version_cs.
# ------------------------------------------------------------------------- #
VERSION_CS_CONTENT = $(call EmitVersionInfoClass,$(VERSION_CS_CLASS_NAME),$(COPYRIGHT_DATE),$(VERSION_CS_CLASS_NAMESPACE),$(VERSION),$(VERSION_SHORT),$(BUILD_REVISION))
export VERSION_CS_CONTENT

# ------------------------------------------------------------------------- #
# VERSION_CS_UPDATE_STATUS_FILE is a status file that is used to determine
# if it is possible to skip regenerating the class. The contents of the
# file are used to determine if regeneration is necessary.
# ------------------------------------------------------------------------- #
VERSION_CS_UPDATE_STATUS_FILE = $(VERSION_CS_CLASS_OUTPUT_DIR)/.$(VERSION_CS_CLASS_NAMESPACE)_$(VERSION_CS_CLASS_NAME).finished

# ------------------------------------------------------------------------- #
# CURR_SVN_REVISION is simply the concatenation of the current revision
# (presumed to be SVN revision) with the number of dirty files and custom
# version text. This is used to determine if an update is necessary.
# ------------------------------------------------------------------------- #
CURR_SVN_REVISION = $(strip $(VERSION_REVISION) $(SVN_DIRTY) $(CUSTOM_BUILD))

# ------------------------------------------------------------------------- #
# PREV_SVN_REVISION is the contents of the VERSION_CS_UPDATE_STATUS_FILE.
# If the file does not exist, the variable will be empty. Subsequently,
# unless VERSION_CS_UPDATE_STATUS_FILE is deleted, regeneration of the
# class file will occur only if the SVN repo revision changes, or the
# number of modified files changes.
# ------------------------------------------------------------------------- #
ifneq ($(wildcard $(VERSION_CS_UPDATE_STATUS_FILE)),)
  PREV_SVN_REVISION = $(strip $(shell cat $(VERSION_CS_UPDATE_STATUS_FILE) 2>&1))
endif
ifeq ($(wildcard $(VERSION_CS_CLASS_OUTPUT_DIR)/$(VERSION_CS)),)
  PREV_SVN_REVISION = -undefined-
endif

# ------------------------------------------------------------------------- #
# Provide additional information if updating file when using Git.
# ------------------------------------------------------------------------- #
ifneq ($(GIT_REPO),)
  # ----------------------------------------------------------------------- #
  # Additional message to display if using Git.
  # ----------------------------------------------------------------------- #
  USING_GIT_NOTE =
  USING_GIT_NOTE += [using Git repo does not update version]
endif

# ------------------------------------------------------------------------- #
# Rule: update_version_cs
# ------------------------------------------------------------------------- #
# This rule will generate a class' source code for use in the C# projects.
# ------------------------------------------------------------------------- #
update_version_cs:
ifneq ($(CURR_SVN_REVISION),$(PREV_SVN_REVISION))
	@echo Generating for version [$(CURR_SVN_REVISION)]: $(VERSION_CS_CLASS_OUTPUT_DIR)/$(VERSION_CS) $(USING_GIT_NOTE) ...
	@echo "$$VERSION_CS_CONTENT" > $(VERSION_CS_CLASS_OUTPUT_DIR)/$(VERSION_CS)
	@echo $(CURR_SVN_REVISION) > $(VERSION_CS_UPDATE_STATUS_FILE)
else
	@echo '$(VERSION_CS_CLASS_OUTPUT_DIR)/$(VERSION_CS)' is up-to-date.
endif

# ------------------------------------------------------------------------- #
# Mac-Specific Rules                                                        #
# ------------------------------------------------------------------------- #
ifeq ($(TARGET_OS),MAC)
  ifneq ($(INFO_PLIST_PATH),)
    # --------------------------------------------------------------------- #
    # INFO_PLIST_UPDATE_STATUS_FILE is a status file that is used to
    # determine if it is possible to skip updating the Info.plist files.
    # The contents of the file are used to determine if an update is needed.
    # --------------------------------------------------------------------- #
    INFO_PLIST_UPDATE_STATUS_FILE = .Info_plist_update.finished

    # --------------------------------------------------------------------- #
    # PREV_SVN_INFO_PLIST_REVISION is the contents of the
    # INFO_PLIST_UPDATE_STATUS_FILE. If the file does not exist, the
    # variable will be empty. Subsequently, unless the file is deleted,
    # updating of the Info.plist file(s) will occur only if the SVN repo
    # revision changes, or the number of modified files changes.
    # --------------------------------------------------------------------- #
    ifneq ($(wildcard $(INFO_PLIST_UPDATE_STATUS_FILE)),)
      PREV_SVN_INFO_PLIST_REVISION = $(shell cat $(INFO_PLIST_UPDATE_STATUS_FILE) 2>&1)
    endif
    ifeq ($(wildcard $(INFO_PLIST_PATH)),)
      PREV_SVN_INFO_PLIST_REVISION = -undefined-
    endif

# ------------------------------------------------------------------------- #
# OLDEST_MACOS32_DEPLOYMENT_TARGET is the oldest version of macOS that can
# be built. This can only be created using old versions of Xamarin Studio,
# such as version 5.8.3 (build 1), and must be built using MonoMac. The
# application will run as a 32-bit process and on modern versions of macOS
# cause a warning that it is not optimized for the machine. At some point,
# Apple will drop 32-bit app support in macOS completely.
# ------------------------------------------------------------------------- #
OLDEST_MACOS32_DEPLOYMENT_TARGET = 10.7

# ------------------------------------------------------------------------- #
# OLDEST_MACOS64_DEPLOYMENT_TARGET is the oldest version of macOS that can
# be built, as of Xamarin Studio Community 6.3 (build 864) with Xamarin.Mac
# version 5.8.0.0. This is also the oldest macOS target supported by Visual
# Studio for Mac - the replacement of Xamarin Studio.
# ------------------------------------------------------------------------- #
OLDEST_MACOS64_DEPLOYMENT_TARGET = 10.9

# ------------------------------------------------------------------------- #
# MACOS_DEPLOYMENT_TARGET is usually specified in the custom.mak file. That
# said, it was added in May 2019, so some custom.mak files may not have
# been created from the template that includes it. If not defined, assume
# the oldest supported version.
# ------------------------------------------------------------------------- #
MACOS_DEPLOYMENT_TARGET ?= $(OLDEST_MACOS32_DEPLOYMENT_TARGET)

# ------------------------------------------------------------------------- #
# Function: EmitInfoPList                                            #
# ------------------------------------------------------------------------- #
# This function generates an Info.plist file which will contain product and
# version information that is supplied to the function call. Note that it
# is not very generic! It also should not be submitted to SVN! You may be
# tempted to edit the file produced from this function in Xamarin Studio /
# MonoDevelop / Visual Studio for Mac. DO NOT DO SO!
# NOTE: This template is TOTALLY NOT GENERIC!!!!
# This function's arguments are:
#   $(1) : the friendly name of the product
#   $(2) : the product copyright date
#   $(3) : the short version number
#   $(4) : the user-friendly build string
#   $(5) : the macOS minimum OS deployment target version
# ------------------------------------------------------------------------- #
define EmitInfoPList
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>LSEnvironment</key>
	<dict>
		<key>VINTAGE_PORT_NOTIFIER_KIND</key>
		<string>0</string>
		<key>LUI_SHOW_SPLASH_SCREEN</key>
		<string>Yes</string>
		<key>LUI_ENABLE_MENU_LAYOUT_MULTISELECT</key>
		<string>Yes</string>
	</dict>
	<key>NSHumanReadableCopyright</key>
	<string>Copyright © 2014-$(2) Steven A. Orth
All Rights Reserved.</string>
	<key>CFBundleExecutable</key>
	<string></string>
	<key>CFBundleName</key>
	<string>$(1)</string>
	<key>CFBundleVersion</key>
	<string>$(4)</string>
	<key>LSMinimumSystemVersion</key>
	<string>$(5)</string>
	<key>NSMainNibFile</key>
	<string>MainMenu</string>
	<key>NSPrincipalClass</key>
	<string>SingleInstanceApplication</string>
	<key>CFBundleDisplayName</key>
	<string>$(1)</string>
	<key>CFBundleShortVersionString</key>
	<string>$(3)</string>
	<key>CFBundleIdentifier</key>
	<string>com.intvfunhouse.ltoflash</string>
	<key>CFBundleIconFile</key>
	<string>LTOFlashApplicationIcon</string>
</dict>
</plist>
endef

# ------------------------------------------------------------------------- #
# INFO_PLIST_CONTENT contains the Info.plist content. Because versions of GNU
# make prior to 4.0 do not support the 'file' function, export the contents
# to an environment variable, which is then consumed by update_info_plist.
# ------------------------------------------------------------------------- #
INFO_PLIST_CONTENT = $(call EmitInfoPList,$(PRODUCT_NAME_FRIENDLY),$(COPYRIGHT_DATE),$(VERSION_SHORT),$(BUILD_REVISION),$(MACOS_DEPLOYMENT_TARGET))
export INFO_PLIST_CONTENT

# ------------------------------------------------------------------------- #
# Rule: update_info_plist
# ------------------------------------------------------------------------- #
# This rule will generate an Info.plist file for an application.
# ------------------------------------------------------------------------- #
update_info_plist:
ifneq ($(CURR_SVN_REVISION),$(PREV_SVN_INFO_PLIST_REVISION))
	@echo Generating '$(INFO_PLIST_PATH)'$(USING_GIT_NOTE) ...
	@echo "$$INFO_PLIST_CONTENT" > $(INFO_PLIST_PATH)
	@echo $(CURR_SVN_REVISION) > $(INFO_PLIST_UPDATE_STATUS_FILE)
else
	@echo '$(INFO_PLIST_PATH)' is up-to-date.
endif

  endif

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# Everything in one convenient place.
# ------------------------------------------------------------------------- #
all: update_version_cs update_info_plist

else

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# Everything in one convenient place.
# ------------------------------------------------------------------------- #
all: update_version_cs

endif

endif

include custom_mak_rule.mak
