#############################################################################
# Really, Really Clean Up                                                   #
# ------------------------------------------------------------------------- #
# This makefile simply offers a convenience mechanism to clean up what      #
# Xamarin Studio / Visual Studio for Mac won't. Heck, maybe regular old     #
# Visual Studio on Windows and MonoDevelop don't either.                    #
#                                                                           #
# Licensed under the GPL License. See license.txt for details.              #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# Source directory we're going to visit.
# ------------------------------------------------------------------------- #
SOURCE_DIR ?= .

# ------------------------------------------------------------------------- #
# Target: .PHONY
# ------------------------------------------------------------------------- #
# No prerequisites for all, so it's phony.
# ------------------------------------------------------------------------- #
.PHONY: all

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# This rule deletes the bin and obj folders for all the components. Yep,
# it's all manually maintained, so if a project is added, this needs fixin'.
# ------------------------------------------------------------------------- #
all: FORCE
	@echo
	@echo ---------------------------- Cleaning Output ---------------------------
	-rm -rf $(SOURCE_DIR)/INTV.Core/bin
	-rm -rf $(SOURCE_DIR)/INTV.Core/obj
	-rm -rf $(SOURCE_DIR)/INTV.Intellicart/bin
	-rm -rf $(SOURCE_DIR)/INTV.Intellicart/obj
	-rm -rf $(SOURCE_DIR)/INTV.jzIntv/bin
	-rm -rf $(SOURCE_DIR)/INTV.jzIntv/obj
	-rm -rf $(SOURCE_DIR)/INTV.LtoFlash/bin
	-rm -rf $(SOURCE_DIR)/INTV.LtoFlash/obj
	-rm -rf $(SOURCE_DIR)/INTV.Ribbon/bin
	-rm -rf $(SOURCE_DIR)/INTV.Ribbon/obj
	-rm -rf $(SOURCE_DIR)/INTV.Shared/bin
	-rm -rf $(SOURCE_DIR)/INTV.Shared/obj
	-rm -rf $(SOURCE_DIR)/Locutus/LtoFlash/bin
	-rm -rf $(SOURCE_DIR)/Locutus/LtoFlash/obj
	-rm -rf $(SOURCE_DIR)/YAML/Abstract/bin
	-rm -rf $(SOURCE_DIR)/YAML/Abstract/obj
	-rm -rf $(SOURCE_DIR)/YAML/Core/bin
	-rm -rf $(SOURCE_DIR)/YAML/Core/obj
	-rm -rf $(SOURCE_DIR)/WindowsAPICodePack/Core/bin
	-rm -rf $(SOURCE_DIR)/WindowsAPICodePack/Core/obj
	-rm -rf $(SOURCE_DIR)/WindowsAPICodePack/Shell/bin
	-rm -rf $(SOURCE_DIR)/WindowsAPICodePack/Shell/obj

FORCE:
