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
                return SocketJSON.SendJSON(
                    "notification", new List<object>() { "Something went wrong.", "Invaid category." }
                );
            }
        }

        static string? returnCategory(string[] args, string category, bool copy, bool notif) {
            bool shouldShowNames = !Program.CurrentSettings.AllCommandHideNames;

            List<FormattableCommand> fancy =
                FormattableCommand.GetMethodsSupportedByAll("fancy");

            List<FormattableCommand> encodings =
                FormattableCommand.GetMethodsSupportedByAll("encodings");

            List<string> converted = new();
            Action<List<FormattableCommand>> allCommandRun =
                (List<FormattableCommand> commandList) => {
                    foreach (FormattableCommand command in commandList) {
                        try {
                            string? output = command.Function!.Invoke(args, false, false);
                            if (output != null) {
                                if (shouldShowNames) {
                                    converted.Add($"{command.CommandName}: {output}");
                                } else { converted.Add(output!); }
                            }
                        } catch { }
                    }
                };

            switch (category) {
                case "everything":
                    allCommandRun(fancy.Concat(encodings).ToList());
                    break;

                case "encodings":
                    allCommandRun(encodings);
                    break;

                case "fancy":
                    allCommandRun(fancy);
                    break;

                default:
                    return null;

            }


            return string.Join("\n", converted);
        }
    }
}