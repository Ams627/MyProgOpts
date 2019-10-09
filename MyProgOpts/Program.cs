using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProgOpts
{
    class CommandLineParser : ICommandLineParser
    {
        readonly Dictionary<string, int> parsedOptions = new Dictionary<string, int>();

        readonly Dictionary<string, string> handleToLongName = new Dictionary<string, string>();
        readonly Dictionary<string, char> handleToShortName = new Dictionary<string, char>();
        readonly Dictionary<string, char> handleToShortName = new Dictionary<string, char>();

        public void AddOptionSpec(string name, char shortName, string longName, Type type)
        {
            throw new NotImplementedException();
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

        public void ParseArgs(IEnumerable<string> args)
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
                        parsedOptions.Add("-", i);
                    }
                    else if (arg[1] == '-')
                    {
                        // here we have two dashes so we have a long option:
                        parsedOptions.Add(arg.Substring(2), i);
                    }
                    else
                    {
                        // here we have a single dash followed by one or more characters which are all short options:
                        foreach (var c in arg.Skip(1))
                        {
                            parsedOptions.Add($"{c}", i);
                        }
                    }
                }
                i++;
            }
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
