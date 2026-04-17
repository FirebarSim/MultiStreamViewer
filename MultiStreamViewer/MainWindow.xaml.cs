using System.Windows;

namespace MultiStreamViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow() {
			InitializeComponent();
		}

		protected override void OnClosed(System.EventArgs e)
		{
			base.OnClosed(e);
			if(DataContext is IDisposable d) d.Dispose();
		}
	}
}
