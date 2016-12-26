#
# Makefile to offer convenience mechanism to clean up what Xamarin Studio won't.
#
# Licensed under the GPL License. See license.txt for details.

SOURCE_DIR ?= .

.PHONY: all
all: FORCE
	@echo
	@echo -------------------- Cleaning Xamarin Studio Output --------------------
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

FORCE:
