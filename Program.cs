namespace utilities_cs_linux_setup {
    class Program {
        public static string Home = Path.Combine("/home", System.Environment.UserName);
        public static string UtilitiesFolderPath = Path.Combine(Home, "utilities-cs");
        public static string IconPath = Path.Combine(UtilitiesFolderPath, "Assets/UtilitiesIcon.png");
        public static string IconURL = "https://raw.githubusercontent.com/prokenz101/utilities-cs-linux/setup/Assets/UtilitiesIcon.ico";
        public static string MainExecutablePath = Path.Combine(UtilitiesFolderPath, "utilities-cs");

        static void Main(string[] args) {
            //* Checking if program is run as sudo
            if (RootChecker.IsRoot()) {
                Console.WriteLine(@"This setup program cannot be run with root permissions.
Please try again without root permissions (sudo).");
                return;
            }

            if (!Directory.Exists(Path.Combine(UtilitiesFolderPath, "Assets"))) {
                Directory.CreateDirectory(Path.Combine(UtilitiesFolderPath, "Assets")); //* Creating folders
            }

            //! Getting the icon and putting it in the Assets folder
            var httpClient = new HttpClient();
            var iconResponse = httpClient.GetAsync(new Uri(IconURL)).Result;
            byte[] iconResponseOutput = iconResponse.Content.ReadAsByteArrayAsync().Result;
            File.WriteAllBytes(IconPath, iconResponseOutput);
            Console.WriteLine("Got the icon and wrote it to a file in Assets.\n");

            //! Getting the executable and putting it in the main folder

            //? Cloning the main repository
            var cloneMainProcess = new System.Diagnostics.Process();
            cloneMainProcess.StartInfo.FileName = "git";
            cloneMainProcess.StartInfo.Arguments = "clone https://github.com/prokenz101/utilities-cs-linux.git";
            cloneMainProcess.StartInfo.CreateNoWindow = true;
            cloneMainProcess.StartInfo.WorkingDirectory = UtilitiesFolderPath;
            cloneMainProcess.Start();
            cloneMainProcess.WaitForExit();
            Console.WriteLine("\nCloned the repository.\n");

            //? Building the cloned project into an executable
            var buildClonedProject = new System.Diagnostics.Process();
            buildClonedProject.StartInfo.FileName = "dotnet";
            buildClonedProject.StartInfo.Arguments = "publish -c Release --self-contained false";
            buildClonedProject.StartInfo.CreateNoWindow = true;
            buildClonedProject.StartInfo.WorkingDirectory = Path.Combine(UtilitiesFolderPath, "utilities-cs-linux");
            buildClonedProject.Start();
            buildClonedProject.WaitForExit();
            Console.WriteLine("\nBuilt the project.");

            //? Move the obtained executable to the root utilities-cs folder
            File.Move(
                Path.Combine(
                    UtilitiesFolderPath,
                    "utilities-cs-linux/bin/Release/net6.0/linux-x64/publish/utilities-cs-linux"
                ),
                MainExecutablePath,
                overwrite: true
            ); Console.WriteLine("\nMoved the obtained executable to root folder.");

            //? Delete the cloned folder
            Directory.Delete(Path.Combine(UtilitiesFolderPath, "utilities-cs-linux"), recursive: true);
            Console.WriteLine("\nDeleted the cloned folder.\n");

            //! Python integrations

            //? Installing required pip packages
            var pipPackagesInstall = new System.Diagnostics.Process();
            pipPackagesInstall.StartInfo.FileName = "pip3";
            pipPackagesInstall.StartInfo.Arguments = "install pyautogui pyperclip pynput";
            pipPackagesInstall.StartInfo.CreateNoWindow = true;
            pipPackagesInstall.StartInfo.WorkingDirectory = Home;
            pipPackagesInstall.Start();
            pipPackagesInstall.WaitForExit();
            Console.WriteLine("\nInstalled required pip packages (python)");

            //TODO: Implement python files stuff

            //! Creating the .desktop file maker

            //? Creating C# project
            var createDesktopFileMaker = new System.Diagnostics.Process();
            createDesktopFileMaker.StartInfo.FileName = "dotnet";
            createDesktopFileMaker.StartInfo.Arguments = "new console -n \"desktopfilemaker\"";
            createDesktopFileMaker.StartInfo.CreateNoWindow = true;
            createDesktopFileMaker.StartInfo.WorkingDirectory = UtilitiesFolderPath;
            createDesktopFileMaker.Start();
            createDesktopFileMaker.WaitForExit();
            Console.WriteLine("\nCreated the .desktop file maker.");

            //? Edit code of the Program.cs file in the project
            File.WriteAllText(
                Path.Combine(UtilitiesFolderPath, "desktopfilemaker/Program.cs"),
                $@"namespace u_cs_desktop_file_maker {{
    class Program {{
        static void Main(string[] args) {{
            if (!(RootChecker.IsRoot())) {{
                Console.WriteLine(@""This program has to be run with root permissions.
Please try again with """"sudo"""" at the beginning of whatever you used ran to run this application."");
                return;
            }}

            var getXsel = new System.Diagnostics.Process();
            getXsel.StartInfo.FileName = ""sudo"";
            getXsel.StartInfo.Arguments = ""apt install xsel"";
            getXsel.StartInfo.CreateNoWindow = true;
            getXsel.StartInfo.WorkingDirectory = ""/"";
            getXsel.Start();
            getXsel.WaitForExit();
            Console.WriteLine(""\nGot \""xsel\"", a dependency for the paste() function in pyperclip."");

            File.WriteAllText(
                ""/usr/share/applications/utilities-cs.desktop"",
                @""[Desktop Entry]
Encoding=UTF-8
Version=1.0
Type=Application
Terminal=false
Exec={MainExecutablePath}
Name=utilities-cs
Icon={IconPath}""
            );

            Console.WriteLine(""\nCreated the .desktop file."");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(""The setup of utilities-cs is now complete."");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(""You can close this window/tab and input \""y\"" in the setup program."");
            Console.ResetColor();
        }}
    }}

    public static class RootChecker {{
        [System.Runtime.InteropServices.DllImport(""libc"")]
        public static extern uint getuid();

        public static bool IsRoot() {{ return getuid() == 0; }}
    }}
}}"
            );
            Console.WriteLine("\nEdited the Program.cs file of the .desktop file maker.");

            //? Edit properties of the .csproj file
            File.WriteAllText(
                Path.Combine(UtilitiesFolderPath, "desktopfilemaker/desktopfilemaker.csproj"),
                @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <RootNamespace>u_cs_desktop_file_maker</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <PublishSingleFile>true</PublishSingleFile>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>"
            );
            Console.WriteLine("\nEdited the .csproj file of the .desktop file maker.\n");

            //? Build project
            var buildDesktopFileMaker = new System.Diagnostics.Process();
            createDesktopFileMaker.StartInfo.FileName = "dotnet";
            createDesktopFileMaker.StartInfo.Arguments = "publish -c Release --self-contained false";
            createDesktopFileMaker.StartInfo.CreateNoWindow = true;
            createDesktopFileMaker.StartInfo.WorkingDirectory = Path.Combine(
                UtilitiesFolderPath, "desktopfilemaker"
            ); createDesktopFileMaker.Start();
            createDesktopFileMaker.WaitForExit();
            Console.WriteLine("\nCreated the .desktop file maker.");

            //? Ask user to run .desktop file maker executable with sudo permissions
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\n\nFINAL SETUP:");
            Console.ResetColor();
            Console.WriteLine($@"
Please run the following command in a new terminal/terminal tab:

ONCE COMPLETED, PLEASE COME BACK TO THIS TERMINAL/TERMINAL TAB");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
                $"\ncd {Path.Combine(UtilitiesFolderPath, "desktopfilemaker/bin/Release/net6.0/linux-x64/publish")} && sudo ./desktopfilemaker\n"
            );

            Console.ResetColor();

            Console.WriteLine(@"This will install a package called xsel which is required for pyperclip (python).
It will also create a .desktop file which will allow you to open utilities-cs from your apps list.");

            Thread.Sleep(9000);

            Console.WriteLine(@"
-----------------------------------------

Have you ran the command? (y/n)

PLEASE READ THIS CAREFULLY
(y) -> Will delete the .desktop file maker and its project files (Completes setup process)
(n) -> Why?! (please run it)

Please enter: y/n");

            string? inputNullable = Console.ReadLine();
            string input = inputNullable == null ? "" : inputNullable;

            if (input == "y") {
                Console.WriteLine("\nSetup Complete.");
            } else if (input == "n") {
                Console.WriteLine("\nIf you did not run the command, then utilities-cs will not work properly.");
                Console.WriteLine("Please run this setup file again if you would like to fix this.");
                Console.WriteLine("Deleted desktop file maker.");
            } else {
                Console.WriteLine("\nInvalid input.");
                Console.WriteLine("Deleting desktop file maker anyway.");
                Console.WriteLine(
                    @"If you did not run the command shown above, then your utilities-cs installation will be broken and will not work properly.
Please re-run the setup file and run that command if you would like to fix this.

If you DID run the command above, then your utilities-cs installation is complete."
                );
            }

            Directory.Delete(Path.Combine(UtilitiesFolderPath, "desktopfilemaker"), recursive: true);
        }
    }

    public static class RootChecker {
        [System.Runtime.InteropServices.DllImport("libc")]
        public static extern uint getuid();

        public static bool IsRoot() { return getuid() == 0; }
    }
}