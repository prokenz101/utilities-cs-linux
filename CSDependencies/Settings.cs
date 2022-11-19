namespace utilities_cs_linux {
    public class Settings {
        public static Settings DefaultSettings = new Settings {
            DisableNotifications = false,
            PermutationsCalculationLimit = 6,
            EscapeBase85OutputText = true,
            CopyingHotkeyDelay = 50,
            AutoPaste = false,
            PressEscape = true,
            AllCommandHideNames = false
        };

        public static Settings GetSettings() {
            try {
                string jsonString = File.ReadAllText(Program.SettingsJSONPath);
                Settings settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(jsonString)!;
                return settings;
            } catch {
                CreateJson();
                return GetSettings();
            }
        }

        public static void CreateJson() {
            string jsonString = System.Text.Json.JsonSerializer.Serialize<Settings>(DefaultSettings);
            File.WriteAllText(Program.SettingsJSONPath, jsonString);
        }

        public static bool SettingsJsonExists() {
            return File.Exists(Program.SettingsJSONPath);
        }
    
        public bool DisableNotifications { get; set; }
        public int PermutationsCalculationLimit { get; set; }
        public bool EscapeBase85OutputText { get; set; }
        public int CopyingHotkeyDelay { get; set; }
        public bool AutoPaste { get; set; }
        public bool PressEscape { get; set; }
        public bool AllCommandHideNames { get; set; }

    }
}