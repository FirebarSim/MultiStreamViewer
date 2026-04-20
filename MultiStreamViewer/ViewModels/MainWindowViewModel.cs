using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using MultiStreamViewer.Models;
using MultiStreamViewer.Services;
using System.Collections.ObjectModel;

namespace MultiStreamViewer.ViewModels
{
    public enum ViewMode
    {
        Option1 = 1,
        Option2 = 2,
        Option3 = 3
    }

    public partial class MainWindowViewModel : ObservableObject, IDisposable
    {
        public ObservableCollection<CameraStream> Streams { get; } = new ObservableCollection<CameraStream>();
        public List<CameraViewModel> AllCameras { get; } = new List<CameraViewModel>();
        public ObservableCollection<CameraViewModel> Cameras { get; } = new ObservableCollection<CameraViewModel>();

        [ObservableProperty]
        private ViewMode _selectedViewMode = ViewMode.Option2;

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

            SetLayout( ViewMode.Option2 );
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

        [RelayCommand]
        private void SetLayout( object commandParameter ) {
            if( commandParameter is ViewMode viewMode ) {
                DoLayout( (int) viewMode, (int) viewMode );
            }
            else if( System.Enum.TryParse<ViewMode>( (string) commandParameter, out ViewMode viewMode1 ) ) {
                DoLayout( (int) viewMode1, (int) viewMode1 );
            }
            else {
                throw new ArgumentException( $"Unsupported commandParameter: {commandParameter}" );
            }

            void DoLayout( int rows, int columns ) {
                Rows = rows;
                Columns = columns;
                var totalCameras = rows * columns;
                for( int i = 0; i < AllCameras.Count(); i++ ) {
                    if( i < totalCameras && !Cameras.Contains( AllCameras[i] ) ) {
                        Cameras.Add( AllCameras[i] );
                    }
                    else if( i >= totalCameras && Cameras.Contains( AllCameras[i] ) ) {
                        AllCameras[i].CloseCommand.Execute( null );
						Cameras.Remove( AllCameras[i] );
                    }
                }
            }
        }

        public void Dispose() {
            foreach( var c in Cameras ) c.Dispose();
        }
    }
}
