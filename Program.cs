using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Mag
{
    public class Program
    {
        public static async Task Main(string[] args) => await Infrasctructure.HostInfrastructure.CreateHostBuilder(args).Build().RunAsync();
    }
}
