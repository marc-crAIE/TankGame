using RayEngine.Core;
using RayEngine.Debug;
using RayEngine.Utils;
using SharpMaths;

namespace TankGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationSpecification spec = new ApplicationSpecification("Sandbox App");

            Profiler.BeginSession("Startup", "startup.json");

            TankGame game = new TankGame(spec);

            Profiler.EndSession();

            Profiler.BeginSession("Runtime", "runtime.json");

            game.Run();

            Profiler.EndSession();
        }
    }
}