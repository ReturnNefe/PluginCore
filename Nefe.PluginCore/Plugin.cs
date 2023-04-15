using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Nefe.PluginCore
{
    // NOTE:
    // XML Documents To Be Added
    // *****
    // About XML Documents:
    //   <exception> to be added.

    /// <summary>
    /// Represents a plugin.
    /// </summary>
    public class Plugin
    {
        // Private Properties
        private PluginBase plugin;

        // Public Properties
        /// <summary>
        /// Returns a collection of the System.Reflection.Assembly instances which has been loaded.
        /// </summary>
        public IEnumerable<Assembly> Assemblies { get => plugin.Assemblies; }

        /// <summary>
        /// Get the name of the plugin. 
        /// </summary>
        public string? Name { get => plugin.Name; }

        /// <summary>
        /// Get the path of the plugin.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Indicates whether unloading is enabled.
        /// </summary>
        public bool IsCollectible { get; set; }

        // Public Events
        public event Func<AssemblyLoadContext, AssemblyName, Assembly?>? Resolving;
        public event Func<Assembly, string, IntPtr>? ResolvingUnmanagedDll;
        public event Action<AssemblyLoadContext>? Unloading;

        // Public Method
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="pluginPath">The path of the plugin file which needs to be loaded.</param>
        /// <param name="isCollectible">
        /// Indicates whether unloading is enabled.
        /// True if the plugin can be unloaded, Otherwise, false.
        /// </param>
        public Plugin(string pluginPath, bool isCollectible = false)
        {
            this.IsCollectible = isCollectible;
            this.Path = pluginPath;

            this.plugin = new PluginBase(pluginPath, this.IsCollectible);

            // NOTE:
            // To Be Tested
            plugin.Resolving += (e, name) => this.Resolving?.Invoke(e, name);
            plugin.ResolvingUnmanagedDll += (assembly, text) => this.ResolvingUnmanagedDll?.Invoke(assembly, text) ?? IntPtr.Zero;
            plugin.Unloading += (e) => this.Unloading?.Invoke(e);
        }

        /// <summary>
        /// Loads the assembly from file stream with a common object file format (COFF)-based image containing a managed assembly.
        /// </summary>
        /// <param name="path">A relative or absolute path for the file that the plugin will encapsulate.</param>
        /// <returns>The loaded assembly.</returns>
        public Assembly? LoadFromFile()
        {
            using var inStream = new FileStream(this.Path, FileMode.Open, FileAccess.Read);
            return this.plugin.LoadFromStream(inStream);
        }

        /*
        
        // NOTE:
        // Changelog 2023/4/14
        // LoadFromStream method has been removed,
        // because I think it isn't necessary.
        
        public Assembly? LoadFromStream(Stream stream)
        {
            this.LoadPlugin();
            return plugin.LoadFromStream(stream);
        }

        public Assembly? LoadFromStream(Stream stream, Stream symbols)
        {
            this.LoadPlugin();
            return plugin.LoadFromStream(stream, symbols);
        }
        */

        public Assembly? LoadFromAssemblyPath()
        {
            return this.plugin.LoadFromAssemblyPath(this.Path);
        }

        // NOTE:
        // To Be Tested
        public Assembly? LoadFromAssemblyName(AssemblyName name)
        {
            return this.plugin.LoadFromAssemblyName(name);
        }



        // NOTE:
        // Changelog 2023/4/4 (to published)
        // Add CreateInstancesFromAssembly Method
        // Use CreateInstancesFromAssembly, not CreateInstances, because CreateInstances will search for all assembly
        // ↓
        // plugin.Assemblies -> { PluginA, PluginA.Reference1, PluginA.Reference2, ... }

        /// <summary>
        /// Create the instances from the assembly by particular type.
        /// </summary>
        /// <param name="assembly">The assembly to which the type belong</param>
        /// <typeparam name="T">The type of instances.</typeparam>
        /// <returns>The instances that were created.</returns>
        public IEnumerable<T> CreateInstancesFromAssembly<T>(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    if (Activator.CreateInstance(type) is T result)
                        yield return result;
                }
            }
        }

        /// <summary>
        /// Create the instances from each assembly of the plugin by particular type.
        /// </summary>
        /// <typeparam name="T">The type of instances.</typeparam>
        /// <returns>The instances that were created.</returns>
        public IEnumerable<T> CreateInstances<T>()
        {
            if (this.plugin == null)
                yield break;

            foreach (var assembly in this.plugin.Assemblies)
            {
                var result = CreateInstancesFromAssembly<T>(assembly);
                foreach (var iter in result)
                    yield return iter;
            }
        }

        // NOTE:
        // Wait for the completition of unloading to be tested.
        public void Unload() => plugin.Unload();
    }
}