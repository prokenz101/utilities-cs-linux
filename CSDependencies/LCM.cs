using System.Numerics;

namespace utilities_cs_linux {
    public class LCMClass {
        public static string? LCMMain(string[] args, bool copy, bool notif) {
            string indexTest = Utils.IndexTest(args);
            if (indexTest != "false") { return indexTest; }

            string text = string.Join(" ", args[1..]);

            List<BigInteger> nums = new();
            Utils.RegexFindAllInts(text).ForEach(x => nums.Add(x));

            try {
                if (nums.Count > 1) {
                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            LCMClass.FindLCM(nums.ToArray<BigInteger>()).ToString()
                            , "Success!", "Check your clipboard."
                        }
                    );
                }
            } catch { }

            return SocketJSON.SendJSON(
                "notification",
                new List<object>() { "Something went wrong.", "Check your paramters." }
            );
        }

        public static BigInteger FindLCM(BigInteger[] elementArray) {
            BigInteger lcmOfArrayElements = 1;
            int divisor = 2;

            while (true) {
                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < elementArray.Length; i++) {

                    if (elementArray[i] == 0) {
                        return 0;
                    } else if (elementArray[i] < 0) { elementArray[i] = elementArray[i] * (-1); }
                    if (elementArray[i] == 1) { counter++; }

                    if (elementArray[i] % divisor == 0) {
                        divisible = true;
                        elementArray[i] = elementArray[i] / divisor;
                    }
                }

                if (divisible) {
                    lcmOfArrayElements = lcmOfArrayElements * divisor;
                } else { divisor++; }

                if (counter == elementArray.Length) {
                    return lcmOfArrayElements;
                }
            }
        }
    }
}