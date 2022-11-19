namespace utilities_cs_linux {
    public class Utils {

        /// <summary>
        /// Formats every character of a string to a value in a dictionary.
        /// </summary>
        /// <param name="input">The input string to be formatted.</param>
        /// <param name="dict">The dictionary to use for the keys and values.</param>
        /// <returns>A formatted string.</returns>
        public static string TextFormatter(string input, Dictionary<string, string> dict) {
            List<string> converted = new();

            foreach (char i in input) {
                if (dict.ContainsKey(i.ToString())) {
                    converted.Add(dict[i.ToString()]);
                } else {
                    converted.Add(i.ToString());
                }
            }

            return string.Join("", converted);
        }

        /// <summary>
        /// Returns true if IndexTest failed, i.e there were no arguments other than the command.
        /// Returns false if the program ran successfully with all arguments.
        /// </summary>
        /// <param name="args">All arguments passed when pressing main Ctrl+F8 function.</param>
        /// <param name="argscount">The index that indextest will check to see if it exists.</param>
        /// <returns>A bool that will be false if IndexTest failed, and true if it didn't.</returns>
        public static string IndexTest(string[] args, int argscount = 1) {
            try {
                string test = args[argscount];
                return "false";
            } catch (IndexOutOfRangeException) {
                return SocketJSON.SendJSON(
                    "notification", new List<object>() { "Something went wrong.", "Check your parameters." }
                );
            }
        }

        /// <summary>
        /// Checks if the command is willing to copy and send a notification.
        /// </summary>
        /// <param name="copy">Bool which denotes whether the command is willing to copy something to the clipboard.</param>
        /// <param name="notif">Bool which denotes whether the command is willing to send a notification.</param>
        /// <param name="args">The arguments for the SocketJSON</param>
        /// <returns>The SocketJSON based on what the command's requirements are.</returns>
        public static string? CopyNotifCheck(bool copy, bool notif, List<object> args) {
            return copy && notif ? SocketJSON.SendJSON("regular", args) : args[0].ToString();
        }

        /// <summary>
        /// Replaces any characters in "chars" with their respective characters in "replacementChars".
        /// </summary>
        /// <param name="text">The text to be replaced.</param>
        /// <param name="chars">The original characters to be replaced. (Split by " ")</param>
        /// <param name="replacementChars">The characters to replace the original characters. (Split by " ")</param>
        /// <returns></returns>
        public static string BulkReplace(string text, string chars, string replacementChars) {
            List<string> charsList = chars.Split(" ").ToList<string>();
            List<string> replacementCharsList = replacementChars.Split(" ").ToList<string>();
            string result = text;

            foreach (string i in charsList) {
                result = result.Replace(i, replacementCharsList[charsList.IndexOf(i)]);
            }

            return result;
        }
    }

    public class SocketJSON {
        public string? Request { get; set; }
        public List<object>? Arguments { get; set; }

        /// <summary>
        /// Returns a JSON string which follows a format that Python understands.
        /// </summary>
        /// <param name="request">The type of request, "regular" or "notification".</param>
        /// <param name="arguments">Arguments for the request that python can read.</param>
        /// <returns>A JSON string based on the request and parameters.</returns>
        public static string SendJSON(string request, List<object> arguments) {
            return System.Text.Json.JsonSerializer.Serialize<SocketJSON>(
                new SocketJSON() { Request = request, Arguments = arguments }
            );
        }
    }
}