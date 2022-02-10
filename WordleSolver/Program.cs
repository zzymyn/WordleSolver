﻿using Mono.Options;
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
                return new Logger(Prefix.Length == 0 ? prefix : $"{Prefix}:{prefix}", Level - 1);
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
            "00000", "00001", "00002", "00010", "00011", "00012", "00020", "00021", "00022", "00100", "00101", "00102", "00110", "00111", "00112", "00120", "00121", "00122", "00200", "00201", "00202", "00210", "00211", "00212", "00220", "00221", "00222", "01000", "01001", "01002", "01010", "01011", "01012", "01020", "01021", "01022", "01100", "01101", "01102", "01110", "01111", "01112", "01120", "01121", "01122", "01200", "01201", "01202", "01210", "01211", "01212", "01220", "01221", "01222", "02000", "02001", "02002", "02010", "02011", "02012", "02020", "02021", "02022", "02100", "02101", "02102", "02110", "02111", "02112", "02120", "02121", "02122", "02200", "02201", "02202", "02210", "02211", "02212", "02220", "02221", "02222", "10000", "10001", "10002", "10010", "10011", "10012", "10020", "10021", "10022", "10100", "10101", "10102", "10110", "10111", "10112", "10120", "10121", "10122", "10200", "10201", "10202", "10210", "10211", "10212", "10220", "10221", "10222", "11000", "11001", "11002", "11010", "11011", "11012", "11020", "11021", "11022", "11100", "11101", "11102", "11110", "11111", "11112", "11120", "11121", "11122", "11200", "11201", "11202", "11210", "11211", "11212", "11220", "11221", "11222", "12000", "12001", "12002", "12010", "12011", "12012", "12020", "12021", "12022", "12100", "12101", "12102", "12110", "12111", "12112", "12120", "12121", "12122", "12200", "12201", "12202", "12210", "12211", "12212", "12220", "12221", "20000", "20001", "20002", "20010", "20011", "20012", "20020", "20021", "20022", "20100", "20101", "20102", "20110", "20111", "20112", "20120", "20121", "20122", "20200", "20201", "20202", "20210", "20211", "20212", "20220", "20221", "20222", "21000", "21001", "21002", "21010", "21011", "21012", "21020", "21021", "21022", "21100", "21101", "21102", "21110", "21111", "21112", "21120", "21121", "21122", "21200", "21201", "21202", "21210", "21211", "21212", "21220", "21221", "22000", "22001", "22002", "22010", "22011", "22012", "22020", "22021", "22022", "22100", "22101", "22102", "22110", "22111", "22112", "22120", "22121", "22200", "22201", "22202", "22210", "22211", "22220", "22222"
        };
        private static readonly Regex ArgMatch = new(@"^([a-zA-Z]{5}):([012]{5})$");

        private static readonly List<(Word guess, Word answer)> Guesses = new();
        private static readonly List<List<Word>> ExplicitGuessWords = new();
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

            var candidateWords = new List<Word>(AllWords);
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
            return new(AllWords);
        }

        private static List<(Word word, int value)> RunLevel1(List<Word> candidateWords, Logger logger)
        {
            return P(GetGuessWordsForPly(0, candidateWords)).Select(guess =>
            {
                var logger2 = logger.SubLogger(guess);

                var maxValue = 0;
                foreach (var answer in AllAnswers)
                {
                    var logger3 = logger2.SubLogger(answer);

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
                        logger3.Log($" - {value}");

                    if (value > maxValue)
                        maxValue = value;
                }

                if (maxValue > 0)
                    logger2.Log($" - {maxValue}");

                return (guess, maxValue);
            }).ToList();
        }

        private static List<(Word word, int value1, int value2)> RunLevel2(List<Word> candidateWords, Logger logger)
        {
            return GetGuessWordsForPly(1, candidateWords).Select(guess1 =>
            {
                var logger2 = logger.SubLogger(guess1);

                var values = AllAnswers.Select(answer1 =>
                {
                    var logger3 = logger2.SubLogger(answer1);
                    var next = RemoveCandidates(candidateWords, guess1, answer1);
                    var values = RunLevel1(next, logger3)
                        .Where(a => a.value > 0).OrderBy(a => a.value).Take(1).ToList();

                    var value1 = next.Count;
                    var value2 = values.Count <= 0 ? 0 : values[0].value;
                    if (value2 > 0)
                    {
                        logger3.Log($" - {value1},{value2}");
                    }
                    return (value1, value2);
                }).ToList();

                var value1 = values.Max(a => a.value1);
                var value2 = values.Max(a => a.value2);

                if (value2 > 0)
                    logger2.Log($" - {value1},{value2}");

                return (guess1, value1, value2);
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