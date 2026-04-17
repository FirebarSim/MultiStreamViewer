using System.Windows;
using System.Windows.Controls;
using MultiStreamViewer.ViewModels;

namespace MultiStreamViewer
{
	/// <summary>
	/// Interaction logic for CameraView.xaml
	/// </summary>
	public partial class CameraView : UserControl {

		public CameraView() {
			InitializeComponent();
			Loaded += OnLoaded;
		}

		private void OnLoaded( object sender, RoutedEventArgs e ) {
			if( DataContext is CameraViewModel vm ) vm.AttachMedia( Media );
		}

		private void Border_Drop( object sender, DragEventArgs e ) {
			if( DataContext is CameraViewModel vm ) vm.DropCommand.Execute( e.Data );
		}
	}
}
