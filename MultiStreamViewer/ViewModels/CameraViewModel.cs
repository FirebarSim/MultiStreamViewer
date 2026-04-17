using MultiStreamViewer.Models;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MultiStreamViewer.ViewModels
{
	public partial class CameraViewModel : ObservableObject, IDisposable
	{
		private Unosquare.FFME.MediaElement _media;

		[ObservableProperty]
		private CameraStream _model;

		[ObservableProperty]
		private bool _logoVisible = true;

		[ObservableProperty]
		private bool _statusMessageVisible;

		[ObservableProperty]
		private string? _statusMessage;
		
		partial void OnStatusMessageChanged( string? value ) {
			if(string.IsNullOrWhiteSpace(value)) StatusMessageVisible = false;
			else StatusMessageVisible = true;
		}

		public ICommand OpenCommand { get; }
		public ICommand PlayCommand { get; }
		public ICommand PauseCommand { get; }
		public ICommand StopCommand { get; }
		public ICommand CloseCommand { get; }

		public CameraViewModel( CameraStream model ) {
			Model = model;
			OpenCommand = new RelayCommand( _ => Open() );
			PlayCommand = new RelayCommand( _ => Play() );
			PauseCommand = new RelayCommand( _ => Pause() );
			StopCommand = new RelayCommand( _ => Stop() );
			CloseCommand = new RelayCommand( _ => Close() );
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
				System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Media Failed ({Model.Name}) {e.ErrorException.Message}" );
				StatusMessage = "Failed...";
			};

			//_media.MessageLogged += ( s, e ) => {
			//	System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Messsage Logged: {Model.Name}: {e.Message}" );
			//};

			_media.MediaStateChanged += ( s, e ) => {
				System.Diagnostics.Debug.WriteLine( $"CameraViewModel: Media State Changed: {Model.Name}: {e.MediaState}" );
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

		public async void Open() {
			await _media.Open( new Uri( Model.Source ) );
		}

		public async void Play() {
			await _media.Play();
		}

		public async void Pause() {
			await _media.Pause();
		}

		public async void Stop() {
			await _media.Stop();
		}

		public async void Close() {
			await _media.Close();
		}

		public void Dispose() {
			if( _media != null ) _media.Dispose();
		}
	}
}
