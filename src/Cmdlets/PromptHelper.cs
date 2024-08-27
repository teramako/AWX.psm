using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace AWX.Cmdlets
{
    internal class AskPrompt
    {
        public AskPrompt(PSHost host)
        {
            _host = host;
        }
        private PSHost _host { get; }

        private void printHeader(string label, string defaultValue, string help)
        {
            var gb = Console.BackgroundColor;
            _host.UI.WriteLine();
            _host.UI.Write(ConsoleColor.Blue, gb, "==> ");
            _host.UI.WriteLine($"Enter {label} (Default: {defaultValue})");
            _host.UI.WriteLine(ConsoleColor.DarkYellow, gb, help);

        }
        private void printHelp(string label, string message, string help)
        {
            var gb = Console.BackgroundColor;
            _host.UI.Write(ConsoleColor.Blue, gb, "==> ");
            _host.UI.WriteLine(label);
            _host.UI.WriteLine(message);
            _host.UI.WriteLine(ConsoleColor.DarkYellow, gb, help);
        }
        /// <summary>
        /// List input prompt
        /// </summary>
        /// <param name="label">Prompt label</param>
        /// <param name="defaultValues"></param>
        /// <param name="answers"></param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool AskList<T>(string label, IEnumerable<string>? defaultValues, out Answer<List<T>> answers)
        {
            var results = new List<T>();
            var index = 0;
            var defaultValuString = $"[{string.Join(", ", defaultValues ?? [])}]";
            var help = "(Type !? : Show help, !> : Suspend)";
            printHeader(label, defaultValuString, help);
            do
            {
                var fieldLabel = $"{label}[{index}]";
                var currentHelpMessage = $"CurrentValues: [{string.Join(", ", results.Select(item => $"{item}"))}]";
                if (!TryPromptOneInput(fieldLabel, out var inputString))
                {
                    answers = new Answer<List<T>>([], true);
                    return false;
                }
                if (inputString.StartsWith('!'))
                {
                    var command = inputString.Substring(1).Trim();
                    switch (command)
                    {
                        case "?":
                            printHelp(label, currentHelpMessage, help);
                            continue;
                        case ">":
                            _host.EnterNestedPrompt();
                            continue;
                    }
                }

                if (string.IsNullOrEmpty(inputString))
                {
                    answers = new Answer<List<T>>(results);
                    return true;
                }
                else
                {
                    try
                    {
                        var val = LanguagePrimitives.ConvertTo<T>(inputString);
                        results.Add(val);
                        index = results.Count;
                    }
                    catch (Exception ex)
                    {
                        _host.UI.WriteLine(ConsoleColor.Red, Console.BackgroundColor, ex.Message);
                    }
                }
            }
            while (true);
        }
        /// <summary>
        /// String input prompt
        /// </summary>
        /// <param name="label">Prompt label</param>
        /// <param name="defaultValue"></param>
        /// <param name="answer"></param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool Ask(string label, string? defaultValue, out Answer<string> answer)
        {
            var defaultValueString = $"\"{defaultValue}\"";
            var help = """
                (!? => Show help, !! => Use default, !> => Suspend, Empty => Skip, ("", '', $null) => Specify empty string)
                """;
            printHeader(label, defaultValueString, help);
            do
            {
                var inputed = false;
                if (!TryPromptOneInput(label, out var inputString))
                {
                    answer = new Answer<string>(string.Empty, true);
                    return false;
                }
                if (inputString.StartsWith('!'))
                {
                    var command = inputString.Substring(1).Trim();
                    switch (command)
                    {
                        case "?":
                            printHelp(label, $"Default: {defaultValueString}", help);
                            continue;
                        case "!":
                            answer = new Answer<string>(defaultValue ?? string.Empty);
                            return true;
                        case ">":
                            _host.EnterNestedPrompt();
                            continue;
                        default:
                            inputString = command;
                            break;
                    }
                }
                switch (inputString)
                {
                    case "\"\"":
                    case "''":
                    case "$null":
                        inputed = true;
                        inputString = string.Empty;
                        break;

                }
                if (string.IsNullOrEmpty(inputString))
                {
                    answer = new Answer<string>(defaultValue ?? string.Empty, !inputed);
                    return true;
                }
                else
                {
                    answer = new Answer<string>(inputString);
                    return true;
                }
            }
            while (true);
        }
        /// <summary>
        /// Number input prompt
        /// </summary>
        /// <typeparam name="T">inputed data type (mainly number type)</typeparam>
        /// <param name="label">Prompt label</param>
        /// <param name="defaultValue"></param>
        /// <param name="answer"></param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool Ask<T>(string label, T? defaultValue, out Answer<T> answer) where T : struct
        {
            var help = """
                (!? => Show help, !! => Use default, !> => Suspend, Empty => Skip)
                """;
            printHeader(label, $"{defaultValue}", help);
            do
            {
                var inputed = false;
                if (!TryPromptOneInput(label, out var inputString))
                {
                    answer = new Answer<T>(default(T), true);
                    return false;
                }
                if (inputString.StartsWith('!'))
                {
                    var command = inputString.Substring(1).Trim();
                    switch (command)
                    {
                        case "?":
                            printHelp(label, $"Default: {defaultValue}", help);
                            continue;
                        case "!":
                            inputed = true;
                            inputString = string.Empty;
                            break;
                        case ">":
                            _host.EnterNestedPrompt();
                            continue;
                        default:
                            inputString = command;
                            break;
                    }
                }
                if (string.IsNullOrEmpty(inputString))
                {
                    if (defaultValue != null)
                    {
                        answer = new Answer<T>((T)defaultValue, !inputed);
                        return true;
                    }
                    _host.UI.Write(ConsoleColor.Red, Console.BackgroundColor,
                            $"default value is null. Please specify value.\n");
                    continue;
                }
                else
                {
                    try
                    {
                        answer = new Answer<T>(LanguagePrimitives.ConvertTo<T>(inputString));
                        return true;
                    }
                    catch(Exception ex)
                    {
                        _host.UI.WriteLine(ConsoleColor.Red, Console.BackgroundColor, ex.Message);
                    }
                }
            }
            while (true);
        }
        private bool TryPromptOneInput(string label, out string inputString)
        {
            inputString = string.Empty;
            var fd = new FieldDescription(label);
            fd.SetParameterType(typeof(string));
            var fdc = new Collection<FieldDescription>() { fd };
            Dictionary<string, PSObject?>? result = _host.UI.Prompt("", "", fdc);
            if (result != null && result.TryGetValue(label, out PSObject? val))
            {
                if (val == null || val.BaseObject == null)
                {
                    return false;
                }
                inputString = val.BaseObject as string ?? string.Empty;
                return true;
            }
            return false;
        }

        public class Answer<T>
        {
            public Answer(T input, bool isEmpty = false)
            {
                Input = input;
                IsEmpty = isEmpty;
            }
            public T Input { get; }
            public bool IsEmpty { get; }
        }
    }
}
