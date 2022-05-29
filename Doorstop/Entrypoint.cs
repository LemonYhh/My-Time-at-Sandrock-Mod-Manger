using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace Doorstop
{
    public class Entrypoint
    {

        public static void Start()
        {
            string dllPath = Environment.GetEnvironmentVariable("DOORSTOP_INVOKE_DLL_PATH");
            string dllDir = Path.GetDirectoryName(dllPath);
            string modsDir = dllDir + "\\Mods";
            if (dllDir == null || !Directory.Exists(modsDir))
            {
                return;
            }
            LoadMods(modsDir);
        }

        private static void LoadMods(string modsDir)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(modsDir);
            foreach (var item in directoryInfo.GetDirectories())
            {
                string infoPath = item.FullName + "\\Info.json";
                if (File.Exists(infoPath))
                {
                    ReadInfo(infoPath, item.FullName);
                }
            }
        }

        private static void ReadInfo(string infoPath, string dirPath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(infoPath))
                {
                    string infoStr = sr.ReadToEnd();
                    Info info = JsonConvert.DeserializeObject<Info>(infoStr);
                    LoadMod(dirPath, info);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void LoadMod(string dirPath, Info info)
        {
            try
            {
                string dllFilePath = dirPath + "\\" + info.AssemblyName;
                Assembly assembly = Assembly.LoadFrom(dllFilePath);
                var type = assembly.GetType(info.ClassName);
                var method = type.GetMethod(info.MethodName);
                info.DllPath = dirPath;
                method.Invoke(null, new string[] { JObject.FromObject(info).ToString() });
            }
            catch (Exception)
            {
            }
        }
    }
}
