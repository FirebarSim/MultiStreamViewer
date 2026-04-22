using MultiStreamViewer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;

namespace MultiStreamViewer.ViewModels
{
	public partial class CameraViewModel : ObservableObject, IDisposable
	{
		public Unosquare.FFME.MediaElement Media { get; protected set; }

		[ObservableProperty]
		private CameraStream? _model;

		partial void OnModelChanged( CameraStream? oldValue, CameraStream? newValue ) {
			if( oldValue != null ) CloseCommand.Execute( null );
			if( newValue != null ) {
				ModelName = newValue.Name;
				OpenCommand.Execute( null );
			}
			else ModelName = string.Empty;
			Debug.WriteLine( $"CameraViewModel: Info : Model changed from '{oldValue?.Name}' to '{newValue?.Name}'" );
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
			Media = media;

			Media.MediaInitializing += ( s, e ) => {
				StatusMessage = "Initializing...";
			};

			Media.MediaOpening += ( s, e ) => {
				StatusMessage = "Opening...";
			};

			Media.MediaOpened += ( s, e ) => {
				StatusMessage = "Opened...";
			};

			Media.MediaReady += async ( s, e ) => {
				System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Debug : Media Ready" );
				StatusMessage = null;
				await Media.Play();
			};

			Media.MediaClosed += ( s, e ) => {
				System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Debug : Media Closed" );
			};

			Media.MediaChanging += ( s, e ) => {
				StatusMessage = "Changing Media...";
			};

			//_media.AudioDeviceStopped += ( s, e ) => {
			//	System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Audio Device Stopped: {Model.Name}" );
			//};

			Media.MediaChanged += ( s, e ) => {
				StatusMessage = "Media Changed...";
			};

			//_media.PositionChanged += ( s, e ) => {
			//	System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Position Changed: {Model.Name}" );
			//};

			Media.MediaFailed += ( s, e ) => {
				System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Media Failed {e.ErrorException.Message}" );
				StatusMessage = "Failed...";
			};

			Media.MessageLogged += ( s, e ) => {
				switch( e.MessageType ) {
					//case Unosquare.FFME.Common.MediaLogMessageType.None:
					//	break;
					//case Unosquare.FFME.Common.MediaLogMessageType.Trace:
					//	break;
					case Unosquare.FFME.Common.MediaLogMessageType.Info:
					case Unosquare.FFME.Common.MediaLogMessageType.Debug:
					case Unosquare.FFME.Common.MediaLogMessageType.Error:
					case Unosquare.FFME.Common.MediaLogMessageType.Warning:
						System.Diagnostics.Debug.WriteLine( $"CameraViewModel: {e.MessageType} : {e.Message}" );
						break;
					default:
						break;
				}
			};

			Media.MediaStateChanged += ( s, e ) => {
				System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Info : Media State Changed to {e.MediaState}" );
				switch( e.MediaState ) {
					case Unosquare.FFME.Common.MediaPlaybackState.Play:
						LogoVisible = false;
						CanPause = true;
						CanPlay = false;
						break;
					case Unosquare.FFME.Common.MediaPlaybackState.Pause:
						LogoVisible = false;
						CanPause = false;
						CanPlay = true;
						break;
					case Unosquare.FFME.Common.MediaPlaybackState.Close:
					case Unosquare.FFME.Common.MediaPlaybackState.Stop:
						LogoVisible = true;
						CanPause = false;
						CanPlay = false;
						break;
					default:
						LogoVisible = false;
						break;
				}
			};

			//Media.DataFrameReceived += ( s, e ) => {
			//	System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Data : {e.Frame}" );
			//};

		}

		[RelayCommand]
		private async Task Open() {
			if( Model == null ) return;
			await Media.Open( new Uri( Model.Source ) );
		}

		[ObservableProperty]
		private bool _canPause;

		[RelayCommand]
		private async Task Play() {
			await Media.Play();
		}

		[ObservableProperty]
		private bool _canPlay;

		[RelayCommand]
		private async Task Pause() {
			await Media.Pause();
		}

		[RelayCommand]
		private async Task Close() {
			await Media.Close();
			Model = null;
		}

		[ObservableProperty]
		private bool _isMuted;

		[RelayCommand]
		private async Task Mute() {
			Media.IsMuted = true;
			IsMuted = true;
			System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Info : Media Muted" );
		}

		[RelayCommand]
		private async Task Unmute() {
			Media.IsMuted = false;
			IsMuted = false;
			System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Info : Media Unmuted" );
		}

		[RelayCommand]
		private void Drop( DataObject droppedItem ) {
			var stream = droppedItem.GetData( typeof( CameraStream ) );
			if( stream is CameraStream cameraStream ) {
				Model = cameraStream;
			}
		}

		public void Dispose() {
			if( Media != null ) Media.Dispose();
		}
	}
}
