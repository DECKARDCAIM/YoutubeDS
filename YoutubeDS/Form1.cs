using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDS
{
    public partial class Form1 : Form
    {
        private readonly YoutubeClient youtubeClient;
        private System.Windows.Forms.Timer timer1;


        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        // Constantes para mover el formulario
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;


        public Form1()
        {
            InitializeComponent();
            youtubeClient = new YoutubeClient();

            // Crear una nueva instancia del Timer
            timer1 = new System.Windows.Forms.Timer();

            // Asociar el evento MouseDown a los controles que quieras que permitan mover el formulario
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            pictureBox1.MouseDown += new MouseEventHandler(Form1_MouseDown); // Mover desde el PictureBox


            dateTimePicker1.Value = DateTime.Now;

            // Configurar el formato personalizado para mostrar fecha y hora
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dddd, dd MMMM yyyy - HH:mm:ss"; // Día, fecha, hora:minutos:segundos

            // Deshabilitar la modificación del DateTimePicker
            dateTimePicker1.Enabled = false;

            // Iniciar el Timer para actualizar la hora en tiempo real
            timer1.Interval = 1000; // 1 segundo
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();

        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            string videoUrl = txtUrl.Text.Trim();

            try
            {
                var video = await youtubeClient.Videos.GetAsync(videoUrl);

                // Obtener el manifiesto de streams
                var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);

                // Obtener todos los flujos de video combinados (audio y video)
                var muxedStreams = streamManifest.GetMuxedStreams();

                // Obtener el mejor flujo de video por calidad (mayor calidad disponible)
                var bestVideoStream = muxedStreams.OrderByDescending(s => s.VideoQuality).First();

                // Mostrar información del video
                MessageBox.Show($"Descargando video: {video.Title}");

                // Guardar el video en una ubicación seleccionada por el usuario
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.FileName = video.Title + ".mp4";
                saveDialog.Filter = "Archivos MP4 (*.mp4)|*.mp4|Todos los archivos (*.*)|*.*";
                saveDialog.Title = "Guardar video";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string savePath = saveDialog.FileName;

                    // Descargar el mejor flujo de video encontrado
                    await youtubeClient.Videos.Streams.DownloadAsync(bestVideoStream, savePath);

                    MessageBox.Show("Descarga completada. El video ha sido guardado en: " + savePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar el video: " + ex.Message);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string videoUrl = txtUrl.Text.Trim();

            try
            {
                var video = await youtubeClient.Videos.GetAsync(videoUrl);

                // Obtener el manifiesto de streams
                var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);

                // Obtener todos los flujos de audio
                var audioStreams = streamManifest.GetAudioOnlyStreams();

                // Ordenar los flujos por calidad de audio (mayor a menor)
                var sortedAudioStreams = audioStreams.OrderByDescending(s => s.Bitrate);

                // Obtener el mejor flujo de audio (mayor calidad disponible)
                var bestAudioStream = sortedAudioStreams.FirstOrDefault();

                // Mostrar información del audio
                MessageBox.Show($"Descargando audio de: {video.Title}");

                // Guardar el audio en una ubicación seleccionada por el usuario
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.FileName = video.Title + ".mp3";
                saveDialog.Filter = "Archivos MP3 (*.mp3)|*.mp3|Todos los archivos (*.*)|*.*";
                saveDialog.Title = "Guardar audio";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string savePath = saveDialog.FileName;

                    // Descargar el flujo de audio en el archivo .mp3
                    await youtubeClient.Videos.Streams.DownloadAsync(bestAudioStream, savePath);

                    MessageBox.Show("Descarga completada. El audio ha sido guardado en: " + savePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar el audio: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
            private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();  // Ocultar Form1

            // Crear y mostrar Form2 como un formulario modal
            Form2 form2 = new Form2();
            form2.ShowDialog();

            // Mostrar Form1 cuando Form2 se cierre
            this.Show();
        }
    }
}
