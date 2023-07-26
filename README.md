# Nefe.PluginCore

A lite plugin framework on .NET(.NET Core), which can be used to build plug-in programs.

## Requirements

.NET 7
.NET 6
.NET Core 3.1

## QuickStart

You need create at least 3 projects.

```shell
dotnet new classlib -o MyApp.Interface
dotnet new classlib -o MyPlugin
dotnet new console -o MyApp
```

#### MyApp.Interface Project

Define the interface of the plugin:

_IPlugin.cs_
```csharp
public interface IPlugin
{
    public string MakeText();
}
```

#### MyPlugin Project

Write code for the first plugin:

_MyPlugin.cs_
```csharp
public class MyPlugin : IPlugin
{
    public string MakeText() => "Hello, I'm MyPlugin!";
}
```

_MyPlugin.csproj_
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!--
      Add option here.
      See https://learn.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
    -->
    <EnableDynamicLoading>true</EnableDynamicLoading>
    
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../MyApp.Interface/MyApp.Interface.csproj">
      
      <!-- And here. -->
      <Private>false</Private>
      <ExcludeAssets>runtime</ExcludeAssets>
      
    </ProjectReference>
  </ItemGroup>
</Project>
```

Build it:

```shell
dotnet build
```

Then, put the ``myplugin.dll`` into the base directory of the main program.

#### MyApp Project

```shell
dotnet add package Nefe.PluginCore
```

_Program.cs_
```csharp
using Nefe.PluginCore;

var plugin = new Plugin(Path.Combine(AppContext.BaseDirectory, "myplugin.dll"));
if (plugin.LoadFromAssemblyPath() is Assembly assembly)
    foreach (var instance in plugin.CreateInstancesFromAssembly<IPlugin>(assembly))
        Console.WriteLine(instance.MakeText());
```

Build & Run it:

```shell
dotnet run
```

If all goes well, you'll see:

```text
Hello, I'm MyPlugin!
```