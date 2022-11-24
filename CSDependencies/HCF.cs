using System.Numerics;

namespace utilities_cs_linux {
    public class HCF {
        public static string? HCFMain(string[] args, bool copy, bool notif) {
            string indexTest = Utils.IndexTest(args);
            if (indexTest != "false") { return indexTest; }

            string text = string.Join(" ", args);
            List<BigInteger> nums = new();
            Utils.RegexFindAllInts(text).ForEach(x => nums.Add(x));

            try {
                if (nums.Count > 1) {
                    return Utils.CopyNotifCheck(
                        copy, notif,
                        new List<object>() {
                            HCF.FindGCD(nums.ToArray<BigInteger>(), nums.ToArray().Length).ToString(),
                            "Success!",
                            "Check your clipboard."
                        }
                    );
                }
            } catch { }

            return SocketJSON.SendJSON(
                "notification",
                new List<object>() { "Something went wrong.", "Check your parameters." }
            );
        }

        public static System.Numerics.BigInteger FindHCF(
            System.Numerics.BigInteger a, System.Numerics.BigInteger b
        ) {
            if (a == 0) { return b; }
            return FindHCF(b % a, a);
        }

        //* Function to find gcd of 
        //* array of numbers
        public static System.Numerics.BigInteger FindGCD(
            System.Numerics.BigInteger[] arr,
            System.Numerics.BigInteger n
        ) {
            System.Numerics.BigInteger result = arr[0];
            for (int i = 1; i < n; i++) {
                result = FindHCF(arr[i], result);

                if (result == 1) {
                    return 1;
                }
            }

            return result;
        }
    }
}