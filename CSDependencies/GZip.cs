namespace utilities_cs_linux {
    public class GZip {
        public static string? GZipMain(string[] args, bool copy, bool notif) {
            string indexTest = Utils.IndexTest(args);
            if (indexTest != "false") { return indexTest; }

            string mode = args[1];
            string text = string.Join(" ", args[2..]);
            if (mode == "to") {
                try {
                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { GZip.Compress(text), "Success!", "Message copied to clipboard." }
                    );
                } catch {
                    return SocketJSON.SendJSON(
                        "notification",
                        new List<object>() { "Something went wrong.", "An exception occured." }
                    );
                }
            } else if (mode == "from") {
                try {
                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() { GZip.Decompress(text), "Success!", "Check your clipboard." }
                    );
                } catch {
                    return SocketJSON.SendJSON(
                        "notification",
                        new List<object>() { "Something went wrong.", "An exception occured." }
                    );
                }
            } else {
                return SocketJSON.SendJSON(
                    "notification",
                    new List<object>() { "Something went wrong.", "Check your parameters." }
                );
            }
        }

        public static string Decompress(string input) {
            byte[] compressed = Convert.FromBase64String(input);
            byte[] decompressed = Decompress(compressed);
            return System.Text.Encoding.UTF8.GetString(decompressed);
        }

        public static string Compress(string input) {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] compressed = Compress(encoded);
            return Convert.ToBase64String(compressed);
        }

        public static byte[] Decompress(byte[] input) {
            using (var source = new MemoryStream(input)) {
                byte[] lengthBytes = new byte[4];
                source.Read(lengthBytes, 0, 4);

                var length = BitConverter.ToInt32(lengthBytes, 0);
                using (var decompressionStream = new System.IO.Compression.GZipStream(source,
                    System.IO.Compression.CompressionMode.Decompress)) {
                    var result = new byte[length];
                    decompressionStream.Read(result, 0, length);
                    return result;
                }
            }
        }

        public static byte[] Compress(byte[] input) {
            using (var result = new MemoryStream()) {
                var lengthBytes = BitConverter.GetBytes(input.Length);
                result.Write(lengthBytes, 0, 4);

                using (var compressionStream = new System.IO.Compression.GZipStream(result,
                    System.IO.Compression.CompressionMode.Compress)) {
                    compressionStream.Write(input, 0, input.Length);
                    compressionStream.Flush();
                }

                return result.ToArray();
            }
        }
    }
}