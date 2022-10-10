namespace utilities_cs_linux_setup {
    class Program {
        public static string UtilitiesFolderPath = Path.Combine(Environment.GetEnvironmentVariable("HOME")!, "utilities-cs");
        public static string IconPath = Path.Combine(UtilitiesFolderPath, "Assets/UtilitiesIcon.ico");
        public static string IconURL = "https://raw.githubusercontent.com/prokenz101/utilities-cs-linux/setup/Assets/UtilitiesIcon.ico";
        static void Main(string[] args) {
        }
    }
}