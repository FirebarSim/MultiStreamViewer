using Microsoft.Extensions.Configuration;

namespace MultiStreamViewer.Services
{
	public interface IFFmpegConfigService
	{
		string GetFFmpegDirectory();
		int GetMaxCameras();
	}

	public class FFmpegConfigService( IConfiguration configuration ) : IFFmpegConfigService
	{
		public string GetFFmpegDirectory() {
			return configuration.GetValue<string>( "AppSettings:FFmpegDirectory" ) ?? @"C:\ffmpeg-n7.1.3-43-g5a1f107b4c-win64-gpl-shared-7.1\bin";
		}

		public int GetMaxCameras() {
			return configuration.GetValue<int>( "AppSettings:MaxCameras" );
		}
	}
}
