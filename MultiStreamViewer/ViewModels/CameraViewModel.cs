using MultiStreamViewer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;

namespace MultiStreamViewer.ViewModels
{
	public partial class CameraViewModel : ObservableObject, IDisposable
	{
		private Unosquare.FFME.MediaElement _media;

		[ObservableProperty]
		private CameraStream? _model;

		partial void OnModelChanged( CameraStream? oldValue, CameraStream? newValue ) {
			if( oldValue != null ) CloseCommand.Execute( null );
			if( newValue != null ) {
				ModelName = newValue.Name;
				OpenCommand.Execute( null );
			}
			else ModelName = string.Empty;
		}

		[ObservableProperty]
		private string _modelName = string.Empty;

		[ObservableProperty]
		private bool _logoVisible = true;

		[ObservableProperty]
		private bool _statusMessageVisible;

		[ObservableProperty]
		private string? _statusMessage;

		partial void OnStatusMessageChanged( string? value ) {
			if( string.IsNullOrWhiteSpace( value ) ) StatusMessageVisible = false;
			else StatusMessageVisible = true;
		}

		public void AttachMedia( Unosquare.FFME.MediaElement media ) {
			_media = media;

			_media.MediaInitializing += ( s, e ) => {
				StatusMessage = "Initializing...";
			};

			_media.MediaOpening += ( s, e ) => {
				StatusMessage = "Opening...";
			};

			_media.MediaOpened += ( s, e ) => {
				StatusMessage = "Opened...";
			};

			_media.MediaReady += async ( s, e ) => {
				StatusMessage = null;
				await _media.Play();
			};

			//_media.MediaClosed += ( s, e ) => {
			//	StatusMessage = "Media Closed...";
			//};

			_media.MediaChanging += ( s, e ) => {
				StatusMessage = "Changing Media...";
			};

			//_media.AudioDeviceStopped += ( s, e ) => {
			//	System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Audio Device Stopped: {Model.Name}" );
			//};

			_media.MediaChanged += ( s, e ) => {
				StatusMessage = "Media Changed...";
			};

			//_media.PositionChanged += ( s, e ) => {
			//	System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Position Changed: {Model.Name}" );
			//};

			_media.MediaFailed += ( s, e ) => {
				System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Media Failed {e.ErrorException.Message}" );
				StatusMessage = "Failed...";
			};

			//_media.MessageLogged += ( s, e ) => {
			//	System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Messsage Logged: {Model.Name}: {e.Message}" );
			//};

			_media.MediaStateChanged += ( s, e ) => {
				System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Media State Changed to {e.MediaState}" );
			};

			_media.MediaStateChanged += ( s, e ) => {
				switch( e.MediaState ) {
					case Unosquare.FFME.Common.MediaPlaybackState.Close:
					case Unosquare.FFME.Common.MediaPlaybackState.Stop:
						LogoVisible = true;
						break;
					default:
						LogoVisible = false;
						break;
				}
			};

			//_media.DataFrameReceived += OnDataFrameReceived;

		}

		[RelayCommand]
		private async Task Open() {
			if( Model == null ) return;
			await _media.Open( new Uri( Model.Source ) );
		}

		[RelayCommand]
		private async Task Play() {
			await _media.Play();
		}

		[RelayCommand]
		private async Task Pause() {
			await _media.Pause();
		}

		[RelayCommand]
		private async Task Close() {
			await _media.Close();
			Model = null;
		}

		[RelayCommand]
		private void Drop( DataObject droppedItem ) {
			var stream = droppedItem.GetData( typeof( CameraStream ));
			if ( stream is CameraStream cameraStream ) {
				Model = cameraStream;
			}
		}

		public void Dispose() {
			if( _media != null ) _media.Dispose();
		}
	}
}
