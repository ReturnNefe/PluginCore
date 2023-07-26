namespace Nefe.PluginCore.Unit.Test.Plugin1
{
    public class Plugin : Interface.PluginBase
    {
        public string MakeText() => "[MD5-1.2.0] " + RetChen.Encryption.MD5Encryption.Encrypt("Hello, PluginCore!");
    }
}
