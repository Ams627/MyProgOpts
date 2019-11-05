using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProgOpts
{
    class CommandLineParser : ICommandLineParser
    {
        readonly Dictionary<string, int> parsedLongOptions = new Dictionary<string, int>();
        readonly Dictionary<char, int> parsedShortOptions = new Dictionary<char, int>();
        readonly Dictionary<string, List<string>> optionParams = new Dictionary<string, List<string>>();

        readonly Dictionary<string, Option> handleToOption = new Dictionary<string, Option>();
        readonly HashSet<string> verbs = new HashSet<string>();

        ILookup<char, Option> shortLookup;

        List<string> restOfArgs = new List<string>();

        int stdin = -1;

        public void AddOptionSpec(string handle, char shortName, string longName, int optionParams = 1)
        {
            var option = new Option(longName, shortName, typeof(string), optionParams);
            handleToOption[handle] = option;
        }

        public void AddOptionSpec<T>(string handle, char shortName, string longName, int optionParams = 1)
        {
            var option = new Option(longName, shortName, typeof(T), optionParams);
            handleToOption[handle] = option;
        }


        /// <summary>
        /// Add a list of boolean options which can then be tested with TestBinOpt
        /// </summary>
        /// <param name="s">A string with short options first followed by a verical bar ("pipe") then followed by a list of long options.
        /// The separator between the long options is also a vertical bar</param>
        void AddBooleanOptions(string s)
        {

        }

        void AddVerbs(params string[] verbs)
        {

        }

        /// <summary>
        /// Test a short boolean option
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool TestBooleanOption(char c)
        {
            var found = this.shortLookup[c];
            return found.Any() && found.First().Found;
        }

        /// <summary>
        /// Test a long boolean option
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        bool TestBooleanOption(string s)
        {
            return false;
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
            invalidLongOptions = new List<string>();
            invalidShortOptions = new List<char>();

            var shortLookup = handleToOption.Select(x => x.Value).ToLookup(y => y.ShortName);
            var longLookup = handleToOption.Select(x => x.Value).ToLookup(y => y.LongName);


            int argIndex = 0;
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
                            stdin = argIndex;
                        }
                        else if (arg[1] == '-')
                        {
                            // here we have two dashes so we have a long option:
                            var parsedLongOption = arg.Substring(2);

                            // we can have an option specified like this --infile=t1.txt - in that case we have an option parameter
                            // included in the arg:
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

                            parsedLongOptions.Add(parsedLongOption, argIndex);

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
                            else
                            {
                                invalidLongOptions.Add(parsedLongOption);
                            }
                        }
                        else
                        {
                            // here we have a single dash followed by one or more characters which are all short options:
                            for (int i = 1; i < arg.Length; i++)
                            {
                                var c = arg[i];
                                var foundOptions = shortLookup[c];
                                if (foundOptions.Any())
                                {
                                    currentOption = foundOptions.First();
                                    currentOption.Found = true;
                                    var numParams = currentOption.NumOptionParams;
                                    if (numParams == 1 && i != arg.Length - 1)
                                    {
                                        currentOption.OptionParams.Add(arg.Substring(i + 1));
                                    }
                                    else if (numParams > 0)
                                    {
                                        skipOptionParmeters = numParams;
                                    }
                                }
                                else
                                {
                                    invalidShortOptions.Add(c);
                                }
                            }
                        }
                    }
                }
                else
                {
                    restOfArgs.Add(arg);
                }

                argIndex++;
            }

            this.shortLookup = handleToOption.ToLookup(x => x.Value.ShortName, x=>x.Value);
            

            //var validShortOptions = new HashSet<char>(handleToOption.Select(x => x.Value.ShortName));
            //var parsedShortOptionsSet = new HashSet<char>(parsedShortOptions.Keys);
            //parsedShortOptionsSet.ExceptWith(validShortOptions);
            //invalidShortOptions = parsedShortOptionsSet.ToList();

            //var validLongOptions = new HashSet<string>(handleToOption.Select(x => x.Value.LongName));
            //var parsedLongOptionsSet = new HashSet<string>(parsedLongOptions.Keys);
            //parsedLongOptionsSet.ExceptWith(validLongOptions);
            //invalidLongOptions = parsedLongOptionsSet.ToList();
        }

        public void ParseArgs(params string[] args)
        {
            ParseArgs(args);
        }
    }
}
