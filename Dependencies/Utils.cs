namespace utilities_cs_linux {
    public class Utils {
        static void Notification(
            string title, string subtitle,
            string? iconPath = null,
            bool lengthOverride = false, bool iconPathOverride = true
        ) {
            if (iconPath == null) { iconPath = Program.IconPath; }

            System.Diagnostics.ProcessStartInfo notifProcess = new System.Diagnostics.ProcessStartInfo(
                "notify-send",
                $"--hint int:transient:1 -i {iconPath} \"{title}\" \"{subtitle}\""
            ) { CreateNoWindow = true };

            if (File.Exists(iconPath)) {
                if ((title.Length <= 29 && subtitle.Length <= 32) | lengthOverride) {
                    System.Diagnostics.Process.Start(notifProcess);
                } else {
                    notifProcess.Arguments = $"--hint int:transient:1 -i {iconPath} \"This notification was too long.\" \"Check your clipboard.\"";
                    System.Diagnostics.Process.Start(notifProcess);
                }
            } else if (iconPathOverride) {
                if ((title.Length <= 29 && subtitle.Length <= 32) | lengthOverride) {
                    notifProcess.Arguments = $"--hint int:transient:1 \"{title}\" \"{subtitle}\"";
                    System.Diagnostics.Process.Start(notifProcess);
                } else {
                    notifProcess.Arguments = $"--hint int:transient:1 -i {iconPath} \"This notification was too long.\" \"Check your clipboard.\"";
                    System.Diagnostics.Process.Start(notifProcess);
                }
            } else {
                throw new FileNotFoundException("The specified icon path was not found.");
            }
        }
    }
}