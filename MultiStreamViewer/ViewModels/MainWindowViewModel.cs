using Microsoft.Extensions.Options;
using MultiStreamViewer.Models;
using MultiStreamViewer.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MultiStreamViewer.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, IDisposable
	{
        public ObservableCollection<CameraStream> Streams { get; } = new ObservableCollection<CameraStream>();
        public List<CameraViewModel> AllCameras { get; } = new List<CameraViewModel>();
        public ObservableCollection<CameraViewModel> Cameras { get; } = new ObservableCollection<CameraViewModel>();

		[ObservableProperty]
		private int _rows;

		[ObservableProperty]
		private int _columns;

        public MainWindowViewModel( IFFmpegConfigService fFmpegConfigService, IStreamsService streamsService ) {
            Unosquare.FFME.Library.FFmpegDirectory = fFmpegConfigService.GetFFmpegDirectory();
            Unosquare.FFME.MediaElement.FFmpegMessageLogged += MediaElement_FFmpegMessageLogged;
            System.Diagnostics.Debug.WriteLine( $"MainWindowViewModel: FFmpeg Directory set to: {Unosquare.FFME.Library.FFmpegDirectory}" );

            LoadCameras( fFmpegConfigService.GetMaxCameras() );
            LoadStreams( streamsService );

            SetLayout( "2x2" );
        }

        private void MediaElement_FFmpegMessageLogged( object? sender, Unosquare.FFME.Common.MediaLogMessageEventArgs e ) {
            System.Diagnostics.Debug.WriteLine( $"FFmpeg: {e.Message}" );
        }

        private void LoadCameras( int cameraCount = 4 ) {
            for( int i = 0; i < cameraCount; i++ ) {
                AllCameras.Add( new CameraViewModel() );
            }
        }

        private void LoadStreams( IStreamsService streamsService ) {
            foreach( var s in streamsService.GetStreams() ) {
                Streams.Add( s );
            }
        }

        private void SetLayout( string layout ) {
            Cameras.Clear();
            switch( layout ) {
                case "1x1":
                    Rows = 1;
                    Columns = 1;
                    Cameras.Add( AllCameras[0] );
                    break;
                case "2x2":
                    Rows = 2;
                    Columns = 2;
                    for( int i = 0; i < 4; i++ ) Cameras.Add( AllCameras[i] );
                    break;
                case "3x3":
                    Rows = 3;
                    Columns = 3;
                    for( int i = 0; i < 9; i++ ) Cameras.Add( AllCameras[i] );
                    break;
                default:
                    throw new ArgumentException( $"Unsupported layout: {layout}" );
            }
		}

		public void Dispose() {
            foreach( var c in Cameras ) c.Dispose();
        }
    }
}
