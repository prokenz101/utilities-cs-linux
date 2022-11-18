using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;

namespace utilities_cs_linux {
    class Program {
        public static string UtilitiesFolderPath = Path.Combine("/home", $"{System.Environment.UserName}/utilities-cs");
        public static string IconPath = Path.Combine(UtilitiesFolderPath, "Assets/UtilitiesIcon.png");
        public static string PythonSockets = "Dependencies/sockets.py"; //TODO Path.Combine(UtilitiesFolderPath, "sockets.py");
        public static string SettingsJSONPath = "settings.json";
        public static SettingsJSON CurrentSettings = SettingsModification.GetSettings();
        public static Uri LocalHost = new Uri("ws://127.0.0.1:1234");
        public static Dictionary<string, Func<string, string>> CommandsDict = new() {
            {"cursive", Commands.Cursive},
            {"doublestruck", Commands.Doublestruck},
            {"creepy", Commands.Creepy},
            {"factorial", Commands.Factorial}
        };

        /// <summary>
        /// The entry-point of the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        static void Main(string[] args) {
            var settings = SettingsModification.GetSettings();
            var pythonProcess = new ProcessStartInfo(
                "python3",
                PythonSockets
            ) { CreateNoWindow = true };
            Process.Start(pythonProcess);
            Console.WriteLine("Python started");

            Task.WaitAll(socketListener());
        }

        static async Task socketListener(string? streamRequest = null) {
            //* creating server on localhost
            int port = 1234;
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            var server = new TcpListener(ipAddress, port);
            server.Start();

            //* creating client object which represents python
            var client = server.AcceptSocket();
            Console.WriteLine("C# connected");

            byte[] buffer = new byte[1024]; //* buffer
            int i; //* this will be the length of the byte array read from the stream

            using (NetworkStream stream = new NetworkStream(client)) {
                while (true) {
                    string data = "";
                    i = await stream.ReadAsync(buffer, 0, buffer.Length);

                    data = Encoding.UTF8.GetString(buffer, 0, i);
                    buffer = new byte[1024]; //* flushing buffer

                    if (data != "") {
                        Console.WriteLine($"C#: Received {data}");

                        string result = InvokeCommand(data.Split(" ")).Result;
                        byte[] msg = Encoding.UTF8.GetBytes(result);
                        if (msg.Length > 1024) { msg = msg[0..1023]; }

                        Console.WriteLine("Completed Command");
                        Console.WriteLine($"Result: {result}");

                        await stream.WriteAsync(msg, 0, msg.Length);
                    }
                }
            }
        }

        static async Task<string> InvokeCommand(string[] input) {
            string command = input[0];
            if (CommandsDict.ContainsKey(command)) {
                var commandFunc = CommandsDict[command];
                return await Task.Run(() => commandFunc.Invoke(string.Join(" ", input[1..])));
            }

            return "Error";
        }
    }

    public class SocketJSON {
        public SocketType Type { get; set; }
        public object[]? ArgumentList { get; set; }

        public enum SocketType {
            Command,
            Notification,
            Copy
        }
    }
}
