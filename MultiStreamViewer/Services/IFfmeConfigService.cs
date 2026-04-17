using Microsoft.Extensions.Configuration;

namespace MultiStreamViewer.Services
{
	public interface IFFmpegConfigService
	{
		string GetFFmpegDirectory();
	}

	public class FFmpegConfigService( IConfiguration configuration ) : IFFmpegConfigService
	{
		public string GetFFmpegDirectory() {
			return configuration.GetSection( "AppSettings:FFmpegDirectory" ).Value ?? @"C:\ffmpeg-n7.1.3-43-g5a1f107b4c-win64-gpl-shared-7.1\bin";
		}
	}
}
