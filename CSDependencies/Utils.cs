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
    }

    public class SocketJSON {
        public string? Request { get; set; }
        public List<object>? Arguments { get; set; }

        public static string SendJSON(string request, List<object> arguments) {
            return System.Text.Json.JsonSerializer.Serialize<SocketJSON>(
                new SocketJSON() { Request = request, Arguments = arguments }
            );
        }
    }
}