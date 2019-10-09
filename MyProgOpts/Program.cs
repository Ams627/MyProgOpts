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

        public int OptionParams { get; set; }
        public Option(string longname, char shortname, Type type, int optionParams)
        {
            longname = LongName;
            shortname = ShortName;
            Type = type;
            OptionParams = optionParams;
        }
    }

    class CommandLineParser : ICommandLineParser
    {
        readonly Dictionary<string, int> parsedLongOptions = new Dictionary<string, int>();
        readonly Dictionary<char, int> parsedShortOptions = new Dictionary<char, int>();
        readonly Dictionary<string, Option> handleToOption = new Dictionary<string, Option>();
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

        public void ParseArgs(IEnumerable<string> args, out List<string> invalidOptions)
        {
            int i = 0;
            foreach (var arg in args)
            {
                // double dash ends options - all further args are not options even if they start with dash
                if (arg == "--")
                {
                    break;
                }
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
                        parsedLongOptions.Add(arg.Substring(2), i);
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
                i++;
            }

            var validShortOptions = new HashSet<char>(handleToOption.Select(x => x.Value.ShortName));
            var shortOptionsSet = new HashSet<char>(parsedShortOptions.Keys);
            shortOptionsSet.ExceptWith(validShortOptions);

            var validLongOptions = handleToOption.Select(x => x.Value.LongName);



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
