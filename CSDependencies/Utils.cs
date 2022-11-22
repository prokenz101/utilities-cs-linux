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
        /// A method that returns a rounded-version of a number if it is close enough to a whole number.
        /// 5.999999998 -> 6, 5.5 = 5.5.
        /// </summary>
        /// <param name="num">The number to be rounded off to, or not.</param>
        /// <returns>
        /// A double based on if the number was rounded or not.
        /// If the number was not rounded off, it returns the same number.s
        /// </returns>
        public static double RoundIfNumberIsNearEnough(double num) {
            System.Text.RegularExpressions.Regex re = new(@"-?\d+\.(?:9){6,}");

            if (re.Matches(num.ToString()).Count == 1) {
                return Math.Round(num);
            } else {
                return num;
            }
        }

        /// <summary>
        /// Checks if a string has only a certain set of characters.
        /// </summary>
        /// <param name="allowableChar">Set of characters that are allowed in the string.</param>
        /// <param name="text">The text that is being checked.</param>
        /// <returns>A bool that will be true if the text matches the format and false if it doesn't.</returns>
        public static bool FormatValid(string allowableChar, string text) {
            foreach (char c in text) {
                if (!allowableChar.Contains(c.ToString()))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Uses Regex to search through a string using an expression.
        /// Returns a List of DIctionaries where the Match is the key and a GroupCollection of that match is the value.
        /// </summary>
        /// <param name="input">The string that is to be searched.</param>
        /// <param name="expression">The regex expression used to search through the input.</param>
        /// <param name="useIsMatch">
        /// If true, the function will use re.IsMatch() instead of re.Matches().Count,
        /// this should only be used for expressions which are designed to have only one match.
        /// </param>
        /// <returns>A dictionary of all the matches which point to their groups.</returns>
        public static Dictionary<System.Text.RegularExpressions.Match, System.Text.RegularExpressions.GroupCollection>? RegexFind(
            string input,
            string expression,
            bool useIsMatch = false,
            Action? ifNotMatch = null
        ) {

            List<Dictionary<System.Text.RegularExpressions.Match, System.Text.RegularExpressions.GroupCollection>> matchesAndGroups = new();
            System.Text.RegularExpressions.Regex re = new(expression);

            Action matched = () => {
                foreach (System.Text.RegularExpressions.Match? match in re.Matches(input)) {
                    if (match != null) {
                        Dictionary<System.Text.RegularExpressions.Match, System.Text.RegularExpressions.GroupCollection> matchToGroups =
                            new() { { match, match.Groups } };
                        matchesAndGroups.Add(matchToGroups);
                    }
                }
            };

            if (!useIsMatch) {
                if (re.Matches(input).Count >= 1) { matched.Invoke(); } else { ifNotMatch?.Invoke(); return null; }
            } else if (useIsMatch) {
                if (re.IsMatch(input)) { matched.Invoke(); } else { ifNotMatch?.Invoke(); return null; }
            }

            return matchesAndGroups[0];
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