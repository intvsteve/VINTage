#############################################################################
## jzIntv Build
##
## When properly configured via custom.mak, builds the jzIntv emulator and
## places the minimum required output files needed for including a copy of
## jzIntv in the LUI / VINTage project.
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

# Support files required for jzIntv to run, readme files, et. al.
JZINTV_SUPPORT_FILES = 

# Define the makefile to use.
ifeq (WIN,$(TARGET_OS))
  TARGET_MAKEFILE ?= Makefile.stdout
  JZINTV_SUPPORT_FILES += bin/SDL.dll
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
  TARGET_DIR = jzIntv/Win
  EXE_SUFFIX = .exe
endif
ifeq (MAC,$(TARGET_OS))
  TARGET_DIR = jzIntv/Mac
endif
ifeq (LINUX,$(TARGET_OS))
  TARGET_DIR = jzIntv/Linux
endif

JZINTV_DIR = $(JZINTV_DIR_$(TARGET_OS))
ADD_ENVIRONMENT_PATH = $(ADD_ENVIRONMENT_PATH_$(TARGET_OS))

## ----------------------------- Validation ------------------------------ ##

ifeq (,$(JZINTV_DIR))
  ifneq (1,$(SKIP_IF_JZINTV_EMPTY))
    $(error Set the JZINTV_DIR variable to the directory containing the jzIntv src directory)
  else
    SKIP_BUILD = 1
  endif
endif

# Directory containing output of jzIntv builds.
BUILD_OUTPUT_DIR = $(JZINTV_DIR)/bin

# The jzIntv executable to be built and included in the INTV.jzIntv output.
JZINTV_EXECUTABLE = jzintv

# Append common support files.
JZINTV_SUPPORT_FILES += doc/jzintv COPYING.txt README.txt Release-Notes.txt

# The actual executable name.
JZINTV_APP = $(addsuffix $(EXE_SUFFIX), $(JZINTV_EXECUTABLE))

# IF SKIP_BUILD == 1, do nothing.
ifneq (1,$(SKIP_BUILD))

# If custom.mak defines an additional environment PATH data, prepend it to existing.
ifneq (,$(ADD_ENVIRONMENT_PATH))
  export PATH := $(ADD_ENVIRONMENT_PATH):$(PATH)
endif

ifneq (,$(CONTENT_DIR))
  CONTENT_COPY_FILES = $(addprefix $(CONTENT_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES))
endif

# BUILD ALL THE THINGS!
all: $(addprefix $(TARGET_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES)) $(CONTENT_COPY_FILES)

# This function defines a rule for a specific jzIntv support file that is to be included
# as part of the output of the build.
define CopySupportFileRule
$(TARGET_DIR)/$(1) : $(JZINTV_DIR)/$(1)
	@echo Copying $(1)...
	cp -fpR $$^ $$(dir $$@)
	@echo

endef

# Declare the targets for the support files to put into the appropriate output directory.
$(foreach f,$(JZINTV_SUPPORT_FILES),$(eval $(call CopySupportFileRule,$(f))))

$(TARGET_DIR)/bin/$(JZINTV_APP) : $(BUILD_OUTPUT_DIR)/$(JZINTV_APP)
	@echo Copying $^...
	cp -fp $^ $(dir $@)
	@echo

$(BUILD_OUTPUT_DIR)/$(JZINTV_APP) : $(JZINTV_DIR)/src/$(TARGET_MAKEFILE)
ifneq (,$(SYNC_JZINTV))
	@echo Syncing $(JZINTV_DIR) ...
	$(SYNC_JZINTV)
endif
	@echo Building jzIntv...
	make -C $(JZINTV_DIR)/src -f $(notdir $^) ../bin/$(JZINTV_APP)
	strip $@
	@echo

ifneq (,$(CONTENT_DIR))

# This function defines a rule for a specific jzIntv support file that is to be included
# as part of the output of the build. On Mac, for some reason copying a directory w/o
# creating the target directory first results in the content of source being copied rather
# than the directory itself... The source (doc/jzintv) does not have a trailing /, so it
# seems odd that the resulting observed behavior happens... It works fine in the above
# rule that copies the files into the project-relative location, after all...
define CopyContentSupportFileRule
$(CONTENT_DIR)/$(1) : $(TARGET_DIR)/$(1)
	@echo Copying to Content $(1)...
	mkdir -p $$(dir $$@)
	cp -fpR $$^ $$(dir $$@)
	@echo

endef

# Declare the targets for the support files to put into the appropriate output directory.
$(foreach f,$(JZINTV_SUPPORT_FILES),$(eval $(call CopyContentSupportFileRule,$(f))))

$(CONTENT_DIR)/bin/$(JZINTV_APP) : $(TARGET_DIR)/bin/$(JZINTV_APP)
	@echo Copying to Content $^...
	mkdir -p $(dir $@)
	cp -fp $^ $(dir $@)
	@echo

endif

# Clean the output of the jzIntv build. NUKE THE MOON!
clean:
	@echo Cleaning jzIntv...
	make -C $(JZINTV_DIR)/src -f $(TARGET_MAKEFILE) clean
	rm -rf $(TARGET_DIR)/bin/$(JZINTV_APP) $(addprefix $(TARGET_DIR)/,$(JZINTV_SUPPORT_FILES)) $(CONTENT_DIR)
	@echo

.PHONY: $(addprefix $(TARGET_DIR)/,$(JZINTV_SUPPORT_FILES)) $(CONTENT_COPY_FILES)

else

# No-op the build.
all:
	@echo build_jzIntv.mak: Skipped jzIntv build.

# No-op the clean.
clean:
	@echo build_jzIntv.mak: Skipped jzIntv clean.

endif
