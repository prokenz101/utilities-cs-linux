namespace utilities_cs_linux {
    public class Commands {
        public static Func<string, string> Cursive = (string input) => {
            List<string> converted = new();

            foreach (char i in input) {
                if (Dictionaries.CursiveDict.ContainsKey(i.ToString())) {
                    converted.Add(Dictionaries.CursiveDict[i.ToString()]);
                } else {
                    converted.Add(i.ToString());
                }
            }

            return string.Join("", converted);
        };

        public static Func<string, string> Doublestruck = (string input) => {
            List<string> converted = new();

            foreach (char i in input) {
                if (Dictionaries.DoublestruckDict.ContainsKey(i.ToString())) {
                    converted.Add(Dictionaries.DoublestruckDict[i.ToString()]);
                } else {
                    converted.Add(i.ToString());
                }
            }

            return string.Join("", converted);
        };

        public static Func<string, string> Creepy = (string input) => {
            List<string> converted = new();

            foreach (char i in input) {
                if (Dictionaries.CreepyDict.ContainsKey(i.ToString())) {
                    converted.Add(Dictionaries.CreepyDict[i.ToString()]);
                } else {
                    converted.Add(i.ToString());
                }
            }

            return string.Join("", converted);
        };

        public static Func<string, string> Factorial = (string input) => {
            System.Numerics.BigInteger n = int.Parse(input);
            System.Numerics.BigInteger i;
            System.Numerics.BigInteger v = 1;
            for (i = 1; i <= n; i++) {v = v * i;}

            return v.ToString();
        };
    }
}