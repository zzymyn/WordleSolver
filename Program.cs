using Mono.Options;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace WordleSolver // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        private struct Word : IEquatable<Word>
        {
            public char C0;
            public char C1;
            public char C2;
            public char C3;
            public char C4;

            private string DebuggerDisplay => $"\"{C0}{C1}{C2}{C3}{C4}\"";

            public char this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return C0;
                        case 1: return C1;
                        case 2: return C2;
                        case 3: return C3;
                        case 4: return C4;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0: C0 = value; break;
                        case 1: C1 = value; break;
                        case 2: C2 = value; break;
                        case 3: C3 = value; break;
                        case 4: C4 = value; break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public Word(char c0, char c1, char c2, char c3, char c4)
            {
                C0 = c0;
                C1 = c1;
                C2 = c2;
                C3 = c3;
                C4 = c4;
            }

            public Word(string txt)
            {
                if (txt == null) throw new ArgumentNullException(nameof(txt));
                if (txt.Length != 5) throw new ArgumentException($"Word '{txt}' does not contain 5 letters.");
                C0 = txt[0];
                C1 = txt[1];
                C2 = txt[2];
                C3 = txt[3];
                C4 = txt[4];
            }

            public static implicit operator Word(string txt)
            {
                return new Word(txt);
            }

            public static implicit operator string(Word w)
            {
                return w.ToString();
            }

            public static bool operator ==(Word left, Word right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Word left, Word right)
            {
                return !(left == right);
            }

            public override string ToString()
            {
                return $"{C0}{C1}{C2}{C3}{C4}";
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(C0, C1, C2, C3, C4);
            }

            public override bool Equals(object? obj)
            {
                return obj is Word word && Equals(word);
            }

            public bool Equals(Word other)
            {
                return C0 == other.C0 &&
                       C1 == other.C1 &&
                       C2 == other.C2 &&
                       C3 == other.C3 &&
                       C4 == other.C4;
            }
        }

        private struct Logger
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
                return new Logger(Prefix.Length == 0 ? prefix : $"{Prefix},{prefix}", Level - 1);
            }

            public void Log(string message)
            {
                if (Level >= 0)
                {
                    Console.WriteLine($"{Prefix}{message}");
                }
            }
        }

        private static readonly Word[] AllWords = new Word[]
        {
            "ABACK", "ABASE", "ABATE", "ABBEY", "ABBOT", "ABHOR", "ABIDE", "ABLED", "ABODE", "ABORT", "ABOUT", "ABOVE", "ABUSE", "ABYSS", "ACORN", "ACRID", "ACTOR", "ACUTE", "ADAGE", "ADAPT", "ADEPT", "ADMIN", "ADMIT", "ADOBE", "ADOPT", "ADORE", "ADORN", "ADULT", "AFFIX", "AFIRE", "AFOOT", "AFOUL", "AFTER", "AGAIN", "AGAPE", "AGATE", "AGENT", "AGILE", "AGING", "AGLOW", "AGONY", "AGORA", "AGREE", "AHEAD", "AIDER", "AISLE", "ALARM", "ALBUM", "ALERT", "ALGAE", "ALIBI", "ALIEN", "ALIGN", "ALIKE", "ALIVE", "ALLAY", "ALLEY", "ALLOT", "ALLOW", "ALLOY", "ALOFT", "ALONE", "ALONG", "ALOOF", "ALOUD", "ALPHA", "ALTAR", "ALTER", "AMASS", "AMAZE", "AMBER", "AMBLE", "AMEND", "AMISS", "AMITY", "AMONG", "AMPLE", "AMPLY", "AMUSE", "ANGEL", "ANGER", "ANGLE", "ANGRY", "ANGST", "ANIME", "ANKLE", "ANNEX", "ANNOY", "ANNUL", "ANODE", "ANTIC", "ANVIL", "AORTA", "APART", "APHID", "APING", "APNEA", "APPLE", "APPLY", "APRON", "APTLY", "ARBOR", "ARDOR", "ARENA", "ARGUE", "ARISE", "ARMOR", "AROMA", "AROSE", "ARRAY", "ARROW", "ARSON", "ARTSY", "ASCOT", "ASHEN", "ASIDE", "ASKEW", "ASSAY", "ASSET", "ATOLL", "ATONE", "ATTIC", "AUDIO", "AUDIT", "AUGUR", "AUNTY", "AVAIL", "AVERT", "AVIAN", "AVOID", "AWAIT", "AWAKE", "AWARD", "AWARE", "AWASH", "AWFUL", "AWOKE", "AXIAL", "AXIOM", "AXION", "AZURE", "BACON", "BADGE", "BADLY", "BAGEL", "BAGGY", "BAKER", "BALER", "BALMY", "BANAL", "BANJO", "BARGE", "BARON", "BASAL", "BASIC", "BASIL", "BASIN", "BASIS", "BASTE", "BATCH", "BATHE", "BATON", "BATTY", "BAWDY", "BAYOU", "BEACH", "BEADY", "BEARD", "BEAST", "BEECH", "BEEFY", "BEFIT", "BEGAN", "BEGAT", "BEGET", "BEGIN", "BEGUN", "BEING", "BELCH", "BELIE", "BELLE", "BELLY", "BELOW", "BENCH", "BERET", "BERRY", "BERTH", "BESET", "BETEL", "BEVEL", "BEZEL", "BIBLE", "BICEP", "BIDDY", "BIGOT", "BILGE", "BILLY", "BINGE", "BINGO", "BIOME", "BIRCH", "BIRTH", "BISON", "BITTY", "BLACK", "BLADE", "BLAME", "BLAND", "BLANK", "BLARE", "BLAST", "BLAZE", "BLEAK", "BLEAT", "BLEED", "BLEEP", "BLEND", "BLESS", "BLIMP", "BLIND", "BLINK", "BLISS", "BLITZ", "BLOAT", "BLOCK", "BLOKE", "BLOND", "BLOOD", "BLOOM", "BLOWN", "BLUER", "BLUFF", "BLUNT", "BLURB", "BLURT", "BLUSH", "BOARD", "BOAST", "BOBBY", "BONEY", "BONGO", "BONUS", "BOOBY", "BOOST", "BOOTH", "BOOTY", "BOOZE", "BOOZY", "BORAX", "BORNE", "BOSOM", "BOSSY", "BOTCH", "BOUGH", "BOULE", "BOUND", "BOWEL", "BOXER", "BRACE", "BRAID", "BRAIN", "BRAKE", "BRAND", "BRASH", "BRASS", "BRAVE", "BRAVO", "BRAWL", "BRAWN", "BREAD", "BREAK", "BREED", "BRIAR", "BRIBE", "BRICK", "BRIDE", "BRIEF", "BRINE", "BRING", "BRINK", "BRINY", "BRISK", "BROAD", "BROIL", "BROKE", "BROOD", "BROOK", "BROOM", "BROTH", "BROWN", "BRUNT", "BRUSH", "BRUTE", "BUDDY", "BUDGE", "BUGGY", "BUGLE", "BUILD", "BUILT", "BULGE", "BULKY", "BULLY", "BUNCH", "BUNNY", "BURLY", "BURNT", "BURST", "BUSED", "BUSHY", "BUTCH", "BUTTE", "BUXOM", "BUYER", "BYLAW", "CABAL", "CABBY", "CABIN", "CABLE", "CACAO", "CACHE", "CACTI", "CADDY", "CADET", "CAGEY", "CAIRN", "CAMEL", "CAMEO", "CANAL", "CANDY", "CANNY", "CANOE", "CANON", "CAPER", "CAPUT", "CARAT", "CARGO", "CAROL", "CARRY", "CARVE", "CASTE", "CATCH", "CATER", "CATTY", "CAULK", "CAUSE", "CAVIL", "CEASE", "CEDAR", "CELLO", "CHAFE", "CHAFF", "CHAIN", "CHAIR", "CHALK", "CHAMP", "CHANT", "CHAOS", "CHARD", "CHARM", "CHART", "CHASE", "CHASM", "CHEAP", "CHEAT", "CHECK", "CHEEK", "CHEER", "CHESS", "CHEST", "CHICK", "CHIDE", "CHIEF", "CHILD", "CHILI", "CHILL", "CHIME", "CHINA", "CHIRP", "CHOCK", "CHOIR", "CHOKE", "CHORD", "CHORE", "CHOSE", "CHUCK", "CHUMP", "CHUNK", "CHURN", "CHUTE", "CIDER", "CIGAR", "CINCH", "CIRCA", "CIVIC", "CIVIL", "CLACK", "CLAIM", "CLAMP", "CLANG", "CLANK", "CLASH", "CLASP", "CLASS", "CLEAN", "CLEAR", "CLEAT", "CLEFT", "CLERK", "CLICK", "CLIFF", "CLIMB", "CLING", "CLINK", "CLOAK", "CLOCK", "CLONE", "CLOSE", "CLOTH", "CLOUD", "CLOUT", "CLOVE", "CLOWN", "CLUCK", "CLUED", "CLUMP", "CLUNG", "COACH", "COAST", "COBRA", "COCOA", "COLON", "COLOR", "COMET", "COMFY", "COMIC", "COMMA", "CONCH", "CONDO", "CONIC", "COPSE", "CORAL", "CORER", "CORNY", "COUCH", "COUGH", "COULD", "COUNT", "COUPE", "COURT", "COVEN", "COVER", "COVET", "COVEY", "COWER", "COYLY", "CRACK", "CRAFT", "CRAMP", "CRANE", "CRANK", "CRASH", "CRASS", "CRATE", "CRAVE", "CRAWL", "CRAZE", "CRAZY", "CREAK", "CREAM", "CREDO", "CREED", "CREEK", "CREEP", "CREME", "CREPE", "CREPT", "CRESS", "CREST", "CRICK", "CRIED", "CRIER", "CRIME", "CRIMP", "CRISP", "CROAK", "CROCK", "CRONE", "CRONY", "CROOK", "CROSS", "CROUP", "CROWD", "CROWN", "CRUDE", "CRUEL", "CRUMB", "CRUMP", "CRUSH", "CRUST", "CRYPT", "CUBIC", "CUMIN", "CURIO", "CURLY", "CURRY", "CURSE", "CURVE", "CURVY", "CUTIE", "CYBER", "CYCLE", "CYNIC", "DADDY", "DAILY", "DAIRY", "DAISY", "DALLY", "DANCE", "DANDY", "DATUM", "DAUNT", "DEALT", "DEATH", "DEBAR", "DEBIT", "DEBUG", "DEBUT", "DECAL", "DECAY", "DECOR", "DECOY", "DECRY", "DEFER", "DEIGN", "DEITY", "DELAY", "DELTA", "DELVE", "DEMON", "DEMUR", "DENIM", "DENSE", "DEPOT", "DEPTH", "DERBY", "DETER", "DETOX", "DEUCE", "DEVIL", "DIARY", "DICEY", "DIGIT", "DILLY", "DIMLY", "DINER", "DINGO", "DINGY", "DIODE", "DIRGE", "DIRTY", "DISCO", "DITCH", "DITTO", "DITTY", "DIVER", "DIZZY", "DODGE", "DODGY", "DOGMA", "DOING", "DOLLY", "DONOR", "DONUT", "DOPEY", "DOUBT", "DOUGH", "DOWDY", "DOWEL", "DOWNY", "DOWRY", "DOZEN", "DRAFT", "DRAIN", "DRAKE", "DRAMA", "DRANK", "DRAPE", "DRAWL", "DRAWN", "DREAD", "DREAM", "DRESS", "DRIED", "DRIER", "DRIFT", "DRILL", "DRINK", "DRIVE", "DROIT", "DROLL", "DRONE", "DROOL", "DROOP", "DROSS", "DROVE", "DROWN", "DRUID", "DRUNK", "DRYER", "DRYLY", "DUCHY", "DULLY", "DUMMY", "DUMPY", "DUNCE", "DUSKY", "DUSTY", "DUTCH", "DUVET", "DWARF", "DWELL", "DWELT", "DYING", "EAGER", "EAGLE", "EARLY", "EARTH", "EASEL", "EATEN", "EATER", "EBONY", "ECLAT", "EDICT", "EDIFY", "EERIE", "EGRET", "EIGHT", "EJECT", "EKING", "ELATE", "ELBOW", "ELDER", "ELECT", "ELEGY", "ELFIN", "ELIDE", "ELITE", "ELOPE", "ELUDE", "EMAIL", "EMBED", "EMBER", "EMCEE", "EMPTY", "ENACT", "ENDOW", "ENEMA", "ENEMY", "ENJOY", "ENNUI", "ENSUE", "ENTER", "ENTRY", "ENVOY", "EPOCH", "EPOXY", "EQUAL", "EQUIP", "ERASE", "ERECT", "ERODE", "ERROR", "ERUPT", "ESSAY", "ESTER", "ETHER", "ETHIC", "ETHOS", "ETUDE", "EVADE", "EVENT", "EVERY", "EVICT", "EVOKE", "EXACT", "EXALT", "EXCEL", "EXERT", "EXILE", "EXIST", "EXPEL", "EXTOL", "EXTRA", "EXULT", "EYING", "FABLE", "FACET", "FAINT", "FAIRY", "FAITH", "FALSE", "FANCY", "FANNY", "FARCE", "FATAL", "FATTY", "FAULT", "FAUNA", "FAVOR", "FEAST", "FECAL", "FEIGN", "FELLA", "FELON", "FEMME", "FEMUR", "FENCE", "FERAL", "FERRY", "FETAL", "FETCH", "FETID", "FETUS", "FEVER", "FEWER", "FIBER", "FIBRE", "FICUS", "FIELD", "FIEND", "FIERY", "FIFTH", "FIFTY", "FIGHT", "FILER", "FILET", "FILLY", "FILMY", "FILTH", "FINAL", "FINCH", "FINER", "FIRST", "FISHY", "FIXER", "FIZZY", "FJORD", "FLACK", "FLAIL", "FLAIR", "FLAKE", "FLAKY", "FLAME", "FLANK", "FLARE", "FLASH", "FLASK", "FLECK", "FLEET", "FLESH", "FLICK", "FLIER", "FLING", "FLINT", "FLIRT", "FLOAT", "FLOCK", "FLOOD", "FLOOR", "FLORA", "FLOSS", "FLOUR", "FLOUT", "FLOWN", "FLUFF", "FLUID", "FLUKE", "FLUME", "FLUNG", "FLUNK", "FLUSH", "FLUTE", "FLYER", "FOAMY", "FOCAL", "FOCUS", "FOGGY", "FOIST", "FOLIO", "FOLLY", "FORAY", "FORCE", "FORGE", "FORGO", "FORTE", "FORTH", "FORTY", "FORUM", "FOUND", "FOYER", "FRAIL", "FRAME", "FRANK", "FRAUD", "FREAK", "FREED", "FREER", "FRESH", "FRIAR", "FRIED", "FRILL", "FRISK", "FRITZ", "FROCK", "FROND", "FRONT", "FROST", "FROTH", "FROWN", "FROZE", "FRUIT", "FUDGE", "FUGUE", "FULLY", "FUNGI", "FUNKY", "FUNNY", "FUROR", "FURRY", "FUSSY", "FUZZY", "GAFFE", "GAILY", "GAMER", "GAMMA", "GAMUT", "GASSY", "GAUDY", "GAUGE", "GAUNT", "GAUZE", "GAVEL", "GAWKY", "GAYER", "GAYLY", "GAZER", "GECKO", "GEEKY", "GEESE", "GENIE", "GENRE", "GHOST", "GHOUL", "GIANT", "GIDDY", "GIPSY", "GIRLY", "GIRTH", "GIVEN", "GIVER", "GLADE", "GLAND", "GLARE", "GLASS", "GLAZE", "GLEAM", "GLEAN", "GLIDE", "GLINT", "GLOAT", "GLOBE", "GLOOM", "GLORY", "GLOSS", "GLOVE", "GLYPH", "GNASH", "GNOME", "GODLY", "GOING", "GOLEM", "GOLLY", "GONAD", "GONER", "GOODY", "GOOEY", "GOOFY", "GOOSE", "GORGE", "GOUGE", "GOURD", "GRACE", "GRADE", "GRAFT", "GRAIL", "GRAIN", "GRAND", "GRANT", "GRAPE", "GRAPH", "GRASP", "GRASS", "GRATE", "GRAVE", "GRAVY", "GRAZE", "GREAT", "GREED", "GREEN", "GREET", "GRIEF", "GRILL", "GRIME", "GRIMY", "GRIND", "GRIPE", "GROAN", "GROIN", "GROOM", "GROPE", "GROSS", "GROUP", "GROUT", "GROVE", "GROWL", "GROWN", "GRUEL", "GRUFF", "GRUNT", "GUARD", "GUAVA", "GUESS", "GUEST", "GUIDE", "GUILD", "GUILE", "GUILT", "GUISE", "GULCH", "GULLY", "GUMBO", "GUMMY", "GUPPY", "GUSTO", "GUSTY", "GYPSY", "HABIT", "HAIRY", "HALVE", "HANDY", "HAPPY", "HARDY", "HAREM", "HARPY", "HARRY", "HARSH", "HASTE", "HASTY", "HATCH", "HATER", "HAUNT", "HAUTE", "HAVEN", "HAVOC", "HAZEL", "HEADY", "HEARD", "HEART", "HEATH", "HEAVE", "HEAVY", "HEDGE", "HEFTY", "HEIST", "HELIX", "HELLO", "HENCE", "HERON", "HILLY", "HINGE", "HIPPO", "HIPPY", "HITCH", "HOARD", "HOBBY", "HOIST", "HOLLY", "HOMER", "HONEY", "HONOR", "HORDE", "HORNY", "HORSE", "HOTEL", "HOTLY", "HOUND", "HOUSE", "HOVEL", "HOVER", "HOWDY", "HUMAN", "HUMID", "HUMOR", "HUMPH", "HUMUS", "HUNCH", "HUNKY", "HURRY", "HUSKY", "HUSSY", "HUTCH", "HYDRO", "HYENA", "HYMEN", "HYPER", "ICILY", "ICING", "IDEAL", "IDIOM", "IDIOT", "IDLER", "IDYLL", "IGLOO", "ILIAC", "IMAGE", "IMBUE", "IMPEL", "IMPLY", "INANE", "INBOX", "INCUR", "INDEX", "INEPT", "INERT", "INFER", "INGOT", "INLAY", "INLET", "INNER", "INPUT", "INTER", "INTRO", "IONIC", "IRATE", "IRONY", "ISLET", "ISSUE", "ITCHY", "IVORY", "JAUNT", "JAZZY", "JELLY", "JERKY", "JETTY", "JEWEL", "JIFFY", "JOINT", "JOIST", "JOKER", "JOLLY", "JOUST", "JUDGE", "JUICE", "JUICY", "JUMBO", "JUMPY", "JUNTA", "JUNTO", "JUROR", "KAPPA", "KARMA", "KAYAK", "KEBAB", "KHAKI", "KINKY", "KIOSK", "KITTY", "KNACK", "KNAVE", "KNEAD", "KNEED", "KNEEL", "KNELT", "KNIFE", "KNOCK", "KNOLL", "KNOWN", "KOALA", "KRILL", "LABEL", "LABOR", "LADEN", "LADLE", "LAGER", "LANCE", "LANKY", "LAPEL", "LAPSE", "LARGE", "LARVA", "LASSO", "LATCH", "LATER", "LATHE", "LATTE", "LAUGH", "LAYER", "LEACH", "LEAFY", "LEAKY", "LEANT", "LEAPT", "LEARN", "LEASE", "LEASH", "LEAST", "LEAVE", "LEDGE", "LEECH", "LEERY", "LEFTY", "LEGAL", "LEGGY", "LEMON", "LEMUR", "LEPER", "LEVEL", "LEVER", "LIBEL", "LIEGE", "LIGHT", "LIKEN", "LILAC", "LIMBO", "LIMIT", "LINEN", "LINER", "LINGO", "LIPID", "LITHE", "LIVER", "LIVID", "LLAMA", "LOAMY", "LOATH", "LOBBY", "LOCAL", "LOCUS", "LODGE", "LOFTY", "LOGIC", "LOGIN", "LOOPY", "LOOSE", "LORRY", "LOSER", "LOUSE", "LOUSY", "LOVER", "LOWER", "LOWLY", "LOYAL", "LUCID", "LUCKY", "LUMEN", "LUMPY", "LUNAR", "LUNCH", "LUNGE", "LUPUS", "LURCH", "LURID", "LUSTY", "LYING", "LYMPH", "LYNCH", "LYRIC", "MACAW", "MACHO", "MACRO", "MADAM", "MADLY", "MAFIA", "MAGIC", "MAGMA", "MAIZE", "MAJOR", "MAKER", "MAMBO", "MAMMA", "MAMMY", "MANGA", "MANGE", "MANGO", "MANGY", "MANIA", "MANIC", "MANLY", "MANOR", "MAPLE", "MARCH", "MARRY", "MARSH", "MASON", "MASSE", "MATCH", "MATEY", "MAUVE", "MAXIM", "MAYBE", "MAYOR", "MEALY", "MEANT", "MEATY", "MECCA", "MEDAL", "MEDIA", "MEDIC", "MELEE", "MELON", "MERCY", "MERGE", "MERIT", "MERRY", "METAL", "METER", "METRO", "MICRO", "MIDGE", "MIDST", "MIGHT", "MILKY", "MIMIC", "MINCE", "MINER", "MINIM", "MINOR", "MINTY", "MINUS", "MIRTH", "MISER", "MISSY", "MOCHA", "MODAL", "MODEL", "MODEM", "MOGUL", "MOIST", "MOLAR", "MOLDY", "MONEY", "MONTH", "MOODY", "MOOSE", "MORAL", "MORON", "MORPH", "MOSSY", "MOTEL", "MOTIF", "MOTOR", "MOTTO", "MOULT", "MOUND", "MOUNT", "MOURN", "MOUSE", "MOUTH", "MOVER", "MOVIE", "MOWER", "MUCKY", "MUCUS", "MUDDY", "MULCH", "MUMMY", "MUNCH", "MURAL", "MURKY", "MUSHY", "MUSIC", "MUSKY", "MUSTY", "MYRRH", "NADIR", "NAIVE", "NANNY", "NASAL", "NASTY", "NATAL", "NAVAL", "NAVEL", "NEEDY", "NEIGH", "NERDY", "NERVE", "NEVER", "NEWER", "NEWLY", "NICER", "NICHE", "NIECE", "NIGHT", "NINJA", "NINNY", "NINTH", "NOBLE", "NOBLY", "NOISE", "NOISY", "NOMAD", "NOOSE", "NORTH", "NOSEY", "NOTCH", "NOVEL", "NUDGE", "NURSE", "NUTTY", "NYLON", "NYMPH", "OAKEN", "OBESE", "OCCUR", "OCEAN", "OCTAL", "OCTET", "ODDER", "ODDLY", "OFFAL", "OFFER", "OFTEN", "OLDEN", "OLDER", "OLIVE", "OMBRE", "OMEGA", "ONION", "ONSET", "OPERA", "OPINE", "OPIUM", "OPTIC", "ORBIT", "ORDER", "ORGAN", "OTHER", "OTTER", "OUGHT", "OUNCE", "OUTDO", "OUTER", "OUTGO", "OVARY", "OVATE", "OVERT", "OVINE", "OVOID", "OWING", "OWNER", "OXIDE", "OZONE", "PADDY", "PAGAN", "PAINT", "PALER", "PALSY", "PANEL", "PANIC", "PANSY", "PAPAL", "PAPER", "PARER", "PARKA", "PARRY", "PARSE", "PARTY", "PASTA", "PASTE", "PASTY", "PATCH", "PATIO", "PATSY", "PATTY", "PAUSE", "PAYEE", "PAYER", "PEACE", "PEACH", "PEARL", "PECAN", "PEDAL", "PENAL", "PENCE", "PENNE", "PENNY", "PERCH", "PERIL", "PERKY", "PESKY", "PESTO", "PETAL", "PETTY", "PHASE", "PHONE", "PHONY", "PHOTO", "PIANO", "PICKY", "PIECE", "PIETY", "PIGGY", "PILOT", "PINCH", "PINEY", "PINKY", "PINTO", "PIPER", "PIQUE", "PITCH", "PITHY", "PIVOT", "PIXEL", "PIXIE", "PIZZA", "PLACE", "PLAID", "PLAIN", "PLAIT", "PLANE", "PLANK", "PLANT", "PLATE", "PLAZA", "PLEAD", "PLEAT", "PLIED", "PLIER", "PLUCK", "PLUMB", "PLUME", "PLUMP", "PLUNK", "PLUSH", "POESY", "POINT", "POISE", "POKER", "POLAR", "POLKA", "POLYP", "POOCH", "POPPY", "PORCH", "POSER", "POSIT", "POSSE", "POUCH", "POUND", "POUTY", "POWER", "PRANK", "PRAWN", "PREEN", "PRESS", "PRICE", "PRICK", "PRIDE", "PRIED", "PRIME", "PRIMO", "PRINT", "PRIOR", "PRISM", "PRIVY", "PRIZE", "PROBE", "PRONE", "PRONG", "PROOF", "PROSE", "PROUD", "PROVE", "PROWL", "PROXY", "PRUDE", "PRUNE", "PSALM", "PUBIC", "PUDGY", "PUFFY", "PULPY", "PULSE", "PUNCH", "PUPAL", "PUPIL", "PUPPY", "PUREE", "PURER", "PURGE", "PURSE", "PUSHY", "PUTTY", "PYGMY", "QUACK", "QUAIL", "QUAKE", "QUALM", "QUARK", "QUART", "QUASH", "QUASI", "QUEEN", "QUEER", "QUELL", "QUERY", "QUEST", "QUEUE", "QUICK", "QUIET", "QUILL", "QUILT", "QUIRK", "QUITE", "QUOTA", "QUOTE", "QUOTH", "RABBI", "RABID", "RACER", "RADAR", "RADII", "RADIO", "RAINY", "RAISE", "RAJAH", "RALLY", "RALPH", "RAMEN", "RANCH", "RANDY", "RANGE", "RAPID", "RARER", "RASPY", "RATIO", "RATTY", "RAVEN", "RAYON", "RAZOR", "REACH", "REACT", "READY", "REALM", "REARM", "REBAR", "REBEL", "REBUS", "REBUT", "RECAP", "RECUR", "RECUT", "REEDY", "REFER", "REFIT", "REGAL", "REHAB", "REIGN", "RELAX", "RELAY", "RELIC", "REMIT", "RENAL", "RENEW", "REPAY", "REPEL", "REPLY", "RERUN", "RESET", "RESIN", "RETCH", "RETRO", "RETRY", "REUSE", "REVEL", "REVUE", "RHINO", "RHYME", "RIDER", "RIDGE", "RIFLE", "RIGHT", "RIGID", "RIGOR", "RINSE", "RIPEN", "RIPER", "RISEN", "RISER", "RISKY", "RIVAL", "RIVER", "RIVET", "ROACH", "ROAST", "ROBIN", "ROBOT", "ROCKY", "RODEO", "ROGER", "ROGUE", "ROOMY", "ROOST", "ROTOR", "ROUGE", "ROUGH", "ROUND", "ROUSE", "ROUTE", "ROVER", "ROWDY", "ROWER", "ROYAL", "RUDDY", "RUDER", "RUGBY", "RULER", "RUMBA", "RUMOR", "RUPEE", "RURAL", "RUSTY", "SADLY", "SAFER", "SAINT", "SALAD", "SALLY", "SALON", "SALSA", "SALTY", "SALVE", "SALVO", "SANDY", "SANER", "SAPPY", "SASSY", "SATIN", "SATYR", "SAUCE", "SAUCY", "SAUNA", "SAUTE", "SAVOR", "SAVOY", "SAVVY", "SCALD", "SCALE", "SCALP", "SCALY", "SCAMP", "SCANT", "SCARE", "SCARF", "SCARY", "SCENE", "SCENT", "SCION", "SCOFF", "SCOLD", "SCONE", "SCOOP", "SCOPE", "SCORE", "SCORN", "SCOUR", "SCOUT", "SCOWL", "SCRAM", "SCRAP", "SCREE", "SCREW", "SCRUB", "SCRUM", "SCUBA", "SEDAN", "SEEDY", "SEGUE", "SEIZE", "SEMEN", "SENSE", "SEPIA", "SERIF", "SERUM", "SERVE", "SETUP", "SEVEN", "SEVER", "SEWER", "SHACK", "SHADE", "SHADY", "SHAFT", "SHAKE", "SHAKY", "SHALE", "SHALL", "SHALT", "SHAME", "SHANK", "SHAPE", "SHARD", "SHARE", "SHARK", "SHARP", "SHAVE", "SHAWL", "SHEAR", "SHEEN", "SHEEP", "SHEER", "SHEET", "SHEIK", "SHELF", "SHELL", "SHIED", "SHIFT", "SHINE", "SHINY", "SHIRE", "SHIRK", "SHIRT", "SHOAL", "SHOCK", "SHONE", "SHOOK", "SHOOT", "SHORE", "SHORN", "SHORT", "SHOUT", "SHOVE", "SHOWN", "SHOWY", "SHREW", "SHRUB", "SHRUG", "SHUCK", "SHUNT", "SHUSH", "SHYLY", "SIEGE", "SIEVE", "SIGHT", "SIGMA", "SILKY", "SILLY", "SINCE", "SINEW", "SINGE", "SIREN", "SISSY", "SIXTH", "SIXTY", "SKATE", "SKIER", "SKIFF", "SKILL", "SKIMP", "SKIRT", "SKULK", "SKULL", "SKUNK", "SLACK", "SLAIN", "SLANG", "SLANT", "SLASH", "SLATE", "SLAVE", "SLEEK", "SLEEP", "SLEET", "SLEPT", "SLICE", "SLICK", "SLIDE", "SLIME", "SLIMY", "SLING", "SLINK", "SLOOP", "SLOPE", "SLOSH", "SLOTH", "SLUMP", "SLUNG", "SLUNK", "SLURP", "SLUSH", "SLYLY", "SMACK", "SMALL", "SMART", "SMASH", "SMEAR", "SMELL", "SMELT", "SMILE", "SMIRK", "SMITE", "SMITH", "SMOCK", "SMOKE", "SMOKY", "SMOTE", "SNACK", "SNAIL", "SNAKE", "SNAKY", "SNARE", "SNARL", "SNEAK", "SNEER", "SNIDE", "SNIFF", "SNIPE", "SNOOP", "SNORE", "SNORT", "SNOUT", "SNOWY", "SNUCK", "SNUFF", "SOAPY", "SOBER", "SOGGY", "SOLAR", "SOLID", "SOLVE", "SONAR", "SONIC", "SOOTH", "SOOTY", "SORRY", "SOUND", "SOUTH", "SOWER", "SPACE", "SPADE", "SPANK", "SPARE", "SPARK", "SPASM", "SPAWN", "SPEAK", "SPEAR", "SPECK", "SPEED", "SPELL", "SPELT", "SPEND", "SPENT", "SPERM", "SPICE", "SPICY", "SPIED", "SPIEL", "SPIKE", "SPIKY", "SPILL", "SPILT", "SPINE", "SPINY", "SPIRE", "SPITE", "SPLAT", "SPLIT", "SPOIL", "SPOKE", "SPOOF", "SPOOK", "SPOOL", "SPOON", "SPORE", "SPORT", "SPOUT", "SPRAY", "SPREE", "SPRIG", "SPUNK", "SPURN", "SPURT", "SQUAD", "SQUAT", "SQUIB", "STACK", "STAFF", "STAGE", "STAID", "STAIN", "STAIR", "STAKE", "STALE", "STALK", "STALL", "STAMP", "STAND", "STANK", "STARE", "STARK", "START", "STASH", "STATE", "STAVE", "STEAD", "STEAK", "STEAL", "STEAM", "STEED", "STEEL", "STEEP", "STEER", "STEIN", "STERN", "STICK", "STIFF", "STILL", "STILT", "STING", "STINK", "STINT", "STOCK", "STOIC", "STOKE", "STOLE", "STOMP", "STONE", "STONY", "STOOD", "STOOL", "STOOP", "STORE", "STORK", "STORM", "STORY", "STOUT", "STOVE", "STRAP", "STRAW", "STRAY", "STRIP", "STRUT", "STUCK", "STUDY", "STUFF", "STUMP", "STUNG", "STUNK", "STUNT", "STYLE", "SUAVE", "SUGAR", "SUING", "SUITE", "SULKY", "SULLY", "SUMAC", "SUNNY", "SUPER", "SURER", "SURGE", "SURLY", "SUSHI", "SWAMI", "SWAMP", "SWARM", "SWASH", "SWATH", "SWEAR", "SWEAT", "SWEEP", "SWEET", "SWELL", "SWEPT", "SWIFT", "SWILL", "SWINE", "SWING", "SWIRL", "SWISH", "SWOON", "SWOOP", "SWORD", "SWORE", "SWORN", "SWUNG", "SYNOD", "SYRUP", "TABBY", "TABLE", "TABOO", "TACIT", "TACKY", "TAFFY", "TAINT", "TAKEN", "TAKER", "TALLY", "TALON", "TAMER", "TANGO", "TANGY", "TAPER", "TAPIR", "TARDY", "TAROT", "TASTE", "TASTY", "TATTY", "TAUNT", "TAWNY", "TEACH", "TEARY", "TEASE", "TEDDY", "TEETH", "TEMPO", "TENET", "TENOR", "TENSE", "TENTH", "TEPEE", "TEPID", "TERRA", "TERSE", "TESTY", "THANK", "THEFT", "THEIR", "THEME", "THERE", "THESE", "THETA", "THICK", "THIEF", "THIGH", "THING", "THINK", "THIRD", "THONG", "THORN", "THOSE", "THREE", "THREW", "THROB", "THROW", "THRUM", "THUMB", "THUMP", "THYME", "TIARA", "TIBIA", "TIDAL", "TIGER", "TIGHT", "TILDE", "TIMER", "TIMID", "TIPSY", "TITAN", "TITHE", "TITLE", "TOAST", "TODAY", "TODDY", "TOKEN", "TONAL", "TONGA", "TONIC", "TOOTH", "TOPAZ", "TOPIC", "TORCH", "TORSO", "TORUS", "TOTAL", "TOTEM", "TOUCH", "TOUGH", "TOWEL", "TOWER", "TOXIC", "TOXIN", "TRACE", "TRACK", "TRACT", "TRADE", "TRAIL", "TRAIN", "TRAIT", "TRAMP", "TRASH", "TRAWL", "TREAD", "TREAT", "TREND", "TRIAD", "TRIAL", "TRIBE", "TRICE", "TRICK", "TRIED", "TRIPE", "TRITE", "TROLL", "TROOP", "TROPE", "TROUT", "TROVE", "TRUCE", "TRUCK", "TRUER", "TRULY", "TRUMP", "TRUNK", "TRUSS", "TRUST", "TRUTH", "TRYST", "TUBAL", "TUBER", "TULIP", "TULLE", "TUMOR", "TUNIC", "TURBO", "TUTOR", "TWANG", "TWEAK", "TWEED", "TWEET", "TWICE", "TWINE", "TWIRL", "TWIST", "TWIXT", "TYING", "UDDER", "ULCER", "ULTRA", "UMBRA", "UNCLE", "UNCUT", "UNDER", "UNDID", "UNDUE", "UNFED", "UNFIT", "UNIFY", "UNION", "UNITE", "UNITY", "UNLIT", "UNMET", "UNSET", "UNTIE", "UNTIL", "UNWED", "UNZIP", "UPPER", "UPSET", "URBAN", "URINE", "USAGE", "USHER", "USING", "USUAL", "USURP", "UTILE", "UTTER", "VAGUE", "VALET", "VALID", "VALOR", "VALUE", "VALVE", "VAPID", "VAPOR", "VAULT", "VAUNT", "VEGAN", "VENOM", "VENUE", "VERGE", "VERSE", "VERSO", "VERVE", "VICAR", "VIDEO", "VIGIL", "VIGOR", "VILLA", "VINYL", "VIOLA", "VIPER", "VIRAL", "VIRUS", "VISIT", "VISOR", "VISTA", "VITAL", "VIVID", "VIXEN", "VOCAL", "VODKA", "VOGUE", "VOICE", "VOILA", "VOMIT", "VOTER", "VOUCH", "VOWEL", "VYING", "WACKY", "WAFER", "WAGER", "WAGON", "WAIST", "WAIVE", "WALTZ", "WARTY", "WASTE", "WATCH", "WATER", "WAVER", "WAXEN", "WEARY", "WEAVE", "WEDGE", "WEEDY", "WEIGH", "WEIRD", "WELCH", "WELSH", "WENCH", "WHACK", "WHALE", "WHARF", "WHEAT", "WHEEL", "WHELP", "WHERE", "WHICH", "WHIFF", "WHILE", "WHINE", "WHINY", "WHIRL", "WHISK", "WHITE", "WHOLE", "WHOOP", "WHOSE", "WIDEN", "WIDER", "WIDOW", "WIDTH", "WIELD", "WIGHT", "WILLY", "WIMPY", "WINCE", "WINCH", "WINDY", "WISER", "WISPY", "WITCH", "WITTY", "WOKEN", "WOMAN", "WOMEN", "WOODY", "WOOER", "WOOLY", "WOOZY", "WORDY", "WORLD", "WORRY", "WORSE", "WORST", "WORTH", "WOULD", "WOUND", "WOVEN", "WRACK", "WRATH", "WREAK", "WRECK", "WREST", "WRING", "WRIST", "WRITE", "WRONG", "WROTE", "WRUNG", "WRYLY", "YACHT", "YEARN", "YEAST", "YIELD", "YOUNG", "YOUTH", "ZEBRA", "ZESTY", "ZONAL"
        };
        private static readonly Word[] AllAnswers = new Word[]
        {
            "00000", "00001", "00002", "00010", "00011", "00012", "00020", "00021", "00022", "00100", "00101", "00102", "00110", "00111", "00112", "00120", "00121", "00122", "00200", "00201", "00202", "00210", "00211", "00212", "00220", "00221", "00222", "01000", "01001", "01002", "01010", "01011", "01012", "01020", "01021", "01022", "01100", "01101", "01102", "01110", "01111", "01112", "01120", "01121", "01122", "01200", "01201", "01202", "01210", "01211", "01212", "01220", "01221", "01222", "02000", "02001", "02002", "02010", "02011", "02012", "02020", "02021", "02022", "02100", "02101", "02102", "02110", "02111", "02112", "02120", "02121", "02122", "02200", "02201", "02202", "02210", "02211", "02212", "02220", "02221", "02222", "10000", "10001", "10002", "10010", "10011", "10012", "10020", "10021", "10022", "10100", "10101", "10102", "10110", "10111", "10112", "10120", "10121", "10122", "10200", "10201", "10202", "10210", "10211", "10212", "10220", "10221", "10222", "11000", "11001", "11002", "11010", "11011", "11012", "11020", "11021", "11022", "11100", "11101", "11102", "11110", "11111", "11112", "11120", "11121", "11122", "11200", "11201", "11202", "11210", "11211", "11212", "11220", "11221", "11222", "12000", "12001", "12002", "12010", "12011", "12012", "12020", "12021", "12022", "12100", "12101", "12102", "12110", "12111", "12112", "12120", "12121", "12122", "12200", "12201", "12202", "12210", "12211", "12212", "12220", "12221", "12222", "20000", "20001", "20002", "20010", "20011", "20012", "20020", "20021", "20022", "20100", "20101", "20102", "20110", "20111", "20112", "20120", "20121", "20122", "20200", "20201", "20202", "20210", "20211", "20212", "20220", "20221", "20222", "21000", "21001", "21002", "21010", "21011", "21012", "21020", "21021", "21022", "21100", "21101", "21102", "21110", "21111", "21112", "21120", "21121", "21122", "21200", "21201", "21202", "21210", "21211", "21212", "21220", "21221", "21222", "22000", "22001", "22002", "22010", "22011", "22012", "22020", "22021", "22022", "22100", "22101", "22102", "22110", "22111", "22112", "22120", "22121", "22122", "22200", "22201", "22202", "22210", "22211", "22212", "22220", "22221", "22222"
        };

        private static readonly List<Word> RestrictWords = new();
        private static int Level = 1;
        private static bool PrintHelp;
        private static int Verbosity;

        static int Main(string[] args)
        {
            var options = new OptionSet()
            {
                { "?|help", "print help.", _ => PrintHelp=true },
                { "w|words=", "only try these words.", a =>
                {
                    foreach (var word in a.Split(' ', ','))
                    {
                        RestrictWords.Add(word);
                    }
                }},
                { "l|level=", "set operation level 1, 2, or 3 (default level 1).", (int a) => Level = a },
                { "v|verbose", "increase verbosity.", _ => ++Verbosity },
            };

            try
            {
                var extraArgs = options.Parse(args);
                if (extraArgs.Count > 0)
                    throw new Exception("too many arguments.");
                if (Level < 1 || Level > 3)
                    throw new Exception("operation level can only be 1, 2 or 3.");
                if (Level == 3 && RestrictWords.Count <= 0)
                    throw new Exception("must provide word list for operation level 3.");
            }
            catch (OptionException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                options.WriteOptionDescriptions(Console.Error);
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
                options.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            var testWords = new List<Word>(AllWords);
            if (RestrictWords.Count > 0)
            {
                testWords = RestrictWords;
            }

            var allValues = RunLevel(testWords, Level, logger);

            logger.Log("");
            Console.WriteLine("Top 10:");

            foreach (var (path, value) in allValues.Where(a => a.value > 0).OrderBy(a => a.value).Take(10))
            {
                Console.WriteLine($"{path}: {value}");
            }

            return 0;
        }

        private static List<(string path, int value)> RunLevel(List<Word> testWords, int level, Logger logger)
        {
            if (level <= 1)
            {
                return testWords.Select(guess =>
                {
                    return (guess.ToString(), RunLevel1(AllWords, guess, logger.SubLogger(guess)));
                }).ToList();
            }
            else if (level <= 2)
            {
                return testWords.Select(guess =>
                {
                    return (guess.ToString(), RunLevel2(AllWords, guess, logger.SubLogger(guess)));
                }).ToList();
            }
            else
            {
                return testWords.SelectMany(guess1 =>
                {
                    return AllWords.AsParallel().Select(guess2 =>
                    {
                        var logger2 = logger.SubLogger($"{guess1},{guess2}");

                        var mmr = AllAnswers.Select(answer1 =>
                        {
                            var next = RemoveCandidates(AllWords, guess1, answer1);
                            return RunLevel1(next, guess2, logger2.SubLogger(answer1));
                        }).ToList();

                        var value = mmr.Count <= 0 ? 0 : mmr.Max();
                        logger2.Log($": {value}");
                        return ($"{guess1},{guess2}", value);
                    });
                }).ToList();
            }
        }

        private static int RunLevel1(IReadOnlyList<Word> words, Word guess, Logger logger)
        {
            var answers = AllAnswers.AsParallel().Select(answer =>
            {
                return RunLevel1Inner(words, guess, answer, logger.SubLogger(answer));
            }).ToList();
            var value = answers.Count <= 0 ? 0 : answers.Max();
            logger.Log($": {value}");
            return value;
        }

        private static int RunLevel1Inner(IReadOnlyList<Word> words, Word guess, Word answer, Logger logger)
        {
            var next = RemoveCandidates(words, guess, answer);
            var value = next.Count;
            logger.Log($": {value}");
            return value;
        }

        private static int RunLevel2(IReadOnlyList<Word> words, Word guess, Logger logger)
        {
            var answers = AllAnswers.AsParallel().Select(answer =>
            {
                return RunLevel2Inner(words, guess, answer, logger.SubLogger(answer));
            }).ToList();
            var value = answers.Count <= 0 ? 0 : answers.Max();
            logger.Log($": {value}");
            return value;
        }

        private static int RunLevel2Inner(IReadOnlyList<Word> words, Word guess, Word answer, Logger logger)
        {
            var next = RemoveCandidates(words, guess, answer);
            var mmr = MinMaxNextGuess(next);
            var value = mmr.Count <= 0 ? 0 : mmr.Max();
            logger.Log($": {value}");
            return value;
        }

        private static List<Word> RemoveCandidates(IReadOnlyList<Word> candidates, Word guess, Word answer)
        {
            var result = new List<Word>();

            foreach (var word in candidates)
            {
                if (GetAnswer(word, guess) == answer)
                    result.Add(word);
            }

            return result;
        }

        private static List<int> MinMaxNextGuess(IReadOnlyList<Word> candidates)
        {
            return candidates.AsParallel().Select(guess =>
            {
                return AllAnswers.Max(answer =>
                {
                    return candidates.Count(solution =>
                    {
                        return GetAnswer(guess, solution) == answer;
                    });
                });
            }).ToList();
        }

        private static Word GetAnswer(Word gs, Word ss)
        {
            var rs = new Word();
            var vs = new Word();

            for (int i = 0; i < 5; i++)
            {
                rs[i] = '0';
            }

            for (var i = 0; i < 5; ++i)
            {
                if (gs[i] == ss[i])
                {
                    rs[i] = '2';
                    vs[i] = '1';
                }
            }

            for (var i = 0; i < 5; ++i)
            {
                for (var j = 0; j < 5; ++j)
                {
                    if (vs[j] == '\0' && gs[i] == ss[j])
                    {
                        rs[i] = '1';
                        vs[j] = '1';
                        break;
                    }
                }
            }

            return rs;
        }
    }
}