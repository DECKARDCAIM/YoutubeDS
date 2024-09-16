using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NReco.VideoConverter;

namespace YoutubeDS
{
    public partial class Form2 : Form
    {

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        // Constantes para mover el formulario
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public Form2()
        {
            InitializeComponent();
            // Asociar el evento MouseDown a los controles que quieras que permitan mover el formulario
            this.MouseDown += new MouseEventHandler(Form2_MouseDown);
            pictureBox1.MouseDown += new MouseEventHandler(Form2_MouseDown); // Mover desde el PictureBox

        }

        private void btnSelectFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Permitir múltiples formatos de video y audio
                openFileDialog.Filter = "Audio/Video Files|*.webm;*.mp4;*.mkv;*.avi;*.mp3;*.wav;*.flac|All Files|*.*";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    listBoxFiles.Items.Clear();  // Limpia el listado de archivos anteriores
                    foreach (var file in openFileDialog.FileNames)
                    {
                        listBoxFiles.Items.Add(file);
                    }
                }
            }
        }

        private void btnSelectOutputFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtOutputFolder.Text = folderDialog.SelectedPath;
                }
            }
        }

        private async void btnConvert_Click(object sender, EventArgs e)

        {
            if (listBoxFiles.Items.Count == 0 || string.IsNullOrEmpty(txtOutputFolder.Text))
            {
                MessageBox.Show("Por favor, selecciona archivos y una ubicación de salida.");
                return;
            }

            var converter = new FFMpegConverter();
            progressBar1.Value = 0;
            progressBar1.Maximum = listBoxFiles.Items.Count;

            foreach (var file in listBoxFiles.Items)
            {
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.ToString());
                string outputFilePath = System.IO.Path.Combine(txtOutputFolder.Text, fileNameWithoutExtension + ".webm");

                await Task.Run(() =>
                {
                    // Convierte cualquier archivo multimedia a MP3
                    converter.ConvertMedia(file.ToString(), outputFilePath, "webm");
                });

                // Incrementa el valor de la barra de progreso
                progressBar1.Value += 1;
            }

            MessageBox.Show("¡Conversión completada!");
        }
        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
