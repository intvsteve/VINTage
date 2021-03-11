#############################################################################
# jzIntv Tools Build                                                        #
# ------------------------------------------------------------------------- #
# When properly configured via custom.mak and custom_jzintv.mak, produces   #
# the tools used by the INTV.jzIntv assembly that is part of the VINTage /  #
# LUI project. The root common.mak file determines the operating system.    #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# This makefile is in a subdirectory, so set ROOT_DIR accordingly.
# ------------------------------------------------------------------------- #
ROOT_DIR = ..

include $(ROOT_DIR)/common.mak
-include $(ROOT_DIR)/custom_jzIntv.mak

# ------------------------------------------------------------------------- #
# Define the destination directory to use for the tool executables.
# ------------------------------------------------------------------------- #
TOOL_OUTPUT_DIR_WIN   = tools
TOOL_OUTPUT_DIR_MAC   = tools/Mac
TOOL_OUTPUT_DIR_LINUX = tools/Linux
TOOL_OUTPUT_DIR       = $(TOOL_OUTPUT_DIR_$(TARGET_OS))

# ------------------------------------------------------------------------- #
# Validate configuration for tools build.
# ------------------------------------------------------------------------- #
ifeq (,$(TOOL_OUTPUT_DIR))
  $(error Set the TOOL_OUTPUT_DIR_$(TARGET_OS) variable appropriately)
endif

# ------------------------------------------------------------------------- #
# Define the directory that will contain the output of jzIntv tool build.
# ------------------------------------------------------------------------- #
TOOL_INPUT_DIR = $(JZINTV_DIR)/bin

# ------------------------------------------------------------------------- #
# Define the prerequisite for producing the tools. By default assume local
# source build. If a jzIntv distribution is used, this is updated later.
# ------------------------------------------------------------------------- #
TOOL_INPUT_PREREQ = $(addprefix $(JZINTV_DIR)/src/,$(TARGET_MAKEFILE))

# ------------------------------------------------------------------------- #
# Declare jzIntv tools to build and include in the INTV.jzIntv output.
# ------------------------------------------------------------------------- #
JZINTV_UTILITIES = \
  bin2luigi \
  bin2rom \
  intvname \
  luigi2bin \
  rom_merge \
  rom2bin \
  rom2luigi

# ------------------------------------------------------------------------- #
# Define the actual tool executable names. $(EXE_SUFFIX) is in common.mak.
# ------------------------------------------------------------------------- #
JZINTV_APPS = $(addsuffix $(EXE_SUFFIX), $(JZINTV_UTILITIES))

# ------------------------------------------------------------------------- #
# If SKIP_BUILD == 1, do nothing. Otherwise, carry on!
# ------------------------------------------------------------------------- #
ifneq (1,$(SKIP_BUILD))

# ------------------------------------------------------------------------- #
# Set up variables if pulling from distribution or building locally. If a
# distribution is used rather than a build directly from source, modify any
# existing variables as needed and set up those required for distribution.
# ------------------------------------------------------------------------- #
ifneq (,$(JZINTV_DIST_URL))
  USING_JZINTV_DIST       = 1
  JZINTV_DOWNLOAD_TARGET  = $(JZINTV_DIR)/$(JZINTV_DIST_FILENAME)
  JZINTV_UNZIP_TARGET_DIR = $(basename $(JZINTV_DIST_FILENAME))
  TOOL_INPUT_DIR          = $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)/bin
  TOOL_INPUT_PREREQ       = $(JZINTV_DOWNLOAD_TARGET)
endif

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# BUILD ALL THE THINGS!
# ------------------------------------------------------------------------- #
all: $(addprefix $(TOOL_OUTPUT_DIR)/, $(JZINTV_APPS))

# ------------------------------------------------------------------------- #
# Define the rule to download the distribution if necessary.
# ------------------------------------------------------------------------- #
ifeq (1,$(USING_JZINTV_DIST))
$(JZINTV_DOWNLOAD_TARGET):
	@echo Retrieving jzIntv Distribution: $@ ...
	@curl -o $@ $(JZINTV_DIST_URL)

endif

# ------------------------------------------------------------------------- #
# Function: CreateToolPrerequisiteRule
# ------------------------------------------------------------------------- #
# This function defines a rule for a jzIntv tool to be copied to the
# INTV.jzIntv/tools directory. It requires the following arguments:
#   $(1) : the tool whose copy-to-INTV.jzIntv rule is to be declared
# ------------------------------------------------------------------------- #
define CreateToolPrerequisiteRule
$(addprefix $(TOOL_OUTPUT_DIR)/,$(1)): $(addprefix $(TOOL_INPUT_DIR)/,$(1))
	@echo Updating $(1)...
	@echo Copying $$^ to $$@
	@mkdir -p $$(dir $$@)
	@cp -fp $$^ $$@
	@echo

endef

# ------------------------------------------------------------------------- #
# Declare the targets for the jzIntv tools to be built and put into the
# appropriate tools directory.
# ------------------------------------------------------------------------- #
$(foreach tool,$(JZINTV_APPS),$(eval $(call CreateToolPrerequisiteRule,$(tool))))

# ------------------------------------------------------------------------- #
# Function: CreateBuildToolRule
# ------------------------------------------------------------------------- #
# This function defines a rule for a specific jzIntv build target that acts
# as the prerequisite for the tool to copy into the appropriate INTV.jzIntv
# tools directory.
# If SYNC_JZINTV is defined, the jzIntv source is synchronized first.
#
# If pulling from a distribution, the prerequisite rule will fetch the
# distribution as needed.
#
# This function requires the following arguments:
#   $(1) : the jzIntv tool to be built
# ------------------------------------------------------------------------- #
define CreateBuildToolRule
$(addprefix $(TOOL_INPUT_DIR)/,$(1)): $(TOOL_INPUT_PREREQ)
ifeq (,$(USING_JZINTV_DIST))
ifneq (,$(SYNC_JZINTV))
	@echo Syncing $(JZINTV_DIR) ...
	$(SYNC_JZINTV)
endif
	@echo Building $(1)...
	make -C $(JZINTV_DIR)/src -f $$(notdir $$^) ../bin/$(1)
	strip $$@
	@echo
else
	@echo Extracting $$@ from $$<
	@unzip -qquo $$< $(JZINTV_UNZIP_TARGET_DIR)/bin/$(1) -d $$(dir $$<)
endif

endef

# ------------------------------------------------------------------------- #
# Declare the targets to be built by the jzIntv build or extracted from dist.
# ------------------------------------------------------------------------- #
$(foreach tool,$(JZINTV_APPS),$(eval $(call CreateBuildToolRule,$(tool))))

# ------------------------------------------------------------------------- #
# Rule: clean
# ------------------------------------------------------------------------- #
# Clean the output of the jzIntv build. NUKE THE MOON!
# ------------------------------------------------------------------------- #
clean:
	@echo Cleaning jzIntv tools ...
ifeq (,$(USING_JZINTV_DIST))
	make -C $(JZINTV_DIR)/src -f $(TARGET_MAKEFILE) clean
else
ifeq (1,$(JZINTV_CLEAN_DIST))
	@echo Removing $(notdir $(JZINTV_DOWNLOAD_TARGET)) in $(dir $(JZINTV_DOWNLOAD_TARGET))
	@rm -f $(JZINTV_DOWNLOAD_TARGET)
	@echo Removing jzIntv tools previously extracted from $(notdir $(JZINTV_DOWNLOAD_TARGET)) ...
	@rm -f $(addprefix $(TOOL_INPUT_DIR)/, $(JZINTV_APPS))
	@rm -frd $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)
endif
endif
	@echo Cleaning tools from $(CURDIR)/$(TOOL_OUTPUT_DIR) ...
	@rm -f $(addprefix $(TOOL_OUTPUT_DIR)/, $(JZINTV_APPS))
	@echo

else
# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# No-op the build.
# ------------------------------------------------------------------------- #
all:
	@echo build_tools.mak: Skipped jzIntv Tools build.

# ------------------------------------------------------------------------- #
# Rule: clean:
# ------------------------------------------------------------------------- #
# No-op the clean.
# ------------------------------------------------------------------------- #
clean:
	@echo build_tools.mak: Skipped jzIntv Tools clean.

endif

# ------------------------------------------------------------------------- #
# Get the rule to generate custom_jzIntv.mak if needed.
# ------------------------------------------------------------------------- #
include $(ROOT_DIR)/custom_jzIntv_mak_rule.mak
