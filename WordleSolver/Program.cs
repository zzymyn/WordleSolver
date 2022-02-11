using Mono.Options;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace WordleSolver
{
    internal partial class Program
    {
        private enum OperationMode
        {
            Level1,
            Level2,
        }

        private static readonly Regex ArgMatch = new(@"^([a-zA-Z]{5}):([012]{5})$");

        private static readonly List<(Word guess, Word answer)> Guesses = new();
        private static readonly List<List<Word>> ExplicitGuessWords = new();
        private static bool AllWords;
        private static bool PrintHelp;
        private static int Verbosity;
        private static bool HardMode;
        private static bool Force;
        private static int Count = 10;
        private static OperationMode Mode = OperationMode.Level1;

        static int Main(string[] args)
        {
            var options = new OptionSet()
            {
                { "?|help", "Print help.", _ => PrintHelp = true },
                { "a|all", "Allow all 12,973 allowed words to be used as a guess.\nBy default WordleSolver only searches guesses from the 2,315 possible solutions.\nEnabling this option will be more optimal, but slower.", _ => AllWords = true },
                { "w|words=", "Only try these words as guesses, comma separated.", a =>
                {
                    var words = new List<Word>();
                    foreach (var word in a.Split(' ', ','))
                    {
                        words.Add(word);
                    }
                    ExplicitGuessWords.Add(words);
                }},
                { "c|count=", "Maximum number of potential guesses to print (default 10)", (int a) => Count = a },
                { "m|mode=", "set operation mode (default 'level1').", a =>
                {
                    if (a == "1" || a.Equals("level1", StringComparison.OrdinalIgnoreCase))
                    {
                        Mode = OperationMode.Level1;
                    }
                    else if (a == "2" || a.Equals("level2", StringComparison.OrdinalIgnoreCase))
                    {
                        Mode = OperationMode.Level2;
                    }
                    else
                    {
                        throw new Exception($"Invalid mode '{a}'.");
                    }
                }},
                { "v|verbose:", "Increase [or set] verbosity level.", (int? a) =>
                {
                    Verbosity = a ?? (Verbosity + 1);
                }},
                { "h|hardmode", "Enable hard-mode (all guesses must be possible solutions).", _=> HardMode = true },
                { "f|force", "Force run (even if it will be really slow or useless).", _ => Force = true },
            };

            try
            {
                var extraArgs = options.Parse(args);

                foreach (var arg in extraArgs)
                {
                    var m = ArgMatch.Match(arg);
                    if (!m.Success)
                        throw new Exception($"Invalid argument '{arg}'.");
                    Guesses.Add((m.Groups[1].Value, m.Groups[2].Value));
                }

                if (Mode == OperationMode.Level2 && ExplicitGuessWords.Count <= 0 && Guesses.Count <= 0 && !Force)
                    throw new Exception("Must provide word list or guresses for operation level 2 (use --force to override).");
            }
            catch (OptionException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }

            var logger = new Logger(Verbosity);

            if (PrintHelp)
            {
                Console.WriteLine($"Arguments:");
                Console.WriteLine($"  Provide guesses using the format GUESS:##### where # is 0, 1, or 2:");
                Console.WriteLine($"    0: a grey square, 1: a yellow square, 2: a green square");
                Console.WriteLine();
                Console.WriteLine($"Optional arguments:");
                options.WriteOptionDescriptions(Console.Out);
                Console.WriteLine($"Example:");
                Console.WriteLine($"  $ WordleSolver.exe RAISE:00102 BOTCH:21000");
                Console.WriteLine($"  Word is: BIOME");
                Console.WriteLine();
                return 0;
            }

            if (ExplicitGuessWords.Count <= 0 && Guesses.Count <= 0 && Mode == OperationMode.Level1 && !Force)
            {
                Console.WriteLine($"Best first guess is: LATER");
                Console.WriteLine($"You can also try: SIREN, TEARY, RISEN, RAISE");
                Console.WriteLine($"These initial guesses are hard-coded, use --force to override.");
                Console.WriteLine();
                return 0;
            }

            var candidateWords = Words.AllSolutions;
            foreach (var (guess, answer) in Guesses)
            {
                candidateWords = RemoveCandidates(candidateWords, guess, answer);
            }

            if (candidateWords.Count <= 0)
            {
                Console.WriteLine("No solutions exist.");
                return 0;
            }
            else if (candidateWords.Count == 1)
            {
                Console.WriteLine($"Word is:");
                Console.WriteLine(candidateWords[0]);
                return 0;
            }
            else if (candidateWords.Count <= 10)
            {
                Console.WriteLine($"{candidateWords.Count} possible words:");
                Console.WriteLine(string.Join(", ", candidateWords));
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"{candidateWords.Count} possible words.");
                Console.WriteLine();
            }

            return Mode switch
            {
                OperationMode.Level1 => MainLevel1(logger, candidateWords),
                OperationMode.Level2 => MainLevel2(logger, candidateWords),
                _ => throw new InvalidOperationException(),
            };
        }

        private static int MainLevel1(Logger logger, List<Word> candidateWords)
        {
            var allValues = RunLevel1(candidateWords, logger);

            if (Verbosity > 0)
                Console.WriteLine();

            Console.WriteLine("Best next guesses:");

            foreach (var (word, value) in allValues.Where(a => a.value > 0).OrderBy(a => a.value).Take(Count))
            {
                Console.WriteLine($"{word} - {value}");
            }

            return 0;
        }

        private static int MainLevel2(Logger logger, List<Word> candidateWords)
        {
            var allValues = RunLevel2(candidateWords, logger);

            if (Verbosity > 0)
                Console.WriteLine();

            Console.WriteLine("Best next guesses:");

            foreach (var (path, value1, value2) in allValues.Where(a => a.value2 > 0).OrderBy(a => a.value2).Take(Count))
            {
                Console.WriteLine($"{path} - {value1},{value2}");
            }

            return 0;
        }

        private static List<Word> GetGuessWordsForPly(int ply, List<Word> candidateWords)
        {
            int index = Mode switch
            {
                OperationMode.Level1 => 0,
                OperationMode.Level2 => 1 - ply,
                _ => 0,
            };
            if (index >= 0 && index < ExplicitGuessWords.Count)
                return ExplicitGuessWords[index];
            if (HardMode)
                return candidateWords;
            if (AllWords)
                return Words.AllGuesses;
            return Words.AllSolutions;
        }

        private static List<(Word word, int value)> RunLevel1(List<Word> candidateWords, Logger? logger)
        {
            return P(GetGuessWordsForPly(0, candidateWords)).Select(guess =>
            {
                var logger2 = logger?.SubLogger(guess);

                var maxValue = 0;
                foreach (var answer in Words.AllAnswers)
                {
                    var logger3 = logger2?.SubLogger(answer);

                    // count valid candidates:
                    var value = 0;
                    foreach (var word in candidateWords)
                    {
                        if (Word.CheckAnswer(guess, word, answer))
                        {
                            ++value;
                        }
                    }

                    if (value > 0)
                        logger3?.Log($" - {value}");

                    if (value > maxValue)
                        maxValue = value;
                }

                if (maxValue > 0)
                    logger2?.Log($" - {maxValue}");

                return (guess, maxValue);
            }).ToList();
        }

        private static List<(Word word, int value1, int value2)> RunLevel2(List<Word> candidateWords, Logger? logger)
        {
            return GetGuessWordsForPly(1, candidateWords).Select(guess1 =>
            {
                var logger2 = logger?.SubLogger(guess1);

                var maxValue1 = 0;
                var maxValue2 = 0;

                foreach (var answer1 in Words.AllAnswers)
                {
                    var logger3 = logger2?.SubLogger(answer1);
                    var next = RemoveCandidates(candidateWords, guess1, answer1);
                    var values = RunLevel1(next, logger3)
                        .Where(a => a.value > 0).OrderBy(a => a.value).Take(1).ToList();

                    var value1 = next.Count;
                    var value2 = values.Count <= 0 ? 0 : values[0].value;

                    if (value2 > 0)
                        logger3?.Log($" - {value1},{value2}");

                    if (value1 > maxValue1)
                        maxValue1 = value1;
                    if (value2 > maxValue2)
                        maxValue2 = value2;
                }

                if (maxValue2 > 0)
                    logger2?.Log($" - {maxValue1},{maxValue2}");

                return (guess1, maxValue1, maxValue2);
            }).ToList();
        }

        private static List<Word> RemoveCandidates(List<Word> candidates, Word guess, Word answer)
        {
            var result = new List<Word>();

            foreach (var word in candidates)
            {
                if (Word.CheckAnswer(guess, word, answer))
                    result.Add(word);
            }

            return result;
        }

        private static ParallelQuery<T> P<T>(List<T> list)
        {
            return Partitioner.Create(list, false).AsParallel();
        }
    }
}