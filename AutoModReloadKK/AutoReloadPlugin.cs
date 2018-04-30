using System;
using System.Collections.Generic;
using BepInEx;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Xml.Linq;
using dnlib.DotNet;

namespace AutoModReloadKK
{
    [BepInPlugin("keelhauled.autoreloadkk", "AutoModReloadKK", "1.0.0")]
    public class AutoReloadPlugin : BaseUnityPlugin
    {
        static string XML_PATH = "/BepInEx/AutoModReload.xml";
        Dictionary<string, AssemblyInfo> classes = new Dictionary<string, AssemblyInfo>();

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightAlt))
            {
                var xml = XElement.Load(Environment.CurrentDirectory + XML_PATH);

                foreach(var item in xml.Element("mods").Elements())
                {
                    var info = new AssemblyInfo(item);
                    if(info.enabled != AssemblyInfo.Enabled.Never)
                    {
                        AssemblyInfo ass;
                        if(classes.TryGetValue(info.dll, out ass))
                        {
                            ass.enabled = info.enabled;
                            ass.target = info.target;
                        }
                        else
                        {
                            classes[info.dll] = info;
                        }
                    }
                }

                string targetFolder = (string)xml.Element("targetfolder");
                if(targetFolder != null)
                {
                    Console.WriteLine(new string('=', 40));
                    foreach(var path in Directory.GetFiles(targetFolder)) InvokeBootstrapWrap(path);
                    Console.WriteLine(new string('=', 40));
                }
            }
        }

        void InvokeBootstrapWrap(string path)
        {
            AssemblyInfo ass;
            if(classes.TryGetValue(Path.GetFileNameWithoutExtension(path), out ass))
            {
                if(ass.enabled == AssemblyInfo.Enabled.Always || !ass.once)
                {
                    if(ass.enabled == AssemblyInfo.Enabled.Once) ass.once = true;
                    LoadDLL(path, ass.target);
                }
            }
        }

        void InvokeBoostrap(string path, string type)
        {
            try
            {
                var f = new FileInfo(path);
                var b = File.ReadAllBytes(f.FullName);
                var ass = Assembly.Load(b);
                var t = ass.GetType(type);
                var m = t.GetMethod("Bootstrap", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                m.Invoke(null, null);
                Console.WriteLine(type);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void LoadDLL(string path, string type)
        {
            var fi = new FileInfo(path);
            if(!fi.Exists)
            {
                Console.WriteLine("File \"{0}\" does not exist.", fi.Name);
                return;
            }

            var b = File.ReadAllBytes(fi.FullName); //Copy to buffer
            var ad = AssemblyDef.Load(b);
            var prevName = ad.Name.ToString();
            prevName = prevName.Substring(0, Math.Min(prevName.Length, 6)); //Arbitrary, take first 6 chars.
            ad.Name = new UTF8String(prevName + DateTime.Now.Ticks.ToString());

            using(var ms = new MemoryStream())
            {
                ad.Write(ms);
                var ass = Assembly.Load(ms.ToArray());
                var t = ass.GetType(type);
                var m = t.GetMethod("Bootstrap", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if(m != null)
                {
                    m.Invoke(null, null);
                    Console.WriteLine(type); 
                }
                else
                {
                    Console.WriteLine("{0} (Bootstrap method not found)", type);
                }
            }
        }

        class AssemblyInfo
        {
            public Enabled enabled;
            public bool once = false;
            public string dll;
            public string target;

            public AssemblyInfo(XElement xElement)
            {
                enabled = (Enabled)Enum.Parse(typeof(Enabled), (string)xElement.Element("enabled"), true);
                dll = (string)xElement.Element("dll");
                target = (string)xElement.Element("target");
            }

            public enum Enabled
            {
                Always,
                Once,
                OncePerScene,
                Never,
            }
        }
    }
}
