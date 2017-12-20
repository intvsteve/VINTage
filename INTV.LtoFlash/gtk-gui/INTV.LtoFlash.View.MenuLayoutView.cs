
// This file has been generated by the GUI designer. Do not modify.
namespace INTV.LtoFlash.View
{
	public partial class MenuLayoutView
	{
		private global::Gtk.VBox vbox1;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Label _menuLayoutTitle;
		
		private global::Gtk.VSeparator vseparator1;
		
		private global::Gtk.Label _rootDirectoryUsage;
		
		private global::Gtk.Image _dirtyIcon;
		
		private global::Gtk.Image _powerIcon;
		
		private global::Gtk.Button _newFolder;
		
		private global::Gtk.Button _deleteSelectedItems;
		
		private global::Gtk.ComboBox _colorChooser;
		
		private global::Gtk.VPaned vpaned1;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.TreeView _menuLayout;
		
		private global::Gtk.HBox hbox3;
		
		private global::Gtk.Label _storageUsedLabel;
		
		private global::Gtk.ProgressBar _storageUsed;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget INTV.LtoFlash.View.MenuLayoutView
			global::Stetic.BinContainer.Attach (this);
			this.Name = "INTV.LtoFlash.View.MenuLayoutView";
			// Container child INTV.LtoFlash.View.MenuLayoutView.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this._menuLayoutTitle = new global::Gtk.Label ();
			this._menuLayoutTitle.Name = "_menuLayoutTitle";
			this._menuLayoutTitle.LabelProp = global::Mono.Unix.Catalog.GetString ("label1");
			this.hbox1.Add (this._menuLayoutTitle);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this._menuLayoutTitle]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vseparator1 = new global::Gtk.VSeparator ();
			this.vseparator1.Name = "vseparator1";
			this.hbox1.Add (this.vseparator1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vseparator1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this._rootDirectoryUsage = new global::Gtk.Label ();
			this._rootDirectoryUsage.Name = "_rootDirectoryUsage";
			this._rootDirectoryUsage.Xalign = 0F;
			this._rootDirectoryUsage.LabelProp = global::Mono.Unix.Catalog.GetString ("label2");
			this.hbox1.Add (this._rootDirectoryUsage);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this._rootDirectoryUsage]));
			w3.Position = 2;
			// Container child hbox1.Gtk.Box+BoxChild
			this._dirtyIcon = new global::Gtk.Image ();
			this._dirtyIcon.Name = "_dirtyIcon";
			this.hbox1.Add (this._dirtyIcon);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this._dirtyIcon]));
			w4.Position = 5;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this._powerIcon = new global::Gtk.Image ();
			this._powerIcon.Name = "_powerIcon";
			this.hbox1.Add (this._powerIcon);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this._powerIcon]));
			w5.Position = 6;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this._newFolder = new global::Gtk.Button ();
			this._newFolder.CanFocus = true;
			this._newFolder.Name = "_newFolder";
			this._newFolder.UseUnderline = true;
			this._newFolder.FocusOnClick = false;
			global::Gtk.Image w6 = new global::Gtk.Image ();
			this._newFolder.Image = w6;
			this.hbox1.Add (this._newFolder);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this._newFolder]));
			w7.Position = 7;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this._deleteSelectedItems = new global::Gtk.Button ();
			this._deleteSelectedItems.CanFocus = true;
			this._deleteSelectedItems.Name = "_deleteSelectedItems";
			this._deleteSelectedItems.UseUnderline = true;
			this._deleteSelectedItems.FocusOnClick = false;
			global::Gtk.Image w8 = new global::Gtk.Image ();
			this._deleteSelectedItems.Image = w8;
			this.hbox1.Add (this._deleteSelectedItems);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this._deleteSelectedItems]));
			w9.Position = 8;
			w9.Expand = false;
			w9.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this._colorChooser = new global::Gtk.ComboBox ();
			this._colorChooser.Name = "_colorChooser";
			this.hbox1.Add (this._colorChooser);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this._colorChooser]));
			w10.PackType = ((global::Gtk.PackType)(1));
			w10.Position = 9;
			w10.Expand = false;
			w10.Fill = false;
			this.vbox1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox1]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.vpaned1 = new global::Gtk.VPaned ();
			this.vpaned1.Name = "vpaned1";
			this.vpaned1.Position = 246;
			// Container child vpaned1.Gtk.Paned+PanedChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this._menuLayout = new global::Gtk.TreeView ();
			this._menuLayout.CanFocus = true;
			this._menuLayout.Name = "_menuLayout";
			this.GtkScrolledWindow.Add (this._menuLayout);
			this.vpaned1.Add (this.GtkScrolledWindow);
			global::Gtk.Paned.PanedChild w13 = ((global::Gtk.Paned.PanedChild)(this.vpaned1 [this.GtkScrolledWindow]));
			w13.Resize = false;
			this.vbox1.Add (this.vpaned1);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.vpaned1]));
			w14.Position = 1;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this._storageUsedLabel = new global::Gtk.Label ();
			this._storageUsedLabel.Name = "_storageUsedLabel";
			this._storageUsedLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Storage Used:");
			this.hbox3.Add (this._storageUsedLabel);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this._storageUsedLabel]));
			w15.Position = 0;
			w15.Expand = false;
			w15.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this._storageUsed = new global::Gtk.ProgressBar ();
			this._storageUsed.Name = "_storageUsed";
			this.hbox3.Add (this._storageUsed);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this._storageUsed]));
			w16.Position = 1;
			this.vbox1.Add (this.hbox3);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox3]));
			w17.Position = 2;
			w17.Expand = false;
			w17.Fill = false;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this._dirtyIcon.Hide ();
			this.Hide ();
			this._newFolder.Clicked += new global::System.EventHandler (this.HandleNewFolderClicked);
			this._deleteSelectedItems.Clicked += new global::System.EventHandler (this.HandleDeleteSelectedItemsClicked);
			this._colorChooser.Changed += new global::System.EventHandler (this.HandleColorSelected);
			this._menuLayout.RowExpanded += new global::Gtk.RowExpandedHandler (this.HandleRowExpanded);
			this._menuLayout.RowCollapsed += new global::Gtk.RowCollapsedHandler (this.HandleRowCollapsed);
			this._menuLayout.DragDataReceived += new global::Gtk.DragDataReceivedHandler (this.HandleDragDataReceived);
			this._menuLayout.DragBegin += new global::Gtk.DragBeginHandler (this.HandleDragBegin);
			this._menuLayout.DragDataGet += new global::Gtk.DragDataGetHandler (this.HandleDragDataGet);
		}
	}
}
