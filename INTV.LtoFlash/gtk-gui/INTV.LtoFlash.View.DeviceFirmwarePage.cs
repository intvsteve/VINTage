
// This file has been generated by the GUI designer. Do not modify.
namespace INTV.LtoFlash.View
{
	public partial class DeviceFirmwarePage
	{
		private global::Gtk.Table table1;
		
		private global::Gtk.Entry _currentVersion;
		
		private global::Gtk.Entry _factoryVersion;
		
		private global::Gtk.Button _updateFirmware;
		
		private global::Gtk.Entry _updateVersion;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label label3;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget INTV.LtoFlash.View.DeviceFirmwarePage
			global::Stetic.BinContainer.Attach (this);
			this.Name = "INTV.LtoFlash.View.DeviceFirmwarePage";
			// Container child INTV.LtoFlash.View.DeviceFirmwarePage.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(4)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			this.table1.BorderWidth = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this._currentVersion = new global::Gtk.Entry ();
			this._currentVersion.CanFocus = true;
			this._currentVersion.Name = "_currentVersion";
			this._currentVersion.IsEditable = false;
			this._currentVersion.InvisibleChar = '•';
			this.table1.Add (this._currentVersion);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this._currentVersion]));
			w1.TopAttach = ((uint)(2));
			w1.BottomAttach = ((uint)(3));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this._factoryVersion = new global::Gtk.Entry ();
			this._factoryVersion.CanFocus = true;
			this._factoryVersion.Name = "_factoryVersion";
			this._factoryVersion.IsEditable = false;
			this._factoryVersion.InvisibleChar = '•';
			this.table1.Add (this._factoryVersion);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this._factoryVersion]));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this._updateFirmware = new global::Gtk.Button ();
			this._updateFirmware.CanFocus = true;
			this._updateFirmware.Name = "_updateFirmware";
			this._updateFirmware.UseUnderline = true;
			this._updateFirmware.Label = global::Mono.Unix.Catalog.GetString ("Update Firmware...");
			this.table1.Add (this._updateFirmware);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this._updateFirmware]));
			w3.TopAttach = ((uint)(3));
			w3.BottomAttach = ((uint)(4));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this._updateVersion = new global::Gtk.Entry ();
			this._updateVersion.CanFocus = true;
			this._updateVersion.Name = "_updateVersion";
			this._updateVersion.IsEditable = false;
			this._updateVersion.InvisibleChar = '•';
			this.table1.Add (this._updateVersion);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this._updateVersion]));
			w4.TopAttach = ((uint)(1));
			w4.BottomAttach = ((uint)(2));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Factory Version:");
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.label1]));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Update Version:");
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w6.TopAttach = ((uint)(1));
			w6.BottomAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Current Version:");
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w7.TopAttach = ((uint)(2));
			w7.BottomAttach = ((uint)(3));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this._updateFirmware.Clicked += new global::System.EventHandler (this.HandleUpdateFirmwareClicked);
		}
	}
}