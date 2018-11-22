using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using BepInEx;
using static BepInEx.Logger;
using BepInEx.Logging;

namespace MakerBridgePatcher
{
    public static class Patcher
    {
        public static IEnumerable<string> TargetDLLs { get; } = new[]{ "Assembly-CSharp.dll" };

        public static void Patch(AssemblyDefinition assembly)
        {
            var type = assembly.MainModule.Types.First(x => x.Name == "ChaControl");
            
            using(var ass = AssemblyDefinition.ReadAssembly(Path.Combine(Paths.ManagedPath, "Assembly-CSharp-firstpass.dll")))
            {
                var typedef = ass.MainModule.Types.First(x => x.Name == "MessagePackObjectAttribute");
                var attrType = Type.GetType(typedef.FullName + ", " + typedef.Module.Assembly.FullName);
                var attrConstructor = assembly.MainModule.ImportReference(attrType.GetConstructor(new Type[]{ typeof(bool) }));
                type.CustomAttributes.Add(new CustomAttribute(attrConstructor));
            }
        }
    }
}