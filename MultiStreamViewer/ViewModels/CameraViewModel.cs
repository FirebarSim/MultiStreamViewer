using MultiStreamViewer.Models;
using MultiStreamViewer.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MultiStreamViewer.ViewModels
{
    public class CameraViewModel : INotifyPropertyChanged
    {
        public CameraStream Model { get; }
        private readonly FfmpegFrameService _service;

        private BitmapImage? _imageSource;
        public BitmapImage? ImageSource
        {
            get => _imageSource;
            private set
            {
                _imageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSource)));
            }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CameraViewModel(CameraStream model)
        {
            Model = model;
            _service = new FfmpegFrameService(model.Source);
            _service.FrameReady += OnFrameReady;

            StartCommand = new RelayCommand(_ => Start());
            StopCommand = new RelayCommand(_ => Stop());
        }

        private void OnFrameReady(byte[] frame)
        {
            // convert to BitmapImage on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    using var ms = new MemoryStream(frame);
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();
                    bi.Freeze();
                    ImageSource = bi;
                }
                catch { }
            });
        }

        public void Start()
        {
            if (!Model.Enabled)
                return;

            try { _service.Start(); } catch { }
        }

        public void Stop()
        {
            try { _service.Stop(); } catch { }
        }

        public void Dispose()
        {
            _service.Dispose();
        }
    }
}
