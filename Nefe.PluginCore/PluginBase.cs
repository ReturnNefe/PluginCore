using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Nefe.PluginCore
{
    /// <summary/>
    internal class PluginBase : AssemblyLoadContext
    {
        /// <summary>
        /// 构造一个插件
        /// </summary>
        /// <param name="isCollectible">是否允许插件被卸载</param>
        public PluginBase(bool isCollectible) : base(isCollectible) { }

        /// <summary>
        /// 加载插件
        /// </summary>
        /// <returns>已加载的插件. 加载失败则返回 null</returns>
        public Assembly? Load(string pluginPath)
        {
            using var inStream = new FileStream(pluginPath, FileMode.Open, FileAccess.Read);
            return this.LoadFromStream(inStream);
        }

        /// <summary>
        /// 卸载插件. 注意, 此方法并非立即卸载, 而是将程序集提交到 GC 列表. 卸载完成后, PluginLoader 被回收变为 null
        /// </summary>
        public new void Unload() => base.Unload();
        
        public void Unload(bool forceWaiting)
        {
            this.Unload();
            
            if (forceWaiting)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// 基于已加载的程序集创建类的实例
        /// </summary>
        /// <typeparam name="T">要创建类的实例的类型</typeparam>
        public IEnumerable<T> CreateInstances<T>()
        {
            foreach (var assembly in this.Assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(T).IsAssignableFrom(type))
                    {
                        if (Activator.CreateInstance(type) is T result)
                        {
                            yield return result;
                        }
                    }
                }
            }
        }
    }
}
