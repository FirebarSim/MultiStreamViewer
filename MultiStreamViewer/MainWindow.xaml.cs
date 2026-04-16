using System.Windows;
using MultiStreamViewer.ViewModels;

namespace MultiStreamViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainWindowViewModel _vm;

		public MainWindow()
		{
			InitializeComponent();
			_vm = new MainWindowViewModel();
			DataContext = _vm;
		}

		protected override void OnClosed(System.EventArgs e)
		{
			base.OnClosed(e);
			_vm.StopAll();
			_vm.DisposeAll();
		}
	}
}
