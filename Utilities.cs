using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;

namespace utilities_cs_linux {
    class Program {
        public static string UtilitiesFolderPath = Path.Combine("/home", $"{System.Environment.UserName}/utilities-cs");
        public static string IconPath = Path.Combine(UtilitiesFolderPath, "Assets/UtilitiesIcon.png");
        public static string PythonSockets = Path.Combine(UtilitiesFolderPath, "PythonDependencies/sockets.py");
        public static string SettingsJSONPath = Path.Combine(UtilitiesFolderPath, "settings.json");
        public static Settings CurrentSettings = Settings.GetSettings();
        public static Uri LocalHost = new Uri("ws://127.0.0.1:8005");
        public static TcpListener Server = new TcpListener(IPAddress.Parse("127.0.0.1"), 1234);
        public static bool ContinueExecution = true;

        /// <summary>
        /// The entry-point of the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        static void Main(string[] args) {
            RegisterCommands.RegisterAllRCommands();
            RegisterCommands.RegisterAllFCommands();
            if (Settings.SettingsJsonExists()) { Settings.CreateJson(); }

            Process.Start(new ProcessStartInfo("python3", PythonSockets) { CreateNoWindow = true });
            Task.WaitAll(socketListener());
        }

        static async Task socketListener(string? streamRequest = null) {
            //* creating server on localhost
            Server.Start();

            var client = Server.AcceptSocket(); //! This represents python.

            byte[] buffer = new byte[1024];
            int i; //* this will be the length of the byte array read from the stream
            NetworkStream stream = new NetworkStream(client);

            while (ContinueExecution) {
                string data = "";
                i = await stream.ReadAsync(buffer, 0, buffer.Length);
                // System.Console.WriteLine("read something");

                data = Encoding.UTF8.GetString(buffer, 0, i);
                buffer = new byte[1024]; //* flushing buffer

                if (data != "") {
                    Console.WriteLine($"C#: Received {data}");

                    string? result = InvokeCommand(data.Split(" ")).Result;
                    if (result != null) {
                        byte[] msg = Encoding.UTF8.GetBytes(result);
                        // if (msg.Length > 1024) { msg = msg[0..1023]; }

                        await stream.WriteAsync(msg, 0, msg.Length);
                    }
                }
            }

            Server.Stop();
        }

        static async Task<string?> InvokeCommand(string[] input) {
            return await Task.Run(() => Command.ExecuteCommand(input, true, true));
        }
    }
}