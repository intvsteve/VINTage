
// This file has been generated by the GUI designer. Do not modify.
namespace INTV.Shared.View
{
	public partial class ReportDialog
	{
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.Label _message;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.TextView _reportText;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.CheckButton _doNotShowAgain;
		
		private global::Gtk.Button _showAttachedFiles;
		
		private global::Gtk.Button _sendErrorReport;
		
		private global::Gtk.Button _copyToClipboard;
		
		private global::Gtk.Button _close;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget INTV.Shared.View.ReportDialog
			this.Name = "INTV.Shared.View.ReportDialog";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Modal = true;
			this.DefaultWidth = 648;
			// Internal child INTV.Shared.View.ReportDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this._message = new global::Gtk.Label ();
			this._message.Name = "_message";
			this._message.Xpad = 4;
			this._message.Xalign = 0F;
			this._message.LabelProp = global::Mono.Unix.Catalog.GetString ("label1");
			this._message.Wrap = true;
			this.vbox2.Add (this._message);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this._message]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			this.GtkScrolledWindow.BorderWidth = ((uint)(4));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this._reportText = new global::Gtk.TextView ();
			this._reportText.CanFocus = true;
			this._reportText.Name = "_reportText";
			this._reportText.Editable = false;
			this.GtkScrolledWindow.Add (this._reportText);
			this.vbox2.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.GtkScrolledWindow]));
			w4.Position = 1;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this._doNotShowAgain = new global::Gtk.CheckButton ();
			this._doNotShowAgain.CanFocus = true;
			this._doNotShowAgain.Name = "_doNotShowAgain";
			this._doNotShowAgain.Label = global::Mono.Unix.Catalog.GetString ("Do not show this error again");
			this._doNotShowAgain.DrawIndicator = true;
			this._doNotShowAgain.UseUnderline = true;
			this._doNotShowAgain.Xalign = 1F;
			this.hbox1.Add (this._doNotShowAgain);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this._doNotShowAgain]));
			w5.PackType = ((global::Gtk.PackType)(1));
			w5.Position = 2;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox1]));
			w6.PackType = ((global::Gtk.PackType)(1));
			w6.Position = 2;
			w6.Expand = false;
			w6.Fill = false;
			w1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox2]));
			w7.Position = 0;
			// Internal child INTV.Shared.View.ReportDialog.ActionArea
			global::Gtk.HButtonBox w8 = this.ActionArea;
			w8.Name = "dialog1_ActionArea";
			w8.Spacing = 10;
			w8.BorderWidth = ((uint)(5));
			w8.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this._showAttachedFiles = new global::Gtk.Button ();
			this._showAttachedFiles.CanFocus = true;
			this._showAttachedFiles.Name = "_showAttachedFiles";
			this._showAttachedFiles.UseUnderline = true;
			this._showAttachedFiles.Label = global::Mono.Unix.Catalog.GetString ("Show Files");
			w8.Add (this._showAttachedFiles);
			global::Gtk.ButtonBox.ButtonBoxChild w9 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w8 [this._showAttachedFiles]));
			w9.Expand = false;
			w9.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this._sendErrorReport = new global::Gtk.Button ();
			this._sendErrorReport.CanFocus = true;
			this._sendErrorReport.Name = "_sendErrorReport";
			this._sendErrorReport.UseUnderline = true;
			this._sendErrorReport.Label = global::Mono.Unix.Catalog.GetString ("Send Error Report");
			w8.Add (this._sendErrorReport);
			global::Gtk.ButtonBox.ButtonBoxChild w10 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w8 [this._sendErrorReport]));
			w10.Position = 1;
			w10.Expand = false;
			w10.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this._copyToClipboard = new global::Gtk.Button ();
			this._copyToClipboard.CanDefault = true;
			this._copyToClipboard.CanFocus = true;
			this._copyToClipboard.Name = "_copyToClipboard";
			this._copyToClipboard.UseUnderline = true;
			this._copyToClipboard.Label = global::Mono.Unix.Catalog.GetString ("Copy to Clipboard");
			w8.Add (this._copyToClipboard);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w8 [this._copyToClipboard]));
			w11.Position = 2;
			w11.Expand = false;
			w11.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this._close = new global::Gtk.Button ();
			this._close.CanDefault = true;
			this._close.CanFocus = true;
			this._close.Name = "_close";
			this._close.UseUnderline = true;
			this._close.Label = global::Mono.Unix.Catalog.GetString ("Close");
			this.AddActionWidget (this._close, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w12 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w8 [this._close]));
			w12.Position = 3;
			w12.Expand = false;
			w12.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultHeight = 300;
			this.Show ();
			this._doNotShowAgain.Toggled += new global::System.EventHandler (this.HandleDoNotShowAgainToggled);
			this._showAttachedFiles.Clicked += new global::System.EventHandler (this.HandleShowAttachedFiles);
			this._sendErrorReport.Clicked += new global::System.EventHandler (this.HandleSendErrorReport);
			this._copyToClipboard.Clicked += new global::System.EventHandler (this.HandleCopyToClipboard);
		}
	}
}