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

        public Logger SubLogger(string prefix, int level = 1)
        {
            return new Logger(Prefix.Length == 0 ? prefix : $"{Prefix}:{prefix}", Level - level);
        }

        public Logger? SubLogger<T>(T a, int level = 1)
            where T : notnull
        {
            if (Level > 0)
            {
                return SubLogger(a.ToString() ?? "", level);
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