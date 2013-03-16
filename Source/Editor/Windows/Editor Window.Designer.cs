namespace BinaryPhoenix.Fusion.Editor.Windows
{
	public partial class EditorWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///		Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorWindow));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.playMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.mapPropertiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.buildProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.insertObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator45 = new System.Windows.Forms.ToolStripSeparator();
            this.insertEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertTilemapToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.insertEmitterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.insertPathMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator46 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90ClockwiseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90AntiClockwiseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.centerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.middleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.horizontalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verticalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToBackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringToFrontMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.shiftBackwardsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shiftForewardsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator47 = new System.Windows.Forms.ToolStripSeparator();
            this.groupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ungroupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.propertyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assetManagerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sceneGraphViewerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.resetCameraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator34 = new System.Windows.Forms.ToolStripSeparator();
            this.viewBoundingboxsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewCollisionboxsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewEventLinesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOriginMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.pencilMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eraserMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paintBucketMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tilePickerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.rectangleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roundedRectangleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ellipseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator37 = new System.Windows.Forms.ToolStripSeparator();
            this.tileFlippingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipTileXMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipTileYMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbarsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator48 = new System.Windows.Forms.ToolStripSeparator();
            this.setBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpContentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tipOfTheDayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.visitWebsiteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.fpsStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.renderTimeStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.zoomStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cursorPositionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.toolsToolStrip = new System.Windows.Forms.ToolStrip();
            this.cameraToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.selectorToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.pencilToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.eraserToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.paintBucketToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tilePickerToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.rectangleToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.roundedRectangleToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ellipseToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.lineToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator36 = new System.Windows.Forms.ToolStripSeparator();
            this.tileFlipXToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tileFlipYToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.commandToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.commandToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.viewConsoleToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.gridWidthToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.gridHeightToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonYAxis = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonXAxis = new System.Windows.Forms.ToolStripButton();
            this.snapToGridToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator35 = new System.Windows.Forms.ToolStripSeparator();
            this.mapPanel = new System.Windows.Forms.Panel();
            this.fileToolStrip = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.playMapToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.redoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.buildProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mapPropertiesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
            this.assetManagerToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sceneGraphToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.aboutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.formatToolStrip = new System.Windows.Forms.ToolStrip();
            this.flipHorizontalToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.flipVerticalToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.rotateClockwiseToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rotateAnticlockwiseToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.alignBottomToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.alignTopToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.alignLeftToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.alignRightToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.alignMiddleToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.alignCenterToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.groupToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ungroupToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
            this.sendToBackToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.bringToFrontToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.shiftBackwardsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.shiftForewardsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.cameraToolStrip = new System.Windows.Forms.ToolStrip();
            this.zoomToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.zoomInToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomOutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.resetCameraToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mapCanvasContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undoContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator39 = new System.Windows.Forms.ToolStripSeparator();
            this.cutContextToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.copyContextToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteContextToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator32 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteContextToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateContextToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator33 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.insertContextToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator41 = new System.Windows.Forms.ToolStripSeparator();
            this.insertEntityContextToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.insertEmitterContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertTilemapContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertPathMarkerContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator42 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90ClockwiseContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90AntiClockwiseContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.topContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.middleContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.centerContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.horizontalContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verticalContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.shiftBackwardsContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shiftForewardsContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator44 = new System.Windows.Forms.ToolStripSeparator();
            this.sendToBackContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringToFrontContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator38 = new System.Windows.Forms.ToolStripSeparator();
            this.groupContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unGroupContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator43 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator40 = new System.Windows.Forms.ToolStripSeparator();
            this.resetCameraContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.propertyContextToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.toolsToolStrip.SuspendLayout();
            this.commandToolStrip.SuspendLayout();
            this.fileToolStrip.SuspendLayout();
            this.formatToolStrip.SuspendLayout();
            this.cameraToolStrip.SuspendLayout();
            this.mapCanvasContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.viewMenuItem,
            this.toolsMenuItem,
            this.windowToolStripMenuItem,
            this.helpMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(895, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.recentFilesMenuItem,
            this.toolStripSeparator2,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.toolStripSeparator20,
            this.playMapMenuItem,
            this.renderMapMenuItem,
            this.toolStripSeparator22,
            this.mapPropertiesMenuItem,
            this.toolStripSeparator3,
            this.buildProjectMenuItem,
            this.preferencesMenuItem,
            this.toolStripSeparator5,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "File";
            // 
            // newMenuItem
            // 
            this.newMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newMenuItem.Image")));
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.ShortcutKeyDisplayString = "";
            this.newMenuItem.Size = new System.Drawing.Size(154, 22);
            this.newMenuItem.Text = "New Map...";
            this.newMenuItem.Click += new System.EventHandler(this.newMenuItem_Click);
            // 
            // openMenuItem
            // 
            this.openMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openMenuItem.Image")));
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.ShortcutKeyDisplayString = "";
            this.openMenuItem.Size = new System.Drawing.Size(154, 22);
            this.openMenuItem.Text = "Open Map...";
            this.openMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
            // 
            // recentFilesMenuItem
            // 
            this.recentFilesMenuItem.Enabled = false;
            this.recentFilesMenuItem.Name = "recentFilesMenuItem";
            this.recentFilesMenuItem.Size = new System.Drawing.Size(154, 22);
            this.recentFilesMenuItem.Text = "Recent Files...";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(151, 6);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveMenuItem.Image")));
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveMenuItem.Text = "Save Map...";
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // saveAsMenuItem
            // 
            this.saveAsMenuItem.Name = "saveAsMenuItem";
            this.saveAsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveAsMenuItem.Text = "Save Map As...";
            this.saveAsMenuItem.Click += new System.EventHandler(this.saveAsMenuItem_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(151, 6);
            // 
            // playMapMenuItem
            // 
            this.playMapMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.play_map;
            this.playMapMenuItem.Name = "playMapMenuItem";
            this.playMapMenuItem.Size = new System.Drawing.Size(154, 22);
            this.playMapMenuItem.Text = "Play Map...";
            this.playMapMenuItem.Click += new System.EventHandler(this.playMapMenuItem_Click);
            // 
            // renderMapMenuItem
            // 
            this.renderMapMenuItem.Name = "renderMapMenuItem";
            this.renderMapMenuItem.Size = new System.Drawing.Size(154, 22);
            this.renderMapMenuItem.Text = "Render Map...";
            this.renderMapMenuItem.Click += new System.EventHandler(this.renderMapMenuItem_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(151, 6);
            // 
            // mapPropertiesMenuItem
            // 
            this.mapPropertiesMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.map_properties;
            this.mapPropertiesMenuItem.Name = "mapPropertiesMenuItem";
            this.mapPropertiesMenuItem.Size = new System.Drawing.Size(154, 22);
            this.mapPropertiesMenuItem.Text = "Map Properties";
            this.mapPropertiesMenuItem.Click += new System.EventHandler(this.mapPropertiesMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(151, 6);
            // 
            // buildProjectMenuItem
            // 
            this.buildProjectMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("buildProjectMenuItem.Image")));
            this.buildProjectMenuItem.Name = "buildProjectMenuItem";
            this.buildProjectMenuItem.Size = new System.Drawing.Size(154, 22);
            this.buildProjectMenuItem.Text = "Build Project";
            this.buildProjectMenuItem.Click += new System.EventHandler(this.buildProjectMenuItem_Click);
            // 
            // preferencesMenuItem
            // 
            this.preferencesMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("preferencesMenuItem.Image")));
            this.preferencesMenuItem.Name = "preferencesMenuItem";
            this.preferencesMenuItem.Size = new System.Drawing.Size(154, 22);
            this.preferencesMenuItem.Text = "Preferences...";
            this.preferencesMenuItem.Click += new System.EventHandler(this.preferencesMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(151, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitMenuItem.Image")));
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(154, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // editMenuItem
            // 
            this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.redoMenuItem,
            this.toolStripSeparator13,
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem,
            this.toolStripSeparator25,
            this.deleteMenuItem,
            this.duplicateMenuItem,
            this.toolStripSeparator14,
            this.toolStripMenuItem2,
            this.toolStripSeparator46,
            this.toolStripMenuItem15,
            this.toolStripSeparator47,
            this.groupMenuItem,
            this.ungroupMenuItem,
            this.toolStripSeparator16,
            this.selectAllMenuItem,
            this.deselectMenuItem,
            this.toolStripSeparator9,
            this.propertyMenuItem});
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editMenuItem.Text = "Edit";
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.undo;
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.Size = new System.Drawing.Size(127, 22);
            this.undoMenuItem.Text = "Undo";
            this.undoMenuItem.Click += new System.EventHandler(this.undoMenuItem_Click);
            // 
            // redoMenuItem
            // 
            this.redoMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.redo;
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.Size = new System.Drawing.Size(127, 22);
            this.redoMenuItem.Text = "Redo";
            this.redoMenuItem.Click += new System.EventHandler(this.redoMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(124, 6);
            // 
            // cutMenuItem
            // 
            this.cutMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.cut;
            this.cutMenuItem.Name = "cutMenuItem";
            this.cutMenuItem.Size = new System.Drawing.Size(127, 22);
            this.cutMenuItem.Text = "Cut";
            this.cutMenuItem.Click += new System.EventHandler(this.cutMenuItem_Click);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.copy;
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.Size = new System.Drawing.Size(127, 22);
            this.copyMenuItem.Text = "Copy";
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            // 
            // pasteMenuItem
            // 
            this.pasteMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.paste;
            this.pasteMenuItem.Name = "pasteMenuItem";
            this.pasteMenuItem.Size = new System.Drawing.Size(127, 22);
            this.pasteMenuItem.Text = "Paste";
            this.pasteMenuItem.Click += new System.EventHandler(this.pasteMenuItem_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(124, 6);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.delete;
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.Size = new System.Drawing.Size(127, 22);
            this.deleteMenuItem.Text = "Delete";
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // duplicateMenuItem
            // 
            this.duplicateMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.duplicate;
            this.duplicateMenuItem.Name = "duplicateMenuItem";
            this.duplicateMenuItem.Size = new System.Drawing.Size(127, 22);
            this.duplicateMenuItem.Text = "Duplicate";
            this.duplicateMenuItem.Click += new System.EventHandler(this.duplicateMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(124, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertObjectToolStripMenuItem,
            this.toolStripSeparator45,
            this.insertEntityToolStripMenuItem,
            this.insertTilemapToolStripMenuItem1,
            this.insertEmitterToolStripMenuItem1,
            this.insertPathMarkerToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem2.Text = "Insert";
            // 
            // insertObjectToolStripMenuItem
            // 
            this.insertObjectToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertObjectToolStripMenuItem.Name = "insertObjectToolStripMenuItem";
            this.insertObjectToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.insertObjectToolStripMenuItem.Text = "Insert Object";
            this.insertObjectToolStripMenuItem.Click += new System.EventHandler(this.insertObjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator45
            // 
            this.toolStripSeparator45.Name = "toolStripSeparator45";
            this.toolStripSeparator45.Size = new System.Drawing.Size(159, 6);
            // 
            // insertEntityToolStripMenuItem
            // 
            this.insertEntityToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertEntityToolStripMenuItem.Name = "insertEntityToolStripMenuItem";
            this.insertEntityToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.insertEntityToolStripMenuItem.Text = "Insert Entity";
            this.insertEntityToolStripMenuItem.Click += new System.EventHandler(this.insertEntityToolStripMenuItem_Click);
            // 
            // insertTilemapToolStripMenuItem1
            // 
            this.insertTilemapToolStripMenuItem1.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertTilemapToolStripMenuItem1.Name = "insertTilemapToolStripMenuItem1";
            this.insertTilemapToolStripMenuItem1.Size = new System.Drawing.Size(162, 22);
            this.insertTilemapToolStripMenuItem1.Text = "Insert Tilemap";
            this.insertTilemapToolStripMenuItem1.Click += new System.EventHandler(this.insertTilemapToolStripMenuItem1_Click);
            // 
            // insertEmitterToolStripMenuItem1
            // 
            this.insertEmitterToolStripMenuItem1.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertEmitterToolStripMenuItem1.Name = "insertEmitterToolStripMenuItem1";
            this.insertEmitterToolStripMenuItem1.Size = new System.Drawing.Size(162, 22);
            this.insertEmitterToolStripMenuItem1.Text = "Insert Emitter";
            this.insertEmitterToolStripMenuItem1.Click += new System.EventHandler(this.insertEmitterToolStripMenuItem1_Click);
            // 
            // insertPathMarkerToolStripMenuItem
            // 
            this.insertPathMarkerToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertPathMarkerToolStripMenuItem.Name = "insertPathMarkerToolStripMenuItem";
            this.insertPathMarkerToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.insertPathMarkerToolStripMenuItem.Text = "Insert Path Node";
            this.insertPathMarkerToolStripMenuItem.Click += new System.EventHandler(this.insertPathMarkerToolStripMenuItem_Click);
            // 
            // toolStripSeparator46
            // 
            this.toolStripSeparator46.Name = "toolStripSeparator46";
            this.toolStripSeparator46.Size = new System.Drawing.Size(124, 6);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rotateMenuItem,
            this.alignMenuItem,
            this.mirrorMenuItem,
            this.orderMenuItem});
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem15.Text = "Format";
            // 
            // rotateMenuItem
            // 
            this.rotateMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rotate90ClockwiseMenuItem,
            this.rotate90AntiClockwiseMenuItem});
            this.rotateMenuItem.Name = "rotateMenuItem";
            this.rotateMenuItem.Size = new System.Drawing.Size(108, 22);
            this.rotateMenuItem.Text = "Rotate";
            // 
            // rotate90ClockwiseMenuItem
            // 
            this.rotate90ClockwiseMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.clockwise;
            this.rotate90ClockwiseMenuItem.Name = "rotate90ClockwiseMenuItem";
            this.rotate90ClockwiseMenuItem.Size = new System.Drawing.Size(170, 22);
            this.rotate90ClockwiseMenuItem.Text = "90º clockwise";
            this.rotate90ClockwiseMenuItem.Click += new System.EventHandler(this.rotateClockwiseToolStripButton_Click);
            // 
            // rotate90AntiClockwiseMenuItem
            // 
            this.rotate90AntiClockwiseMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.anticlockwise;
            this.rotate90AntiClockwiseMenuItem.Name = "rotate90AntiClockwiseMenuItem";
            this.rotate90AntiClockwiseMenuItem.Size = new System.Drawing.Size(170, 22);
            this.rotate90AntiClockwiseMenuItem.Text = "90º anti-clockwise";
            this.rotate90AntiClockwiseMenuItem.Click += new System.EventHandler(this.rotateAnticlockwiseToolStripButton_Click);
            // 
            // alignMenuItem
            // 
            this.alignMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bottomMenuItem,
            this.topMenuItem,
            this.leftMenuItem,
            this.rightMenuItem,
            this.centerMenuItem,
            this.middleMenuItem});
            this.alignMenuItem.Name = "alignMenuItem";
            this.alignMenuItem.Size = new System.Drawing.Size(108, 22);
            this.alignMenuItem.Text = "Align";
            // 
            // bottomMenuItem
            // 
            this.bottomMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.bottom;
            this.bottomMenuItem.Name = "bottomMenuItem";
            this.bottomMenuItem.Size = new System.Drawing.Size(114, 22);
            this.bottomMenuItem.Text = "Bottom";
            this.bottomMenuItem.Click += new System.EventHandler(this.alignBottomToolStripButton_Click);
            // 
            // topMenuItem
            // 
            this.topMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.top;
            this.topMenuItem.Name = "topMenuItem";
            this.topMenuItem.Size = new System.Drawing.Size(114, 22);
            this.topMenuItem.Text = "Top";
            this.topMenuItem.Click += new System.EventHandler(this.alignTopToolStripButton_Click);
            // 
            // leftMenuItem
            // 
            this.leftMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.left;
            this.leftMenuItem.Name = "leftMenuItem";
            this.leftMenuItem.Size = new System.Drawing.Size(114, 22);
            this.leftMenuItem.Text = "Left";
            this.leftMenuItem.Click += new System.EventHandler(this.alignLeftToolStripButton_Click);
            // 
            // rightMenuItem
            // 
            this.rightMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.right;
            this.rightMenuItem.Name = "rightMenuItem";
            this.rightMenuItem.Size = new System.Drawing.Size(114, 22);
            this.rightMenuItem.Text = "Right";
            this.rightMenuItem.Click += new System.EventHandler(this.alignRightToolStripButton_Click);
            // 
            // centerMenuItem
            // 
            this.centerMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.center;
            this.centerMenuItem.Name = "centerMenuItem";
            this.centerMenuItem.Size = new System.Drawing.Size(114, 22);
            this.centerMenuItem.Text = "Center";
            this.centerMenuItem.Click += new System.EventHandler(this.alignCenterToolStripButton_Click);
            // 
            // middleMenuItem
            // 
            this.middleMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.middle;
            this.middleMenuItem.Name = "middleMenuItem";
            this.middleMenuItem.Size = new System.Drawing.Size(114, 22);
            this.middleMenuItem.Text = "Middle";
            this.middleMenuItem.Click += new System.EventHandler(this.alignMiddleToolStripButton_Click);
            // 
            // mirrorMenuItem
            // 
            this.mirrorMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.horizontalMenuItem,
            this.verticalMenuItem});
            this.mirrorMenuItem.Name = "mirrorMenuItem";
            this.mirrorMenuItem.Size = new System.Drawing.Size(108, 22);
            this.mirrorMenuItem.Text = "Mirror";
            // 
            // horizontalMenuItem
            // 
            this.horizontalMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.horizontal;
            this.horizontalMenuItem.Name = "horizontalMenuItem";
            this.horizontalMenuItem.Size = new System.Drawing.Size(129, 22);
            this.horizontalMenuItem.Text = "Horizontal";
            this.horizontalMenuItem.Click += new System.EventHandler(this.flipHorizontalToolStripButton_Click);
            // 
            // verticalMenuItem
            // 
            this.verticalMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.vertical;
            this.verticalMenuItem.Name = "verticalMenuItem";
            this.verticalMenuItem.Size = new System.Drawing.Size(129, 22);
            this.verticalMenuItem.Text = "Vertical";
            this.verticalMenuItem.Click += new System.EventHandler(this.flipVerticalToolStripButton_Click);
            // 
            // orderMenuItem
            // 
            this.orderMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendToBackMenuItem,
            this.bringToFrontMenuItem,
            this.toolStripSeparator15,
            this.shiftBackwardsMenuItem,
            this.shiftForewardsMenuItem});
            this.orderMenuItem.Name = "orderMenuItem";
            this.orderMenuItem.Size = new System.Drawing.Size(108, 22);
            this.orderMenuItem.Text = "Order";
            // 
            // sendToBackMenuItem
            // 
            this.sendToBackMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.back;
            this.sendToBackMenuItem.Name = "sendToBackMenuItem";
            this.sendToBackMenuItem.Size = new System.Drawing.Size(157, 22);
            this.sendToBackMenuItem.Text = "Send To Back";
            this.sendToBackMenuItem.Click += new System.EventHandler(this.sendToBackToolStripButton_Click);
            // 
            // bringToFrontMenuItem
            // 
            this.bringToFrontMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.front;
            this.bringToFrontMenuItem.Name = "bringToFrontMenuItem";
            this.bringToFrontMenuItem.Size = new System.Drawing.Size(157, 22);
            this.bringToFrontMenuItem.Text = "Bring To Front";
            this.bringToFrontMenuItem.Click += new System.EventHandler(this.bringToFrontToolStripButton_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(154, 6);
            // 
            // shiftBackwardsMenuItem
            // 
            this.shiftBackwardsMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.backwards;
            this.shiftBackwardsMenuItem.Name = "shiftBackwardsMenuItem";
            this.shiftBackwardsMenuItem.Size = new System.Drawing.Size(157, 22);
            this.shiftBackwardsMenuItem.Text = "Shift Backwards";
            this.shiftBackwardsMenuItem.Click += new System.EventHandler(this.shiftBackwardsToolStripButton_Click);
            // 
            // shiftForewardsMenuItem
            // 
            this.shiftForewardsMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.forwards;
            this.shiftForewardsMenuItem.Name = "shiftForewardsMenuItem";
            this.shiftForewardsMenuItem.Size = new System.Drawing.Size(157, 22);
            this.shiftForewardsMenuItem.Text = "Shift Forewards";
            this.shiftForewardsMenuItem.Click += new System.EventHandler(this.shiftForewardsToolStripButton_Click);
            // 
            // toolStripSeparator47
            // 
            this.toolStripSeparator47.Name = "toolStripSeparator47";
            this.toolStripSeparator47.Size = new System.Drawing.Size(124, 6);
            // 
            // groupMenuItem
            // 
            this.groupMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.group;
            this.groupMenuItem.Name = "groupMenuItem";
            this.groupMenuItem.Size = new System.Drawing.Size(127, 22);
            this.groupMenuItem.Text = "Group";
            this.groupMenuItem.Click += new System.EventHandler(this.groupToolStripButton_Click);
            // 
            // ungroupMenuItem
            // 
            this.ungroupMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.ungroup;
            this.ungroupMenuItem.Name = "ungroupMenuItem";
            this.ungroupMenuItem.Size = new System.Drawing.Size(127, 22);
            this.ungroupMenuItem.Text = "Ungroup";
            this.ungroupMenuItem.Click += new System.EventHandler(this.ungroupToolStripButton_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(124, 6);
            // 
            // selectAllMenuItem
            // 
            this.selectAllMenuItem.Name = "selectAllMenuItem";
            this.selectAllMenuItem.Size = new System.Drawing.Size(127, 22);
            this.selectAllMenuItem.Text = "Select All";
            this.selectAllMenuItem.Click += new System.EventHandler(this.selectAllMenuItem_Click);
            // 
            // deselectMenuItem
            // 
            this.deselectMenuItem.Name = "deselectMenuItem";
            this.deselectMenuItem.Size = new System.Drawing.Size(127, 22);
            this.deselectMenuItem.Text = "Deselect";
            this.deselectMenuItem.Click += new System.EventHandler(this.deselectMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(124, 6);
            // 
            // propertyMenuItem
            // 
            this.propertyMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.map_properties;
            this.propertyMenuItem.Name = "propertyMenuItem";
            this.propertyMenuItem.Size = new System.Drawing.Size(127, 22);
            this.propertyMenuItem.Text = "Properties";
            this.propertyMenuItem.Click += new System.EventHandler(this.propertyMenuItem_Click);
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.assetManagerMenuItem,
            this.sceneGraphViewerMenuItem,
            this.consoleMenuItem,
            this.toolStripSeparator24,
            this.resetCameraMenuItem,
            this.zoomOutToolStripMenuItem,
            this.zoomInToolStripMenuItem,
            this.toolStripSeparator34,
            this.viewBoundingboxsMenuItem,
            this.viewCollisionboxsMenuItem,
            this.viewEventLinesMenuItem,
            this.viewOriginMenuItem});
            this.viewMenuItem.Name = "viewMenuItem";
            this.viewMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewMenuItem.Text = "View";
            // 
            // assetManagerMenuItem
            // 
            this.assetManagerMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.asset_manager;
            this.assetManagerMenuItem.Name = "assetManagerMenuItem";
            this.assetManagerMenuItem.Size = new System.Drawing.Size(187, 22);
            this.assetManagerMenuItem.Text = "Asset Manager...";
            this.assetManagerMenuItem.Click += new System.EventHandler(this.assetManagerMenuItem_Click);
            // 
            // sceneGraphViewerMenuItem
            // 
            this.sceneGraphViewerMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.scene_graph;
            this.sceneGraphViewerMenuItem.Name = "sceneGraphViewerMenuItem";
            this.sceneGraphViewerMenuItem.Size = new System.Drawing.Size(187, 22);
            this.sceneGraphViewerMenuItem.Text = "Scene Graph Viewer...";
            this.sceneGraphViewerMenuItem.Click += new System.EventHandler(this.sceneGraphViewerMenuItem_Click);
            // 
            // consoleMenuItem
            // 
            this.consoleMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.console;
            this.consoleMenuItem.Name = "consoleMenuItem";
            this.consoleMenuItem.Size = new System.Drawing.Size(187, 22);
            this.consoleMenuItem.Text = "Console...";
            this.consoleMenuItem.Click += new System.EventHandler(this.consoleMenuItem_Click);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            this.toolStripSeparator24.Size = new System.Drawing.Size(184, 6);
            // 
            // resetCameraMenuItem
            // 
            this.resetCameraMenuItem.Name = "resetCameraMenuItem";
            this.resetCameraMenuItem.Size = new System.Drawing.Size(187, 22);
            this.resetCameraMenuItem.Text = "Reset Camera";
            this.resetCameraMenuItem.Click += new System.EventHandler(this.resetCameraMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // toolStripSeparator34
            // 
            this.toolStripSeparator34.Name = "toolStripSeparator34";
            this.toolStripSeparator34.Size = new System.Drawing.Size(184, 6);
            // 
            // viewBoundingboxsMenuItem
            // 
            this.viewBoundingboxsMenuItem.Name = "viewBoundingboxsMenuItem";
            this.viewBoundingboxsMenuItem.Size = new System.Drawing.Size(187, 22);
            this.viewBoundingboxsMenuItem.Text = "View Bounding Boxs";
            this.viewBoundingboxsMenuItem.Click += new System.EventHandler(this.viewBoundingboxsMenuItem_Click);
            // 
            // viewCollisionboxsMenuItem
            // 
            this.viewCollisionboxsMenuItem.Name = "viewCollisionboxsMenuItem";
            this.viewCollisionboxsMenuItem.Size = new System.Drawing.Size(187, 22);
            this.viewCollisionboxsMenuItem.Text = "View Collision Boxs";
            this.viewCollisionboxsMenuItem.Click += new System.EventHandler(this.viewCollisionboxsMenuItem_Click);
            // 
            // viewEventLinesMenuItem
            // 
            this.viewEventLinesMenuItem.Checked = true;
            this.viewEventLinesMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewEventLinesMenuItem.Name = "viewEventLinesMenuItem";
            this.viewEventLinesMenuItem.Size = new System.Drawing.Size(187, 22);
            this.viewEventLinesMenuItem.Text = "View Event Lines";
            this.viewEventLinesMenuItem.Click += new System.EventHandler(this.viewEventLinesMenuItem_Click);
            // 
            // viewOriginMenuItem
            // 
            this.viewOriginMenuItem.Checked = true;
            this.viewOriginMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewOriginMenuItem.Name = "viewOriginMenuItem";
            this.viewOriginMenuItem.Size = new System.Drawing.Size(187, 22);
            this.viewOriginMenuItem.Text = "View Origin";
            this.viewOriginMenuItem.Click += new System.EventHandler(this.viewOriginMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraMenuItem,
            this.selectorMenuItem,
            this.toolStripSeparator18,
            this.pencilMenuItem,
            this.eraserMenuItem,
            this.paintBucketMenuItem,
            this.tilePickerMenuItem,
            this.toolStripSeparator19,
            this.rectangleMenuItem,
            this.roundedRectangleMenuItem,
            this.ellipseMenuItem,
            this.lineMenuItem,
            this.toolStripSeparator37,
            this.tileFlippingMenuItem});
            this.toolsMenuItem.Name = "toolsMenuItem";
            this.toolsMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsMenuItem.Text = "Tools";
            // 
            // cameraMenuItem
            // 
            this.cameraMenuItem.Checked = true;
            this.cameraMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cameraMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.camera;
            this.cameraMenuItem.Name = "cameraMenuItem";
            this.cameraMenuItem.Size = new System.Drawing.Size(177, 22);
            this.cameraMenuItem.Text = "Camera";
            this.cameraMenuItem.Click += new System.EventHandler(this.cameraMenuItem_Click);
            // 
            // selectorMenuItem
            // 
            this.selectorMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("selectorMenuItem.Image")));
            this.selectorMenuItem.Name = "selectorMenuItem";
            this.selectorMenuItem.Size = new System.Drawing.Size(177, 22);
            this.selectorMenuItem.Text = "Selector";
            this.selectorMenuItem.Click += new System.EventHandler(this.selectorMenuItem_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(174, 6);
            // 
            // pencilMenuItem
            // 
            this.pencilMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pencilMenuItem.Image")));
            this.pencilMenuItem.Name = "pencilMenuItem";
            this.pencilMenuItem.Size = new System.Drawing.Size(177, 22);
            this.pencilMenuItem.Text = "Pencil";
            this.pencilMenuItem.Click += new System.EventHandler(this.pencilMenuItem_Click);
            // 
            // eraserMenuItem
            // 
            this.eraserMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("eraserMenuItem.Image")));
            this.eraserMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.eraserMenuItem.Name = "eraserMenuItem";
            this.eraserMenuItem.Size = new System.Drawing.Size(177, 22);
            this.eraserMenuItem.Text = "Eraser";
            this.eraserMenuItem.Click += new System.EventHandler(this.eraserMenuItem_Click);
            // 
            // paintBucketMenuItem
            // 
            this.paintBucketMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("paintBucketMenuItem.Image")));
            this.paintBucketMenuItem.Name = "paintBucketMenuItem";
            this.paintBucketMenuItem.Size = new System.Drawing.Size(177, 22);
            this.paintBucketMenuItem.Text = "Paint Bucket";
            this.paintBucketMenuItem.Click += new System.EventHandler(this.paintBucketMenuItem_Click);
            // 
            // tilePickerMenuItem
            // 
            this.tilePickerMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tilePickerMenuItem.Image")));
            this.tilePickerMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.tilePickerMenuItem.Name = "tilePickerMenuItem";
            this.tilePickerMenuItem.Size = new System.Drawing.Size(177, 22);
            this.tilePickerMenuItem.Text = "Tile Picker";
            this.tilePickerMenuItem.Click += new System.EventHandler(this.tilePickerMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(174, 6);
            // 
            // rectangleMenuItem
            // 
            this.rectangleMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("rectangleMenuItem.Image")));
            this.rectangleMenuItem.Name = "rectangleMenuItem";
            this.rectangleMenuItem.Size = new System.Drawing.Size(177, 22);
            this.rectangleMenuItem.Text = "Rectangle";
            this.rectangleMenuItem.Click += new System.EventHandler(this.rectangleMenuItem_Click);
            // 
            // roundedRectangleMenuItem
            // 
            this.roundedRectangleMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("roundedRectangleMenuItem.Image")));
            this.roundedRectangleMenuItem.Name = "roundedRectangleMenuItem";
            this.roundedRectangleMenuItem.Size = new System.Drawing.Size(177, 22);
            this.roundedRectangleMenuItem.Text = "Rounded Rectangle";
            this.roundedRectangleMenuItem.Click += new System.EventHandler(this.roundedRectangleMenuItem_Click);
            // 
            // ellipseMenuItem
            // 
            this.ellipseMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ellipseMenuItem.Image")));
            this.ellipseMenuItem.Name = "ellipseMenuItem";
            this.ellipseMenuItem.Size = new System.Drawing.Size(177, 22);
            this.ellipseMenuItem.Text = "Ellipse";
            this.ellipseMenuItem.Click += new System.EventHandler(this.ellipseMenuItem_Click);
            // 
            // lineMenuItem
            // 
            this.lineMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.line;
            this.lineMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.lineMenuItem.Name = "lineMenuItem";
            this.lineMenuItem.Size = new System.Drawing.Size(177, 22);
            this.lineMenuItem.Text = "Line";
            this.lineMenuItem.Click += new System.EventHandler(this.lineMenuItem_Click);
            // 
            // toolStripSeparator37
            // 
            this.toolStripSeparator37.Name = "toolStripSeparator37";
            this.toolStripSeparator37.Size = new System.Drawing.Size(174, 6);
            // 
            // tileFlippingMenuItem
            // 
            this.tileFlippingMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flipTileXMenuItem,
            this.flipTileYMenuItem});
            this.tileFlippingMenuItem.Name = "tileFlippingMenuItem";
            this.tileFlippingMenuItem.Size = new System.Drawing.Size(177, 22);
            this.tileFlippingMenuItem.Text = "Tile Flipping";
            // 
            // flipTileXMenuItem
            // 
            this.flipTileXMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.horizontal;
            this.flipTileXMenuItem.Name = "flipTileXMenuItem";
            this.flipTileXMenuItem.Size = new System.Drawing.Size(157, 22);
            this.flipTileXMenuItem.Text = "Flip Horizontaly";
            this.flipTileXMenuItem.Click += new System.EventHandler(this.flipTileXMenuItem_Click);
            // 
            // flipTileYMenuItem
            // 
            this.flipTileYMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.vertical;
            this.flipTileYMenuItem.Name = "flipTileYMenuItem";
            this.flipTileYMenuItem.Size = new System.Drawing.Size(157, 22);
            this.flipTileYMenuItem.Text = "Flip Verticaly";
            this.flipTileYMenuItem.Click += new System.EventHandler(this.flipTileYMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarsToolStripMenuItem,
            this.toolStripSeparator48,
            this.setBackgroundToolStripMenuItem,
            this.clearBackgroundToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // toolbarsToolStripMenuItem
            // 
            this.toolbarsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolbarToolStripMenuItem,
            this.formatToolbarToolStripMenuItem,
            this.toolsToolbarToolStripMenuItem,
            this.commandToolbarToolStripMenuItem,
            this.cameraToolbarToolStripMenuItem});
            this.toolbarsToolStripMenuItem.Name = "toolbarsToolStripMenuItem";
            this.toolbarsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.toolbarsToolStripMenuItem.Text = "Toolbars";
            // 
            // fileToolbarToolStripMenuItem
            // 
            this.fileToolbarToolStripMenuItem.Checked = true;
            this.fileToolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fileToolbarToolStripMenuItem.Name = "fileToolbarToolStripMenuItem";
            this.fileToolbarToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.fileToolbarToolStripMenuItem.Text = "File";
            this.fileToolbarToolStripMenuItem.Click += new System.EventHandler(this.fileToolbarToolStripMenuItem_Click);
            // 
            // formatToolbarToolStripMenuItem
            // 
            this.formatToolbarToolStripMenuItem.Checked = true;
            this.formatToolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.formatToolbarToolStripMenuItem.Name = "formatToolbarToolStripMenuItem";
            this.formatToolbarToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.formatToolbarToolStripMenuItem.Text = "Format ";
            this.formatToolbarToolStripMenuItem.Click += new System.EventHandler(this.formatToolbarToolStripMenuItem_Click);
            // 
            // toolsToolbarToolStripMenuItem
            // 
            this.toolsToolbarToolStripMenuItem.Checked = true;
            this.toolsToolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolsToolbarToolStripMenuItem.Name = "toolsToolbarToolStripMenuItem";
            this.toolsToolbarToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.toolsToolbarToolStripMenuItem.Text = "Tools";
            this.toolsToolbarToolStripMenuItem.Click += new System.EventHandler(this.toolsToolbarToolStripMenuItem_Click);
            // 
            // commandToolbarToolStripMenuItem
            // 
            this.commandToolbarToolStripMenuItem.Checked = true;
            this.commandToolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.commandToolbarToolStripMenuItem.Name = "commandToolbarToolStripMenuItem";
            this.commandToolbarToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.commandToolbarToolStripMenuItem.Text = "Command";
            this.commandToolbarToolStripMenuItem.Click += new System.EventHandler(this.commandToolbarToolStripMenuItem_Click);
            // 
            // cameraToolbarToolStripMenuItem
            // 
            this.cameraToolbarToolStripMenuItem.Checked = true;
            this.cameraToolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cameraToolbarToolStripMenuItem.Name = "cameraToolbarToolStripMenuItem";
            this.cameraToolbarToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.cameraToolbarToolStripMenuItem.Text = "Camera";
            this.cameraToolbarToolStripMenuItem.Click += new System.EventHandler(this.cameraToolbarToolStripMenuItem_Click);
            // 
            // toolStripSeparator48
            // 
            this.toolStripSeparator48.Name = "toolStripSeparator48";
            this.toolStripSeparator48.Size = new System.Drawing.Size(165, 6);
            // 
            // setBackgroundToolStripMenuItem
            // 
            this.setBackgroundToolStripMenuItem.Name = "setBackgroundToolStripMenuItem";
            this.setBackgroundToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.setBackgroundToolStripMenuItem.Text = "Set Background";
            this.setBackgroundToolStripMenuItem.Click += new System.EventHandler(this.setBackgroundToolStripMenuItem_Click);
            // 
            // clearBackgroundToolStripMenuItem
            // 
            this.clearBackgroundToolStripMenuItem.Enabled = false;
            this.clearBackgroundToolStripMenuItem.Name = "clearBackgroundToolStripMenuItem";
            this.clearBackgroundToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.clearBackgroundToolStripMenuItem.Text = "Clear Background";
            this.clearBackgroundToolStripMenuItem.Click += new System.EventHandler(this.clearBackgroundToolStripMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpContentsMenuItem,
            this.tipOfTheDayMenuItem,
            this.toolStripSeparator6,
            this.visitWebsiteMenuItem,
            this.checkForUpdatesMenuItem,
            this.toolStripSeparator1,
            this.aboutMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpMenuItem.Text = "Help";
            // 
            // helpContentsMenuItem
            // 
            this.helpContentsMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("helpContentsMenuItem.Image")));
            this.helpContentsMenuItem.Name = "helpContentsMenuItem";
            this.helpContentsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.helpContentsMenuItem.Text = "Help Contents...";
            this.helpContentsMenuItem.Click += new System.EventHandler(this.helpContentsMenuItem_Click);
            // 
            // tipOfTheDayMenuItem
            // 
            this.tipOfTheDayMenuItem.Name = "tipOfTheDayMenuItem";
            this.tipOfTheDayMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tipOfTheDayMenuItem.Text = "Tip Of The Day...";
            this.tipOfTheDayMenuItem.Click += new System.EventHandler(this.tipOfTheDayMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(177, 6);
            // 
            // visitWebsiteMenuItem
            // 
            this.visitWebsiteMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("visitWebsiteMenuItem.Image")));
            this.visitWebsiteMenuItem.Name = "visitWebsiteMenuItem";
            this.visitWebsiteMenuItem.Size = new System.Drawing.Size(180, 22);
            this.visitWebsiteMenuItem.Text = "Visit Website...";
            this.visitWebsiteMenuItem.Click += new System.EventHandler(this.visitWebsiteMenuItem_Click);
            // 
            // checkForUpdatesMenuItem
            // 
            this.checkForUpdatesMenuItem.Name = "checkForUpdatesMenuItem";
            this.checkForUpdatesMenuItem.Size = new System.Drawing.Size(180, 22);
            this.checkForUpdatesMenuItem.Text = "Check for Updates...";
            this.checkForUpdatesMenuItem.Click += new System.EventHandler(this.checkForUpdatesMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutMenuItem.Image")));
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutMenuItem.Text = "About...";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.fpsStatusLabel,
            this.renderTimeStatusLabel,
            this.zoomStatusLabel,
            this.cursorPositionStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 575);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(895, 22);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // fpsStatusLabel
            // 
            this.fpsStatusLabel.Name = "fpsStatusLabel";
            this.fpsStatusLabel.Size = new System.Drawing.Size(119, 17);
            this.fpsStatusLabel.Text = "Frames Per Second: 0";
            // 
            // renderTimeStatusLabel
            // 
            this.renderTimeStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.renderTimeStatusLabel.Name = "renderTimeStatusLabel";
            this.renderTimeStatusLabel.Size = new System.Drawing.Size(119, 17);
            this.renderTimeStatusLabel.Text = "Rendering Time: 0ms";
            // 
            // zoomStatusLabel
            // 
            this.zoomStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.zoomStatusLabel.Name = "zoomStatusLabel";
            this.zoomStatusLabel.Size = new System.Drawing.Size(73, 17);
            this.zoomStatusLabel.Text = "Zoom: 100%";
            // 
            // cursorPositionStatusLabel
            // 
            this.cursorPositionStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.cursorPositionStatusLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cursorPositionStatusLabel.Name = "cursorPositionStatusLabel";
            this.cursorPositionStatusLabel.Size = new System.Drawing.Size(569, 17);
            this.cursorPositionStatusLabel.Spring = true;
            this.cursorPositionStatusLabel.Text = "X:0 Y:0 x:0 y:0";
            this.cursorPositionStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cursorPositionStatusLabel.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(23, 23);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(732, 524);
            this.panel1.TabIndex = 9;
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.BottomToolStripPanel
            // 
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.toolsToolStrip);
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.commandToolStrip);
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.BackColor = System.Drawing.Color.Transparent;
            this.toolStripContainer.ContentPanel.Controls.Add(this.mapPanel);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(895, 501);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(895, 575);
            this.toolStripContainer.TabIndex = 3;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.cameraToolStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.fileToolStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.formatToolStrip);
            this.toolStripContainer.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // toolsToolStrip
            // 
            this.toolsToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraToolStripButton,
            this.selectorToolStripButton,
            this.toolStripSeparator21,
            this.pencilToolStripButton,
            this.eraserToolStripButton,
            this.paintBucketToolStripButton,
            this.tilePickerToolStripButton,
            this.toolStripSeparator23,
            this.rectangleToolStripButton,
            this.roundedRectangleToolStripButton,
            this.ellipseToolStripButton,
            this.lineToolStripButton,
            this.toolStripSeparator36,
            this.tileFlipXToolStripButton,
            this.tileFlipYToolStripButton});
            this.toolsToolStrip.Location = new System.Drawing.Point(3, 0);
            this.toolsToolStrip.Name = "toolsToolStrip";
            this.toolsToolStrip.Size = new System.Drawing.Size(306, 25);
            this.toolsToolStrip.TabIndex = 0;
            this.toolsToolStrip.EndDrag += new System.EventHandler(this.palleteToolStrip_EndDrag);
            // 
            // cameraToolStripButton
            // 
            this.cameraToolStripButton.Checked = true;
            this.cameraToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cameraToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cameraToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.camera;
            this.cameraToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cameraToolStripButton.Name = "cameraToolStripButton";
            this.cameraToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cameraToolStripButton.Text = "Camera";
            this.cameraToolStripButton.Click += new System.EventHandler(this.cameraToolStripButton_Click);
            // 
            // selectorToolStripButton
            // 
            this.selectorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectorToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("selectorToolStripButton.Image")));
            this.selectorToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectorToolStripButton.Name = "selectorToolStripButton";
            this.selectorToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.selectorToolStripButton.Text = "Selector";
            this.selectorToolStripButton.Click += new System.EventHandler(this.selectorToolStripButton_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(6, 25);
            // 
            // pencilToolStripButton
            // 
            this.pencilToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pencilToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pencilToolStripButton.Image")));
            this.pencilToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pencilToolStripButton.Name = "pencilToolStripButton";
            this.pencilToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pencilToolStripButton.Text = "Pencil";
            this.pencilToolStripButton.Click += new System.EventHandler(this.pencilToolStripButton_Click);
            // 
            // eraserToolStripButton
            // 
            this.eraserToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.eraserToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("eraserToolStripButton.Image")));
            this.eraserToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.eraserToolStripButton.Name = "eraserToolStripButton";
            this.eraserToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.eraserToolStripButton.Text = "Eraser";
            this.eraserToolStripButton.Click += new System.EventHandler(this.eraserToolStripButton_Click);
            // 
            // paintBucketToolStripButton
            // 
            this.paintBucketToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.paintBucketToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("paintBucketToolStripButton.Image")));
            this.paintBucketToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.paintBucketToolStripButton.Name = "paintBucketToolStripButton";
            this.paintBucketToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.paintBucketToolStripButton.Text = "Paint Bucket";
            this.paintBucketToolStripButton.Click += new System.EventHandler(this.paintBucketToolStripButton_Click);
            // 
            // tilePickerToolStripButton
            // 
            this.tilePickerToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tilePickerToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("tilePickerToolStripButton.Image")));
            this.tilePickerToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.tilePickerToolStripButton.Name = "tilePickerToolStripButton";
            this.tilePickerToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.tilePickerToolStripButton.Text = "Tile Picker";
            this.tilePickerToolStripButton.Click += new System.EventHandler(this.tilePickerToolStripButton_Click);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(6, 25);
            // 
            // rectangleToolStripButton
            // 
            this.rectangleToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rectangleToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("rectangleToolStripButton.Image")));
            this.rectangleToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rectangleToolStripButton.Name = "rectangleToolStripButton";
            this.rectangleToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.rectangleToolStripButton.Text = "Rectangle";
            this.rectangleToolStripButton.Click += new System.EventHandler(this.rectangleToolStripButton_Click);
            // 
            // roundedRectangleToolStripButton
            // 
            this.roundedRectangleToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.roundedRectangleToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("roundedRectangleToolStripButton.Image")));
            this.roundedRectangleToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.roundedRectangleToolStripButton.Name = "roundedRectangleToolStripButton";
            this.roundedRectangleToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.roundedRectangleToolStripButton.Text = "Rounded Rectangle";
            this.roundedRectangleToolStripButton.Click += new System.EventHandler(this.roundedRectangleToolStripButton_Click);
            // 
            // ellipseToolStripButton
            // 
            this.ellipseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ellipseToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ellipseToolStripButton.Image")));
            this.ellipseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ellipseToolStripButton.Name = "ellipseToolStripButton";
            this.ellipseToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.ellipseToolStripButton.Text = "Ellipse";
            this.ellipseToolStripButton.Click += new System.EventHandler(this.ellipseToolStripButton_Click);
            // 
            // lineToolStripButton
            // 
            this.lineToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lineToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.line;
            this.lineToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.lineToolStripButton.Name = "lineToolStripButton";
            this.lineToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.lineToolStripButton.Text = "Line";
            this.lineToolStripButton.Click += new System.EventHandler(this.lineMenuItem_Click);
            // 
            // toolStripSeparator36
            // 
            this.toolStripSeparator36.Name = "toolStripSeparator36";
            this.toolStripSeparator36.Size = new System.Drawing.Size(6, 25);
            // 
            // tileFlipXToolStripButton
            // 
            this.tileFlipXToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tileFlipXToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.horizontal;
            this.tileFlipXToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tileFlipXToolStripButton.Name = "tileFlipXToolStripButton";
            this.tileFlipXToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.tileFlipXToolStripButton.Text = "toolStripButton9";
            this.tileFlipXToolStripButton.ToolTipText = "Flip Tiles Horizontaly";
            this.tileFlipXToolStripButton.Click += new System.EventHandler(this.tileFlipXToolStripButton_Click);
            // 
            // tileFlipYToolStripButton
            // 
            this.tileFlipYToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tileFlipYToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.vertical;
            this.tileFlipYToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tileFlipYToolStripButton.Name = "tileFlipYToolStripButton";
            this.tileFlipYToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.tileFlipYToolStripButton.Text = "toolStripButton10";
            this.tileFlipYToolStripButton.ToolTipText = "Flip Tiles Verticaly";
            this.tileFlipYToolStripButton.Click += new System.EventHandler(this.tileFlipYToolStripButton_Click);
            // 
            // commandToolStrip
            // 
            this.commandToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.commandToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.commandToolStripTextBox,
            this.viewConsoleToolStripButton,
            this.toolStripSeparator10,
            this.toolStripLabel2,
            this.gridWidthToolStripTextBox,
            this.gridHeightToolStripTextBox,
            this.toolStripButtonYAxis,
            this.toolStripButtonXAxis,
            this.snapToGridToolStripButton,
            this.toolStripSeparator35});
            this.commandToolStrip.Location = new System.Drawing.Point(309, 0);
            this.commandToolStrip.Name = "commandToolStrip";
            this.commandToolStrip.Size = new System.Drawing.Size(488, 25);
            this.commandToolStrip.TabIndex = 0;
            this.commandToolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.commandToolStrip_ItemClicked);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(64, 22);
            this.toolStripLabel1.Text = "Command";
            // 
            // commandToolStripTextBox
            // 
            this.commandToolStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.commandToolStripTextBox.Name = "commandToolStripTextBox";
            this.commandToolStripTextBox.Size = new System.Drawing.Size(200, 25);
            this.commandToolStripTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.commandToolStripTextBox_KeyPress);
            // 
            // viewConsoleToolStripButton
            // 
            this.viewConsoleToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewConsoleToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.console;
            this.viewConsoleToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewConsoleToolStripButton.Name = "viewConsoleToolStripButton";
            this.viewConsoleToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.viewConsoleToolStripButton.Text = "Console";
            this.viewConsoleToolStripButton.Click += new System.EventHandler(this.viewConsoleToolStripButton_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(52, 22);
            this.toolStripLabel2.Text = "Grid Size";
            // 
            // gridWidthToolStripTextBox
            // 
            this.gridWidthToolStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridWidthToolStripTextBox.Name = "gridWidthToolStripTextBox";
            this.gridWidthToolStripTextBox.Size = new System.Drawing.Size(25, 25);
            this.gridWidthToolStripTextBox.Text = "8";
            this.gridWidthToolStripTextBox.TextChanged += new System.EventHandler(this.gridWidthToolStripTextBox_TextChanged);
            // 
            // gridHeightToolStripTextBox
            // 
            this.gridHeightToolStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridHeightToolStripTextBox.Name = "gridHeightToolStripTextBox";
            this.gridHeightToolStripTextBox.Size = new System.Drawing.Size(25, 25);
            this.gridHeightToolStripTextBox.Text = "8";
            this.gridHeightToolStripTextBox.TextChanged += new System.EventHandler(this.gridHeightToolStripTextBox_TextChanged);
            // 
            // toolStripButtonYAxis
            // 
            this.toolStripButtonYAxis.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonYAxis.Checked = true;
            this.toolStripButtonYAxis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonYAxis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonYAxis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonYAxis.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonYAxis.Image")));
            this.toolStripButtonYAxis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonYAxis.Name = "toolStripButtonYAxis";
            this.toolStripButtonYAxis.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonYAxis.Text = "Y";
            this.toolStripButtonYAxis.Click += new System.EventHandler(this.toolStripButtonYAxis_Click);
            // 
            // toolStripButtonXAxis
            // 
            this.toolStripButtonXAxis.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonXAxis.Checked = true;
            this.toolStripButtonXAxis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonXAxis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonXAxis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonXAxis.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonXAxis.Image")));
            this.toolStripButtonXAxis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonXAxis.Name = "toolStripButtonXAxis";
            this.toolStripButtonXAxis.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonXAxis.Text = "X";
            this.toolStripButtonXAxis.Click += new System.EventHandler(this.toolStripButtonXAxis_Click);
            // 
            // snapToGridToolStripButton
            // 
            this.snapToGridToolStripButton.Checked = true;
            this.snapToGridToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.snapToGridToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.snapToGridToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.snaptogrid;
            this.snapToGridToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.snapToGridToolStripButton.Name = "snapToGridToolStripButton";
            this.snapToGridToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.snapToGridToolStripButton.Text = "Snap To Grid";
            this.snapToGridToolStripButton.Click += new System.EventHandler(this.snapToGridToolStripButton_Click);
            // 
            // toolStripSeparator35
            // 
            this.toolStripSeparator35.Name = "toolStripSeparator35";
            this.toolStripSeparator35.Size = new System.Drawing.Size(6, 25);
            // 
            // mapPanel
            // 
            this.mapPanel.BackColor = System.Drawing.Color.LightGray;
            this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPanel.Location = new System.Drawing.Point(0, 0);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(895, 501);
            this.mapPanel.TabIndex = 4;
            // 
            // fileToolStrip
            // 
            this.fileToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.fileToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator7,
            this.playMapToolStripButton,
            this.toolStripSeparator26,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.toolStripSeparator8,
            this.undoToolStripButton,
            this.redoToolStripButton,
            this.toolStripSeparator11,
            this.buildProjectToolStripButton,
            this.mapPropertiesToolStripButton,
            this.toolStripSeparator30,
            this.assetManagerToolStripButton,
            this.sceneGraphToolStripButton,
            this.toolStripSeparator31,
            this.helpToolStripButton,
            this.aboutToolStripButton});
            this.fileToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.fileToolStrip.Location = new System.Drawing.Point(3, 24);
            this.fileToolStrip.Name = "fileToolStrip";
            this.fileToolStrip.Size = new System.Drawing.Size(393, 25);
            this.fileToolStrip.TabIndex = 2;
            this.fileToolStrip.Text = "toolStrip1";
            this.fileToolStrip.EndDrag += new System.EventHandler(this.fileToolStrip_EndDrag);
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "New Map";
            this.newToolStripButton.Click += new System.EventHandler(this.newToolStripButton_Click);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "Open Map";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "Save Map";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // playMapToolStripButton
            // 
            this.playMapToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.playMapToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.play_map;
            this.playMapToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playMapToolStripButton.Name = "playMapToolStripButton";
            this.playMapToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.playMapToolStripButton.Text = "Play Map";
            this.playMapToolStripButton.Click += new System.EventHandler(this.playMapToolStripButton_Click);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            this.toolStripSeparator26.Size = new System.Drawing.Size(6, 25);
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.cut;
            this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cutToolStripButton.Text = "Cut";
            this.cutToolStripButton.Click += new System.EventHandler(this.cutToolStripButton_Click);
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.copy;
            this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.copyToolStripButton.Text = "Copy";
            this.copyToolStripButton.Click += new System.EventHandler(this.copyToolStripButton_Click);
            // 
            // pasteToolStripButton
            // 
            this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.paste;
            this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripButton.Name = "pasteToolStripButton";
            this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pasteToolStripButton.Text = "Paste";
            this.pasteToolStripButton.Click += new System.EventHandler(this.pasteToolStripButton_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // undoToolStripButton
            // 
            this.undoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.undo;
            this.undoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoToolStripButton.Name = "undoToolStripButton";
            this.undoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.undoToolStripButton.Text = "Undo";
            this.undoToolStripButton.Click += new System.EventHandler(this.undoToolStripButton_Click);
            // 
            // redoToolStripButton
            // 
            this.redoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redoToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.redo;
            this.redoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redoToolStripButton.Name = "redoToolStripButton";
            this.redoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.redoToolStripButton.Text = "Redo";
            this.redoToolStripButton.Click += new System.EventHandler(this.redoToolStripButton_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // buildProjectToolStripButton
            // 
            this.buildProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buildProjectToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.build_project;
            this.buildProjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buildProjectToolStripButton.Name = "buildProjectToolStripButton";
            this.buildProjectToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.buildProjectToolStripButton.Text = "Build Project";
            this.buildProjectToolStripButton.Click += new System.EventHandler(this.buildProjectToolStripButton_Click);
            // 
            // mapPropertiesToolStripButton
            // 
            this.mapPropertiesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mapPropertiesToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.map_properties;
            this.mapPropertiesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mapPropertiesToolStripButton.Name = "mapPropertiesToolStripButton";
            this.mapPropertiesToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.mapPropertiesToolStripButton.Text = "Map Properties";
            this.mapPropertiesToolStripButton.Click += new System.EventHandler(this.mapPropertiesToolStripButton_Click);
            // 
            // toolStripSeparator30
            // 
            this.toolStripSeparator30.Name = "toolStripSeparator30";
            this.toolStripSeparator30.Size = new System.Drawing.Size(6, 25);
            // 
            // assetManagerToolStripButton
            // 
            this.assetManagerToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.assetManagerToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.asset_manager;
            this.assetManagerToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.assetManagerToolStripButton.Name = "assetManagerToolStripButton";
            this.assetManagerToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.assetManagerToolStripButton.Text = "Asset Manager";
            this.assetManagerToolStripButton.Click += new System.EventHandler(this.assetManagerToolStripButton_Click);
            // 
            // sceneGraphToolStripButton
            // 
            this.sceneGraphToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sceneGraphToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.scene_graph;
            this.sceneGraphToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sceneGraphToolStripButton.Name = "sceneGraphToolStripButton";
            this.sceneGraphToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.sceneGraphToolStripButton.Text = "Scene Graph";
            this.sceneGraphToolStripButton.Click += new System.EventHandler(this.sceneGraphToolStripButton_Click);
            // 
            // toolStripSeparator31
            // 
            this.toolStripSeparator31.Name = "toolStripSeparator31";
            this.toolStripSeparator31.Size = new System.Drawing.Size(6, 25);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.help;
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.helpToolStripButton.Text = "Help";
            this.helpToolStripButton.Click += new System.EventHandler(this.helpToolStripButton_Click);
            // 
            // aboutToolStripButton
            // 
            this.aboutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.aboutToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.information;
            this.aboutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.aboutToolStripButton.Name = "aboutToolStripButton";
            this.aboutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.aboutToolStripButton.Text = "About";
            this.aboutToolStripButton.Click += new System.EventHandler(this.aboutToolStripButton_Click);
            // 
            // formatToolStrip
            // 
            this.formatToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.formatToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flipHorizontalToolStripButton,
            this.flipVerticalToolStripButton,
            this.toolStripSeparator12,
            this.rotateClockwiseToolStripButton,
            this.rotateAnticlockwiseToolStripButton,
            this.toolStripSeparator27,
            this.alignBottomToolStripButton,
            this.alignTopToolStripButton,
            this.alignLeftToolStripButton,
            this.alignRightToolStripButton,
            this.alignMiddleToolStripButton,
            this.alignCenterToolStripButton,
            this.toolStripSeparator28,
            this.groupToolStripButton,
            this.ungroupToolStripButton,
            this.toolStripSeparator29,
            this.sendToBackToolStripButton,
            this.bringToFrontToolStripButton,
            this.shiftBackwardsToolStripButton,
            this.shiftForewardsToolStripButton});
            this.formatToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.formatToolStrip.Location = new System.Drawing.Point(591, 24);
            this.formatToolStrip.Name = "formatToolStrip";
            this.formatToolStrip.Size = new System.Drawing.Size(304, 25);
            this.formatToolStrip.TabIndex = 5;
            this.formatToolStrip.EndDrag += new System.EventHandler(this.rotateToolStrip_EndDrag);
            // 
            // flipHorizontalToolStripButton
            // 
            this.flipHorizontalToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.flipHorizontalToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.horizontal;
            this.flipHorizontalToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.flipHorizontalToolStripButton.Name = "flipHorizontalToolStripButton";
            this.flipHorizontalToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.flipHorizontalToolStripButton.Text = "Flip Horizontal";
            this.flipHorizontalToolStripButton.Click += new System.EventHandler(this.flipHorizontalToolStripButton_Click);
            // 
            // flipVerticalToolStripButton
            // 
            this.flipVerticalToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.flipVerticalToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.vertical;
            this.flipVerticalToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.flipVerticalToolStripButton.Name = "flipVerticalToolStripButton";
            this.flipVerticalToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.flipVerticalToolStripButton.Text = "Flip Vertical";
            this.flipVerticalToolStripButton.Click += new System.EventHandler(this.flipVerticalToolStripButton_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // rotateClockwiseToolStripButton
            // 
            this.rotateClockwiseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateClockwiseToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.clockwise;
            this.rotateClockwiseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateClockwiseToolStripButton.Name = "rotateClockwiseToolStripButton";
            this.rotateClockwiseToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.rotateClockwiseToolStripButton.Text = "Rotate Clockwise";
            this.rotateClockwiseToolStripButton.Click += new System.EventHandler(this.rotateClockwiseToolStripButton_Click);
            // 
            // rotateAnticlockwiseToolStripButton
            // 
            this.rotateAnticlockwiseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateAnticlockwiseToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.anticlockwise;
            this.rotateAnticlockwiseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateAnticlockwiseToolStripButton.Name = "rotateAnticlockwiseToolStripButton";
            this.rotateAnticlockwiseToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.rotateAnticlockwiseToolStripButton.Text = "Rotate Anti-Clockwise";
            this.rotateAnticlockwiseToolStripButton.Click += new System.EventHandler(this.rotateAnticlockwiseToolStripButton_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            this.toolStripSeparator27.Size = new System.Drawing.Size(6, 25);
            // 
            // alignBottomToolStripButton
            // 
            this.alignBottomToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.alignBottomToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.bottom;
            this.alignBottomToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.alignBottomToolStripButton.Name = "alignBottomToolStripButton";
            this.alignBottomToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.alignBottomToolStripButton.Text = "Align At Bottom";
            this.alignBottomToolStripButton.Click += new System.EventHandler(this.alignBottomToolStripButton_Click);
            // 
            // alignTopToolStripButton
            // 
            this.alignTopToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.alignTopToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.top;
            this.alignTopToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.alignTopToolStripButton.Name = "alignTopToolStripButton";
            this.alignTopToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.alignTopToolStripButton.Text = "Align At Top";
            this.alignTopToolStripButton.Click += new System.EventHandler(this.alignTopToolStripButton_Click);
            // 
            // alignLeftToolStripButton
            // 
            this.alignLeftToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.alignLeftToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.left;
            this.alignLeftToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.alignLeftToolStripButton.Name = "alignLeftToolStripButton";
            this.alignLeftToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.alignLeftToolStripButton.Text = "Align At Left Side";
            this.alignLeftToolStripButton.Click += new System.EventHandler(this.alignLeftToolStripButton_Click);
            // 
            // alignRightToolStripButton
            // 
            this.alignRightToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.alignRightToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.right;
            this.alignRightToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.alignRightToolStripButton.Name = "alignRightToolStripButton";
            this.alignRightToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.alignRightToolStripButton.Text = "Align At Right Side";
            this.alignRightToolStripButton.Click += new System.EventHandler(this.alignRightToolStripButton_Click);
            // 
            // alignMiddleToolStripButton
            // 
            this.alignMiddleToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.alignMiddleToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.middle;
            this.alignMiddleToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.alignMiddleToolStripButton.Name = "alignMiddleToolStripButton";
            this.alignMiddleToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.alignMiddleToolStripButton.Text = "Align In Middle";
            this.alignMiddleToolStripButton.Click += new System.EventHandler(this.alignMiddleToolStripButton_Click);
            // 
            // alignCenterToolStripButton
            // 
            this.alignCenterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.alignCenterToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.center;
            this.alignCenterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.alignCenterToolStripButton.Name = "alignCenterToolStripButton";
            this.alignCenterToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.alignCenterToolStripButton.Text = "Align In Center";
            this.alignCenterToolStripButton.Click += new System.EventHandler(this.alignCenterToolStripButton_Click);
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            this.toolStripSeparator28.Size = new System.Drawing.Size(6, 25);
            // 
            // groupToolStripButton
            // 
            this.groupToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.groupToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.group;
            this.groupToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.groupToolStripButton.Name = "groupToolStripButton";
            this.groupToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.groupToolStripButton.Text = "Group";
            this.groupToolStripButton.Click += new System.EventHandler(this.groupToolStripButton_Click);
            // 
            // ungroupToolStripButton
            // 
            this.ungroupToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ungroupToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.ungroup;
            this.ungroupToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ungroupToolStripButton.Name = "ungroupToolStripButton";
            this.ungroupToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.ungroupToolStripButton.Text = "Ungroup";
            this.ungroupToolStripButton.Click += new System.EventHandler(this.ungroupToolStripButton_Click);
            // 
            // toolStripSeparator29
            // 
            this.toolStripSeparator29.Name = "toolStripSeparator29";
            this.toolStripSeparator29.Size = new System.Drawing.Size(6, 25);
            // 
            // sendToBackToolStripButton
            // 
            this.sendToBackToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sendToBackToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.back;
            this.sendToBackToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sendToBackToolStripButton.Name = "sendToBackToolStripButton";
            this.sendToBackToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.sendToBackToolStripButton.Text = "Send To Back";
            this.sendToBackToolStripButton.Click += new System.EventHandler(this.sendToBackToolStripButton_Click);
            // 
            // bringToFrontToolStripButton
            // 
            this.bringToFrontToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bringToFrontToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.front;
            this.bringToFrontToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bringToFrontToolStripButton.Name = "bringToFrontToolStripButton";
            this.bringToFrontToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.bringToFrontToolStripButton.Text = "Bring To Front";
            this.bringToFrontToolStripButton.Click += new System.EventHandler(this.bringToFrontToolStripButton_Click);
            // 
            // shiftBackwardsToolStripButton
            // 
            this.shiftBackwardsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.shiftBackwardsToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.backwards;
            this.shiftBackwardsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.shiftBackwardsToolStripButton.Name = "shiftBackwardsToolStripButton";
            this.shiftBackwardsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.shiftBackwardsToolStripButton.Text = "Shift Backwards";
            this.shiftBackwardsToolStripButton.Click += new System.EventHandler(this.shiftBackwardsToolStripButton_Click);
            // 
            // shiftForewardsToolStripButton
            // 
            this.shiftForewardsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.shiftForewardsToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.forwards;
            this.shiftForewardsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.shiftForewardsToolStripButton.Name = "shiftForewardsToolStripButton";
            this.shiftForewardsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.shiftForewardsToolStripButton.Text = "Shift Forewards";
            this.shiftForewardsToolStripButton.Click += new System.EventHandler(this.shiftForewardsToolStripButton_Click);
            // 
            // cameraToolStrip
            // 
            this.cameraToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.cameraToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToolStripComboBox,
            this.zoomInToolStripButton,
            this.zoomOutToolStripButton,
            this.toolStripSeparator4,
            this.resetCameraToolStripButton});
            this.cameraToolStrip.Location = new System.Drawing.Point(396, 24);
            this.cameraToolStrip.Name = "cameraToolStrip";
            this.cameraToolStrip.Size = new System.Drawing.Size(195, 25);
            this.cameraToolStrip.TabIndex = 6;
            // 
            // zoomToolStripComboBox
            // 
            this.zoomToolStripComboBox.AutoCompleteCustomSource.AddRange(new string[] {
            "25%",
            "50%",
            "100%",
            "200%",
            "400%",
            "600%",
            "1000%"});
            this.zoomToolStripComboBox.Items.AddRange(new object[] {
            "25%",
            "50%",
            "100%",
            "200%",
            "400%",
            "800%",
            "1000%"});
            this.zoomToolStripComboBox.Name = "zoomToolStripComboBox";
            this.zoomToolStripComboBox.Size = new System.Drawing.Size(75, 25);
            this.zoomToolStripComboBox.Text = "100%";
            this.zoomToolStripComboBox.TextChanged += new System.EventHandler(this.zoomToolStripComboBox_TextChanged);
            // 
            // zoomInToolStripButton
            // 
            this.zoomInToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.magnifier_zoom_in;
            this.zoomInToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInToolStripButton.Name = "zoomInToolStripButton";
            this.zoomInToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.zoomInToolStripButton.Text = "toolStripButton9";
            this.zoomInToolStripButton.ToolTipText = "Zoom In";
            this.zoomInToolStripButton.Click += new System.EventHandler(this.zoomInToolStripButton_Click);
            // 
            // zoomOutToolStripButton
            // 
            this.zoomOutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.magifier_zoom_out;
            this.zoomOutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutToolStripButton.Name = "zoomOutToolStripButton";
            this.zoomOutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.zoomOutToolStripButton.Text = "toolStripButton10";
            this.zoomOutToolStripButton.ToolTipText = "Zoom Out";
            this.zoomOutToolStripButton.Click += new System.EventHandler(this.zoomOutToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // resetCameraToolStripButton
            // 
            this.resetCameraToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resetCameraToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.camera_go;
            this.resetCameraToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resetCameraToolStripButton.Name = "resetCameraToolStripButton";
            this.resetCameraToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.resetCameraToolStripButton.Text = "toolStripButton11";
            this.resetCameraToolStripButton.ToolTipText = "Reset Camera";
            this.resetCameraToolStripButton.Click += new System.EventHandler(this.resetCameraToolStripButton_Click);
            // 
            // mapCanvasContextMenuStrip
            // 
            this.mapCanvasContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoContextToolStripMenuItem,
            this.redoContextToolStripMenuItem,
            this.toolStripSeparator39,
            this.cutContextToolStripButton,
            this.copyContextToolStripButton,
            this.pasteContextToolStripButton,
            this.toolStripSeparator32,
            this.deleteContextToolStripButton,
            this.duplicateContextToolStripButton,
            this.toolStripSeparator33,
            this.toolStripMenuItem6,
            this.toolStripSeparator42,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripSeparator38,
            this.groupContextToolStripMenuItem,
            this.unGroupContextToolStripMenuItem,
            this.toolStripSeparator43,
            this.selectAllContextToolStripMenuItem,
            this.deselectContextToolStripMenuItem,
            this.toolStripSeparator40,
            this.resetCameraContextToolStripMenuItem,
            this.toolStripSeparator17,
            this.propertyContextToolStripButton});
            this.mapCanvasContextMenuStrip.Name = "mapcanvasContextMenuStrip";
            this.mapCanvasContextMenuStrip.Size = new System.Drawing.Size(147, 448);
            // 
            // undoContextToolStripMenuItem
            // 
            this.undoContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.undo;
            this.undoContextToolStripMenuItem.Name = "undoContextToolStripMenuItem";
            this.undoContextToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.undoContextToolStripMenuItem.Text = "Undo";
            this.undoContextToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripButton_Click);
            // 
            // redoContextToolStripMenuItem
            // 
            this.redoContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.redo;
            this.redoContextToolStripMenuItem.Name = "redoContextToolStripMenuItem";
            this.redoContextToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.redoContextToolStripMenuItem.Text = "Redo";
            this.redoContextToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripButton_Click);
            // 
            // toolStripSeparator39
            // 
            this.toolStripSeparator39.Name = "toolStripSeparator39";
            this.toolStripSeparator39.Size = new System.Drawing.Size(143, 6);
            // 
            // cutContextToolStripButton
            // 
            this.cutContextToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.cut;
            this.cutContextToolStripButton.Name = "cutContextToolStripButton";
            this.cutContextToolStripButton.Size = new System.Drawing.Size(146, 22);
            this.cutContextToolStripButton.Text = "Cut";
            this.cutContextToolStripButton.Click += new System.EventHandler(this.cutContextToolStripButton_Click);
            // 
            // copyContextToolStripButton
            // 
            this.copyContextToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.copy;
            this.copyContextToolStripButton.Name = "copyContextToolStripButton";
            this.copyContextToolStripButton.Size = new System.Drawing.Size(146, 22);
            this.copyContextToolStripButton.Text = "Copy";
            this.copyContextToolStripButton.Click += new System.EventHandler(this.copyContextToolStripButton_Click);
            // 
            // pasteContextToolStripButton
            // 
            this.pasteContextToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.paste;
            this.pasteContextToolStripButton.Name = "pasteContextToolStripButton";
            this.pasteContextToolStripButton.Size = new System.Drawing.Size(146, 22);
            this.pasteContextToolStripButton.Text = "Paste";
            this.pasteContextToolStripButton.Click += new System.EventHandler(this.pasteContextToolStripButton_Click);
            // 
            // toolStripSeparator32
            // 
            this.toolStripSeparator32.Name = "toolStripSeparator32";
            this.toolStripSeparator32.Size = new System.Drawing.Size(143, 6);
            // 
            // deleteContextToolStripButton
            // 
            this.deleteContextToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.delete;
            this.deleteContextToolStripButton.Name = "deleteContextToolStripButton";
            this.deleteContextToolStripButton.Size = new System.Drawing.Size(146, 22);
            this.deleteContextToolStripButton.Text = "Delete";
            this.deleteContextToolStripButton.Click += new System.EventHandler(this.deleteContextToolStripButton_Click);
            // 
            // duplicateContextToolStripButton
            // 
            this.duplicateContextToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.duplicate;
            this.duplicateContextToolStripButton.Name = "duplicateContextToolStripButton";
            this.duplicateContextToolStripButton.Size = new System.Drawing.Size(146, 22);
            this.duplicateContextToolStripButton.Text = "Duplicate";
            this.duplicateContextToolStripButton.Click += new System.EventHandler(this.duplicateContextToolStripButton_Click);
            // 
            // toolStripSeparator33
            // 
            this.toolStripSeparator33.Name = "toolStripSeparator33";
            this.toolStripSeparator33.Size = new System.Drawing.Size(143, 6);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertContextToolStripButton,
            this.toolStripSeparator41,
            this.insertEntityContextToolStripButton,
            this.insertEmitterContextToolStripMenuItem,
            this.insertTilemapContextToolStripMenuItem,
            this.insertPathMarkerContextToolStripMenuItem});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItem6.Text = "Insert";
            // 
            // insertContextToolStripButton
            // 
            this.insertContextToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertContextToolStripButton.Name = "insertContextToolStripButton";
            this.insertContextToolStripButton.Size = new System.Drawing.Size(170, 22);
            this.insertContextToolStripButton.Text = "Insert Object";
            this.insertContextToolStripButton.Click += new System.EventHandler(this.insertObjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator41
            // 
            this.toolStripSeparator41.Name = "toolStripSeparator41";
            this.toolStripSeparator41.Size = new System.Drawing.Size(167, 6);
            // 
            // insertEntityContextToolStripButton
            // 
            this.insertEntityContextToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertEntityContextToolStripButton.Name = "insertEntityContextToolStripButton";
            this.insertEntityContextToolStripButton.Size = new System.Drawing.Size(170, 22);
            this.insertEntityContextToolStripButton.Text = "Insert Entity";
            this.insertEntityContextToolStripButton.Click += new System.EventHandler(this.insertEntityContextToolStripButton_Click);
            // 
            // insertEmitterContextToolStripMenuItem
            // 
            this.insertEmitterContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertEmitterContextToolStripMenuItem.Name = "insertEmitterContextToolStripMenuItem";
            this.insertEmitterContextToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.insertEmitterContextToolStripMenuItem.Text = "Insert Emitter";
            this.insertEmitterContextToolStripMenuItem.Click += new System.EventHandler(this.insertEmitterToolStripMenuItem1_Click);
            // 
            // insertTilemapContextToolStripMenuItem
            // 
            this.insertTilemapContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertTilemapContextToolStripMenuItem.Name = "insertTilemapContextToolStripMenuItem";
            this.insertTilemapContextToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.insertTilemapContextToolStripMenuItem.Text = "Insert Tilemap";
            this.insertTilemapContextToolStripMenuItem.Click += new System.EventHandler(this.insertTilemapToolStripMenuItem1_Click);
            // 
            // insertPathMarkerContextToolStripMenuItem
            // 
            this.insertPathMarkerContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.entity_insert;
            this.insertPathMarkerContextToolStripMenuItem.Name = "insertPathMarkerContextToolStripMenuItem";
            this.insertPathMarkerContextToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.insertPathMarkerContextToolStripMenuItem.Text = "Insert Path Marker";
            this.insertPathMarkerContextToolStripMenuItem.Click += new System.EventHandler(this.insertPathMarkerToolStripMenuItem_Click);
            // 
            // toolStripSeparator42
            // 
            this.toolStripSeparator42.Name = "toolStripSeparator42";
            this.toolStripSeparator42.Size = new System.Drawing.Size(143, 6);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rotate90ClockwiseContextToolStripMenuItem,
            this.rotate90AntiClockwiseContextToolStripMenuItem});
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItem7.Text = "Rotate";
            // 
            // rotate90ClockwiseContextToolStripMenuItem
            // 
            this.rotate90ClockwiseContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.clockwise;
            this.rotate90ClockwiseContextToolStripMenuItem.Name = "rotate90ClockwiseContextToolStripMenuItem";
            this.rotate90ClockwiseContextToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.rotate90ClockwiseContextToolStripMenuItem.Text = "90º clockwise";
            this.rotate90ClockwiseContextToolStripMenuItem.Click += new System.EventHandler(this.rotateClockwiseToolStripButton_Click);
            // 
            // rotate90AntiClockwiseContextToolStripMenuItem
            // 
            this.rotate90AntiClockwiseContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.anticlockwise;
            this.rotate90AntiClockwiseContextToolStripMenuItem.Name = "rotate90AntiClockwiseContextToolStripMenuItem";
            this.rotate90AntiClockwiseContextToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.rotate90AntiClockwiseContextToolStripMenuItem.Text = "90º anti-clockwise";
            this.rotate90AntiClockwiseContextToolStripMenuItem.Click += new System.EventHandler(this.rotateAnticlockwiseToolStripButton_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.topContextToolStripMenuItem,
            this.bottomContextToolStripMenuItem,
            this.leftContextToolStripMenuItem,
            this.rightContextToolStripMenuItem,
            this.middleContextToolStripMenuItem,
            this.centerContextToolStripMenuItem});
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItem8.Text = "Align";
            // 
            // topContextToolStripMenuItem
            // 
            this.topContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.top;
            this.topContextToolStripMenuItem.Name = "topContextToolStripMenuItem";
            this.topContextToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.topContextToolStripMenuItem.Text = "Top";
            this.topContextToolStripMenuItem.Click += new System.EventHandler(this.alignTopToolStripButton_Click);
            // 
            // bottomContextToolStripMenuItem
            // 
            this.bottomContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.bottom;
            this.bottomContextToolStripMenuItem.Name = "bottomContextToolStripMenuItem";
            this.bottomContextToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.bottomContextToolStripMenuItem.Text = "Bottom";
            this.bottomContextToolStripMenuItem.Click += new System.EventHandler(this.alignBottomToolStripButton_Click);
            // 
            // leftContextToolStripMenuItem
            // 
            this.leftContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.left;
            this.leftContextToolStripMenuItem.Name = "leftContextToolStripMenuItem";
            this.leftContextToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.leftContextToolStripMenuItem.Text = "Left";
            this.leftContextToolStripMenuItem.Click += new System.EventHandler(this.alignLeftToolStripButton_Click);
            // 
            // rightContextToolStripMenuItem
            // 
            this.rightContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.right;
            this.rightContextToolStripMenuItem.Name = "rightContextToolStripMenuItem";
            this.rightContextToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.rightContextToolStripMenuItem.Text = "Right";
            this.rightContextToolStripMenuItem.Click += new System.EventHandler(this.alignRightToolStripButton_Click);
            // 
            // middleContextToolStripMenuItem
            // 
            this.middleContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.middle;
            this.middleContextToolStripMenuItem.Name = "middleContextToolStripMenuItem";
            this.middleContextToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.middleContextToolStripMenuItem.Text = "Middle";
            this.middleContextToolStripMenuItem.Click += new System.EventHandler(this.alignMiddleToolStripButton_Click);
            // 
            // centerContextToolStripMenuItem
            // 
            this.centerContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.center;
            this.centerContextToolStripMenuItem.Name = "centerContextToolStripMenuItem";
            this.centerContextToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.centerContextToolStripMenuItem.Text = "Center";
            this.centerContextToolStripMenuItem.Click += new System.EventHandler(this.alignCenterToolStripButton_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.horizontalContextToolStripMenuItem,
            this.verticalContextToolStripMenuItem});
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItem9.Text = "Mirror";
            // 
            // horizontalContextToolStripMenuItem
            // 
            this.horizontalContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.horizontal;
            this.horizontalContextToolStripMenuItem.Name = "horizontalContextToolStripMenuItem";
            this.horizontalContextToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.horizontalContextToolStripMenuItem.Text = "Horizontal";
            this.horizontalContextToolStripMenuItem.Click += new System.EventHandler(this.flipHorizontalToolStripButton_Click);
            // 
            // verticalContextToolStripMenuItem
            // 
            this.verticalContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.vertical;
            this.verticalContextToolStripMenuItem.Name = "verticalContextToolStripMenuItem";
            this.verticalContextToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.verticalContextToolStripMenuItem.Text = "Vertical";
            this.verticalContextToolStripMenuItem.Click += new System.EventHandler(this.flipVerticalToolStripButton_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shiftBackwardsContextToolStripMenuItem,
            this.shiftForewardsContextToolStripMenuItem,
            this.toolStripSeparator44,
            this.sendToBackContextToolStripMenuItem,
            this.bringToFrontContextToolStripMenuItem});
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItem10.Text = "Order";
            // 
            // shiftBackwardsContextToolStripMenuItem
            // 
            this.shiftBackwardsContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.backwards;
            this.shiftBackwardsContextToolStripMenuItem.Name = "shiftBackwardsContextToolStripMenuItem";
            this.shiftBackwardsContextToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.shiftBackwardsContextToolStripMenuItem.Text = "Shift Backwards";
            this.shiftBackwardsContextToolStripMenuItem.Click += new System.EventHandler(this.shiftBackwardsToolStripButton_Click);
            // 
            // shiftForewardsContextToolStripMenuItem
            // 
            this.shiftForewardsContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.forwards;
            this.shiftForewardsContextToolStripMenuItem.Name = "shiftForewardsContextToolStripMenuItem";
            this.shiftForewardsContextToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.shiftForewardsContextToolStripMenuItem.Text = "Shift Forewards";
            this.shiftForewardsContextToolStripMenuItem.Click += new System.EventHandler(this.shiftForewardsToolStripButton_Click);
            // 
            // toolStripSeparator44
            // 
            this.toolStripSeparator44.Name = "toolStripSeparator44";
            this.toolStripSeparator44.Size = new System.Drawing.Size(154, 6);
            // 
            // sendToBackContextToolStripMenuItem
            // 
            this.sendToBackContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.back;
            this.sendToBackContextToolStripMenuItem.Name = "sendToBackContextToolStripMenuItem";
            this.sendToBackContextToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.sendToBackContextToolStripMenuItem.Text = "Send to Back";
            this.sendToBackContextToolStripMenuItem.Click += new System.EventHandler(this.sendToBackToolStripButton_Click);
            // 
            // bringToFrontContextToolStripMenuItem
            // 
            this.bringToFrontContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.front;
            this.bringToFrontContextToolStripMenuItem.Name = "bringToFrontContextToolStripMenuItem";
            this.bringToFrontContextToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.bringToFrontContextToolStripMenuItem.Text = "Bring to Front";
            this.bringToFrontContextToolStripMenuItem.Click += new System.EventHandler(this.bringToFrontToolStripButton_Click);
            // 
            // toolStripSeparator38
            // 
            this.toolStripSeparator38.Name = "toolStripSeparator38";
            this.toolStripSeparator38.Size = new System.Drawing.Size(143, 6);
            // 
            // groupContextToolStripMenuItem
            // 
            this.groupContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.group;
            this.groupContextToolStripMenuItem.Name = "groupContextToolStripMenuItem";
            this.groupContextToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.groupContextToolStripMenuItem.Text = "Group";
            this.groupContextToolStripMenuItem.Click += new System.EventHandler(this.groupToolStripButton_Click);
            // 
            // unGroupContextToolStripMenuItem
            // 
            this.unGroupContextToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.ungroup;
            this.unGroupContextToolStripMenuItem.Name = "unGroupContextToolStripMenuItem";
            this.unGroupContextToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.unGroupContextToolStripMenuItem.Text = "Ungroup";
            this.unGroupContextToolStripMenuItem.Click += new System.EventHandler(this.ungroupToolStripButton_Click);
            // 
            // toolStripSeparator43
            // 
            this.toolStripSeparator43.Name = "toolStripSeparator43";
            this.toolStripSeparator43.Size = new System.Drawing.Size(143, 6);
            // 
            // selectAllContextToolStripMenuItem
            // 
            this.selectAllContextToolStripMenuItem.Name = "selectAllContextToolStripMenuItem";
            this.selectAllContextToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.selectAllContextToolStripMenuItem.Text = "Select All";
            this.selectAllContextToolStripMenuItem.Click += new System.EventHandler(this.selectAllMenuItem_Click);
            // 
            // deselectContextToolStripMenuItem
            // 
            this.deselectContextToolStripMenuItem.Name = "deselectContextToolStripMenuItem";
            this.deselectContextToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.deselectContextToolStripMenuItem.Text = "Deselect";
            this.deselectContextToolStripMenuItem.Click += new System.EventHandler(this.deselectMenuItem_Click);
            // 
            // toolStripSeparator40
            // 
            this.toolStripSeparator40.Name = "toolStripSeparator40";
            this.toolStripSeparator40.Size = new System.Drawing.Size(143, 6);
            // 
            // resetCameraContextToolStripMenuItem
            // 
            this.resetCameraContextToolStripMenuItem.Name = "resetCameraContextToolStripMenuItem";
            this.resetCameraContextToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.resetCameraContextToolStripMenuItem.Text = "Reset Camera";
            this.resetCameraContextToolStripMenuItem.Click += new System.EventHandler(this.resetCameraMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(143, 6);
            // 
            // propertyContextToolStripButton
            // 
            this.propertyContextToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.map_properties;
            this.propertyContextToolStripButton.Name = "propertyContextToolStripButton";
            this.propertyContextToolStripButton.Size = new System.Drawing.Size(146, 22);
            this.propertyContextToolStripButton.Text = "Properties";
            this.propertyContextToolStripButton.Click += new System.EventHandler(this.propertyContextToolStripButton_Click);
            // 
            // EditorWindow
            // 
            this.ClientSize = new System.Drawing.Size(895, 597);
            this.Controls.Add(this.toolStripContainer);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "EditorWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fusion Editor ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorWindow_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.toolsToolStrip.ResumeLayout(false);
            this.toolsToolStrip.PerformLayout();
            this.commandToolStrip.ResumeLayout(false);
            this.commandToolStrip.PerformLayout();
            this.fileToolStrip.ResumeLayout(false);
            this.fileToolStrip.PerformLayout();
            this.formatToolStrip.ResumeLayout(false);
            this.formatToolStrip.PerformLayout();
            this.cameraToolStrip.ResumeLayout(false);
            this.cameraToolStrip.PerformLayout();
            this.mapCanvasContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem buildProjectMenuItem;
		private System.Windows.Forms.ToolStripMenuItem preferencesMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpContentsMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem visitWebsiteMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkForUpdatesMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem openMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recentFilesMenuItem;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
		private System.Windows.Forms.ToolStripButton toolStripButton5;
		private System.Windows.Forms.ToolStripButton toolStripButton6;
		private System.Windows.Forms.ToolStripButton toolStripButton7;
		private System.Windows.Forms.ToolStripButton toolStripButton8;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripMenuItem editMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewMenuItem;
		private System.Windows.Forms.ToolStrip fileToolStrip;
		private System.Windows.Forms.ToolStripButton openToolStripButton;
		private System.Windows.Forms.ToolStripButton saveToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripContainer toolStripContainer;
		private System.Windows.Forms.ToolStrip commandToolStrip;
		private System.Windows.Forms.ToolStripButton newToolStripButton;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripTextBox commandToolStripTextBox;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
		private System.Windows.Forms.ToolStripButton snapToGridToolStripButton;
		private System.Windows.Forms.ToolStripLabel toolStripLabel2;
		private System.Windows.Forms.ToolStripTextBox gridWidthToolStripTextBox;
		private System.Windows.Forms.ToolStripTextBox gridHeightToolStripTextBox;
		private System.Windows.Forms.ToolStripButton playMapToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem playMapMenuItem;
		private System.Windows.Forms.ToolStrip formatToolStrip;
		private System.Windows.Forms.ToolStripButton flipHorizontalToolStripButton;
		private System.Windows.Forms.ToolStripButton flipVerticalToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private System.Windows.Forms.ToolStripButton rotateClockwiseToolStripButton;
		private System.Windows.Forms.ToolStripButton rotateAnticlockwiseToolStripButton;
		private System.Windows.Forms.ToolStripStatusLabel fpsStatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel renderTimeStatusLabel;
		private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
		private System.Windows.Forms.ToolStripMenuItem cutMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
		private System.Windows.Forms.ToolStripMenuItem selectAllMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deselectMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sceneGraphViewerMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cameraMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pencilMenuItem;
		private System.Windows.Forms.ToolStripMenuItem lineMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paintBucketMenuItem;
		private System.Windows.Forms.ToolStripMenuItem assetManagerMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectorMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
		private System.Windows.Forms.ToolStripMenuItem eraserMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tilePickerMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
		private System.Windows.Forms.ToolStripMenuItem rectangleMenuItem;
		private System.Windows.Forms.ToolStripMenuItem roundedRectangleMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ellipseMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripMenuItem mapPropertiesMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
		private System.Windows.Forms.ToolStripMenuItem consoleMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tipOfTheDayMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
		private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
		private System.Windows.Forms.ToolStripMenuItem duplicateMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
		private System.Windows.Forms.ToolStripButton cutToolStripButton;
		private System.Windows.Forms.ToolStripButton copyToolStripButton;
		private System.Windows.Forms.ToolStripButton pasteToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripButton undoToolStripButton;
		private System.Windows.Forms.ToolStripButton redoToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
		private System.Windows.Forms.ToolStripButton alignBottomToolStripButton;
		private System.Windows.Forms.ToolStripButton alignTopToolStripButton;
		private System.Windows.Forms.ToolStripButton alignLeftToolStripButton;
		private System.Windows.Forms.ToolStripButton alignRightToolStripButton;
		private System.Windows.Forms.ToolStripButton alignMiddleToolStripButton;
		private System.Windows.Forms.ToolStripButton alignCenterToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
		private System.Windows.Forms.ToolStripButton groupToolStripButton;
		private System.Windows.Forms.ToolStripButton ungroupToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator29;
		private System.Windows.Forms.ToolStripButton sendToBackToolStripButton;
		private System.Windows.Forms.ToolStripButton bringToFrontToolStripButton;
		private System.Windows.Forms.ToolStripButton shiftBackwardsToolStripButton;
		private System.Windows.Forms.ToolStripButton shiftForewardsToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
		private System.Windows.Forms.ToolStripButton buildProjectToolStripButton;
		private System.Windows.Forms.ToolStripButton mapPropertiesToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator30;
		private System.Windows.Forms.ToolStripButton assetManagerToolStripButton;
		private System.Windows.Forms.ToolStripButton sceneGraphToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator31;
		private System.Windows.Forms.ToolStripButton helpToolStripButton;
		private System.Windows.Forms.ToolStripButton aboutToolStripButton;
		private System.Windows.Forms.ToolStripButton viewConsoleToolStripButton;
		private System.Windows.Forms.ContextMenuStrip mapCanvasContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem cutContextToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem copyContextToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem pasteContextToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator32;
		private System.Windows.Forms.ToolStripMenuItem deleteContextToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem duplicateContextToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator33;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
		private System.Windows.Forms.ToolStripStatusLabel zoomStatusLabel;
		private System.Windows.Forms.ToolStripButton toolStripButtonYAxis;
		private System.Windows.Forms.ToolStripButton toolStripButtonXAxis;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripMenuItem propertyMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
		private System.Windows.Forms.ToolStripMenuItem propertyContextToolStripButton;
		private System.Windows.Forms.ToolStripStatusLabel cursorPositionStatusLabel;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator37;
		private System.Windows.Forms.ToolStripMenuItem tileFlippingMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flipTileXMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipTileYMenuItem;
		private System.Windows.Forms.ToolStrip toolsToolStrip;
		private System.Windows.Forms.ToolStripButton cameraToolStripButton;
		private System.Windows.Forms.ToolStripButton selectorToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
		private System.Windows.Forms.ToolStripButton pencilToolStripButton;
		private System.Windows.Forms.ToolStripButton eraserToolStripButton;
		private System.Windows.Forms.ToolStripButton paintBucketToolStripButton;
		private System.Windows.Forms.ToolStripButton tilePickerToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
		private System.Windows.Forms.ToolStripButton rectangleToolStripButton;
		private System.Windows.Forms.ToolStripButton roundedRectangleToolStripButton;
		private System.Windows.Forms.ToolStripButton ellipseToolStripButton;
		private System.Windows.Forms.ToolStripButton lineToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator36;
		private System.Windows.Forms.ToolStripButton tileFlipXToolStripButton;
		private System.Windows.Forms.ToolStripButton tileFlipYToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator35;
        private System.Windows.Forms.ToolStripMenuItem resetCameraMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator34;
        private System.Windows.Forms.ToolStripMenuItem viewBoundingboxsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewCollisionboxsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewEventLinesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewOriginMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator39;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator38;
        private System.Windows.Forms.ToolStripMenuItem resetCameraContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deselectContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator40;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem insertContextToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator41;
        private System.Windows.Forms.ToolStripMenuItem insertEntityContextToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem insertEmitterContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertTilemapContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator42;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem rotate90ClockwiseContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate90AntiClockwiseContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator43;
        private System.Windows.Forms.ToolStripMenuItem unGroupContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem middleContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem centerContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem horizontalContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verticalContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shiftBackwardsContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shiftForewardsContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator44;
        private System.Windows.Forms.ToolStripMenuItem sendToBackContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bringToFrontContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem insertObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator45;
        private System.Windows.Forms.ToolStripMenuItem insertEntityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertTilemapToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem insertEmitterToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator46;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem rotateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate90ClockwiseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate90AntiClockwiseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alignMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightMenuItem;
        private System.Windows.Forms.ToolStripMenuItem centerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem middleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolbarsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commandToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mirrorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem horizontalMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verticalMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendToBackMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bringToFrontMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem shiftBackwardsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shiftForewardsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator47;
        private System.Windows.Forms.ToolStripMenuItem groupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ungroupMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.Panel mapPanel;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStrip cameraToolStrip;
        private System.Windows.Forms.ToolStripComboBox zoomToolStripComboBox;
        private System.Windows.Forms.ToolStripButton zoomInToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomOutToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem cameraToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton resetCameraToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem renderMapMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator48;
        private System.Windows.Forms.ToolStripMenuItem setBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertPathMarkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertPathMarkerContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearBackgroundToolStripMenuItem;
	}
}