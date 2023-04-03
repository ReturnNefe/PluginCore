using System.Linq;
using Nefe.PluginCore;
using Nefe.PluginCore.Unit.Test.Interface;

namespace Nefe.PluginCore.Unit.Test.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var plugins = new string[] { "Plugin1", "Plugin2" };
            foreach (var pluginName in plugins)
            {
                var plugin = new Plugin(true);
                var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins", pluginName, $"Nefe.PluginCore.Unit.Test.{pluginName}.dll");
                plugin.Unloading += (e) => Console.WriteLine("[App] Unloading...");

                plugin.LoadFromFile(pluginPath);
                //plugin.LoadFromStream(new FileStream(pluginPath, FileMode.Open));
                //plugin.LoadFromAssemblyPath(pluginPath);

                Console.WriteLine(string.Join(Environment.CommandLine,
                                              plugin.CreateInstances<Interface.PluginBase>().Select((inst) => inst.MakeText())));

                plugin.Unload();
            }
        }
    }
}