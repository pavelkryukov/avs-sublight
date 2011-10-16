using Microsoft.Win32;

namespace sublight_sv
{
    static class RegistryReader
    {
        private static readonly string[] Extensions = {@".avi", @".mkv", @".mp4"};

        static private string GetNameWithVariable()
        {
            foreach (var extension in Extensions)
            {
                RegistryKey aviEntry = Registry.ClassesRoot.OpenSubKey(extension);
                if (aviEntry == null)
                {
                    continue;
                }
                if (aviEntry.GetValue("") == null)
                {
                    continue;
                }
                var command = Registry.ClassesRoot.OpenSubKey(aviEntry.GetValue("").ToString());
                if (command == null)
                {
                    continue;
                }
                command = command.OpenSubKey("Shell");
                if (command == null)
                {
                    continue;
                }
                command = command.OpenSubKey("Open");
                if (command == null)
                {
                    continue;
                }
                command = command.OpenSubKey("Command");
                if (command == null)
                {
                    continue;
                }
                var value = command.GetValue("");
                if (value == null)
                {
                    continue;
                }
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
