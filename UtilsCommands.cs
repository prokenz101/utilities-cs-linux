namespace utilities_cs_linux {
    /// <summary>
    /// The base class of all command-classes for all commands in utilities-cs.
    /// </summary>

    public class Command {
        /// <summary>
        /// The primary name of the command.
        /// </summary>
        public string? CommandName { get; set; }

        /// <summary>
        /// A command's aliases.
        /// </summary>
        public string[]? Aliases { get; set; }

        /// <summary>
        /// A dictionary of command names to methods (For FormattableCommands).
        /// </summary>
        public static Dictionary<string, Func<string[], bool, bool, string?>> FCommands = new();

        /// <summary>
        /// A dictionary of command names to methods (For RegularCommands).
        /// </summary>
        public static Dictionary<string, Func<string[], string>> RCommands = new();
        /// <summary>
        /// Executes a command in either the RCommands dictionary or the FCommands dictionary.
        /// </summary>
        /// <param name="cmd">The name of the command to be excuted.</param>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        /// <param name="copy">Controls whether the function is willing to copy text to the clipboard.</param>
        /// <param name="notif">Controls whether the function is willing to send a notification.</param>
        /// <returns>A string of the output of the command. This can also be null.</returns>
        public static string? ExecuteCommand(string[] args, bool copy = true, bool notif = true) {
            string cmd = args[0].ToLower();

            if (FCommands.ContainsKey(cmd)) {
                string? output = FCommands[cmd].Invoke(args, copy, notif);
                return output != null ? output : null;
            } else if (RCommands.ContainsKey(cmd)) {
                string? output = RCommands[cmd].Invoke(args);
                return output != null ? output : null;
                // } else if (Force.AreAnyForced()) {
                //     args = Enumerable.Concat(new string[] { "cmd" }, args).ToArray<string>();
                //     string? output = Force.forced!.Function!.Invoke(args, copy, notif);
                //     if (output != null) { return output; } else { return null; }
            } else {
                return SocketJSON.SendJSON(
                    "notification", new List<object>() { "Command not found.", "Try again." }
                );
            }
        }

        /// <summary>
        /// A simple method that checks if a certain command exists.
        /// </summary>
        /// <param name="cmd">The name of the command</param>
        /// <returns>True or False based on if the command exists, or not.</returns>
        public static bool Exists(string cmd) {
            if (FCommands.ContainsKey(cmd)) {
                return true;
            } else if (RCommands.ContainsKey(cmd)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Gets the Method of a Formattable OR Regular Command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>Returns the method of the formattable/regular command.</returns>
        public static object? GetMethod(string commandName) {
            if (Command.Exists(commandName)) {
                if (FCommands.ContainsKey(commandName)) {
                    return FCommands[commandName];
                } else if (RCommands.ContainsKey(commandName)) {
                    return RCommands[commandName];
                } else {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all the aliases for a command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>A list of all the aliases, or null if the command does not exist.</returns>
        public static List<string>? GetAliases(string commandName) {
            if (FCommands.ContainsKey(commandName)) {
                var aliases = FCommands.Where(kvp => kvp.Value == FCommands[commandName])
                    .Select(kvp => kvp.Key)
                    .ToList();

                return aliases;
            } else if (RCommands.ContainsKey(commandName)) {
                var aliases = RCommands.Where(kvp => kvp.Value == RCommands[commandName])
                    .Select(kvp => kvp.Key)
                    .ToList();

                return aliases;
            }

            return null;
        }
    }

    /// <summary>
    /// The class that supports formattable commands.
    /// </summary>
    public class FormattableCommand : Command {
        /// <summary>
        /// The function that will be executed when the command is called.
        /// </summary>
        public Func<string[], bool, bool, string?>? Function;

        /// <summary>
        /// Denotes whether this specific command will be used in the all command.
        /// </summary>
        public bool UseInAllCommand;

        /// <summary>
        /// If UseInAllCommand is true, then this denotes what all-command-mode the command will be used in.
        /// </summary>
        public string? AllCommandMode;

        /// <summary>
        /// List of all registered FormattableCommands.
        /// </summary>
        public static List<FormattableCommand> FormattableCommands = new();

        /// <summary>
        /// Initializes a new instance of a FormattableCommand.
        /// </summary>
        /// <param name="commandName">The commandName for the FormattableCommand.</param>
        /// <param name="function">The function for the FormattableCommand.</param>
        /// <param name="aliases">The aliases for the FormattableCommand.</param>
        /// <param name="useInAllCommand">Denotes whether the command should be included in the all command.</param>
        /// <param name="allCommandMode">The mode for the all command that the command is to be included in.</param>
        public FormattableCommand(
            string commandName,
            Func<string[], bool, bool, string?> function,
            string[]? aliases = null,
            bool useInAllCommand = false,
            string allCommandMode = "none"
        ) {
            //* setting all attributes for instance
            CommandName = commandName; Function = function; Aliases = aliases;
            UseInAllCommand = useInAllCommand; AllCommandMode = allCommandMode;

            if (aliases != null) {
                FCommands.Add(commandName, function);
                foreach (string alias in aliases) { FCommands.Add(alias, function); }
            } else { FCommands.Add(commandName, function); }

            FormattableCommands.Add(this);
        }

        /// <summary>
        /// A non-static command that allows you to execute a command immediately.
        /// </summary>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        /// <param name="copy">Controls whether the function is willing to copy text to the clipboard.</param>
        /// <param name="notif">Controls whether the function is willing to send a notification.</param>
        //! Mostly unused method. Only used for testing purposes.
        public string? Execute(string[] args, bool copy, bool notif) {
            if (this.Function != null) {
                string? output = this.Function.Invoke(args, copy, notif);
                if (output != null) { Console.WriteLine(output); return output; }
            }

            return null;
        }

        /// <summary>
        /// Lists all currently registered FormattableCommands.
        /// </summary>
        /// <returns>A string with all currently registered Commands, seperated by newlines.</returns>
        public static string ListAllFCommands() {
            List<string> fCommandsList = new();
            foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> i in Command.FCommands) {
                fCommandsList.Add(i.Key);
            }

            return string.Join("\n", fCommandsList);
        }

        /// <summary>
        /// Finds the command in the fCommands dictionary and then executes it.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        /// <param name="copy">Controls whether the function is willing to copy text to the clipboard.</param>
        /// <param name="notif">Controls whether the function is willing to send a notification.</param>
        /// <returns>The output of the method that is ran. Value can be null.</returns>
        public static string? FindAndInvoke(string[] args, bool copy, bool notif) {
            if (FCommands.ContainsKey(args[0])) {
                string? output = FCommands[args[0]].Invoke(args, copy, notif);
                if (output != null) { return output; } else { return null; }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns every command that supports use in the 'all' command.
        /// </summary>
        /// <param name="mode">Mode for the command, fancy/encoding</param>
        /// <returns></returns>
        public static List<FormattableCommand> GetMethodsSupportedByAll(string mode) {
            List<FormattableCommand> methodsSupportedByAll = new();

            if (FormattableCommands != null) {
                FormattableCommands.ForEach(
                    i => {
                        if (i.UseInAllCommand && i.AllCommandMode == mode) {
                            methodsSupportedByAll.Add(i);
                        }
                    }
                );
            }

            return methodsSupportedByAll;
        }

        /// <summary>
        /// Returns a FormattableCommand using the name of that command.
        /// </summary>
        /// <param name="cmd">The name of the command that is used to find the method and return it.</param>
        /// <returns>The method of that command name.</returns>
        public static Func<string[], bool, bool, string?>? GetFMethod(string cmd) {
            if (FCommands.ContainsKey(cmd)) {
                Func<string[], bool, bool, string?> func = FCommands[cmd];
                return func;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns a FormattableCommand using the name of that command.
        /// </summary>
        /// <param name="cmd">The name of the command.</param>
        /// <returns>A FormattableCommand based on the "cmd" that is passed.</returns>
        public static FormattableCommand? GetFormattableCommand(string cmd) {
            foreach (FormattableCommand i in FormattableCommands) {
                if (i.CommandName == cmd) {
                    return i;
                } else if (i.Aliases != null) {
                    if (i.Aliases.Any(x => x == cmd)) { return i; }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a FormattableCommand exists using the name of its name.
        /// </summary>
        /// <param name="cmd">The name of the command.</param>
        /// <returns>True if the command exists, else false.</returns>
        public static bool FCommandExists(string cmd) { return FCommands.ContainsKey(cmd); }
    }

    /// <summary>
    /// The class that supports regular commands.
    /// </summary>
    public class RegularCommand : Command {
        public Func<string[], string>? Function;
        public static List<RegularCommand> RegularCommands = new();
        /// <summary>
        /// Initializes a new instance of a RegularCommand.
        /// </summary>
        /// <param name="commandName">The name of the regular command.</param>
        /// <param name="function">The function to be run.</param>
        /// <param name="aliases">The aliases for the command.</param>
        public RegularCommand(string commandName, Func<string[], string> function, string[]? aliases = null) {
            //* setting all attributes for instance
            CommandName = commandName.ToLower(); Function = function; Aliases = aliases;
            if (aliases != null) {
                RCommands.Add(commandName, function);
                foreach (string alias in aliases) { RCommands.Add(alias, function); }
            } else {
                RCommands.Add(commandName, function);
            }

            RegularCommands.Add(this);
        }

        /// <summary>
        /// Lists all currently registered Regular Commands.
        /// </summary>
        /// <returns>A string with every RegularCommand, seperated by newlines.</returns>
        public static string ListAllRCommands() {
            List<string> rCommandsList = new();
            foreach (KeyValuePair<string, Func<string[], string>> i in Command.RCommands) {
                rCommandsList.Add(i.Key);
            }

            return string.Join("\n", rCommandsList);
        }

        /// <summary>
        /// Gets a RegularCommand using the name of that command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>An instance of the RegularCommand class, or null.</returns>
        public static RegularCommand? GetRegularCommand(string commandName) {
            foreach (RegularCommand i in RegularCommands!) {
                if (i.CommandName == commandName) {
                    return i;
                } else if (i.Aliases != null) {
                    if (i.Aliases.Any(x => x == commandName)) {
                        return i;
                    }
                }
            }

            return null;
        }

        /// <summary>A non-static method that executes a command immediately.</summary>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        //! Mostly unused method. Only used for testing purposes.
        public void Execute(string[] args) {
            this.Function?.Invoke(args);
        }
    }

    public class RegisterCommands {
        public static void RegisterAllRCommands() {
            RegularCommand exit = new(
                commandName: "exit",
                function: (string[] args) => {
                    Program.ContinueExecution = false;
                    return SocketJSON.SendJSON("command", new List<object>() { "exit" });
                },
                aliases: new string[] { "quit" }
            );

            RegularCommand format = new(
                commandName: "format",
                function: Format.FormatMain
            );
        }

        public static void RegisterAllFCommands() {
            FormattableCommand all = new(
                commandName: "all",
                function: All.AllMain
            );

            FormattableCommand getAliases = new(
                commandName: "aliases",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string cmd = args[1];
                    List<string>? aliases = Command.GetAliases(cmd);

                    if (aliases != null) {
                        string aliasesString = string.Join(", ", aliases);

                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object>() {
                                aliasesString, "Success!", "Check your clipboard."
                            }
                        );
                    } else {
                        return SocketJSON.SendJSON(
                            "notification", new List<object>() { "No aliases found.", "Could not find any." }
                        );
                    }
                },
                aliases: new string[] { "getaliases", "getalias", "get-alias", "get-aliases" }
            );

            FormattableCommand escape = new(
                commandName: "escape",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    string ans = Utils.BulkReplace(
                        text,
                        "! @ # $ % ^ & * ( ) _ + , . / ; ' [ ] < > ? : \" { } ` ~ \\",
                        "\\" + string.Join(" \\", "! @ # $ % ^ & * ( ) _ + , . / ; ' [ ] < > ? : \" { } ` ~ \\".Split(" "))
                    );

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { ans, "Success!", "Message copied to clipboard." }
                    );
                }
            );

            FormattableCommand base32 = new(
                commandName: "base32",
                function: Base32Convert.Base32ConvertMain,
                aliases: new string[] { "b32" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand base64 = new(
                commandName: "base64",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    Func<string, bool> isBase64 = (string s) => {
                        s = s.Trim();
                        bool isB64 = (s.Length % 4 == 0) && System.Text.RegularExpressions.Regex.IsMatch(
                            s, @"^[a-zA-Z0-9\+/]*={0,3}$",
                            System.Text.RegularExpressions.RegexOptions.None
                        ); return isB64;
                    };

                    string text = string.Join(" ", args[1..]);
                    if (isBase64(text)) {
                        try {
                            string ans = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(text));
                            return Utils.CopyNotifCheck(
                                copy, notif, new List<object>() { ans, "Success!", $"Check your clipboard." }
                            );
                        } catch {
                            return SocketJSON.SendJSON(
                                "notification", new List<object>() { "Something went wrong.", "An exception occured." }
                            );
                        }
                    } else {
                        string ans = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object>() { ans, "Success!", "Check your clipboard." }
                        );
                    }
                },
                aliases: new string[] { "b64" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand isBase64 = new(
                commandName: "isbase64",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);

                    Func<string, bool> isBase64 = (string s) => {
                        s = s.Trim();
                        bool isB64 = (s.Length % 4 == 0) && System.Text.RegularExpressions.Regex.IsMatch(
                            s, @"^[a-zA-Z0-9\+/]*={0,3}$",
                            System.Text.RegularExpressions.RegexOptions.None
                        ); return isB64;
                    };

                    if (isBase64(text)) {
                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object>() { "True", "Yes.", "The string is Base64." }
                        );
                    } else {
                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object>() { "False", "No.", "The string is not Base64." }
                        );
                    }
                },
                aliases: new string[] { "isb64" }
            );

            FormattableCommand base85 = new(
                commandName: "base85",
                function: Ascii85.Base85Main,
                aliases: new string[] { "ascii85", "b85" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand urlencode = new(
                commandName: "urlencode",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    string url = System.Web.HttpUtility.UrlEncode(text);

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { url, "Success!", "Check your clipboard." }
                    );
                }
            );

            FormattableCommand urldecode = new(
                commandName: "urldecode",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    string url = System.Web.HttpUtility.UrlDecode(text);

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { url, "Success!", "Check your clipboard." }
                    );
                }
            );

            FormattableCommand binary = new(
                commandName: "binary",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    if (!Utils.FormatValid("01 ", text)) {
                        byte[] ConvertToByteArray(string str, System.Text.Encoding encoding) {
                            return encoding.GetBytes(str);
                        }

                        string ToBinary(Byte[] data) {
                            return string.Join(
                                " ",
                                data.Select(
                                    byt => Convert.ToString(byt, 2).PadLeft(8, '0')
                                )
                            );
                        }

                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object> {
                                ToBinary(ConvertToByteArray(text, System.Text.Encoding.ASCII)),
                                "Success!", "Message copied to clipboard."
                            }
                        );

                    } else {
                        try {
                            string[] textList = text.Split(" ");
                            var chars = from split in textList select ((char)Convert.ToInt32(split, 2)).ToString();

                            return Utils.CopyNotifCheck(
                                copy, notif, new List<object>() {
                                    string.Join("", chars), "Success!", "Check your clipboard."
                                }
                            );
                        } catch {
                            return SocketJSON.SendJSON(
                                "notification", new List<object>() { "Something went wrong.", "An exception occured." }
                            );
                        }
                    }
                },
                aliases: new string[] { "bin" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand bubbletext = new(
                commandName: "bubbletext",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.BubbleDict);
                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { result, "Success!", "Message copied to clipboard." }
                    );
                },
                aliases: new string[] { "bubble" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand commaseperator = new(
                commandName: "commaseperator",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string input = string.Join(" ", args[1..]);
                    System.Text.RegularExpressions.Regex re = new(@"(?<num>-?\d+)(?:\.(?<decimals>\d+))?");

                    if (re.IsMatch(input)) {
                        System.Numerics.BigInteger num =
                            System.Numerics.BigInteger.Parse(re.Match(input).Groups["num"].Value);

                        System.Numerics.BigInteger decimals =
                            re.Match(input).Groups["decimals"].Value != ""
                                ? System.Numerics.BigInteger.Parse(re.Match(input).Groups["decimals"].Value)

                            : 0;

                        string result =
                            decimals == 0 ? string.Format("{0:n0}", num)
                            : string.Format("{0:n0}", num) + "." + decimals.ToString();

                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object>() { result, "Success!", "Message copied to clipboard." }
                        );
                    } else {
                        return SocketJSON.SendJSON(
                            "notification", new List<object>() { "Something went wrong.", "You sure that was a number?" }
                        );
                    }
                },
                aliases: new string[] { "cms" }
            );

            FormattableCommand copypaste = new(
                commandName: "copypaste",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);

                    return Dictionaries.CopypasteDict.ContainsKey(text) ? Utils.CopyNotifCheck(
                        copy, notif, new List<object>() {
                            Dictionaries.CopypasteDict[text], "Success!", "Message copied to clipboard."
                        }
                    ) : SocketJSON.SendJSON(
                        "notification", new List<object>() { "Something went wrong.", "Prompt not found." }
                    );
                },
                aliases: new string[] { "cp" }
            );

            FormattableCommand creepy = new(
                commandName: "creepy",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() {
                            Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.CreepyDict),
                            "Success!", "Message copied to clipboard."
                        }
                    );
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand wingdings = new(
                commandName: "wingdings",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() {
                            Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.WingdingsDict),
                            "Success!",
                            "Message copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "wd" }
            );

            FormattableCommand exponent = new(
                commandName: "exponent",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() {
                            Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.ExponentDict),
                            "Success!",
                            "Message copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "ep" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand subscript = new(
                commandName: "subscript",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() {
                            Utils.TextFormatter(string.Join(" ", args[1..]).ToLower(), Dictionaries.SubscriptDict),
                            "Success!",
                            "Message copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "sub" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand factorial = new(
                commandName: "factorial",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    try {
                        System.Numerics.BigInteger n = System.Numerics.BigInteger.Parse(args[1]);
                        System.Numerics.BigInteger i = 1;
                        System.Numerics.BigInteger v = 1;
                        while (i <= n) { v *= i; i += 1; }

                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object>() { v.ToString(), "Success!", "Message copied to clipboard." }
                        );
                    } catch {
                        return SocketJSON.SendJSON(
                            "notification", new List<object>() { "Something went wrong.", "idk how this happened" }
                        );
                    }
                }
            );

            FormattableCommand raise = new(
                commandName: "raise",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    bool cancel = false;

                    var matchToGroups =
                        Utils.RegexFind(
                            text,
                            @"(?<base>-?\d+\.\d+|-?\d+) to (?<power>-?\d+\.\d+|-?\d+)",
                            useIsMatch: true,
                            ifNotMatch: () => cancel = true
                        );

                    if (cancel) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check the syntax and parameters." }
                        );
                    }

                    if (matchToGroups != null) {
                        foreach (var kvp in matchToGroups) {
                            System.Numerics.BigInteger result = 1;
                            System.Numerics.BigInteger baseNum =
                                System.Numerics.BigInteger.Parse(kvp.Key.Groups["base"].Value);

                            System.Numerics.BigInteger powerNum =
                                System.Numerics.BigInteger.Parse(kvp.Key.Groups["power"].Value);

                            for (System.Numerics.BigInteger i = 0; i < powerNum; i += 1) { result *= baseNum; }

                            return Utils.CopyNotifCheck(
                                copy, notif, new List<object>() { result.ToString(), "Success!", "Check your clipboard." }
                            );
                        }
                    } else {
                        return SocketJSON.SendJSON(
                            "notification", new List<object>() { "Something went wrong.", "Check your parameters." }
                        );
                    }

                    return SocketJSON.SendJSON(
                        "notification", new List<object>() { "Something went wrong.", "An exception occured." }
                    );
                }
            );

            FormattableCommand root = new(
                commandName: "root",
                function: (string[] args, bool copy, bool notif) => {
                    if (args[1] == "calc" | args[1] == "calculator") {
                        string text = string.Join(" ", args[2..]);
                        System.Text.RegularExpressions.Regex regex =
                            new(@"(?<root>-?\d+\.\d+|-?\d+)(?:st|nd|rd|th) root of (?<num>-?\d+\.\d+|-?\d+)");

                        if (regex.IsMatch(text)) {
                            var match = regex.Match(text);
                            try {
                                double root = Convert.ToDouble(match.Groups["root"].Value);
                                double num = Convert.ToDouble(match.Groups["num"].Value);
                                double answer = Utils.RoundIfNumberIsNearEnough(Math.Pow(num, 1 / root));

                                return Utils.CopyNotifCheck(
                                    copy, notif, new List<object>() { answer.ToString(), "Success!", "Check your clipboard." }
                                );
                            } catch (OverflowException) {
                                return SocketJSON.SendJSON(
                                    "notification",
                                    new List<object>() { "Overflow exception.", "Number may be too large." }
                                );
                            }
                        } else {
                            return SocketJSON.SendJSON(
                                "notification",
                                new List<object>() { "Something went wrong.", "Check the syntax and parameters." }
                            );
                        }
                    } else if (args[1] == "get") {
                        try {
                            System.Numerics.BigInteger num = System.Numerics.BigInteger.Parse(args[2]);

                            string? exp = exponent.Execute(new string[] { "exponent", num.ToString() }, false, false);
                            string? toCopy = num == 2 ? "√" : exp != null ? $"{exp}√" : null;

                            return toCopy != null ? Utils.CopyNotifCheck(
                                copy, notif, new List<object>() { toCopy, "Success!", "Check your clipboard." }
                            ) : SocketJSON.SendJSON(
                                "notification", new List<object>() { "Something went wrong.", "You sure that was a number?" }
                            );
                        } catch (FormatException) {
                            return SocketJSON.SendJSON(
                                "notification",
                                new List<object>() { "Something went wrong.", "You sure that was a number?" }
                            );
                        }
                    } else {
                        return SocketJSON.SendJSON(
                            "notification", new List<object>() { "Something went wrong.", "Check your syntax." }
                        );
                    }
                }
            );

            FormattableCommand cuberoot = new(
                commandName: "cuberoot",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    
                    //* Testing if inputted string is a number.
                    try {
                        Convert.ToDouble(text);
                    } catch (FormatException) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your syntax." }
                        );
                    }

                    //* Removing the commas of inputted number.
                    if (text.Contains(",")) { text = text.Replace(",", string.Empty); }

                    double num = Convert.ToDouble(text);
                    string result = Math.Pow(num, ((double)1 / 3)).ToString();

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() {
                            Utils.RoundIfNumberIsNearEnough(Convert.ToDouble(result)).ToString(),
                            "Success!", "Check your clipboard."
                        }
                    );
                },
                aliases: new string[] { "cbrt" }
            );

            FormattableCommand cursive = new(
                commandName: "cursive",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.CursiveDict);
                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { result, "Success!", "Message copied to clipboard." }
                    );
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand doublestruck = new(
                commandName: "doublestruck",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.DoublestruckDict);
                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { result, "Success!", "Message copied to clipboard." }
                    );
                },
                aliases: new string[] { "dbs" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand emojify = new(
                commandName: "emojify",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }
                    
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();

                    foreach (char i in text) {
                        if (Utils.FormatValid(
                            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
                           i.ToString()
                        )) {
                            converted.Add($":regional_indicator_{i.ToString().ToLower()}:");
                        } else if (Dictionaries.EmojifySpecialCharDict.ContainsKey(i.ToString())) {
                            converted.Add(Dictionaries.EmojifySpecialCharDict[i.ToString()]);
                        } else {
                            converted.Add(i.ToString());
                        }
                    }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { string.Join(" ", converted), "Success!", "Message copied to clipboard." }
                    );
                }
            );

            FormattableCommand leet = new(
                commandName: "leet",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    Dictionary<string, string> leetChar = new() {
                        { "E", "3" }, { "I", "1" }, { "O", "0" }, { "A", "4" }, { "S", "5" }
                    };

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() {
                            Utils.TextFormatter(string.Join(" ", args[1..]).ToUpper(), leetChar),
                            "Success!", "Message copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "numberize", "numberise" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand pi = new(
                commandName: "pi",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    try {
                        int digits = int.Parse(args[1]);

                        Func<System.Numerics.BigInteger, string> calculatePi = (System.Numerics.BigInteger digits) => {
                            digits++;

                            uint[] x = new uint[(int)digits * 10 / 3 + 2];
                            uint[] r = new uint[(int)digits * 10 / 3 + 2];
                            uint[] pi = new uint[(int)digits];

                            for (int j = 0; j < x.Length; j++) { x[j] = 20; }

                            for (int i = 0; i < digits; i++) {
                                uint carry = 0;
                                for (int j = 0; j < x.Length; j++) {
                                    uint num = (uint)(x.Length - j - 1);
                                    uint dem = num * 2 + 1;

                                    x[j] += carry;

                                    uint q = x[j] / dem;
                                    r[j] = x[j] % dem;

                                    carry = q * num;
                                }


                                pi[i] = (x[x.Length - 1] / 10);
                                r[x.Length - 1] = x[x.Length - 1] % 10; ;
                                for (int j = 0; j < x.Length; j++) { x[j] = r[j] * 10; }
                            }

                            var result = "";
                            uint c = 0;

                            for (int i = pi.Length - 1; i >= 0; i--) {
                                pi[i] += c;
                                c = pi[i] / 10;

                                result = (pi[i] % 10).ToString() + result;
                            }

                            return result;
                        };

                        return digits <= 0 ? SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Invalid number of digits." }
                        ) : digits <= 1000 ? Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { "3." + PiDigits.piDigits[0..digits], "Success!", "Check your clipboard." }
                        ) : Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { "3." + calculatePi(digits)[1..], "Success!", "Check your clipboard." }
                        );

                    } catch (FormatException) {
                        return SocketJSON.SendJSON(
                            "notification", new List<object>() { "Something went wrong.", "Was that really a number?" }
                        );
                    } catch {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "An exception has occured." }
                        );
                    }
                }
            );

            FormattableCommand lorem = new(
                commandName: "lorem",
                function: LoremIpsum.LoremMain,
                aliases: new string[] { "loremipsum" }
            );

            FormattableCommand flip = new(
                commandName: "flip",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();

                    foreach (char f in text) {
                        var replaced = Dictionaries.FlipDict.GetValueOrDefault(f.ToString(), "");
                        if (replaced != "") {
                            converted.Add(replaced!);
                        } else {
                            converted.Add(f.ToString());
                        }
                    }

                    converted.Reverse();
                    var answer = string.Join("", converted);

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { answer, "Success!", "Message copied to clipboard." }
                    );
                },
                aliases: new string[] { "flipped", "upside-down" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand shuffle = new(
                commandName: "shuffle",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }
                    
                    string text = string.Join(" ", args[1..]);

                    //* shuffle text
                    var chars = text.ToCharArray();
                    var random = new Random();
                    for (int i = chars.Length - 1; i > 0; i--) {
                        int r = random.Next(i + 1);
                        var tmp = chars[i];
                        chars[i] = chars[r];
                        chars[r] = tmp;
                    }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { new string(chars), "Success!", "Message copied to clipboard." }
                    );
                }
            );

            FormattableCommand permutations = new(
                commandName: "permutations",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    char[] textAsCharArray = string.Join(" ", args[1..]).ToCharArray();

                    // checks if textAsCharArray is than the permutationsCalculationLimit
                    int limit = Program.CurrentSettings.PermutationsCalculationLimit;

                    if (textAsCharArray.Length <= limit) {
                        Permutations permutation = new();
                        permutation.GetPer(textAsCharArray);
                        HashSet<string> hashSetAnswer = permutation.Permutation;

                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() {
                                string.Join("\n", hashSetAnswer),
                                "Success!",
                                "Check your clipboard."
                            }
                        );
                    } else {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Prompt too long.", "Check your parameters." }
                        );
                    }
                },
                aliases: new string[] { "getpermutations", "get-permutations" }
            );
            
            FormattableCommand fraction = new(
                commandName: "fraction",
                function: Fractions.FractionsMain,
                aliases: new string[] { "fc" }
            );

            FormattableCommand gzip = new(
                commandName: "gzip",
                function: GZip.GZipMain
            );

            FormattableCommand hcf = new(
                commandName: "hcf",
                function: HCF.HCFMain,
                aliases: new string[] { "gcd" }
            );

            FormattableCommand factors = new(
                commandName: "factors",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    Func<System.Numerics.BigInteger, List<System.Numerics.BigInteger>> findFactors =
                        (System.Numerics.BigInteger num) => {
                            List<System.Numerics.BigInteger> factors = new();

                            for (System.Numerics.BigInteger i = 1; i < num; i++) {
                                if (num % i == 0) {
                                    factors.Add(i);
                                }
                            }

                            return factors;
                        };

                    try {
                        System.Numerics.BigInteger num = System.Numerics.BigInteger.Parse(args[1]);
                        List<System.Numerics.BigInteger> factors = findFactors(num);

                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { string.Join(", ", factors), "Success!", "Check your clipboard." }
                        );
                    } catch (FormatException) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your parameters." }
                        );
                    }
                },
                aliases: new string[] { "factorise" }
            );

            FormattableCommand primefactors = new(
                commandName: "primefactors",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    try {
                        System.Numerics.BigInteger number = System.Numerics.BigInteger.Parse(args[1]);

                        List<int> factors = new List<int>();
                        int divisor = 2;

                        while (number > 1) {
                            if (number % divisor == 0 && factors.Find(x => x % divisor == 0 && x != divisor) == 0) {
                                number /= divisor;
                                factors.Add(divisor);
                            } else {
                                divisor++;
                            }
                        }

                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { string.Join("×", factors), "Success!", "Check your clipboard." }
                        );
                    } catch (FormatException) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your parameters." }
                        );
                    }
                },
                aliases: new string[] { "primefactorise" }
            );

            FormattableCommand mathitalic = new(
                commandName: "mathitalic",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.MathItalicDict);
                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { result, "Success!", "Message copied to clipboard." }
                    );
                },
                aliases: new string[] { "mai" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );
        }
    }
}