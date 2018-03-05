#############################################################################
# Build File for Creating DMG Files                                         #
# ------------------------------------------------------------------------- #
# The DMG packager looks for a template.dmg.bz2 for using as its            #
# DMG template. If it doesn't find one, it generates a clean one.           #
#                                                                           #
# If you create a DMG template, you should make one containing all          #
# the files listed in $(SOURCE_FILES) below, and arrange everything to suit #
# your style. The contents of the files themselves does not matter, so      #
# they can be empty (they will be overwritten later).                       #
#                                                                           #
# Remko Tronçon                                                             #
# https://el-tramo.be                                                       #
# Licensed under the MIT License. See COPYING for details.                  #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# This makefile is in a subdirectory, so set ROOT_DIR accordingly.
# ------------------------------------------------------------------------- #
ROOT_DIR = ../..

include $(ROOT_DIR)/version.mak

# ------------------------------------------------------------------------- #
# Customizable Variables                                                    #
# ------------------------------------------------------------------------- #
# The rules to create the DMG file require a few variables to be defined:
#
#   PRODUCT_NAME  : the base of the .dmg file name
#   VERSION       : the full version of the product defined via version.mak
#   SOURCE_DIR    : location of the program to place on the DMG
#   SOURCE_FILES  : files to include, located in SOURCE_DIR
#   TEMPLATE_DMG  : the template DMG file to use
#   TEMPLATE_SIZE : the size of the template DMG
# ------------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# Set the base name of the DMG to be created.
# ------------------------------------------------------------------------- #
PRODUCT_NAME ?= LTOFlash

# ------------------------------------------------------------------------- #
# If there are modified files, indicate this in the file name.
# ------------------------------------------------------------------------- #
ifneq ($(SVN_DIRTY),0)
  # ----------------------------------------------------------------------- #
  # Set this variable to modify the output file name indicating it's not
  # from pristine sources.
  # ----------------------------------------------------------------------- #
  LOCAL_MODS = -$(SVN_DIRTY)m
endif

# ------------------------------------------------------------------------- #
# If version has not been set, do so, using an obviously bogus value.
# ------------------------------------------------------------------------- #
VERSION ?= 1.0.0.undefined

# ------------------------------------------------------------------------- #
# If not already assigned, set the source directory for files to copy.
# ------------------------------------------------------------------------- #
SOURCE_DIR ?= ../LtoFlash/bin/Release/Mac

# ------------------------------------------------------------------------- #
# If not already assigned, set the source files.
# ------------------------------------------------------------------------- #
SOURCE_FILES ?= LtoFlash.app README.Mac.txt

# ------------------------------------------------------------------------- #
# If not already assigned, set the template DMG file name.
# ------------------------------------------------------------------------- #
TEMPLATE_DMG ?= template.dmg

# ------------------------------------------------------------------------- #
# If not already assigned, set the template DMG size.
# ------------------------------------------------------------------------- #
TEMPLATE_SIZE ?= 40m

# ========================================================================= #
# DMG building. No editing should be needed beyond this point.              #
# ------------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# Name of the disk image (DMG) we're creating.
# ------------------------------------------------------------------------- #
MASTER_DMG=$(PRODUCT_NAME)-$(VERSION)$(LOCAL_MODS).dmg

# ------------------------------------------------------------------------- #
# Name of the zip file for the DMG that gets created.
# ------------------------------------------------------------------------- #
MASTER_ZIP=$(PRODUCT_NAME)-Mac-$(VERSION)$(LOCAL_MODS).zip

# ------------------------------------------------------------------------- #
# Name for the working copy DMG.
# ------------------------------------------------------------------------- #
WC_DMG=wc.dmg

# ------------------------------------------------------------------------- #
# Name for the working copy directory.
# ------------------------------------------------------------------------- #
WC_DIR=wc

# ------------------------------------------------------------------------- #
# Rule: .PHONY
# ------------------------------------------------------------------------- #
# Wow, all is phony? Who'd a thunk it?
# ------------------------------------------------------------------------- #
.PHONY: all

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# It's all here.
# ------------------------------------------------------------------------- #
all: $(MASTER_ZIP)

# ------------------------------------------------------------------------- #
# Rule: TEMPLATE_DMG
# ------------------------------------------------------------------------- #
# Unzips a bzip2-zipped template to a template DMG.
# ------------------------------------------------------------------------- #
$(TEMPLATE_DMG): $(TEMPLATE_DMG).bz2
	bunzip2 -k $<

# ------------------------------------------------------------------------- #
# Rule: TEMPLATE_DMG.bz2
# ------------------------------------------------------------------------- #
# Generates a zipped empty template (using bzip2).
# ------------------------------------------------------------------------- #
$(TEMPLATE_DMG).bz2: 
	@echo
	@echo --------------------- Generating empty template --------------------
	mkdir template
	hdiutil create -fs HFSX -layout SPUD -size $(TEMPLATE_SIZE) "$(TEMPLATE_DMG)" -srcfolder template -format UDRW -volname "$(PRODUCT_NAME)" -quiet
	rmdir template
	bzip2 "$(TEMPLATE_DMG)"
	@echo

# ------------------------------------------------------------------------- #
# Rule: WC_DMG
# ------------------------------------------------------------------------- #
# Creates a working copy of the template DMG.
# ------------------------------------------------------------------------- #
$(WC_DMG): $(TEMPLATE_DMG)
	cp $< $@

# ------------------------------------------------------------------------- #
# Rule: MASTER_DMG
# ------------------------------------------------------------------------- #
# This rule creates the master disk image from an existing working copy of
# the template DMG. First, it mounts the working copy image. It then copies
# the contents specified by $(SOURCE_FILES) into the mounted location. The
# operation completes by creating a new DMG from the mounted working copy
# directory.
# ------------------------------------------------------------------------- #
$(MASTER_DMG): $(WC_DMG) $(addprefix $(SOURCE_DIR)/,$(SOURCE_FILES))
	@echo
	@echo --------------------- Creating Disk Image --------------------
	mkdir -p $(WC_DIR)
	hdiutil attach "$(WC_DMG)" -nobrowse -noverify -ignorebadchecksums -verbose -mountpoint "$(WC_DIR)"
	for i in $(SOURCE_FILES); do  \
		rm -rf "$(WC_DIR)/$$i"; \
		ditto -rsrc "$(SOURCE_DIR)/$$i" "$(WC_DIR)/$$i"; \
	done
	#rm -f "$@"
	#hdiutil create -srcfolder "$(WC_DIR)" -format UDZO -imagekey zlib-level=9 "$@" -volname "$(PRODUCT_NAME) $(VERSION)$(LOCAL_MODS)" -scrub -quiet
	WC_DEV=`hdiutil info | grep "$(WC_DIR)" | grep "Apple_HFS" | awk '{print $$1}'` && \
	hdiutil detach $$WC_DEV -quiet -force
	rm -f "$(MASTER_DMG)"
	hdiutil convert "$(WC_DMG)" -quiet -format UDZO -imagekey zlib-level=9 -o "$@"
	rm -rf $(WC_DIR)
	@echo

# ------------------------------------------------------------------------- #
# Rule: MASTER_ZIP
# ------------------------------------------------------------------------- #
# This rule generates a ZIP file containing the disk image.
# ------------------------------------------------------------------------- #
$(MASTER_ZIP): $(MASTER_DMG)
	@echo ----------------- Creating ZIP of Disk Image -----------------
	rm -f "$(MASTER_ZIP)"
	zip "$@" "$<"
	@echo

# ------------------------------------------------------------------------- #
# Rule: .PHONY
# ------------------------------------------------------------------------- #
# Declare clean as a phony target.
# ------------------------------------------------------------------------- #
.PHONY: clean

# ------------------------------------------------------------------------- #
# Rule: clean
# ------------------------------------------------------------------------- #
# Remove intermediate and output files. Note that because we generate the
# names based on a computed value for the version (see version.mak), this
# rule will not clean up things if you build, edit a new file, and then
# make clean. There are myriad ways this can happen. So clean, but verify.
# ------------------------------------------------------------------------- #
clean:
	-rm -rf $(TEMPLATE_DMG) $(MASTER_DMG) $(WC_DMG) $(MASTER_ZIP)

# ------------------------------------------------------------------------- #
# Get the rule to generate custom.mak if needed.
# ------------------------------------------------------------------------- #
include $(ROOT_DIR)/custom_mak_rule.mak
