using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ASCOM.Utilities;
using ASCOM.Sony;

namespace ASCOM.Sony
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        public SetupDialogForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            AppPathTextBox.Text = Camera.SonyAppPath;
            chkTrace.Checked = Camera.TraceEnabled;
            checkAutoDeleteFile.Checked = Camera.AutoDeleteImage;
            cbBulbMode.Checked = Camera.BulbMode;

            cbImageFormat.Items.Clear();
            cbImageFormat.Items.AddRange(new[]
            {   (object)
                ImageFormat.CFA,
                ImageFormat.Debayered,
                ImageFormat.JPG
            });
            cbImageFormat.SelectedItem = Camera.ImageFormat;

            cbCameraModel.Items.Clear();

            cbCameraModel.Items.AddRange(Camera.Settings.CameraModels.Select(cm => cm.ID).ToArray());
            cbCameraModel.SelectedItem = Camera.SelectedCameraId;

            cbSelectedIso.Items.Clear();
            cbSelectedIso.Items.AddRange(ShortToObjectArray(Camera.GetSelectedCameraModel().Gains));
            cbSelectedIso.SelectedItem = Camera.SelectedIso;

            PopulateCameraSettings(Camera.GetSelectedCameraModel());
        }

        private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            // Update Camera class variables with results from the dialogue
            Camera.SelectedCameraId = cbCameraModel.SelectedItem as string;
            short.TryParse(cbSelectedIso.SelectedItem.ToString(), out Camera.SelectedIso);
            Camera.ImageFormat = (ImageFormat?)cbImageFormat.SelectedItem ?? ImageFormat.CFA;
            Camera.AutoDeleteImage = checkAutoDeleteFile.Checked;
            Camera.TraceEnabled = chkTrace.Checked;
            Camera.SonyAppPath = AppPathTextBox.Text;
            Camera.BulbMode = cbBulbMode.Checked;
        }

        private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void cbCameraModel_SelectedValueChanged(object sender, EventArgs e)
        {
            var selectedCameraId = cbCameraModel.SelectedItem as string;
            CameraModel selectedCameraModel = Camera.GetSelectedCameraModel(selectedCameraId);

            cbSelectedIso.Items.Clear();
            if (selectedCameraModel != null)
            {
                cbSelectedIso.Items.AddRange(ShortToObjectArray(selectedCameraModel.Gains));
                if (selectedCameraModel.Gains.Contains(Camera.SelectedIso))
                {
                    cbSelectedIso.SelectedItem = Camera.SelectedIso;
                }
                else
                {
                    cbSelectedIso.SelectedItem = selectedCameraModel.Gains.FirstOrDefault();
                }
            }

            PopulateCameraSettings(selectedCameraModel);
        }

        private object[] ShortToObjectArray(short[] shortArray)
        {
            return shortArray.Select(value => (object)value).ToArray();
        }

        private void PopulateCameraSettings(CameraModel cameraModel)
        {
            if (cameraModel != null)
            {
                txtCameraModel.Text = cameraModel.Name;
                txtSensorName.Text = cameraModel.Sensor.Name;
                txtSensorSizeWidth.Text = cameraModel.Sensor.Width.ToString();
                txtSensorSizeHeight.Text = cameraModel.Sensor.Height.ToString();
                txtFrameSizeWidth.Text = cameraModel.Sensor.FrameWidth.ToString();
                txtFrameSizeHeight.Text = cameraModel.Sensor.FrameHeight.ToString();
                txtCropSizeWidth.Text = cameraModel.Sensor.CropWidth.ToString();
                txtCropSizeHeight.Text = cameraModel.Sensor.CropHeight.ToString();
                txtPixelSizeWidth.Text = cameraModel.Sensor.PixelSizeWidth.ToString();
                txtPixelSizeHeight.Text = cameraModel.Sensor.PixelSizeHeight.ToString();
                txtExposureMin.Text = cameraModel.ExposureMin.ToString();
                txtExposureMax.Text = cameraModel.ExposureMax.ToString();

                lbISO.Items.Clear();
                lbISO.Items.AddRange(new ListBox.ObjectCollection(lbISO, cameraModel.Gains.OrderBy(g => g).Select(g => (object)g.ToString()).ToArray()));
                
                lbShutterSpeed.Items.Clear();
                lbShutterSpeed.Items.AddRange(new ListBox.ObjectCollection(lbShutterSpeed, cameraModel.ShutterSpeeds.OrderBy(s => s.DurationSeconds).Select(s => (object)$"{s.Name};{s.DurationSeconds.ToString()}").ToArray()));
            }
            else
            {
                txtCameraModel.Text = string.Empty;
                txtSensorName.Text = string.Empty;
                txtSensorSizeWidth.Text = string.Empty;
                txtSensorSizeHeight.Text = string.Empty;
                txtFrameSizeWidth.Text = string.Empty;
                txtFrameSizeHeight.Text = string.Empty;
                txtCropSizeWidth.Text = string.Empty;
                txtCropSizeHeight.Text = string.Empty;
                txtPixelSizeWidth.Text = string.Empty;
                txtPixelSizeHeight.Text = string.Empty;
                txtExposureMin.Text = string.Empty;
                txtExposureMax.Text = string.Empty;
                lbISO.Items.Clear();
                lbShutterSpeed.Items.Clear();
            }
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }
    }
}