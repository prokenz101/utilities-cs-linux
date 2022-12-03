namespace utilities_cs_linux {
    public class Force {
        public static FormattableCommand? forced;

        public static string ForceMain(string[] args) {
            string commandName = args[1];

            //* check if command exists
            if (FormattableCommand.FCommandExists(commandName)) {
                //* check if command is already forced
                if (Force.AreAnyForced()) {
                    if (Force.IsSpecificCmdForced(commandName)) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "That command is already forced." }
                        );
                    }
                }

                Force.ForceCommand(commandName);
                return SocketJSON.SendJSON(
                    "notification",
                    new List<object>() { "Success!", "That command was forced." }
                );
            } else {
                return SocketJSON.SendJSON(
                    "notification",
                    new List<object>() { "Soemthing went wrong.", "That command does not exist." }
                );
            }
        }

        public static string? UnforceMain(string[] args) {
            //* check if command is enabled
            if (Force.AreAnyForced()) {
                Force.UnForceCommand();
                return SocketJSON.SendJSON(
                    "notification",
                    new List<object>() { "Success!", "That command was unforced." }
                );
            } else {
                return SocketJSON.SendJSON(
                    "notification",
                    new List<object>() { "Something went wrong.", "That command was never forced." }
                );
            }
        }

        public static void ForceCommand(string cmdName) { forced = FormattableCommand.GetFormattableCommand(cmdName); }

        public static bool AreAnyForced() { return forced != null; }

        public static bool IsSpecificCmdForced(string cmdName) { return forced!.CommandName == cmdName; }

        public static void UnForceCommand() { forced = null; }
    }
}