#############################################################################
# custom.make Generator                                                     #
# ------------------------------------------------------------------------- #
# This makefile defines the rule to generate the custom.mak file if needed. #
# It uses the custom.mak.template file to do so. This makefile must be      #
# included at the bottom of any makefile that needs to make use of the      #
# custom.mak file.                                                          #
#                                                                           #
# Example:                                                                  #
#                                                                           #
# include common.mak                                                        #
# -include custom.mak                                                       #
#                                                                           #
# <makefile body and rules>                                                 #
#                                                                           #
# include custom_mak_rule.mak                                               #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# Rule: $(ROOT_DIR)/custom.mak
# ------------------------------------------------------------------------- #
# The machine-specific configuration is not defined in source control.
# Instead, a template version of the file, unconfigured, is included. When
# the build is invoked the first time, this rule runs, which copies the
# template to the expected location, with default settings.
# ------------------------------------------------------------------------- #
$(ROOT_DIR)/custom.mak:
	@echo Creating initial version of custom.mak from custom.mak.template.
	@echo You will need to adjust the configuration for your system.
	@cp -fp $(ROOT_DIR)/custom.mak.template $(ROOT_DIR)/custom.mak
