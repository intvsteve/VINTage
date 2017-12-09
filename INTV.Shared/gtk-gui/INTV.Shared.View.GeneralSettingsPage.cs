
// This file has been generated by the GUI designer. Do not modify.
namespace INTV.Shared.View
{
	public partial class GeneralSettingsPage
	{
		private global::Gtk.VBox vbox3;
		
		private global::Gtk.CheckButton _checkForUpdates;
		
		private global::Gtk.Table _directories;
		
		private global::Gtk.Label _backupsLabel;
		
		private global::Gtk.Entry _backupsPath;
		
		private global::Gtk.Label _boxesLabel;
		
		private global::Gtk.Entry _boxesPath;
		
		private global::Gtk.Label _errorLogsLabel;
		
		private global::Gtk.Entry _errorLogsPath;
		
		private global::Gtk.Label _labelsLabel;
		
		private global::Gtk.Entry _labelsPath;
		
		private global::Gtk.Label _manualsLabel;
		
		private global::Gtk.Entry _manualsPath;
		
		private global::Gtk.Label _overlaysLabel;
		
		private global::Gtk.Entry _overlaysPath;
		
		private global::Gtk.Label _romsLabel;
		
		private global::Gtk.Entry _romsPath;
		
		private global::Gtk.Button _showBackupsDir;
		
		private global::Gtk.Button _showBoxesDir;
		
		private global::Gtk.Button _showErrorLogsDir;
		
		private global::Gtk.Button _showLabelsDir;
		
		private global::Gtk.Button _showManualsDir;
		
		private global::Gtk.Button _showOverlaysDir;
		
		private global::Gtk.Button _showRomsDir;
		
		private global::Gtk.CheckButton _showDetailedErrors;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget INTV.Shared.View.GeneralSettingsPage
			global::Stetic.BinContainer.Attach (this);
			this.Name = "INTV.Shared.View.GeneralSettingsPage";
			// Container child INTV.Shared.View.GeneralSettingsPage.Gtk.Container+ContainerChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 4;
			// Container child vbox3.Gtk.Box+BoxChild
			this._checkForUpdates = new global::Gtk.CheckButton ();
			this._checkForUpdates.CanFocus = true;
			this._checkForUpdates.Name = "_checkForUpdates";
			this._checkForUpdates.Label = global::Mono.Unix.Catalog.GetString ("Check for application updates at startup");
			this._checkForUpdates.DrawIndicator = true;
			this._checkForUpdates.UseUnderline = true;
			this.vbox3.Add (this._checkForUpdates);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this._checkForUpdates]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this._directories = new global::Gtk.Table (((uint)(7)), ((uint)(3)), false);
			this._directories.Name = "_directories";
			this._directories.RowSpacing = ((uint)(6));
			this._directories.ColumnSpacing = ((uint)(6));
			this._directories.BorderWidth = ((uint)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._backupsLabel = new global::Gtk.Label ();
			this._backupsLabel.Name = "_backupsLabel";
			this._backupsLabel.Xalign = 0F;
			this._backupsLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("ROM List Backups:");
			this._directories.Add (this._backupsLabel);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this._directories [this._backupsLabel]));
			w2.TopAttach = ((uint)(5));
			w2.BottomAttach = ((uint)(6));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._backupsPath = new global::Gtk.Entry ();
			this._backupsPath.CanFocus = true;
			this._backupsPath.Name = "_backupsPath";
			this._backupsPath.IsEditable = false;
			this._backupsPath.InvisibleChar = '•';
			this._directories.Add (this._backupsPath);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this._directories [this._backupsPath]));
			w3.TopAttach = ((uint)(5));
			w3.BottomAttach = ((uint)(6));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._boxesLabel = new global::Gtk.Label ();
			this._boxesLabel.Name = "_boxesLabel";
			this._boxesLabel.Xalign = 0F;
			this._boxesLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Boxes:");
			this._directories.Add (this._boxesLabel);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this._directories [this._boxesLabel]));
			w4.TopAttach = ((uint)(3));
			w4.BottomAttach = ((uint)(4));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._boxesPath = new global::Gtk.Entry ();
			this._boxesPath.CanFocus = true;
			this._boxesPath.Name = "_boxesPath";
			this._boxesPath.IsEditable = false;
			this._boxesPath.InvisibleChar = '•';
			this._directories.Add (this._boxesPath);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this._directories [this._boxesPath]));
			w5.TopAttach = ((uint)(3));
			w5.BottomAttach = ((uint)(4));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._errorLogsLabel = new global::Gtk.Label ();
			this._errorLogsLabel.Name = "_errorLogsLabel";
			this._errorLogsLabel.Xalign = 0F;
			this._errorLogsLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Error Logs:");
			this._directories.Add (this._errorLogsLabel);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this._directories [this._errorLogsLabel]));
			w6.TopAttach = ((uint)(6));
			w6.BottomAttach = ((uint)(7));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._errorLogsPath = new global::Gtk.Entry ();
			this._errorLogsPath.CanFocus = true;
			this._errorLogsPath.Name = "_errorLogsPath";
			this._errorLogsPath.IsEditable = false;
			this._errorLogsPath.InvisibleChar = '•';
			this._directories.Add (this._errorLogsPath);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this._directories [this._errorLogsPath]));
			w7.TopAttach = ((uint)(6));
			w7.BottomAttach = ((uint)(7));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._labelsLabel = new global::Gtk.Label ();
			this._labelsLabel.Name = "_labelsLabel";
			this._labelsLabel.Xalign = 0F;
			this._labelsLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Labels:");
			this._directories.Add (this._labelsLabel);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this._directories [this._labelsLabel]));
			w8.TopAttach = ((uint)(4));
			w8.BottomAttach = ((uint)(5));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._labelsPath = new global::Gtk.Entry ();
			this._labelsPath.CanFocus = true;
			this._labelsPath.Name = "_labelsPath";
			this._labelsPath.IsEditable = false;
			this._labelsPath.InvisibleChar = '•';
			this._directories.Add (this._labelsPath);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this._directories [this._labelsPath]));
			w9.TopAttach = ((uint)(4));
			w9.BottomAttach = ((uint)(5));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._manualsLabel = new global::Gtk.Label ();
			this._manualsLabel.Name = "_manualsLabel";
			this._manualsLabel.Xalign = 0F;
			this._manualsLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Manuals:");
			this._directories.Add (this._manualsLabel);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this._directories [this._manualsLabel]));
			w10.TopAttach = ((uint)(1));
			w10.BottomAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._manualsPath = new global::Gtk.Entry ();
			this._manualsPath.CanFocus = true;
			this._manualsPath.Name = "_manualsPath";
			this._manualsPath.IsEditable = false;
			this._manualsPath.InvisibleChar = '•';
			this._directories.Add (this._manualsPath);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this._directories [this._manualsPath]));
			w11.TopAttach = ((uint)(1));
			w11.BottomAttach = ((uint)(2));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._overlaysLabel = new global::Gtk.Label ();
			this._overlaysLabel.Name = "_overlaysLabel";
			this._overlaysLabel.Xalign = 0F;
			this._overlaysLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Overlays:");
			this._directories.Add (this._overlaysLabel);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this._directories [this._overlaysLabel]));
			w12.TopAttach = ((uint)(2));
			w12.BottomAttach = ((uint)(3));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._overlaysPath = new global::Gtk.Entry ();
			this._overlaysPath.CanFocus = true;
			this._overlaysPath.Name = "_overlaysPath";
			this._overlaysPath.IsEditable = false;
			this._overlaysPath.InvisibleChar = '•';
			this._directories.Add (this._overlaysPath);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this._directories [this._overlaysPath]));
			w13.TopAttach = ((uint)(2));
			w13.BottomAttach = ((uint)(3));
			w13.LeftAttach = ((uint)(1));
			w13.RightAttach = ((uint)(2));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._romsLabel = new global::Gtk.Label ();
			this._romsLabel.Name = "_romsLabel";
			this._romsLabel.Xalign = 0F;
			this._romsLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Copied ROMs:");
			this._directories.Add (this._romsLabel);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this._directories [this._romsLabel]));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._romsPath = new global::Gtk.Entry ();
			this._romsPath.CanFocus = true;
			this._romsPath.Name = "_romsPath";
			this._romsPath.IsEditable = false;
			this._romsPath.InvisibleChar = '•';
			this._directories.Add (this._romsPath);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this._directories [this._romsPath]));
			w15.LeftAttach = ((uint)(1));
			w15.RightAttach = ((uint)(2));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._showBackupsDir = new global::Gtk.Button ();
			this._showBackupsDir.CanFocus = true;
			this._showBackupsDir.Name = "_showBackupsDir";
			this._showBackupsDir.UseUnderline = true;
			this._showBackupsDir.Label = global::Mono.Unix.Catalog.GetString ("Show");
			this._directories.Add (this._showBackupsDir);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this._directories [this._showBackupsDir]));
			w16.TopAttach = ((uint)(5));
			w16.BottomAttach = ((uint)(6));
			w16.LeftAttach = ((uint)(2));
			w16.RightAttach = ((uint)(3));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._showBoxesDir = new global::Gtk.Button ();
			this._showBoxesDir.CanFocus = true;
			this._showBoxesDir.Name = "_showBoxesDir";
			this._showBoxesDir.UseUnderline = true;
			this._showBoxesDir.Label = global::Mono.Unix.Catalog.GetString ("Show");
			this._directories.Add (this._showBoxesDir);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this._directories [this._showBoxesDir]));
			w17.TopAttach = ((uint)(3));
			w17.BottomAttach = ((uint)(4));
			w17.LeftAttach = ((uint)(2));
			w17.RightAttach = ((uint)(3));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._showErrorLogsDir = new global::Gtk.Button ();
			this._showErrorLogsDir.CanFocus = true;
			this._showErrorLogsDir.Name = "_showErrorLogsDir";
			this._showErrorLogsDir.UseUnderline = true;
			this._showErrorLogsDir.Label = global::Mono.Unix.Catalog.GetString ("Show");
			this._directories.Add (this._showErrorLogsDir);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this._directories [this._showErrorLogsDir]));
			w18.TopAttach = ((uint)(6));
			w18.BottomAttach = ((uint)(7));
			w18.LeftAttach = ((uint)(2));
			w18.RightAttach = ((uint)(3));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._showLabelsDir = new global::Gtk.Button ();
			this._showLabelsDir.CanFocus = true;
			this._showLabelsDir.Name = "_showLabelsDir";
			this._showLabelsDir.UseUnderline = true;
			this._showLabelsDir.Label = global::Mono.Unix.Catalog.GetString ("Show");
			this._directories.Add (this._showLabelsDir);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this._directories [this._showLabelsDir]));
			w19.TopAttach = ((uint)(4));
			w19.BottomAttach = ((uint)(5));
			w19.LeftAttach = ((uint)(2));
			w19.RightAttach = ((uint)(3));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._showManualsDir = new global::Gtk.Button ();
			this._showManualsDir.CanFocus = true;
			this._showManualsDir.Name = "_showManualsDir";
			this._showManualsDir.UseUnderline = true;
			this._showManualsDir.Label = global::Mono.Unix.Catalog.GetString ("Show");
			this._directories.Add (this._showManualsDir);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this._directories [this._showManualsDir]));
			w20.TopAttach = ((uint)(1));
			w20.BottomAttach = ((uint)(2));
			w20.LeftAttach = ((uint)(2));
			w20.RightAttach = ((uint)(3));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._showOverlaysDir = new global::Gtk.Button ();
			this._showOverlaysDir.CanFocus = true;
			this._showOverlaysDir.Name = "_showOverlaysDir";
			this._showOverlaysDir.UseUnderline = true;
			this._showOverlaysDir.Label = global::Mono.Unix.Catalog.GetString ("Show");
			this._directories.Add (this._showOverlaysDir);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this._directories [this._showOverlaysDir]));
			w21.TopAttach = ((uint)(2));
			w21.BottomAttach = ((uint)(3));
			w21.LeftAttach = ((uint)(2));
			w21.RightAttach = ((uint)(3));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child _directories.Gtk.Table+TableChild
			this._showRomsDir = new global::Gtk.Button ();
			this._showRomsDir.CanFocus = true;
			this._showRomsDir.Name = "_showRomsDir";
			this._showRomsDir.UseUnderline = true;
			this._showRomsDir.Label = global::Mono.Unix.Catalog.GetString ("Show");
			this._directories.Add (this._showRomsDir);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this._directories [this._showRomsDir]));
			w22.LeftAttach = ((uint)(2));
			w22.RightAttach = ((uint)(3));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox3.Add (this._directories);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this._directories]));
			w23.Position = 1;
			w23.Expand = false;
			w23.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this._showDetailedErrors = new global::Gtk.CheckButton ();
			this._showDetailedErrors.CanFocus = true;
			this._showDetailedErrors.Name = "_showDetailedErrors";
			this._showDetailedErrors.Label = global::Mono.Unix.Catalog.GetString ("Show detailed error messages");
			this._showDetailedErrors.DrawIndicator = true;
			this._showDetailedErrors.UseUnderline = true;
			this.vbox3.Add (this._showDetailedErrors);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this._showDetailedErrors]));
			w24.Position = 2;
			w24.Expand = false;
			w24.Fill = false;
			this.Add (this.vbox3);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this._checkForUpdates.Toggled += new global::System.EventHandler (this.HandleCheckForUpdatesToggle);
			this._showRomsDir.Clicked += new global::System.EventHandler (this.HandleShowButton);
			this._showOverlaysDir.Clicked += new global::System.EventHandler (this.HandleShowButton);
			this._showManualsDir.Clicked += new global::System.EventHandler (this.HandleShowButton);
			this._showLabelsDir.Clicked += new global::System.EventHandler (this.HandleShowButton);
			this._showErrorLogsDir.Clicked += new global::System.EventHandler (this.HandleShowButton);
			this._showBoxesDir.Clicked += new global::System.EventHandler (this.HandleShowButton);
			this._showBackupsDir.Clicked += new global::System.EventHandler (this.HandleShowButton);
			this._showDetailedErrors.Toggled += new global::System.EventHandler (this.HandleShowDetailedErrorsToggle);
		}
	}
}