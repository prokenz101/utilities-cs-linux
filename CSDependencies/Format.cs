namespace utilities_cs_linux {
    public static class Format {
        public static string FormatMain(string[] args) {
            string indexTest = Utils.IndexTest(args);
            if (indexTest != "false") { return indexTest; }

            string text = string.Join(" ", args[1..]);

            Dictionary<string, string> formatDict = new();
            System.Text.RegularExpressions.Regex re =
                new System.Text.RegularExpressions.Regex(@"{(?<command>[^}]+)}");

            System.Text.RegularExpressions.MatchCollection matches = re.Matches(text);

            foreach (System.Text.RegularExpressions.Match? i in matches) {
                if (i != null) {
                    System.Text.RegularExpressions.GroupCollection groups = i.Groups;
                    System.Text.RegularExpressions.Group mainGroup = groups["command"];

                    string cmd = mainGroup.ToString();
                    string[] splitcommand = cmd.Split(" ");

                    string? output = FormattableCommand.FindAndInvoke(splitcommand, false, false);

                    // splitcommand.ToList<string>().ForEach(x => Console.WriteLine());
                    // Console.WriteLine(splitcommand.Length);

                    if (output == null) {
                        output = "errored";
                        formatDict[cmd] = output;
                    } else { formatDict[cmd] = output; }
                }
            }

            return SocketJSON.SendJSON(
                "regular", new List<object>() { replaceKeyInString(formatDict, text), "Success!", "Message copied to clipboard." }
            );
        }

        static string replaceKeyInString(Dictionary<string, string> dictionary, string inputString) {
            var regex = new System.Text.RegularExpressions.Regex("{(.*?)}");
            var matches = regex.Matches(inputString);
            foreach (System.Text.RegularExpressions.Match? match in matches) {
                if (match != null) {
                    var valueWithoutBrackets = match.Groups[1].Value;
                    var valueWithBrackets = match.Value;

                    if (dictionary.ContainsKey(valueWithoutBrackets))
                        inputString = inputString.Replace(valueWithBrackets, dictionary[valueWithoutBrackets]);
                }
            }

            return inputString;
        }
    }
}