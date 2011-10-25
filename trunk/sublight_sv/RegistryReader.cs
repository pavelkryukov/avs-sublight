using System.Linq;
using Microsoft.Win32;

namespace sublight_sv
{
    static class RegistryReader
    {
        private static readonly string[] Extensions = {@".avi", @".mkv", @".mp4"};

        static private string GetNameWithVariable()
        {
            // F**k. Original code is in rev109
            foreach (var value in from extension in Extensions
                                  select Registry.ClassesRoot.OpenSubKey(extension)
                                  into aviEntry where aviEntry != null where aviEntry.GetValue("") != null 
                                  select Registry.ClassesRoot.OpenSubKey(aviEntry.GetValue("").ToString())
                                  into command where command != null select command.OpenSubKey("Shell")
                                  into command where command != null select command.OpenSubKey("Open")
                                  into command where command != null select command.OpenSubKey("Command")
                                  into command where command != null select command.GetValue("")
                                  into value where value != null select value)
            {
                return value.ToString();
            }
            return "";
        }

        static public string GetName()
        {
            var name = GetNameWithVariable();
            return name == "" ? "" : name.Replace("\"%1\"", "");
        }
    }
}
