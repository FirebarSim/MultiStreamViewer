using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Markup;

namespace MultiStreamViewer
{
	public class ResolveExtension : MarkupExtension
	{
		public Type Type { get; set; }

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return App.Host.Services.GetRequiredService( Type );
		}
	}
}
