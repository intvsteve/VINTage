#############################################################################
# jzIntv Build                                                              #
# ------------------------------------------------------------------------- #
# When properly configured via custom.mak, builds the jzIntv emulator and   #
# places the minimum required output files needed for including a copy of   #
# jzIntv in the LUI / VINTage project.                                      #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# This makefile is in a subdirectory, so set ROOT_DIR accordingly.
# ------------------------------------------------------------------------- #
ROOT_DIR = ..

include $(ROOT_DIR)/common.mak
-include $(ROOT_DIR)/custom_jzIntv.mak

# ------------------------------------------------------------------------- #
# Define the target directory to put jzIntv into.
# ------------------------------------------------------------------------- #
TARGET_DIR_WIN   = jzIntv/Win
TARGET_DIR_MAC   = jzIntv/Mac
TARGET_DIR_LINUX = jzIntv/Linux
TARGET_DIR       = $(TARGET_DIR_$(TARGET_OS))

# ------------------------------------------------------------------------- #
# Validate configuration for the jzIntv build.
# ------------------------------------------------------------------------- #
ifeq (,$(TARGET_DIR))
  $(error Set the TARGET_DIR_$(TARGET_OS) variable appropriately)
endif

# ------------------------------------------------------------------------- #
# Directory containing output of jzIntv builds.
# ------------------------------------------------------------------------- #
BUILD_OUTPUT_DIR = $(JZINTV_DIR)/bin

# ------------------------------------------------------------------------- #
# Define files required for jzIntv to run, readme files, et. al.
# ------------------------------------------------------------------------- #
JZINTV_SUPPORT_FILES_WIN   = bin/SDL.dll
JZINTV_SUPPORT_FILES_MAC   =
JZINTV_SUPPORT_FILES_LINUX =
JZINTV_SUPPORT_FILES = $(JZINTV_SUPPORT_FILES_$(TARGET_OS)) \
                       doc/jzintv \
                       COPYING.txt \
                       README.txt \
                       Release-Notes.txt

# ------------------------------------------------------------------------- #
# The jzIntv executable to be built and included in the INTV.jzIntv output.
# ------------------------------------------------------------------------- #
JZINTV_EXECUTABLE = jzintv

# ------------------------------------------------------------------------- #
# The actual executable file name.
# ------------------------------------------------------------------------- #
JZINTV_APP = $(addsuffix $(EXE_SUFFIX), $(JZINTV_EXECUTABLE))

# ------------------------------------------------------------------------- #
# If SKIP_BUILD == 1, do nothing.
# ------------------------------------------------------------------------- #
ifneq (1,$(SKIP_BUILD))

  # ----------------------------------------------------------------------- #
  # If CONTENT_DIR is defined, set CONTENT_COPY_FILES to be the collection
  # of jzIntv files to be copied to the CONTENT_DIR.
  # ----------------------------------------------------------------------- #
  ifneq (,$(CONTENT_DIR))
    # ----------------------------------------------------------------------- #
    # Declare the jzintv executable and support files to be copied.
    # ----------------------------------------------------------------------- #
    CONTENT_COPY_FILES = $(addprefix $(CONTENT_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES))
  endif

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# BUILD ALL THE THINGS!
# ------------------------------------------------------------------------- #
all: $(addprefix $(TARGET_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES)) $(CONTENT_COPY_FILES)

# ------------------------------------------------------------------------- #
# Function: CopySupportFileRule
# ------------------------------------------------------------------------- #
# This function defines a rule to copy a specific jzIntv support file from
# the jzIntv built output to TARGET_DIR. It requires the following arguments:
#   $(1) : the support file to be copied
# ------------------------------------------------------------------------- #
define CopySupportFileRule
$(TARGET_DIR)/$(1) : $(JZINTV_DIR)/$(1)
	@echo Copying $(1)...
	cp -fpR $$^ $$(dir $$@)
	@echo

endef

# ------------------------------------------------------------------------- #
# Declare the targets for the support files to copy into the appropriate
# output directory.
# ------------------------------------------------------------------------- #
$(foreach f,$(JZINTV_SUPPORT_FILES),$(eval $(call CopySupportFileRule,$(f))))

# ------------------------------------------------------------------------- #
# Rule: Copy jzintv executable
# ------------------------------------------------------------------------- #
# This rule copies the jzintv executable from the jzintv build output to
# the defined target directory.
# ------------------------------------------------------------------------- #
$(TARGET_DIR)/bin/$(JZINTV_APP) : $(BUILD_OUTPUT_DIR)/$(JZINTV_APP)
	@echo Copying $^...
	cp -fp $^ $(dir $@)
	@echo

# ------------------------------------------------------------------------- #
# Rule: Build jzIntv
# ------------------------------------------------------------------------- #
# This rule invokes the jzIntv build at its source. If SYNC_JZINTV is
# defined, the jzIntv source is synchronized first.
# ------------------------------------------------------------------------- #
$(BUILD_OUTPUT_DIR)/$(JZINTV_APP) : $(JZINTV_DIR)/src/$(TARGET_MAKEFILE)
ifneq (,$(SYNC_JZINTV))
	@echo Syncing $(JZINTV_DIR) ...
	$(SYNC_JZINTV)
endif
	@echo Building jzIntv...
	make -C $(JZINTV_DIR)/src -f $(notdir $^) ../bin/$(JZINTV_APP)
	strip $@
	@echo

  # ----------------------------------------------------------------------- #
  # If CONTENT_DIR is defined, the rules necessary to copy files to the
  # content directory will be defined.
  # ----------------------------------------------------------------------- #
  ifneq (,$(CONTENT_DIR))

# ------------------------------------------------------------------------- #
# Function: CopyContentSupportFileRule
# ------------------------------------------------------------------------- #
# This function defines a rule for a specific jzIntv support file that is to
# be included as part of the output of the build. On Mac, for some reason
# copying a directory w/o creating the target directory first results in the
# content of source being copied rather than the directory itself... The
# source (doc/jzintv) does not have a trailing /, so it  seems odd that the
# resulting observed behavior happens... It works fine in the above rule that
# copies the files into the project-relative location, after all... Anyhow,
# this function requires the following arguments:
#   $(1) : the content file to copy
# ------------------------------------------------------------------------- #
define CopyContentSupportFileRule
$(CONTENT_DIR)/$(1) : $(TARGET_DIR)/$(1)
	@echo Copying to Content $(1)...
	mkdir -p $$(dir $$@)
	cp -fpR $$^ $$(dir $$@)
	@echo

endef

# ------------------------------------------------------------------------- #
# Declare the targets for the support files to put into the appropriate
# output directory.
# ------------------------------------------------------------------------- #
$(foreach f,$(JZINTV_SUPPORT_FILES),$(eval $(call CopyContentSupportFileRule,$(f))))

# ------------------------------------------------------------------------- #
# Ruile: Copy jzintv executable to content directory
# ------------------------------------------------------------------------- #
# This rule copies the jzIntv emulator executable to the content directory.
# ------------------------------------------------------------------------- #
$(CONTENT_DIR)/bin/$(JZINTV_APP) : $(TARGET_DIR)/bin/$(JZINTV_APP)
	@echo Copying to Content $^...
	mkdir -p $(dir $@)
	cp -fp $^ $(dir $@)
	@echo

endif

# ------------------------------------------------------------------------- #
# Rule: clean
# ------------------------------------------------------------------------- #
# Clean the output of the jzIntv build. NUKE THE MOON!
# ------------------------------------------------------------------------- #
clean:
	@echo Cleaning jzIntv...
	make -C $(JZINTV_DIR)/src -f $(TARGET_MAKEFILE) clean
	rm -rf $(TARGET_DIR)/bin/$(JZINTV_APP) $(addprefix $(TARGET_DIR)/,$(JZINTV_SUPPORT_FILES)) $(CONTENT_DIR)
	@echo

# ------------------------------------------------------------------------- #
# Target: .PHONY
# ------------------------------------------------------------------------- #
# Use the .PHONY target to 
# ------------------------------------------------------------------------- #
#.PHONY: $(addprefix $(TARGET_DIR)/,$(JZINTV_SUPPORT_FILES)) $(CONTENT_COPY_FILES)

else

# No-op the build.
all:
	@echo build_jzIntv.mak: Skipped jzIntv build.

# No-op the clean.
clean:
	@echo build_jzIntv.mak: Skipped jzIntv clean.

endif

# ------------------------------------------------------------------------- #
# Get the rule to generate custom_jzIntv.mak if needed.
# ------------------------------------------------------------------------- #
include $(ROOT_DIR)/custom_jzIntv_mak_rule.mak
