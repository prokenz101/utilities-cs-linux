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

            //! Cloning the main repository
            var cloneMainProcess = new System.Diagnostics.Process();
            cloneMainProcess.StartInfo.FileName = "git";
            cloneMainProcess.StartInfo.Arguments = "clone https://github.com/prokenz101/utilities-cs-linux.git";
            cloneMainProcess.StartInfo.CreateNoWindow = true;
            cloneMainProcess.StartInfo.WorkingDirectory = UtilitiesFolderPath;
            cloneMainProcess.Start();
            cloneMainProcess.WaitForExit();
            Console.WriteLine("\nCloned the repository.\n");

            //! Building the cloned project into an executable
            var buildClonedProject = new System.Diagnostics.Process();
            buildClonedProject.StartInfo.FileName = "dotnet";
            buildClonedProject.StartInfo.Arguments = "publish -c Release --self-contained false";
            buildClonedProject.StartInfo.CreateNoWindow = true;
            buildClonedProject.StartInfo.WorkingDirectory = Path.Combine(UtilitiesFolderPath, "utilities-cs-linux");
            buildClonedProject.Start();
            buildClonedProject.WaitForExit();
            Console.WriteLine("\nBuilt the project.");

            //! Move the obtained executable to the root utilities-cs folder
            File.Move(
                Path.Combine(UtilitiesFolderPath, "utilities-cs-linux/bin/Release/net6.0/linux-x64/publish/utilities-cs-linux"),
                Path.Combine(UtilitiesFolderPath, "utilities-cs")
            ); Console.WriteLine("\nMoved the obtained executable to root folder.");

            //! Delete the cloned folder
            Directory.Delete(Path.Combine(UtilitiesFolderPath, "utilities-cs-linux"), recursive: true);
            Console.WriteLine("\nDeleted the cloned folder.");
        }
    }
}