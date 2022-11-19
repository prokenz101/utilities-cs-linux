namespace utilities_cs_linux {
    public class All {
        public static string? AllMain(string[] args, bool copy, bool notif) {
            string indexTest = Utils.IndexTest(args);
            if (indexTest != "false") { return indexTest; }

            string category = args[1];
            string? all = returnCategory(args[1..], category, copy, notif);
            if (all != null) {
                return Utils.CopyNotifCheck(
                    copy, notif, new List<object>() { all, "Success!", "Text copied to clipboard." }
                );
            } else {
                return SocketJSON.SendJSON("notification", new List<object>() {  });
            }
        }

        static string? returnCategory(string[] args, string category, bool copy, bool notif) {
            bool shouldShowNames = Program.CurrentSettings.AllCommandHideNames;

            var buildCommandDictionary = (List<FormattableCommand> commands) => (
                    from command in commands
                    select new Tuple<string, Func<string[], bool, bool, string>>(
                        command.CommandName!,
                        command.Function!
                )
            ).ToDictionary(t => t.Item1, t => t.Item2);

            Dictionary<string, Func<string[], bool, bool, string?>> fancy =
                buildCommandDictionary(FormattableCommand.GetMethodsSupportedByAll("fancy"));

            Dictionary<string, Func<string[], bool, bool, string?>> encodings =
                buildCommandDictionary(FormattableCommand.GetMethodsSupportedByAll("encodings"));

            List<string> converted = new();
            Action<Dictionary<string, Func<string[], bool, bool, string?>>> allCommandRun =
                (Dictionary<string, Func<string[], bool, bool, string?>> dict) => {
                    foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> kvp in dict) {
                        try {
                            string? output = kvp.Value.Invoke(args, false, false);
                            if (output != null) {
                                if (!shouldShowNames) {
                                    converted.Add($"{kvp.Key}: {output}");
                                } else { converted.Add(output!); }
                            }
                        } catch { }
                    }
                };

            switch (category) {
                case "everything":
                    allCommandRun(
                        fancy.Concat(encodings).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    );
                    break;

                case "encodings":
                    allCommandRun(encodings);
                    break;

                case "fancy":
                    foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> kvp in fancy) {
                        try {
                            string? output = kvp.Value.Invoke(args, false, false);
                            if (output != null) {
                                if (!shouldShowNames) {
                                    converted.Add($@"{kvp.Key}: {output}");
                                } else { converted.Add(output); }
                            }
                        } catch { }
                    }
                    break;

                default:
                    return null;

            }


            return string.Join("\n", converted);
        }
    }
}