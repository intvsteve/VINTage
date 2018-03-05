#############################################################################
# Build file for creating source code distribution as a .zip file.          #
# ------------------------------------------------------------------------- #
# Licensed under the GPL License. See license.txt for details.              #
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
# The rules to create the source distribution uses these variables:
#
#   PRODUCT_NAME    : the base of the .zip file name
#   VERSION         : the full version defined via version.mak
#   SVN_REPO        : if specified, used to determine version information
#                     and retrieve source code
#   GIT_REPO        : if specified, used to retrieve source code
#   GIT_REPO_REMOTE : OPTIONAL indicate support for 'svn export' using Git
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

ifneq (,$(GIT_REPO))
  ifneq ($(MAKECMDGOALS),clean)
    $(info ---------------------------- CREATING GIT EXPORT -----------------------------)
  else
    $(info ---------------------------- CLEANING GIT EXPORT -----------------------------)
  endif

  # ----------------------------------------------------------------------- #
  # This assumes that if GIT_REPO_REMOTE == 1, you're referring to a remote
  # GitHub repo, which supports 'svn export' to create a clean export of the
  # remote repo. GitHub doesn't support 'git archive --remote' as of this
  # writing. Otherwise, archive the local repo.
  # ----------------------------------------------------------------------- #
  ifeq ($(GIT_REPO_REMOTE),1)
    SVN_REPO := $(GIT_REPO)
    SOURCE_DIR := $(SVN_REPO)
    GIT_REPO := 
  else
    # --------------------------------------------------------------------- #
    # It's assumed $(CURDIR) is the directory containing this makefile, and that
    # $(GIT_REPO) is a relative path to the desired local repo.
    # --------------------------------------------------------------------- #
    SOURCE_DIR ?= $(abspath $(CURDIR)/$(GIT_REPO))
  endif
else
  ifneq (,$(SVN_REPO))
    ifneq ($(MAKECMDGOALS),clean)
      $(info ----------------------------- CREATING SVN EXPORT ----------------------------)
    else
      $(info ----------------------------- CLEANING SVN EXPORT ----------------------------)
    endif
    SOURCE_DIR = $(SVN_REPO)
  else
    SOURCE_DIR ?= ../..
  endif
endif

# ------------------------------------------------------------------------- #
# This is the legacy approach, which will include all the local junk that may
# have polluted your local copy of the source. Beware! Who knows if this
# still works any more!
# ------------------------------------------------------------------------- #
ifeq (,$(SVN_REPO)$(GIT_REPO))
  ifneq ($(MAKECMDGOALS),clean)
    $(info -------------------------- DIRECT SOURCE EXPORT --------------------------)
  else
    $(info --------------------- CLEANING DIRECT SOURCE EXPORT ----------------------)
  endif
  SOURCE_SUBDIRS ?= \
    Locutus

  SOURCE_DIRS ?=     \
    INTV.Core        \
    INTV.Intellicart \
    INTV.jzIntv      \
    INTV.jzIntvUI    \
    INTV.LtoFlash    \
    INTV.Ribbon      \
    INTV.Shared      \
    Locutus/LtoFlash

  SOURCE_FILES ?=                         \
    cleanup.mak                           \
    common.mak                            \
    custom.mak                            \
    custom_jzIntv.mak                     \
    version_tools.mak                     \
    version.mak                           \
    VersionInfo.cs                        \
    Locutus/license.txt                   \
    Locutus/Locutus.svn.sln               \
    Locutus/Locutus.installer.svn.sln     \
    Locutus/Locutus.Mac.svn.sln           \
    Locutus/Locutus.Mac.installer.svn.sln \
    Locutus/Locutus.xp.svn.sln            \
    Locutus/Locutus.xp.installer.svn.sln  \
    Locutus/README.txt

endif

TARGET_DIR = src

# ------------------------------------------------------------------------- #
# TARGET_ZIP is the eventual target - the source distribution ZIP file.
# ------------------------------------------------------------------------- #
TARGET_ZIP = $(PRODUCT_NAME).source-$(VERSION)$(LOCAL_MODS).zip

.PHONY: all
all: $(TARGET_ZIP)

# ------------------------------------------------------------------------- #
# Implement the rules based on which kind of source repository is being
# processed. Note that this may have started off with a valid GIT_REPO, but
# pulled a switcheroo above to use the 'svn export' command by resetting
# GIT_REPO to empty, and assigning it to SVN_REPO.
# ------------------------------------------------------------------------- #
ifneq (,$(GIT_REPO))
  # --------------------------------------------------------------------- #
  # Implement the rules needed for a straight up export from Git.
  # --------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# Rule: TARGET_ZIP
# ------------------------------------------------------------------------- #
# Implements the rule to export the local Git repo to a ZIP file.
# ------------------------------------------------------------------------- #
$(TARGET_ZIP):
	@echo
	@echo -------------------- Creating Source Distribution --------------------
	@echo ... Exporting from $(SOURCE_DIR) ...
	cd $(SOURCE_DIR); git archive -o $(CURDIR)/$@ HEAD
	@echo

else
  ifneq (,$(SVN_REPO))
    # ----------------------------------------------------------------------- #
    # Looks like we're exporting from a SVN repo. Or, at least we're going to
    # use the 'svn export' command - possibly on a GitHub repo.
    # ----------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# Rule: TARGET_DIR
# ------------------------------------------------------------------------- #
# Implements the rule to export the repo. The repo could be a true SVN repo
# (local or remote), or a (remote) GitHub repo.
# ------------------------------------------------------------------------- #
$(TARGET_DIR):
	@echo
	@echo --------------------- Exporting Source From Repo ---------------------
	@echo ... Exporting from $(SOURCE_DIR) ...
	svn export $(SOURCE_DIR) $(TARGET_DIR)
	@rm -f $(TARGET_DIR)/.gitattributes
	@rm -f $(TARGET_DIR)/.gitignore

# ------------------------------------------------------------------------- #
# Rule: TARGET_ZIP
# ------------------------------------------------------------------------- #
# Implements the rule to zip the export created by the TARGET_DIR rule.
# ------------------------------------------------------------------------- #
$(TARGET_ZIP): $(TARGET_DIR)
	@echo
	@echo -------------------- Creating Source Distribution --------------------
	@echo Zipping contents....
	@zip -rq $@ "$(TARGET_DIR)"
	@echo Zipping complete: $@
	@echo

  else
    # --------------------------------------------------------------------- #
    # Implement the legacy "dump whatever's here locally" rules.
    # --------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# Rule: TARGET_DIR
# ------------------------------------------------------------------------- #
# This rule simply ensures the export directory exists.
# ------------------------------------------------------------------------- #
$(TARGET_DIR):
	@mkdir -p $@

# ------------------------------------------------------------------------- #
# Function: CreateTargetSubDirRule
# ------------------------------------------------------------------------- #
# This function is used to declare a rule to create destination directory
# for source files that are to be included in the export. It requires the
# following parameters:
#   $(1) : a subdirectory containing source files to export
# ------------------------------------------------------------------------- #
define CreateTargetSubDirRule
$(addprefix $(TARGET_DIR)/,$(1)): $(TARGET_DIR)
	@echo
	@echo -------------------- Copying Source --------------------
	@mkdir -p $$^/$(1)

endef

# ------------------------------------------------------------------------- #
# Generate rules to create target subdirectories.
# ------------------------------------------------------------------------- #
$(foreach srcsubdir,$(SOURCE_SUBDIRS),$(eval $(call CreateTargetSubDirRule,$(srcsubdir))))

# ------------------------------------------------------------------------- #
# Function: CreateCopySourceDirRule
# ------------------------------------------------------------------------- #
# This function is used to declare a rule for copying a source directory to
# the location that will eventually be zipped up for the source
# distribution. It will remove most intermediate and output files that may
# exist from a 'dirty' source location that may contain previous build
# output. It requires the following parameters:
#   $(1) : a subdirectory containing source files to copy to the export
# ------------------------------------------------------------------------- #
define CreateCopySourceDirRule
$(addprefix $(TARGET_DIR)/,$(1)): $(addprefix $(SOURCE_DIR)/,$(1))
	@echo Copying $(1)...
	@cp -a $$^ $$@
	@rm -rf $$(addprefix $$@/,bin)
	@rm -rf $$(addprefix $$@/,obj)
	@rm -rf `find $$@ -type d -name .svn`
	@rm -rf `find $$@ -type f -name .DS_Store`
	@rm -rf `find $$@ -type f -name blame.txt`
	@rm -rf `find $$@ -type f -name transform_gameinfo.xslt`
	

endef

# ------------------------------------------------------------------------- #
# Generate rules to copy source directories.
# ------------------------------------------------------------------------- #
$(foreach srcdir,$(SOURCE_DIRS),$(eval $(call CreateCopySourceDirRule,$(srcdir))))

# ------------------------------------------------------------------------- #
# Function: CreateCopySourceFileRule
# ------------------------------------------------------------------------- #
# This function declares a rule to copy a source file to its export
# location. It requires the following arguments:
#   $(1) : a source file to copy
# ------------------------------------------------------------------------- #
define CreateCopySourceFileRule
$(addprefix $(TARGET_DIR)/,$(1)): $(addprefix $(SOURCE_DIR)/,$(1))
	@echo Copying $(1)...
	@cp -p $$^ $$@

endef

# ------------------------------------------------------------------------- #
# Generate rules to copy specific source files.
# ------------------------------------------------------------------------- #
$(foreach srcfile,$(SOURCE_FILES),$(eval $(call CreateCopySourceFileRule,$(srcfile))))

# ------------------------------------------------------------------------- #
# Rule: TARGET_ZIP
# ------------------------------------------------------------------------- #
# Implements the rule to zip the export created by the TARGET_DIR rule.
# ------------------------------------------------------------------------- #
$(TARGET_ZIP): $(addprefix $(TARGET_DIR)/,$(SOURCE_SUBDIRS)) $(addprefix $(TARGET_DIR)/,$(SOURCE_DIRS)) $(addprefix $(TARGET_DIR)/,$(SOURCE_FILES))
	@echo
	@echo -------------------- Creating Source Distribution --------------------
	@echo Zipping contents....
	@zip -rq $@ "$(TARGET_DIR)"
	@echo

  endif
endif

# ------------------------------------------------------------------------- #
# Rule: .PHONY
# ------------------------------------------------------------------------- #
# Declare our phony targets.
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
	-rm -rf $(TARGET_DIR) $(TARGET_ZIP)

# ------------------------------------------------------------------------- #
# Get the rule to generate custom.mak if needed.
# ------------------------------------------------------------------------- #
include $(ROOT_DIR)/custom_mak_rule.mak
