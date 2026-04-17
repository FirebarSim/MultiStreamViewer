using Microsoft.Extensions.Options;
using MultiStreamViewer.Models;
using MultiStreamViewer.Services;
using System.Collections.ObjectModel;

namespace MultiStreamViewer.ViewModels
{
    public class MainWindowViewModel: IDisposable
    {
        public ObservableCollection<CameraStream> Streams { get; } = new ObservableCollection<CameraStream>();
		public ObservableCollection<CameraViewModel> Cameras { get; } = new ObservableCollection<CameraViewModel>();

        public MainWindowViewModel( IFFmpegConfigService fFmpegConfigService, IStreamsService streamsService ) {
			Unosquare.FFME.Library.FFmpegDirectory = fFmpegConfigService.GetFFmpegDirectory();
			Unosquare.FFME.MediaElement.FFmpegMessageLogged += MediaElement_FFmpegMessageLogged;
			System.Diagnostics.Debug.WriteLine( $"MainWindowViewModel: FFmpeg Directory set to: {Unosquare.FFME.Library.FFmpegDirectory}");

            LoadStreams( streamsService );
        }

		private void MediaElement_FFmpegMessageLogged( object? sender, Unosquare.FFME.Common.MediaLogMessageEventArgs e ) {
			System.Diagnostics.Debug.WriteLine( $"FFmpeg: {e.Message}" );
		}

		private void LoadStreams( IStreamsService streamsService )
        {
			// Load the stream list from appsettings.json
			try {
                foreach (var s in streamsService.GetStreams())
                {
                    Streams.Add(s);
                    //Streams.Add(new CameraViewModel(s));
                }
            }
            catch { }
        }

        public void Dispose()
        {
            foreach (var c in Cameras ) c.Dispose();
        }
    }
}
