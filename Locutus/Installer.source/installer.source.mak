#
# Build file for creating source code distribution as a .zip file.
#
# Licensed under the GPL License. See license.txt for details.

################################################################################
# Customizable variables
################################################################################

NAME ?= LTOFlash
VERSION ?= 1.0.0.2350

SOURCE_DIR ?= ../..

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

TARGET_DIR = src

################################################################################
# Rules for making the source distribution ZIP file.
################################################################################

TARGET_ZIP = $(NAME).source-$(VERSION).zip

.PHONY: all
all: $(TARGET_ZIP)

$(TARGET_DIR):
	@mkdir -p $@

define CreateTargetSubDirRule
$(addprefix $(TARGET_DIR)/,$(1)): $(TARGET_DIR)
	@echo
	@echo -------------------- Copying Source --------------------
	@mkdir -p $$^/$(1)

endef

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

$(foreach srcdir,$(SOURCE_DIRS),$(eval $(call CreateCopySourceDirRule,$(srcdir))))

define CreateCopySourceFileRule
$(addprefix $(TARGET_DIR)/,$(1)): $(addprefix $(SOURCE_DIR)/,$(1))
	@echo Copying $(1)...
	@cp -p $$^ $$@

endef

$(foreach srcfile,$(SOURCE_FILES),$(eval $(call CreateCopySourceFileRule,$(srcfile))))


$(TARGET_ZIP): $(addprefix $(TARGET_DIR)/,$(SOURCE_SUBDIRS)) $(addprefix $(TARGET_DIR)/,$(SOURCE_DIRS)) $(addprefix $(TARGET_DIR)/,$(SOURCE_FILES))
	@echo
	@echo -------------------- Creating Source Distribution --------------------
	@echo Zipping contents....
	@zip -rq $@ "$(TARGET_DIR)"
	@echo

.PHONY: clean
clean:
	-rm -rf $(TARGET_DIR) $(TARGET_ZIP)
