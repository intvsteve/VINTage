
// This file has been generated by the GUI designer. Do not modify.
namespace INTV.LtoFlash.View
{
	public partial class DeviceSettingsPage
	{
		private global::Gtk.VBox vbox2;

		private global::Gtk.HBox hbox1;

		private global::Gtk.Table table1;

		private global::Gtk.ComboBox _ecsCompatibility;

		private global::Gtk.ComboBox _intellivisionIICompatibility;

		private global::Gtk.ComboBox _saveMenuPositionSetting;

		private global::Gtk.ComboBox _titleScreenSetting;

		private global::Gtk.Label label1;

		private global::Gtk.Label label2;

		private global::Gtk.Label label3;

		private global::Gtk.Label label4;

		private global::Gtk.CheckButton _keyClicks;

		private global::Gtk.CheckButton _backgroundGC;

		private global::Gtk.CheckButton _enableCartConfigMenu;

		private global::Gtk.CheckButton _randomizeLtoFlashRam;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget INTV.LtoFlash.View.DeviceSettingsPage
			global::Stetic.BinContainer.Attach(this);
			this.Name = "INTV.LtoFlash.View.DeviceSettingsPage";
			// Container child INTV.LtoFlash.View.DeviceSettingsPage.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.vbox2.BorderWidth = ((uint)(6));
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table(((uint)(4)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this._ecsCompatibility = new global::Gtk.ComboBox();
			this._ecsCompatibility.Name = "_ecsCompatibility";
			this.table1.Add(this._ecsCompatibility);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1[this._ecsCompatibility]));
			w1.TopAttach = ((uint)(2));
			w1.BottomAttach = ((uint)(3));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this._intellivisionIICompatibility = new global::Gtk.ComboBox();
			this._intellivisionIICompatibility.Name = "_intellivisionIICompatibility";
			this.table1.Add(this._intellivisionIICompatibility);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1[this._intellivisionIICompatibility]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this._saveMenuPositionSetting = new global::Gtk.ComboBox();
			this._saveMenuPositionSetting.Name = "_saveMenuPositionSetting";
			this.table1.Add(this._saveMenuPositionSetting);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1[this._saveMenuPositionSetting]));
			w3.TopAttach = ((uint)(3));
			w3.BottomAttach = ((uint)(4));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this._titleScreenSetting = new global::Gtk.ComboBox();
			this._titleScreenSetting.Name = "_titleScreenSetting";
			this.table1.Add(this._titleScreenSetting);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1[this._titleScreenSetting]));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label();
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString("Show Title Screen:");
			this.table1.Add(this.label1);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1[this.label1]));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString("Intellivision II Compatibility:");
			this.table1.Add(this.label2);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1[this.label2]));
			w6.TopAttach = ((uint)(1));
			w6.BottomAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString("ECS ROM Enabled:");
			this.table1.Add(this.label3);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1[this.label3]));
			w7.TopAttach = ((uint)(2));
			w7.BottomAttach = ((uint)(3));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString("Save Menu Position:");
			this.table1.Add(this.label4);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1[this.label4]));
			w8.TopAttach = ((uint)(3));
			w8.BottomAttach = ((uint)(4));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			this.hbox1.Add(this.table1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.table1]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			this.vbox2.Add(this.hbox1);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox1]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this._keyClicks = new global::Gtk.CheckButton();
			this._keyClicks.CanFocus = true;
			this._keyClicks.Name = "_keyClicks";
			this._keyClicks.Label = global::Mono.Unix.Catalog.GetString("Keyclicks");
			this._keyClicks.DrawIndicator = true;
			this._keyClicks.UseUnderline = true;
			this.vbox2.Add(this._keyClicks);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox2[this._keyClicks]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this._backgroundGC = new global::Gtk.CheckButton();
			this._backgroundGC.CanFocus = true;
			this._backgroundGC.Name = "_backgroundGC";
			this._backgroundGC.Label = global::Mono.Unix.Catalog.GetString("Perform file system maintenance in background");
			this._backgroundGC.DrawIndicator = true;
			this._backgroundGC.UseUnderline = true;
			this.vbox2.Add(this._backgroundGC);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox2[this._backgroundGC]));
			w12.Position = 2;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this._enableCartConfigMenu = new global::Gtk.CheckButton();
			this._enableCartConfigMenu.CanFocus = true;
			this._enableCartConfigMenu.Name = "_enableCartConfigMenu";
			this._enableCartConfigMenu.Label = global::Mono.Unix.Catalog.GetString("Enable configuration menu");
			this._enableCartConfigMenu.DrawIndicator = true;
			this._enableCartConfigMenu.UseUnderline = true;
			this.vbox2.Add(this._enableCartConfigMenu);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox2[this._enableCartConfigMenu]));
			w13.Position = 3;
			w13.Expand = false;
			w13.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this._randomizeLtoFlashRam = new global::Gtk.CheckButton();
			this._randomizeLtoFlashRam.CanFocus = true;
			this._randomizeLtoFlashRam.Name = "_randomizeLtoFlashRam";
			this._randomizeLtoFlashRam.Label = global::Mono.Unix.Catalog.GetString("Randomize LTO Flash! RAM before loading ROM");
			this._randomizeLtoFlashRam.DrawIndicator = true;
			this._randomizeLtoFlashRam.UseUnderline = true;
			this.vbox2.Add(this._randomizeLtoFlashRam);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2[this._randomizeLtoFlashRam]));
			w14.Position = 4;
			w14.Expand = false;
			w14.Fill = false;
			this.Add(this.vbox2);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
			this._titleScreenSetting.Changed += new global::System.EventHandler(this.HandleShowTitleScreenChanged);
			this._saveMenuPositionSetting.Changed += new global::System.EventHandler(this.HandleSaveMenuPositionChanged);
			this._intellivisionIICompatibility.Changed += new global::System.EventHandler(this.HandleIntellivisionIICompatibilityChanged);
			this._ecsCompatibility.Changed += new global::System.EventHandler(this.HandleEcsCompatibilityChanged);
			this._keyClicks.Toggled += new global::System.EventHandler(this.HandleKeyclicksChanged);
			this._backgroundGC.Toggled += new global::System.EventHandler(this.HandleBackgroundGCChanged);
			this._enableCartConfigMenu.Toggled += new global::System.EventHandler(this.HandleEnableConfigMenuChanged);
			this._randomizeLtoFlashRam.Toggled += new global::System.EventHandler(this.HandleRandomizeLtoFlashRamChanged);
		}
	}
}
