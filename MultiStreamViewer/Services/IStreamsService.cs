using Microsoft.Extensions.Configuration;
using MultiStreamViewer.Models;

namespace MultiStreamViewer.Services
{
    public interface IStreamsService
    {
        List<CameraStream> GetStreams();
    }

    public class StreamsService( IConfiguration configuration ) : IStreamsService {
		public List<CameraStream> GetStreams() {
			return configuration.GetSection( "AppSettings:Streams" ).Get<List<CameraStream>>() ?? new List<CameraStream>();
        }
	}
}
