using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProgOpts
{
    class Option
    {
        public string LongName { get; set; }
        public char ShortName { get; set; }
        public Type Type { get; set; }

        public int NumOptionParams { get; set; }

        public List<string> OptionParams { get; set; }
        public Option(string longname, char shortname, Type type, int optionParams)
        {
            LongName = longname;
            ShortName = shortname;
            Type = type;
            NumOptionParams = optionParams;
            OptionParams = new List<string>();
        }
    }

    class CommandLineParser : ICommandLineParser
    {
        readonly Dictionary<string, int> parsedLongOptions = new Dictionary<string, int>();
        readonly Dictionary<char, int> parsedShortOptions = new Dictionary<char, int>();
        readonly Dictionary<string, List<string>> optionParams = new Dictionary<string, List<string>>();

        readonly Dictionary<string, Option> handleToOption = new Dictionary<string, Option>();

        List<string> restOfArgs = new List<string>();

        int stdin = -1;

        public void AddOptionSpec(string handle, char shortName, string longName, Type type, int optionParams = 1)
        {
            var option = new Option(longName, shortName, type, optionParams);
            handleToOption[handle] = option;
        }

        public bool GetOptionValue(string opt, out int i)
        {
            throw new NotImplementedException();
        }

        public bool GetOptionValue(string opt, out double d)
        {
            throw new NotImplementedException();
        }

        public bool GetOptionValue(string opt)
        {
            throw new NotImplementedException();
        }

        public bool GetOptionValue(string name, out string d)
        {
            throw new NotImplementedException();
        }

        public void ParseArgs(IEnumerable<string> args, out List<string> invalidLongOptions, out List<char> invalidShortOptions)
        {
            var shortLookup = handleToOption.Select(x => x.Value).ToLookup(y => y.ShortName);
            var longLookup = handleToOption.Select(x => x.Value).ToLookup(y => y.LongName);


            int i = 0;
            bool argsFinished = false;
            var skipOptionParmeters = 0;
            Option currentOption = null;

            foreach (var arg in args)
            {
                if (skipOptionParmeters != 0 && currentOption != null)
                {
                    currentOption.OptionParams.Add(arg);
                    skipOptionParmeters--;
                }
                // double dash ends options - all further args are not options even if they start with dash
                else if (arg == "--")
                {
                    argsFinished = true;
                }
                else if (!argsFinished)
                {
                    if (arg[0] == '-')
                    {
                        if (arg.Length == 1)
                        {
                            // a single dash on its own CAN mean stdin
                            stdin = i;
                        }
                        else if (arg[1] == '-')
                        {
                            // here we have two dashes so we have a long option:
                            var parsedLongOption = arg.Substring(2);

                            // hold an option parameter after equals:
                            var tempParam = string.Empty;

                            var splitEquals = parsedLongOption.Split('=').Where(x=>!string.IsNullOrEmpty(x)).ToArray();
                            if (splitEquals.Count() != 0 && splitEquals.Count() !=2 )
                            {
                                throw new CommandLineParseException($"Invalid option: {parsedLongOption}");
                            }

                            if (splitEquals.Count() == 2)
                            {
                                parsedLongOption = splitEquals[0];
                                tempParam = splitEquals[1];
                            }

                            parsedLongOptions.Add(parsedLongOption, i);

                            var foundOptions = longLookup[parsedLongOption];
                            if (foundOptions.Any())
                            {
                                currentOption = foundOptions.First();
                                if (currentOption.NumOptionParams == 1 && !string.IsNullOrEmpty(tempParam))
                                {
                                    currentOption.OptionParams.Add(tempParam);
                                }
                                else if (currentOption.NumOptionParams > 1 && !string.IsNullOrEmpty(tempParam))
                                {
                                    throw new CommandLineParseException($"equals used in the option {currentOption.LongName} but it takes more than one parameter.");
                                }
                                else
                                {
                                    skipOptionParmeters = currentOption.NumOptionParams;
                                }
                            }
                        }
                        else
                        {
                            // here we have a single dash followed by one or more characters which are all short options:
                            foreach (var c in arg.Skip(1))
                            {
                                parsedShortOptions.Add(c, i);
                            }
                        }
                    }
                }
                else
                {
                    restOfArgs.Add(arg);
                }

                i++;
            }

            var validShortOptions = new HashSet<char>(handleToOption.Select(x => x.Value.ShortName));
            var parsedShortOptionsSet = new HashSet<char>(parsedShortOptions.Keys);
            parsedShortOptionsSet.ExceptWith(validShortOptions);
            invalidShortOptions = parsedShortOptionsSet.ToList();

            var validLongOptions = new HashSet<string>(handleToOption.Select(x => x.Value.LongName));
            var parsedLongOptionsSet = new HashSet<string>(parsedLongOptions.Keys);
            parsedLongOptionsSet.ExceptWith(validLongOptions);
            invalidLongOptions = parsedLongOptionsSet.ToList();
        }

        public void ParseArgs(params string[] args)
        {
            ParseArgs(args);
        }
    }
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var args1 = new[] { "--name=1" };
                var options = new CommandLineParser();
                options.AddOptionSpec("name", 'n', "name", typeof(int));
                options.ParseArgs(args1, out var invalidShort, out var invalidLong);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
