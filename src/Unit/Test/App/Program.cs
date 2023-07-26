using System.Linq;
using System.Runtime.CompilerServices;
using Nefe.PluginCore;
using Nefe.PluginCore.Unit.Test.Interface;

namespace Nefe.PluginCore.Unit.Test.App
{
    class Program
    {
        static Dictionary<string, Plugin> pluginMap = new();

        static void Main(string[] args)
        {
            var plugins = new string[] { "Plugin1", "Plugin2" };

            foreach (var pluginName in plugins)
            {
                var pluginPath = Path.Combine(AppContext.BaseDirectory, "plugins", pluginName, $"Nefe.PluginCore.Unit.Test.{pluginName}.dll");
                var plugin = new Plugin(pluginPath, isCollectible: true);
                plugin.Unloading += (e) => Console.WriteLine("[App] Unloading...");

                var assembly = plugin.LoadFromAssemblyName(new System.Reflection.AssemblyName($"Nefe.PluginCore.Unit.Test.{pluginName}"));

                if (assembly != null)
                    Console.WriteLine(string.Join(Environment.NewLine,
                                                  plugin.CreateInstancesFromAssembly<Interface.PluginBase>(assembly).Select((inst) => inst.MakeText())));
                else
                    Console.WriteLine("Failed");

                pluginMap.Add(pluginName, plugin);
            }

            // Unload all plugins
            foreach (var pluginName in plugins)
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                void RemovePlugin(string name, out WeakReference weakRef)
                {
                    pluginMap[name].Unload();
                    weakRef = new WeakReference(pluginMap[name], true);
                    pluginMap.Remove(name);
                }

                WeakReference weakRef;
                RemovePlugin(pluginName, out weakRef);

                var i = 0;
                for (i = 0; weakRef.IsAlive && (i < 100); ++i)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                Console.Write($"{i}");
                Console.ReadLine();
            }

            Console.ReadLine();
        }
    }
}