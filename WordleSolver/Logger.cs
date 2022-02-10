namespace WordleSolver
{
    public struct Logger
    {
        public string Prefix { get; private set; }
        public int Level { get; private set; }

        public Logger(int level)
        {
            Prefix = "";
            Level = level;
        }

        private Logger(string prefix, int level)
        {
            Prefix = prefix;
            Level = level;
        }

        public Logger SubLogger(string prefix)
        {
            return new Logger(Prefix.Length == 0 ? prefix : $"{Prefix}:{prefix}", Level - 1);
        }

        public Logger? SubLogger<T>(T a)
            where T : notnull
        {
            if (Level > 0)
            {
                return SubLogger(a.ToString() ?? "");
            }
            else
            {
                return null;
            }
        }

        public void Log(string message)
        {
            if (Level >= 0)
            {
                Console.WriteLine($"{Prefix}{message}");
            }
        }
    }
}