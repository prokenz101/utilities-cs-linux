namespace utilities_cs_linux {
    public class SettingsModification {
        public static SettingsJSON DefaultSettings = new SettingsJSON {
            DisableNotifications = false,
            PermutationsCalculationLimit = 6,
            EscapeBase85OutputText = true,
            CopyingHotkeyDelay = 50,
            AutoPaste = false,
            PressEscape = true,
            AllCommandHideNames = false
        };

        public static SettingsJSON GetSettings() {
            try {
                string jsonString = File.ReadAllText(Program.SettingsJSONPath);
                SettingsJSON settings = System.Text.Json.JsonSerializer.Deserialize<SettingsJSON>(jsonString)!;
                return settings;
            } catch {
                CreateJson();
                Thread.Sleep(250);
                return GetSettings();
            }
        }

        public static void CreateJson() {
            string jsonString = System.Text.Json.JsonSerializer.Serialize<SettingsJSON>(DefaultSettings);
            File.WriteAllText(Program.SettingsJSONPath, jsonString);
        }

        public static bool SettingsJsonExists() {
            return File.Exists(Program.SettingsJSONPath);
        }
    }
    
    public class SettingsJSON {
        public bool DisableNotifications { get; set; }
        public int PermutationsCalculationLimit { get; set; }
        public bool EscapeBase85OutputText { get; set; }
        public int CopyingHotkeyDelay { get; set; }
        public bool AutoPaste { get; set; }
        public bool PressEscape { get; set; }
        public bool AllCommandHideNames { get; set; }
    }
}