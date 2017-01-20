#
# Build file for creating source code distribution as a .zip file.
#
# Licensed under the GPL License. See license.txt for details.

################################################################################
# Customizable variables
################################################################################

NAME ?= LTOFlash
VERSION ?= 1.0.0.2587

ifneq (,$(GIT_REPO))
  # This assumes GitHub, which supports 'svn export' to create a clean export
  # of the remote repo. GitHub doesn't support 'git archive --remote' yet.
  # Alternative would be to clone and then archve the local copy... or just
  # use git archive in the local directory: git archive -o <output.zip> HEAD
  $(info --------------------------- CREATING GITHUB EXPORT ---------------------------)
  SVN_REPO = $(GIT_REPO)
  SOURCE_DIR = $(SVN_REPO)
else
  ifneq (,$(SVN_REPO))
    $(info ----------------------------- CREATING SVN EXPORT ----------------------------)
    SOURCE_DIR = $(SVN_REPO)
  else
    SOURCE_DIR ?= ../..
  endif
endif

# This is the legacy approach, which will include all the local junk that may
# have polluted your local copy of the source. Beware!
ifeq (,$(SVN_REPO))
  SOURCE_SUBDIRS ?= \
    Locutus

  SOURCE_DIRS ?= \
    INTV.Core \
    INTV.Intellicart \
    INTV.jzIntv \
    INTV.LtoFlash \
    INTV.Ribbon \
    INTV.Shared \
    Locutus/LtoFlash

  SOURCE_FILES ?= \
    Locutus/license.txt \
    Locutus/Locutus.svn.sln \
    Locutus/Locutus.installer.svn.sln \
    Locutus/Locutus.Mac.svn.sln \
    Locutus/Locutus.xp.svn.sln \
    Locutus/Locutus.xp.installer.svn.sln \
    Locutus/README.txt

endif

TARGET_DIR = src

################################################################################
# Rules for making the source distribution ZIP file.
################################################################################

TARGET_ZIP = $(NAME).source-$(VERSION).zip

.PHONY: all
all: $(TARGET_ZIP)

ifneq (,$(SVN_REPO))

$(TARGET_DIR):
	@echo
	@echo --------------------- Exporting Source From Repo ---------------------
	@echo ... Exporting from $(SOURCE_DIR) ...
	svn export $(SOURCE_DIR) $(TARGET_DIR)
	@rm -f $(TARGET_DIR)/.gitattributes
	@rm -f $(TARGET_DIR)/.gitignore

$(TARGET_ZIP): $(TARGET_DIR)
	@echo
	@echo -------------------- Creating Source Distribution --------------------
	@echo Zipping contents....
	@zip -rq $@ "$(TARGET_DIR)"
	@echo

else

$(TARGET_DIR):
	@mkdir -p $@

define CreateTargetSubDirRule
$(addprefix $(TARGET_DIR)/,$(1)): $(TARGET_DIR)
	@echo
	@echo -------------------- Copying Source --------------------
	@mkdir -p $$^/$(1)

endef

# Generate rules to create target subdirectories.
$(foreach srcsubdir,$(SOURCE_SUBDIRS),$(eval $(call CreateTargetSubDirRule,$(srcsubdir))))

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

# Generate rules to copy source directories.
$(foreach srcdir,$(SOURCE_DIRS),$(eval $(call CreateCopySourceDirRule,$(srcdir))))

define CreateCopySourceFileRule
$(addprefix $(TARGET_DIR)/,$(1)): $(addprefix $(SOURCE_DIR)/,$(1))
	@echo Copying $(1)...
	@cp -p $$^ $$@

endef

# Generate rules to copy specific source files.
$(foreach srcfile,$(SOURCE_FILES),$(eval $(call CreateCopySourceFileRule,$(srcfile))))

$(TARGET_ZIP): $(addprefix $(TARGET_DIR)/,$(SOURCE_SUBDIRS)) $(addprefix $(TARGET_DIR)/,$(SOURCE_DIRS)) $(addprefix $(TARGET_DIR)/,$(SOURCE_FILES))
	@echo
	@echo -------------------- Creating Source Distribution --------------------
	@echo Zipping contents....
	@zip -rq $@ "$(TARGET_DIR)"
	@echo

endif

.PHONY: clean
clean:
	-rm -rf $(TARGET_DIR) $(TARGET_ZIP)
