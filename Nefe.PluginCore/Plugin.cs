using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;

namespace Nefe.PluginCore
{
    // NOTE:
    // Documents To Be Added
    
    public class Plugin
    {
        // Private Properties
        private PluginBase? plugin;
        
        // Public Properties
        public IEnumerable<Assembly>? Assemblies { get => plugin?.Assemblies; }
        public bool IsCollectible { get => plugin?.IsCollectible ?? false; }
        public string? Name { get => plugin?.Name; }
        
        // Public Events
        public event Func<AssemblyLoadContext, AssemblyName, Assembly?> Resolving;
        public event Func<Assembly, string, IntPtr> ResolvingUnmanagedDll;
        public event Action<AssemblyLoadContext> Unloading;

        // Public Method
        public Plugin(bool isCollectible = false)
        {
            plugin = new PluginBase(isCollectible);
            
            // NOTE:
            // To Be Tested
            plugin.Resolving += this.Resolving;
            plugin.ResolvingUnmanagedDll += this.ResolvingUnmanagedDll;
            plugin.Unloading += this.Unloading;
        }

        public Assembly? Load(string path) => plugin?.Load(path);

        public Assembly? Load(Stream stream) => plugin?.LoadFromStream(stream);
        public Assembly? Load(Stream stream, Stream symbols) => plugin?.LoadFromStream(stream, symbols);

        public IEnumerable<T> CreateInstance<T>() => plugin?.CreateInstances<T>() ?? Array.Empty<T>();
        
        public void Unload(bool forceWaiting = false) => plugin?.Unload(forceWaiting);
    }
}