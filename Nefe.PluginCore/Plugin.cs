using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;

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
        private PluginBase? plugin;

        // Private Method
        private void LoadPlugin(string path = "")
        {
            plugin = new PluginBase(path, this.IsCollectible);

            // NOTE:
            // To Be Tested
            plugin.Resolving += (e, name) => this.Resolving?.Invoke(e, name);
            plugin.ResolvingUnmanagedDll += (assembly, text) => this.ResolvingUnmanagedDll?.Invoke(assembly, text) ?? IntPtr.Zero;
            plugin.Unloading += (e) => this.Unloading?.Invoke(e);
        }

        // Public Properties
        /// <summary>
        /// Returns a collection of the System.Reflection.Assembly instances which has been loaded.
        /// </summary>
        public IEnumerable<Assembly>? Assemblies { get => plugin?.Assemblies; }

        /// <summary>
        /// Indicates whether unloading is enabled.
        /// </summary>
        public bool IsCollectible { get; set; }

        /// <summary>
        /// Get the name of the plugin. 
        /// </summary>
        public string? Name { get => plugin?.Name; }

        // Public Events
        public event Func<AssemblyLoadContext, AssemblyName, Assembly?>? Resolving;
        public event Func<Assembly, string, IntPtr>? ResolvingUnmanagedDll;
        public event Action<AssemblyLoadContext>? Unloading;

        // Public Method
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="isCollectible">
        /// Indicates whether unloading is enabled.
        /// True if the plugin can be unloaded, Otherwise, false.
        /// </param>
        public Plugin(bool isCollectible = false)
        {
            this.IsCollectible = isCollectible;
        }

        /// <summary>
        /// Loads the assembly from file stream with a common object file format (COFF)-based image containing a managed assembly.
        /// </summary>
        /// <param name="path">A relative or absolute path for the file that the plugin will encapsulate.</param>
        /// <returns>The loaded assembly.</returns>
        public Assembly? LoadFromFile(string path)
        {
            this.LoadPlugin(path);
            using var inStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return plugin?.LoadFromStream(inStream);
        }

        public Assembly? LoadFromStream(Stream stream)
        {
            this.LoadPlugin();
            return plugin?.LoadFromStream(stream);
        }

        public Assembly? LoadFromStream(Stream stream, Stream symbols)
        {
            this.LoadPlugin();
            return plugin?.LoadFromStream(stream, symbols);
        }

        public Assembly? LoadFromAssemblyPath(string path)
        {
            this.LoadPlugin(path);
            return plugin?.LoadFromAssemblyPath(path);
        }

        // NOTE:
        // To Be Tested
        public Assembly? LoadFromAssemblyName(string path, AssemblyName name)
        {
            this.LoadPlugin(path);
            return plugin?.LoadFromAssemblyName(name);
        }
        
        /// <summary>
        /// Create the instances from the plugin by particular type.
        /// </summary>
        /// <typeparam name="T">The type of instances.</typeparam>
        /// <returns>The instances that were created.</returns>
        public IEnumerable<T> CreateInstances<T>()
        {
            if (plugin == null)
                yield break;

            foreach (var assembly in plugin.Assemblies)
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
        }

        // NOTE:
        // ``foreceWaiting`` To Be Fixed
        public void Unload(bool forceWaiting = false)
        {
            if (plugin != null)
            {
                plugin.Unload();

                if (forceWaiting)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
    }
}