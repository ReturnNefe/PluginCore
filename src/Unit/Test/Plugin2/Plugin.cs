namespace Nefe.PluginCore.Unit.Test.Plugin2
{
    public class Plugin : Interface.PluginBase
    {
        public string MakeText() => "[MD5-1.2.2] " + RetChen.Encryption.MD5Encryptor.Encrypt("Hello, PluginCore!", ":");
    }
}
