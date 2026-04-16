using MultiStreamViewer.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace MultiStreamViewer.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<CameraViewModel> Streams { get; } = new ObservableCollection<CameraViewModel>();

        public ICommand StartAllCommand { get; }
        public ICommand StopAllCommand { get; }

        public MainWindowViewModel()
        {
            StartAllCommand = new RelayCommand(_ => StartAll());
            StopAllCommand = new RelayCommand(_ => StopAll());

            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "streams.json");
                CameraStream[]? list = null;

                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    list = JsonSerializer.Deserialize<CameraStream[]>(json);
                }

                // If no config present, provide a small default prototype list
                if (list == null || list.Length == 0)
                {
                    list = new[]
                    {
                        new CameraStream { Name = "Sample RTSP", Source = "rtsp://your_rtsp_server/stream", Enabled = true },
                        new CameraStream { Name = "Sample UDP", Source = "udp://@239.0.0.1:1234", Enabled = true }
                    };
                }

                foreach (var s in list)
                {
                    Streams.Add(new CameraViewModel(s));
                }
            }
            catch { }
        }

        public void StartAll()
        {
            foreach (var s in Streams) s.Start();
        }

        public void StopAll()
        {
            foreach (var s in Streams) s.Stop();
        }

        public void DisposeAll()
        {
            foreach (var s in Streams) s.Dispose();
        }
    }
}
