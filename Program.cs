namespace utilities_cs_linux_setup {
    class Program {
        public static string UtilitiesFolderPath = Path.Combine(Environment.GetEnvironmentVariable("HOME")!, "utilities-cs");
        public static string IconPath = Path.Combine(UtilitiesFolderPath, "Assets/UtilitiesIcon.ico");
        public static string IconURL = "https://raw.githubusercontent.com/prokenz101/utilities-cs-linux/setup/Assets/UtilitiesIcon.ico";
        static void Main(string[] args) {
            Directory.CreateDirectory(Path.Combine(UtilitiesFolderPath, "Assets")); //* Creating folders

            //! Getting the icon and putting it in the Assets folder
            var httpClient = new HttpClient();
            var iconResponse = httpClient.GetAsync(new Uri(IconURL)).Result;
            byte[] iconResponseOutput = iconResponse.Content.ReadAsByteArrayAsync().Result;
            File.WriteAllBytes(IconPath, iconResponseOutput);
            Console.WriteLine("Got the icon and wrote it to a file in Assets.\n");
        }
    }
}