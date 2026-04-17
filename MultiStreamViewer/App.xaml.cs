using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MultiStreamViewer.Services;
using System.Windows;
using MultiStreamViewer.ViewModels;

namespace MultiStreamViewer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IHost Host { get; private set; }

		private async void Application_Startup( object sender, StartupEventArgs e ) {
			var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();

			builder.Services.AddSingleton<IStreamsService, StreamsService>();
			builder.Services.AddSingleton<IFFmpegConfigService, FFmpegConfigService>();
			builder.Services.AddSingleton<MainWindow>();
			builder.Services.AddSingleton<MainWindowViewModel>();

			Host = builder.Build();

			await Host.StartAsync();

			MainWindow mainWindow = Host.Services.GetRequiredService<MainWindow>();
			mainWindow.Show();
		}

		private async void Application_Exit( object sender, ExitEventArgs e ) {
			using( Host ) {
				await Host.StopAsync();
			}
		}
	}
}
