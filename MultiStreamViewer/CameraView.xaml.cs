using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
	}
}
