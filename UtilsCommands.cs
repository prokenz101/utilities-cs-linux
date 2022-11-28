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
        public static Dictionary<string, Func<string[], string?>> RCommands = new();
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

            try {
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
            } catch {
                return SocketJSON.SendJSON(
                    "notification",
                    new List<object>() { "Something went wrong.", "An exception occured." }
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
        public Func<string[], string?>? Function;
        public static List<RegularCommand> RegularCommands = new();
        /// <summary>
        /// Initializes a new instance of a RegularCommand.
        /// </summary>
        /// <param name="commandName">The name of the regular command.</param>
        /// <param name="function">The function to be run.</param>
        /// <param name="aliases">The aliases for the command.</param>
        public RegularCommand(string commandName, Func<string[], string?> function, string[]? aliases = null) {
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
            foreach (KeyValuePair<string, Func<string[], string?>> i in Command.RCommands) {
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
            //TODO: RegularCommand autoclick = new();
            //TODO: RegularCommand send = new();
            //TODO: RegularCommand spam = new();
            //TODO: RegularCommand settings = new();
            //TODO: RegularCommand force = new();
            //TODO: RegularCommand unforce = new();

            RegularCommand format = new(
                commandName: "format",
                function: Format.FormatMain
            );

            //TODO: RegularCommand update = new();

            RegularCommand exit = new(
                commandName: "exit",
                function: (string[] args) => {
                    Program.ContinueExecution = false;
                    return SocketJSON.SendJSON("command", new List<object>() { "exit" });
                },
                aliases: new string[] { "quit" }
            );

            //TODO: RegularCommand help = new();
            //TODO: RegularCommand notification = new();
            //TODO: RegularCommand remind = new();
            //TODO: RegularCommand googleSearch = new();
            //TODO: RegularCommand youtubeSearch = new();
            //TODO: RegularCommand imageSearch = new();
            //TODO: RegularCommand translate = new();

            RegularCommand getcommandcount = new(
                commandName: "getcommandcount",
                function: (string[] args) => {
                    int regularCommandsCount = RegularCommand.RegularCommands.Count;
                    int formattableCommandsCount = FormattableCommand.FormattableCommands.Count;

                    return Utils.CopyNotifCheck(
                        true, true,
                        new List<object>() {
                            $@"Total Commands: {regularCommandsCount + formattableCommandsCount}
RegularCommands Count: {regularCommandsCount}
FormattableCommands Count: {formattableCommandsCount}",
                            "Success!",
                            "Count copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "totalcommandcount", "get-commandcount" }
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
                        string ans = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(text));
                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object>() { ans, "Success!", $"Check your clipboard." }
                        );
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
                        string[] textList = text.Split(" ");
                        var chars = from split in textList select ((char)Convert.ToInt32(split, 2)).ToString();

                        return Utils.CopyNotifCheck(
                            copy, notif, new List<object>() {
                                    string.Join("", chars), "Success!", "Check your clipboard."
                            }
                        );
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

                    System.Numerics.BigInteger n = System.Numerics.BigInteger.Parse(args[1]);
                    System.Numerics.BigInteger i = 1;
                    System.Numerics.BigInteger v = 1;
                    while (i <= n) { v *= i; i += 1; }

                    return Utils.CopyNotifCheck(
                        copy, notif, new List<object>() { v.ToString(), "Success!", "Message copied to clipboard." }
                    );
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

            FormattableCommand hexadecimal = new(
                commandName: "hexadecimal",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);

                    Func<string, string> fromTextToHex = (string text) => {
                        byte[] ba = System.Text.Encoding.Default.GetBytes(text);
                        var hexString = BitConverter.ToString(ba);
                        hexString = hexString.Replace("-", " ");
                        hexString = hexString.ToLower();

                        return hexString;
                    };

                    Func<string, byte[]> fromHexToText = (string hex) => {
                        hex = hex.Replace("-", "");
                        byte[] raw = new byte[hex.Length / 2];
                        for (int i = 0; i < raw.Length; i++) {
                            raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                        }

                        return raw;
                    };

                    Func<IEnumerable<char>, bool> isHex = (IEnumerable<char> chars) => {
                        bool isHex;
                        foreach (var c in chars) {
                            isHex = ((c >= '0' && c <= '9') ||
                                     (c >= 'a' && c <= 'f') ||
                                     (c >= 'A' && c <= 'F'));

                            if (!isHex)
                                return false;
                        }

                        return true;
                    };

                    string[] textList = text.Split(" ");
                    string hexWithDash = string.Join("-", textList);

                    if (isHex(string.Join("", args[1..]))) {
                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() {
                                    System.Text.Encoding.ASCII.GetString(fromHexToText(hexWithDash)),
                                    "Success!",
                                    "Check your clipboard."
                            }
                        );
                    } else {
                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { fromTextToHex(text), "Success!", "Message copied to clipboard." }
                        );
                    }
                },
                aliases: new string[] { "hex" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand ascii = new(
                commandName: "ascii",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    Func<string, string> toAscii = (string text) => {
                        List<string> ascii = new();
                        foreach (char i in text) {
                            ascii.Add(((int)i).ToString());
                        }

                        return string.Join(" ", ascii);
                    };

                    Func<string, List<int>, string> fromAscii = (string ascii, List<int> nums) => {
                        List<string> chars = new();
                        foreach (int i in nums) {
                            chars.Add(((char)i).ToString());
                        }

                        return string.Join("", chars);
                    };

                    string text = string.Join(" ", args[1..]);
                    if (Utils.FormatValid("0123456789 ", text)) {
                        List<int> values = new();
                        try {
                            Utils.RegexFindAllInts(text).ForEach(x => values.Add((int)x));
                        } catch (OverflowException) {
                            return SocketJSON.SendJSON(
                                "notification",
                                new List<object>() { "Something went wrong.", "An exception occured." }
                            );
                        }

                        List<bool> valuesAreValid = new();
                        foreach (int i in values) {
                            if (i.ToString().Length == 2 | i.ToString().Length == 3) {
                                valuesAreValid.Add(true);
                            } else {
                                valuesAreValid.Add(false);
                            }
                        }

                        if (!valuesAreValid.Contains(false)) {
                            return Utils.CopyNotifCheck(
                                copy, notif,
                                new List<object>() {
                                    fromAscii(string.Join(" ", values), values),
                                    "Success!", "Check your clipboard."
                                }
                            );
                        } else {
                            return Utils.CopyNotifCheck(
                                copy, notif,
                                new List<object>() { toAscii(text), "Success!", "Message copied to clipboard." }
                            );
                        }
                    } else {
                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { toAscii(text), "Success!", "Message copied to clipboard." }
                        );
                    }
                },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand lcm = new(
                commandName: "lcm",
                function: LCMClass.LCMMain
            );

            FormattableCommand length = new(
                commandName: "length",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    string len = $@"Character count: {text.Length.ToString()}
Word count: {args[1..].Length}";

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { len, "Success!", "Check your clipboard." }
                    );
                },
                aliases: new string[] { "len" }
            );

            FormattableCommand characterDistribution = new(
                commandName: "characterdistribution",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    HashSet<char> uniqueChars = new();
                    foreach (char c in text) { uniqueChars.Add(c); }

                    Dictionary<char, string> charDistrDict = new();
                    uniqueChars.ToList().ForEach(
                        i => charDistrDict.Add(i, $"{i}: {text.Count(f => (f == i))}\n")
                    );

                    List<char> firstLetters = charDistrDict.Keys.ToList();
                    firstLetters.Sort();
                    List<string> charDistr = new();

                    foreach (var i in firstLetters) { charDistr.Add(charDistrDict[i]); }
                    string result = string.Join("", charDistr);

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { result, "Success!", "Check your clipboard." }
                    );
                },
                aliases: new string[] { "chardistr", "chardistribution", "characterdistr" }
            );

            FormattableCommand replace = new(
                commandName: "replace",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);

                    System.Text.RegularExpressions.Regex re =
                        new(@"[""'](?<old>.+)[""'] with [""'](?<new>.+|)[""'] in [""'](?<text>.+)[""']");

                    if (re.IsMatch(text)) {
                        var match = re.Match(text);

                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() {
                                match.Groups["text"].Value.Replace(
                                    match.Groups["old"].Value, match.Groups["new"].Value
                                ),
                                "Success!",
                                "Message copied to clipboard."
                            }
                        );
                    } else {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your syntax." }
                        );
                    }
                }
            );

            FormattableCommand characterCount = new(
                commandName: "charactercount",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    bool failed = false;
                    var matchToGroups = Utils.RegexFind(
                        input: text,
                        expression: "(?<char>.) in (?<text>.+)",
                        useIsMatch: true,
                        ifNotMatch: () => { failed = true; }
                    );

                    if (failed) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your syntax." }
                        );
                    }

                    if (matchToGroups != null) {
                        foreach (var kvp in matchToGroups) {
                            char? character = kvp.Key.Groups["char"].Value.ToCharArray()[0];
                            string? textToSearch = kvp.Key.Groups["text"].Value;

                            if (character != null && textToSearch != null) {
                                return Utils.CopyNotifCheck(
                                    copy, notif,
                                    new List<object>() {
                                        $"\"{character}\": {textToSearch.Count(f => (f == character))}",
                                        "Success!", "Check your clipboard."
                                    }
                                );
                            }
                        }
                    }

                    return SocketJSON.SendJSON(
                        "notification",
                        new List<object>() { "Something went wrong.", "An exception occured." }
                    );
                },
                aliases: new string[] { "charcount" }
            );

            FormattableCommand lowercase = new(
                commandName: "lowercase",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            string.Join(" ", args[1..]).ToLower(),
                            "Success!", "Message copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "lower" }
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

            FormattableCommand morse = new(
                commandName: "morse",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]).ToLower();

                    Func<string, bool, bool, string?> toMorse = (string text, bool copy, bool notif) => {
                        List<string> morseConverted = new();

                        foreach (char t in text) {
                            if (Dictionaries.MorseToTextDict.ContainsKey(t.ToString())) {
                                morseConverted.Add(Dictionaries.MorseToTextDict[t.ToString()]);
                                morseConverted.Add(" ");
                            } else {
                                morseConverted.Add(t.ToString());
                            }
                        }

                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() {
                                string.Join("", morseConverted),
                                "Success!", "Message copied to clipboard."
                            }
                        );
                    };

                    Func<string, bool, bool, string?> toText = (string morse, bool copy, bool notif) => {
                        List<string> convertedText = new();
                        Dictionary<string, string> morseToText = Utils.InvertKeyAndValue(Dictionaries.MorseToTextDict);
                        string[] textArray = morse.Split(" ");

                        foreach (string m in textArray) {
                            if (morseToText.ContainsKey(m.ToString())) {
                                convertedText.Add(morseToText[m.ToString()]);
                            } else {
                                convertedText.Add(m.ToString());
                            }
                        }

                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() {
                                string.Join("", convertedText),
                                "Success!",
                                "Check your clipboard."
                            }
                        );
                    };

                    if (Utils.FormatValid("-./ ", text)) {
                        return toText(text, copy, notif);
                    } else {
                        return toMorse(text, copy, notif);
                    }
                },
                aliases: new string[] { "morsecode" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand reciprocal = new(
                commandName: "reciprocal",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    try {
                        double number = double.Parse(args[1]);
                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() {
                                Math.ReciprocalEstimate(number).ToString(),
                                "Success!",
                                "Check your clipboard."
                            }
                        );
                    } catch (OverflowException) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Number might be too large." }
                        );
                    } catch (FormatException) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Are you sure that was a number?" }
                        );
                    }
                }
            );

            FormattableCommand divide = new(
                commandName: "divide",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    List<System.Numerics.BigInteger> ints = Utils.RegexFindAllInts(text);

                    System.Numerics.BigInteger dividedNum =
                        ints[0] / ints[1]; System.Numerics.BigInteger remainder = ints[0] % ints[1];

                    Func<string, string?> returnNum = (string ans) => {
                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { ans, "Success!", "Check your clipboard." }
                        );
                    };

                    if (remainder != 0 && ints.Count > 1) {
                        return returnNum($"Answer: {dividedNum} and Remainder: {remainder}");
                    } else if (remainder == 0) {
                        return returnNum($"Answer: {dividedNum}");
                    } else {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your parameters." }
                        );
                    }
                }
            );

            FormattableCommand percentage = new(
                commandName: "percentage",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    //* making regex
                    System.Text.RegularExpressions.Regex findNumberFromPercentage =
                        new(@"(?<percent>-?\d+\.\d+|-?\d+)% of (?<number>-?\d+\.\d+|-?\d+)");
                    System.Text.RegularExpressions.Regex findPercentageFromNumbers =
                        new(@"get (?<num1>-?\d+\.\d+|-?\d+) and (?<num2>-?\d+\.\d+|-?\d+)");

                    if (findNumberFromPercentage.IsMatch(text)) {
                        var matches = findNumberFromPercentage.Matches(text);
                        float percent = (float.Parse(matches[0].Groups["percent"].Value));
                        float number = float.Parse(matches[0].Groups["number"].Value);

                        string ans = Utils.RoundIfNumberIsNearEnough((percent / 100) * number).ToString();

                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { ans, "Success!", $"{ans} is {percent}% of {number}" }
                        );
                    } else if (findPercentageFromNumbers.IsMatch(text)) {
                        var matches = findPercentageFromNumbers.Matches(text);
                        float num1 = float.Parse(matches[0].Groups["num1"].Value);
                        float num2 = float.Parse(matches[0].Groups["num2"].Value);

                        string ans = Utils.RoundIfNumberIsNearEnough((num1 / num2) * 100).ToString();

                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { ans, "Success!", $"{num1} is {ans}% of {num2}" }
                        );
                    } else {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your parameters." }
                        );
                    }
                },
                aliases: new string[] { "percent", "%" }
            );

            FormattableCommand randchar = new(
                commandName: "randchar",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string[] asciiCharacters = {
                        "a", "b", "c", "d", "e",
                        "f", "g", "h", "i", "j",
                        "k", "l", "m", "n", "o", "p",
                        "q", "r", "s", "t", "u", "v",
                        "w", "x", "y", "z", "A", "B",
                        "C", "D", "E", "F", "G", "H",
                        "I", "J", "K", "L", "M", "N",
                        "O", "P", "Q", "R", "S", "T",
                        "U", "V", "W", "X", "Y", "Z"
                    };

                    string text = string.Join(" ", args[1..]);

                    //* testing if text is a number
                    try {
                        int.Parse(text);
                    } catch (FormatException) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Was that really a number?" }
                        );
                    } catch (OverflowException) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Number might be too large." }
                        );
                    }

                    Random rand = new Random();
                    List<string> randomChar = new();

                    foreach (int i in Enumerable.Range(1, int.Parse(text))) {
                        randomChar.Add(asciiCharacters[rand.Next(0, asciiCharacters.Length - 1)]);
                    }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { string.Join("", randomChar), "Success!", "Text copied to clipboard." }
                    );
                }
            );

            FormattableCommand randint = new(
                commandName: "randint",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    List<int> nums = new();
                    try {
                        Utils.RegexFindAllInts(text).ForEach(num => nums.Add((int)num));
                    } catch (OverflowException) {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Number might be too large." }
                        );
                    }

                    if (nums.Count > 1) {
                        //* quick check to see if the first num is greater than second
                        return nums[0] > nums[1] ? SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your parameters." }
                        ) : Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() {
                                new Random().Next(nums[0], nums[1] + 1),
                                "Success!", "Check your clipboard."
                            }
                        );
                    } else {
                        return SocketJSON.SendJSON(
                            "notification",
                            new List<object>() { "Something went wrong.", "Check your parameters." }
                        );
                    }
                },
                aliases: new string[] { "randnum" }
            );

            FormattableCommand reverse = new(
                commandName: "reverse",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    List<char> textList = text.ToCharArray().ToList();

                    textList.Reverse();
                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            string.Join("", textList), "Success!", "Message copied to clipboard."
                        }
                    );
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand sarcasm = new(
                commandName: "sarcasm",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    char currentCase = 'u';

                    foreach (char i in text) {
                        string iStr = i.ToString();
                        if (currentCase == 'u') {
                            converted.Add(iStr.ToUpper());
                            currentCase = 'l';
                        } else if (currentCase == 'l') {
                            converted.Add(iStr.ToLower());
                            currentCase = 'u';
                        }
                    }
                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            string.Join("", converted), "Success!", "Message copied to clipboard."
                        }
                    );
                }
            );

            FormattableCommand sha1 = new(
                commandName: "sha1",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    using (System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create()) {
                        System.Text.Encoding enc = System.Text.Encoding.UTF8;
                        Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                        foreach (Byte b in result)
                            sb.Append(b.ToString("x2"));
                    }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { sb.ToString(), "Success!", "Message copied to clipboard." }
                    );
                }
            );

            FormattableCommand sha256 = new(
                commandName: "sha256",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    using (System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256.Create()) {
                        System.Text.Encoding enc = System.Text.Encoding.UTF8;
                        Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                        foreach (Byte b in result)
                            sb.Append(b.ToString("x2"));
                    }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { sb.ToString(), "Success!", "Message copied to clipboard." }
                    );
                }
            );

            FormattableCommand sha384 = new(
                commandName: "sha384",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    using (System.Security.Cryptography.SHA384 hash = System.Security.Cryptography.SHA384.Create()) {
                        System.Text.Encoding enc = System.Text.Encoding.UTF8;
                        Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                        foreach (Byte b in result)
                            sb.Append(b.ToString("x2"));
                    }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { sb.ToString(), "Success!", "Message copied to clipboard." }
                    );
                }
            );

            FormattableCommand sha512 = new(
                commandName: "sha512",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    using (System.Security.Cryptography.SHA512 hash = System.Security.Cryptography.SHA512.Create()) {
                        System.Text.Encoding enc = System.Text.Encoding.UTF8;
                        Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                        foreach (Byte b in result)
                            sb.Append(b.ToString("x2"));
                    }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { sb.ToString(), "Success!", "Message copied to clipboard." }
                    );
                }
            );

            FormattableCommand md5 = new(
                commandName: "md5",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    Func<string, string> MD5Hasher = (string input) => {
                        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
                            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                            byte[] hashBytes = md5.ComputeHash(inputBytes);

                            return Convert.ToHexString(hashBytes);
                        }
                    };

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            MD5Hasher(string.Join(" ", args[1..])),
                            "Success!", "Hash copied to clipboard."
                        }
                    );
                }
            );

            FormattableCommand spacer = new(
                commandName: "spacer",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    foreach (char i in text) { converted.Add(i.ToString()); converted.Add(" "); }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { string.Join("", converted), "Success!", "Message copied to clipboard." }
                    );
                }
            );

            FormattableCommand spoilerspam = new(
                commandName: "spoilerspam",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    foreach (char i in text) { converted.Add($"||{i}"); }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            $"{string.Join("||", converted)}||",
                            "Success!", "Message copied to clipboard."
                        }
                    );
                }
            );

            FormattableCommand title = new(
                commandName: "titlecase",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(
                                string.Join(" ", string.Join(" ", args[1..]).ToLower())
                            ),
                            "Success!",
                            "Message copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "title" }
            );

            FormattableCommand uppercase = new(
                commandName: "uppercase",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            string.Join(" ", args[1..]).ToUpper(),
                            "Success!", "Message copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "upper" }
            );

            FormattableCommand camelcase = new(
                commandName: "camelcase",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    Func<string, string?> output = (string result) => {
                        return Utils.CopyNotifCheck(
                            copy, notif,
                            new List<object>() { result, "Success!", "Message copied to clipboard." }
                        );
                    };

                    List<string> ans = new();
                    ans.Add(args[1].ToLower());

                    try {
                        var test = args[2];
                        foreach (string i in args[2..]) {
                            ans.Add(Utils.Capitalise(i));
                        }
                    } catch (IndexOutOfRangeException) {
                        return output(ans[0]);
                    }

                    return output(string.Join("", ans));
                }
            );

            FormattableCommand pascalcase = new(
                commandName: "pascalcase",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    List<string> output = new();
                    args[1..].ToList<string>().ForEach(i => output.Add(Utils.Capitalise(i)));

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            string.Join("", output), "Success!", "Message copied to clipboard."
                        }
                    );
                },
                aliases: new string[] { "pascal" }
            );

            FormattableCommand snakecase = new(
                commandName: "snakecase",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            string.Join("_", args[1..]).ToLower(),
                            "Success!",
                            "Message copied to clipboard."
                        }
                    );
                }
            );

            FormattableCommand piglatin = new(
                commandName: "piglatin",
                function: (string[] args, bool copy, bool notif) => {
                    string indexTest = Utils.IndexTest(args);
                    if (indexTest != "false") { return indexTest; }

                    List<string> pigLatin = new();
                    foreach (string word in args[1..]) {
                        if (
                            word.StartsWith("a")
                            | word.StartsWith("e")
                            | word.StartsWith("i")
                            | word.StartsWith("o")
                            | word.StartsWith("u")
                        ) {
                            pigLatin.Add(word + "ay");
                        } else {
                            List<string> lettersX = word[1..].Split().ToList(); //* all letters of word except the first
                            string firstLetter = word[0].ToString(); //* first letter of word

                            //* add first letter and "ay" to end of word
                            lettersX.Add(firstLetter); lettersX.Add("ay");

                            //* join letters together
                            pigLatin.Add(string.Join("", lettersX));
                        }
                    }

                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            string.Join(" ", pigLatin),
                            "Success!", "Message copied to clipboard."
                        }
                    );
                }
            );
        }
    }
}