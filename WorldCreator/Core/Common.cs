using System;
using System.IO;

namespace WorldCreator.Core;

public static class Common
{
    public static string NativeReposPath() => Path.Combine(AppContext.BaseDirectory , "repos") ;
    public static string NativeRepoPath(string name) =>  Path.Combine(NativeReposPath(),name);
    public static string NativeConfigPath(string name) => Path.Combine(NativeRepoPath(name),"config.json");
}