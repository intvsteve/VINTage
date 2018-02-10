#############################################################################
# jzIntv Tools Build                                                        #
# ------------------------------------------------------------------------- #
# When properly configured via custom.mak, builds the utilities used by     #
# the INTV.jzIntv assembly that is part of the VINTage / LUI project.       #
# The root common.mak file determines the operating system.                 #
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
# Define the directory that will contain the output of jzIntv builds.
# ------------------------------------------------------------------------- #
TOOL_INPUT_DIR = $(JZINTV_DIR)/bin

# ------------------------------------------------------------------------- #
# Declare jzIntv utilities to build and include in the INTV.jzIntv output.
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
# Define the actual tool executable names.
# ------------------------------------------------------------------------- #
JZINTV_APPS = $(addsuffix $(EXE_SUFFIX), $(JZINTV_UTILITIES))

# ------------------------------------------------------------------------- #
# If SKIP_BUILD == 1, do nothing. Otherwise, carry on!
# ------------------------------------------------------------------------- #
ifneq (1,$(SKIP_BUILD))

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# BUILD ALL THE THINGS!
# ------------------------------------------------------------------------- #
all: $(addprefix $(TOOL_OUTPUT_DIR)/, $(JZINTV_APPS))

# ------------------------------------------------------------------------- #
# Function: CreateToolPrerequisiteRule
# ------------------------------------------------------------------------- #
# This function defines a rule for a jzIntv utility to be copied to the
# INTV.jzIntv/tools directory. It requires the following arguments:
#   $(1) : the tool whose copy-to-INTV.jzIntv rule is to be declared
# ------------------------------------------------------------------------- #
define CreateToolPrerequisiteRule
$(addprefix $(TOOL_OUTPUT_DIR)/,$(1)): $(addprefix $(TOOL_INPUT_DIR)/,$(1))
	@echo Updating $(1)...
	@echo Copying $$^ to $$@
	@cp -fp $$^ $$@
	@echo

endef

# ------------------------------------------------------------------------- #
# Declare the targets for the jzIntv utilities to be built and put into the
# appropriate tools directory.
# ------------------------------------------------------------------------- #
$(foreach tool,$(JZINTV_APPS),$(eval $(call CreateToolPrerequisiteRule,$(tool))))

# ------------------------------------------------------------------------- #
# Function: CreateBuildToolRule
# ------------------------------------------------------------------------- #
# This function defines a rule for a specific jzIntv build target that acts
# as the prerequisite for the utility to copy into the appropriate
# INTV.jzIntv tools directory. If SYNC_JZINTV is defined, the jzIntv source
# is synchronized first. This function requires the following arguments:
#   $(1) : the jzIntv tool to be built
# ------------------------------------------------------------------------- #
define CreateBuildToolRule
$(addprefix $(TOOL_INPUT_DIR)/,$(1)) : $(addprefix $(JZINTV_DIR)/src/,$(TARGET_MAKEFILE))
ifneq (,$(SYNC_JZINTV))
	@echo Syncing $(JZINTV_DIR) ...
	$(SYNC_JZINTV)
endif
	@echo Building $(1)...
	make -C $(JZINTV_DIR)/src -f $$(notdir $$^) ../bin/$(1)
	strip $$@
	@echo

endef

# ------------------------------------------------------------------------- #
# Declare the targets to be built by the jzIntv build.
# ------------------------------------------------------------------------- #
$(foreach tool,$(JZINTV_APPS),$(eval $(call CreateBuildToolRule,$(tool))))

# ------------------------------------------------------------------------- #
# Rule: clean
# ------------------------------------------------------------------------- #
# Clean the output of the jzIntv build. NUKE THE MOON!
# ------------------------------------------------------------------------- #
clean:
	@echo Cleaning jzIntv...
	make -C $(JZINTV_DIR)/src -f $(TARGET_MAKEFILE) clean
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
