namespace ASCOM.Sony
{
    partial class SetupDialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.picASCOM = new System.Windows.Forms.PictureBox();
            this.chkTrace = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbCameraModel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSelectedIso = new System.Windows.Forms.ComboBox();
            this.checkAutoDeleteFile = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.cbImageFormat = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pnlCustomCamera = new System.Windows.Forms.Panel();
            this.lbShutterSpeed = new System.Windows.Forms.ListBox();
            this.label22 = new System.Windows.Forms.Label();
            this.lbISO = new System.Windows.Forms.ListBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.txtExposureMax = new System.Windows.Forms.TextBox();
            this.txtExposureMin = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtSensorName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtSensorSizeHeight = new System.Windows.Forms.TextBox();
            this.txtSensorSizeWidth = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtPixelSizeHeight = new System.Windows.Forms.TextBox();
            this.txtPixelSizeWidth = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCropSizeHeight = new System.Windows.Forms.TextBox();
            this.txtCropSizeWidth = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txtFrameSizeHeight = new System.Windows.Forms.TextBox();
            this.txtFrameSizeWidth = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtCameraModel = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.AppPathTextBox = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.cbBulbMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.pnlCustomCamera.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(519, 319);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(58, 24);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(519, 349);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(58, 24);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // picASCOM
            // 
            this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.Sony.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(12, 318);
            this.picASCOM.Name = "picASCOM";
            this.picASCOM.Size = new System.Drawing.Size(48, 56);
            this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picASCOM.TabIndex = 3;
            this.picASCOM.TabStop = false;
            this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
            this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
            // 
            // chkTrace
            // 
            this.chkTrace.AutoSize = true;
            this.chkTrace.Location = new System.Drawing.Point(15, 200);
            this.chkTrace.Name = "chkTrace";
            this.chkTrace.Size = new System.Drawing.Size(69, 17);
            this.chkTrace.TabIndex = 6;
            this.chkTrace.Text = "Trace on";
            this.toolTip1.SetToolTip(this.chkTrace, "Only check this if you need to diagnose problems with driver, as it affects perfo" +
        "rmance");
            this.chkTrace.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(89, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Camera Model";
            // 
            // cbCameraModel
            // 
            this.cbCameraModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCameraModel.FormattingEnabled = true;
            this.cbCameraModel.Location = new System.Drawing.Point(168, 34);
            this.cbCameraModel.Margin = new System.Windows.Forms.Padding(2);
            this.cbCameraModel.Name = "cbCameraModel";
            this.cbCameraModel.Size = new System.Drawing.Size(124, 21);
            this.cbCameraModel.TabIndex = 10;
            this.cbCameraModel.SelectedValueChanged += new System.EventHandler(this.cbCameraModel_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(139, 63);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "ISO";
            // 
            // cbSelectedIso
            // 
            this.cbSelectedIso.FormattingEnabled = true;
            this.cbSelectedIso.Location = new System.Drawing.Point(168, 59);
            this.cbSelectedIso.Margin = new System.Windows.Forms.Padding(2);
            this.cbSelectedIso.Name = "cbSelectedIso";
            this.cbSelectedIso.Size = new System.Drawing.Size(82, 21);
            this.cbSelectedIso.TabIndex = 12;
            // 
            // checkAutoDeleteFile
            // 
            this.checkAutoDeleteFile.AutoSize = true;
            this.checkAutoDeleteFile.Location = new System.Drawing.Point(15, 178);
            this.checkAutoDeleteFile.Margin = new System.Windows.Forms.Padding(2);
            this.checkAutoDeleteFile.Name = "checkAutoDeleteFile";
            this.checkAutoDeleteFile.Size = new System.Drawing.Size(127, 17);
            this.checkAutoDeleteFile.TabIndex = 13;
            this.checkAutoDeleteFile.Text = "Auto-delete image file";
            this.toolTip1.SetToolTip(this.checkAutoDeleteFile, "Check if you want captured image file be deleted automatically after image data r" +
        "eturned by driver (will keep your monitoring folder clear)");
            this.checkAutoDeleteFile.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(93, 87);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Image Format";
            // 
            // cbImageFormat
            // 
            this.cbImageFormat.FormattingEnabled = true;
            this.cbImageFormat.Location = new System.Drawing.Point(168, 83);
            this.cbImageFormat.Margin = new System.Windows.Forms.Padding(2);
            this.cbImageFormat.Name = "cbImageFormat";
            this.cbImageFormat.Size = new System.Drawing.Size(82, 21);
            this.cbImageFormat.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 110);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Note: Image Format selection is not automated.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 125);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(213, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Please ensure you have matched file format";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 140);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(190, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "selected in Imaging Edge Remote app.";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Location = new System.Drawing.Point(0, 306);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(902, 2);
            this.label7.TabIndex = 20;
            // 
            // pnlCustomCamera
            // 
            this.pnlCustomCamera.Controls.Add(this.lbShutterSpeed);
            this.pnlCustomCamera.Controls.Add(this.label22);
            this.pnlCustomCamera.Controls.Add(this.lbISO);
            this.pnlCustomCamera.Controls.Add(this.label21);
            this.pnlCustomCamera.Controls.Add(this.label20);
            this.pnlCustomCamera.Controls.Add(this.txtExposureMax);
            this.pnlCustomCamera.Controls.Add(this.txtExposureMin);
            this.pnlCustomCamera.Controls.Add(this.label19);
            this.pnlCustomCamera.Controls.Add(this.txtSensorName);
            this.pnlCustomCamera.Controls.Add(this.label18);
            this.pnlCustomCamera.Controls.Add(this.label13);
            this.pnlCustomCamera.Controls.Add(this.label16);
            this.pnlCustomCamera.Controls.Add(this.txtSensorSizeHeight);
            this.pnlCustomCamera.Controls.Add(this.txtSensorSizeWidth);
            this.pnlCustomCamera.Controls.Add(this.label17);
            this.pnlCustomCamera.Controls.Add(this.label14);
            this.pnlCustomCamera.Controls.Add(this.txtPixelSizeHeight);
            this.pnlCustomCamera.Controls.Add(this.txtPixelSizeWidth);
            this.pnlCustomCamera.Controls.Add(this.label15);
            this.pnlCustomCamera.Controls.Add(this.label12);
            this.pnlCustomCamera.Controls.Add(this.label9);
            this.pnlCustomCamera.Controls.Add(this.label10);
            this.pnlCustomCamera.Controls.Add(this.txtCropSizeHeight);
            this.pnlCustomCamera.Controls.Add(this.txtCropSizeWidth);
            this.pnlCustomCamera.Controls.Add(this.label11);
            this.pnlCustomCamera.Controls.Add(this.label8);
            this.pnlCustomCamera.Controls.Add(this.label23);
            this.pnlCustomCamera.Controls.Add(this.txtFrameSizeHeight);
            this.pnlCustomCamera.Controls.Add(this.txtFrameSizeWidth);
            this.pnlCustomCamera.Controls.Add(this.label24);
            this.pnlCustomCamera.Controls.Add(this.txtCameraModel);
            this.pnlCustomCamera.Controls.Add(this.label25);
            this.pnlCustomCamera.Location = new System.Drawing.Point(297, 2);
            this.pnlCustomCamera.Margin = new System.Windows.Forms.Padding(2);
            this.pnlCustomCamera.Name = "pnlCustomCamera";
            this.pnlCustomCamera.Size = new System.Drawing.Size(292, 303);
            this.pnlCustomCamera.TabIndex = 23;
            // 
            // lbShutterSpeed
            // 
            this.lbShutterSpeed.FormattingEnabled = true;
            this.lbShutterSpeed.Location = new System.Drawing.Point(115, 238);
            this.lbShutterSpeed.Margin = new System.Windows.Forms.Padding(2);
            this.lbShutterSpeed.Name = "lbShutterSpeed";
            this.lbShutterSpeed.Size = new System.Drawing.Size(164, 56);
            this.lbShutterSpeed.TabIndex = 109;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(28, 238);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(82, 13);
            this.label22.TabIndex = 108;
            this.label22.Text = "Shutter Speed *";
            // 
            // lbISO
            // 
            this.lbISO.FormattingEnabled = true;
            this.lbISO.Location = new System.Drawing.Point(115, 173);
            this.lbISO.Margin = new System.Windows.Forms.Padding(2);
            this.lbISO.Name = "lbISO";
            this.lbISO.Size = new System.Drawing.Size(164, 56);
            this.lbISO.TabIndex = 107;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(80, 171);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(32, 13);
            this.label21.TabIndex = 106;
            this.label21.Text = "ISO *";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(218, 148);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(47, 13);
            this.label20.TabIndex = 105;
            this.label20.Text = "seconds";
            // 
            // txtExposureMax
            // 
            this.txtExposureMax.Location = new System.Drawing.Point(176, 146);
            this.txtExposureMax.Margin = new System.Windows.Forms.Padding(2);
            this.txtExposureMax.Name = "txtExposureMax";
            this.txtExposureMax.Size = new System.Drawing.Size(43, 20);
            this.txtExposureMax.TabIndex = 104;
            // 
            // txtExposureMin
            // 
            this.txtExposureMin.Location = new System.Drawing.Point(115, 146);
            this.txtExposureMin.Margin = new System.Windows.Forms.Padding(2);
            this.txtExposureMin.Name = "txtExposureMin";
            this.txtExposureMin.Size = new System.Drawing.Size(43, 20);
            this.txtExposureMin.TabIndex = 103;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(7, 148);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(109, 13);
            this.label19.TabIndex = 102;
            this.label19.Text = "Exposure Min / Max *";
            // 
            // txtSensorName
            // 
            this.txtSensorName.Location = new System.Drawing.Point(115, 32);
            this.txtSensorName.Margin = new System.Windows.Forms.Padding(2);
            this.txtSensorName.Name = "txtSensorName";
            this.txtSensorName.Size = new System.Drawing.Size(164, 20);
            this.txtSensorName.TabIndex = 101;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(33, 34);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(74, 13);
            this.label18.TabIndex = 100;
            this.label18.Text = "Sensor Name ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(221, 57);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(23, 13);
            this.label13.TabIndex = 99;
            this.label13.Text = "mm";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(160, 148);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(12, 13);
            this.label16.TabIndex = 98;
            this.label16.Text = "/";
            // 
            // txtSensorSizeHeight
            // 
            this.txtSensorSizeHeight.Location = new System.Drawing.Point(176, 54);
            this.txtSensorSizeHeight.Margin = new System.Windows.Forms.Padding(2);
            this.txtSensorSizeHeight.Name = "txtSensorSizeHeight";
            this.txtSensorSizeHeight.Size = new System.Drawing.Size(43, 20);
            this.txtSensorSizeHeight.TabIndex = 97;
            // 
            // txtSensorSizeWidth
            // 
            this.txtSensorSizeWidth.Location = new System.Drawing.Point(115, 54);
            this.txtSensorSizeWidth.Margin = new System.Windows.Forms.Padding(2);
            this.txtSensorSizeWidth.Name = "txtSensorSizeWidth";
            this.txtSensorSizeWidth.Size = new System.Drawing.Size(43, 20);
            this.txtSensorSizeWidth.TabIndex = 96;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(42, 57);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 13);
            this.label17.TabIndex = 95;
            this.label17.Text = "Sensor Size  ";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(160, 125);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(12, 13);
            this.label14.TabIndex = 94;
            this.label14.Text = "x";
            // 
            // txtPixelSizeHeight
            // 
            this.txtPixelSizeHeight.Location = new System.Drawing.Point(176, 124);
            this.txtPixelSizeHeight.Margin = new System.Windows.Forms.Padding(2);
            this.txtPixelSizeHeight.Name = "txtPixelSizeHeight";
            this.txtPixelSizeHeight.Size = new System.Drawing.Size(43, 20);
            this.txtPixelSizeHeight.TabIndex = 93;
            // 
            // txtPixelSizeWidth
            // 
            this.txtPixelSizeWidth.Location = new System.Drawing.Point(115, 124);
            this.txtPixelSizeWidth.Margin = new System.Windows.Forms.Padding(2);
            this.txtPixelSizeWidth.Name = "txtPixelSizeWidth";
            this.txtPixelSizeWidth.Size = new System.Drawing.Size(43, 20);
            this.txtPixelSizeWidth.TabIndex = 92;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(53, 125);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(55, 13);
            this.label15.TabIndex = 91;
            this.label15.Text = "Pixel Size ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(221, 125);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(21, 13);
            this.label12.TabIndex = 90;
            this.label12.Text = "μm";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(221, 102);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 89;
            this.label9.Text = "pixels";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(160, 102);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(12, 13);
            this.label10.TabIndex = 88;
            this.label10.Text = "x";
            // 
            // txtCropSizeHeight
            // 
            this.txtCropSizeHeight.Location = new System.Drawing.Point(176, 101);
            this.txtCropSizeHeight.Margin = new System.Windows.Forms.Padding(2);
            this.txtCropSizeHeight.Name = "txtCropSizeHeight";
            this.txtCropSizeHeight.Size = new System.Drawing.Size(43, 20);
            this.txtCropSizeHeight.TabIndex = 87;
            // 
            // txtCropSizeWidth
            // 
            this.txtCropSizeWidth.Location = new System.Drawing.Point(115, 101);
            this.txtCropSizeWidth.Margin = new System.Windows.Forms.Padding(2);
            this.txtCropSizeWidth.Name = "txtCropSizeWidth";
            this.txtCropSizeWidth.Size = new System.Drawing.Size(43, 20);
            this.txtCropSizeWidth.TabIndex = 86;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(53, 102);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 85;
            this.label11.Text = "Crop Size  ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(221, 80);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 84;
            this.label8.Text = "pixels";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(160, 80);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(12, 13);
            this.label23.TabIndex = 83;
            this.label23.Text = "x";
            // 
            // txtFrameSizeHeight
            // 
            this.txtFrameSizeHeight.Location = new System.Drawing.Point(176, 78);
            this.txtFrameSizeHeight.Margin = new System.Windows.Forms.Padding(2);
            this.txtFrameSizeHeight.Name = "txtFrameSizeHeight";
            this.txtFrameSizeHeight.Size = new System.Drawing.Size(43, 20);
            this.txtFrameSizeHeight.TabIndex = 82;
            // 
            // txtFrameSizeWidth
            // 
            this.txtFrameSizeWidth.Location = new System.Drawing.Point(115, 78);
            this.txtFrameSizeWidth.Margin = new System.Windows.Forms.Padding(2);
            this.txtFrameSizeWidth.Name = "txtFrameSizeWidth";
            this.txtFrameSizeWidth.Size = new System.Drawing.Size(43, 20);
            this.txtFrameSizeWidth.TabIndex = 81;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(44, 80);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(66, 13);
            this.label24.TabIndex = 80;
            this.label24.Text = "Frame Size *";
            // 
            // txtCameraModel
            // 
            this.txtCameraModel.Location = new System.Drawing.Point(115, 9);
            this.txtCameraModel.Margin = new System.Windows.Forms.Padding(2);
            this.txtCameraModel.Name = "txtCameraModel";
            this.txtCameraModel.Size = new System.Drawing.Size(164, 20);
            this.txtCameraModel.TabIndex = 79;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(69, 11);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(43, 13);
            this.label25.TabIndex = 78;
            this.label25.Text = "Model *";
            // 
            // AppPathTextBox
            // 
            this.AppPathTextBox.Location = new System.Drawing.Point(168, 11);
            this.AppPathTextBox.Name = "AppPathTextBox";
            this.AppPathTextBox.Size = new System.Drawing.Size(124, 20);
            this.AppPathTextBox.TabIndex = 24;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(12, 14);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(150, 13);
            this.label26.TabIndex = 25;
            this.label26.Text = "Sony Remote App Folder Path";
            // 
            // cbBulbMode
            // 
            this.cbBulbMode.AutoSize = true;
            this.cbBulbMode.Location = new System.Drawing.Point(15, 156);
            this.cbBulbMode.Name = "cbBulbMode";
            this.cbBulbMode.Size = new System.Drawing.Size(76, 17);
            this.cbBulbMode.TabIndex = 26;
            this.cbBulbMode.Text = "Bulb mode";
            this.cbBulbMode.UseVisualStyleBackColor = true;
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 386);
            this.Controls.Add(this.cbBulbMode);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.AppPathTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbImageFormat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkAutoDeleteFile);
            this.Controls.Add(this.cbSelectedIso);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbCameraModel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkTrace);
            this.Controls.Add(this.picASCOM);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.pnlCustomCamera);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupDialogForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ASCOM Sony Camera Settings";
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
            this.pnlCustomCamera.ResumeLayout(false);
            this.pnlCustomCamera.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.PictureBox picASCOM;
        private System.Windows.Forms.CheckBox chkTrace;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbCameraModel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSelectedIso;
        private System.Windows.Forms.CheckBox checkAutoDeleteFile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbImageFormat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel pnlCustomCamera;
        private System.Windows.Forms.ListBox lbShutterSpeed;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ListBox lbISO;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtExposureMax;
        private System.Windows.Forms.TextBox txtExposureMin;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtSensorName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtSensorSizeHeight;
        private System.Windows.Forms.TextBox txtSensorSizeWidth;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtPixelSizeHeight;
        private System.Windows.Forms.TextBox txtPixelSizeWidth;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtCropSizeHeight;
        private System.Windows.Forms.TextBox txtCropSizeWidth;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtFrameSizeHeight;
        private System.Windows.Forms.TextBox txtFrameSizeWidth;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtCameraModel;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox AppPathTextBox;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.CheckBox cbBulbMode;
    }
}