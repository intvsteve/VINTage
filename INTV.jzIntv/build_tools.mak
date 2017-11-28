#############################################################################
## jzIntv Tools Build
##
## When properly configured via custom.mak, builds the utilities used by
## the INTV.jzIntv assembly that is part of the VINTage / LUI project.
##
#############################################################################

include custom.mak

# Determine the target OS. Modified from the technique found here:
#  http://stackoverflow.com/questions/714100/os-detecting-makefile
ifeq ($(OS),Windows_NT)
  TARGET_OS ?= WIN
else
  UNAME_S := $(shell uname -s)
  ifeq ($(UNAME_S),Linux)
    TARGET_OS ?= LINUX
  endif
  ifeq ($(UNAME_S),Darwin)
    TARGET_OS ?= MAC
  endif
endif

ifeq (,$(TARGET_OS))
  $(error Unable to determine target operating system! Please specify TARGET_OS variable)
endif

# Define the makefile to use.
ifeq (WIN,$(TARGET_OS))
  TARGET_MAKEFILE ?= Makefile.stdout
endif
ifeq (MAC,$(TARGET_OS))
  TARGET_MAKEFILE ?= Makefile.osx_framework
endif
ifeq (LINUX,$(TARGET_OS))
  TARGET_MAKEFILE ?= 
endif

ifeq (,$(TARGET_MAKEFILE))
  $(error Unable to determine target makefile! Please specify TARGET_MAKEFILE variable)
endif

ifeq (WIN,$(TARGET_OS))
  TOOL_OUTPUT_DIR = tools
  EXE_SUFFIX = .exe
endif
ifeq (MAC,$(TARGET_OS))
  TOOL_OUTPUT_DIR = tools/Mac
endif
ifeq (LINUX,$(TARGET_OS))
  TOOL_OUTPUT_DIR = tools/Linux
endif

# Directory containing output of jzIntv builds.
TOOL_INPUT_DIR = $(JZINTV_DIR)/bin

# The jzIntv utilities to be built and included in the INTV.jzIntv output.
JZINTV_UTILITIES = \
  bin2luigi \
  bin2rom \
  intvname \
  luigi2bin \
  rom_merge \
  rom2bin \
  rom2luigi

# The actual executable names.
JZINTV_APPS = $(addsuffix $(EXE_SUFFIX), $(JZINTV_UTILITIES))

# IF SKIP_BUILD == 1, do nothing.
ifneq (1,$(SKIP_BUILD))

# If custom.mak defines an additional environment PATH data, prepend it to existing.
ifneq (,$(ADD_ENVIRONMENT_PATH))
  export PATH := $(ADD_ENVIRONMENT_PATH):$(PATH)
endif

# BUILD ALL THE THINGS!
all: $(addprefix $(TOOL_OUTPUT_DIR)/, $(JZINTV_APPS))

# This function defines a rule for a jzIntv utility to be included in the tools directory.
define CreateToolPrerequisiteRule
$(addprefix $(TOOL_OUTPUT_DIR)/,$(1)): $(addprefix $(TOOL_INPUT_DIR)/,$(1))
	@echo Updating $(1)...
	@echo Moving $$^ to $$@
	@mv -f $$^ $$@
	@echo

endef

# Declare the targets for the utilities to put into the appropriate tools directory.
$(foreach tool,$(JZINTV_APPS),$(eval $(call CreateToolPrerequisiteRule,$(tool))))

# This function defines a rule for a specific jzIntv build target that is the
# prerequisite for the utility to place in the appropriate INTV.jzIntv tools directory.
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

# Declare the targets in the jzIntv build.
$(foreach tool,$(JZINTV_APPS),$(eval $(call CreateBuildToolRule,$(tool))))

# Clean the output of the jzIntv build. NUKE THE MOON!
clean:
	@echo Cleaning jzIntv...
	make -C $(JZINTV_DIR)/src -f $(TARGET_MAKEFILE) clean
	@echo

else

# No-op the build.
all:
	@echo build_tools.mak: Skipped jzIntv Tools build.

# No-op the clean.
clean:
	@echo build_tools.mak: Skipped jzIntv Tools clean.

endif
