using MultiStreamViewer.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultiStreamViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ListView sourceBox;

		public MainWindow() {
			InitializeComponent();
		}

		// Called when the mouse moves over one of the 
		// TextBlocks displaying our items
		private void TextBlock_MouseMove( object sender, MouseEventArgs e ) {
			// If the mousebutton isn't pressed, return immediately;
			if( e.LeftButton != MouseButtonState.Pressed ) return;

			// Cast the sender (TextBox) to a FrameworkElement
			// So we can grab the DataContext
			FrameworkElement fe = sender as FrameworkElement;
			if( fe == null ) return;

			// Initiate the drag-and-drop operation.
			DragDrop.DoDragDrop( StreamList, fe.DataContext, DragDropEffects.Move );
		}

		protected override void OnClosed(System.EventArgs e)
		{
			base.OnClosed(e);
			if(DataContext is IDisposable d) d.Dispose();
		}
	}
}
