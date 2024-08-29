using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Runtime.InteropServices;
using System.Security;

namespace AWX.Cmdlets
{
    internal class AskPrompt
    {
        public AskPrompt(PSHost host)
        {
            _host = host;
        }
        private PSHost _host { get; }

        private void printHeader(string label, string defaultValue, string helpMessage = "", string helpIndicator = "", bool showDefault = true)
        {
            var gb = Console.BackgroundColor;
            _host.UI.Write(ConsoleColor.Blue, gb, "==> ");
            _host.UI.Write($"{label}");
            if (showDefault)
            {
                _host.UI.Write(" (Default: ");
                _host.UI.Write(ConsoleColor.DarkYellow, gb, $"{defaultValue}");
                _host.UI.WriteLine(")");
            }
            else
            {
                _host.UI.WriteLine();
            }
            if (!string.IsNullOrEmpty(helpMessage))
            {
                _host.UI.WriteLine(helpMessage);
            }
            if (!string.IsNullOrEmpty(helpIndicator))
            {
                _host.UI.WriteLine(ConsoleColor.DarkYellow, gb, helpIndicator);
            }
        }
        private void printHelp(string label, string helpMessage = "", string helpIndicator = "")
        {
            var gb = Console.BackgroundColor;
            _host.UI.Write(ConsoleColor.Blue, gb, "==> ");
            _host.UI.WriteLine(label);
            if (!string.IsNullOrEmpty(helpMessage))
            {
                _host.UI.WriteLine(helpMessage);
            }
            if (!string.IsNullOrEmpty(helpIndicator))
            {
                _host.UI.WriteLine(ConsoleColor.DarkYellow, gb, helpIndicator);
            }
        }
        /// <summary>
        /// List input prompt
        /// </summary>
        /// <param name="label">Prompt label</param>
        /// <param name="defaultValues"></param>
        /// <param name="helpMessage"></param>
        /// <param name="answers"></param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool AskList<T>(string label, IEnumerable<string>? defaultValues, string helpMessage, out Answer<List<T>> answers)
        {
            var results = new List<T>();
            var index = 0;
            var defaultValuString = $"[{string.Join(", ", defaultValues ?? [])}]";
            var helpIndicator = "(Type !? : Show help, !> : Suspend)";
            printHeader(label, defaultValuString, helpMessage, helpIndicator);
            do
            {
                var fieldLabel = $"{label}[{index}]";
                var currentHelpMessage = (string.IsNullOrEmpty(helpMessage) ? "" : $"{helpMessage}\n")
                                         + $"CurrentValues: [{string.Join(", ", results.Select(item => $"{item}"))}]";
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
                            printHelp(label, currentHelpMessage, helpIndicator);
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
        /// <param name="helpMessage"></param>
        /// <param name="answer"></param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool Ask(string label, string? defaultValue, string helpMessage, out Answer<string> answer)
        {
            var defaultValueString = $"\"{defaultValue}\"";
            var helpIndicator = """
                (!? => Show help, !! => Use default, !> => Suspend, Empty => Skip, ("", '', $null) => Specify empty string)
                """;
            printHeader(label, defaultValueString, helpMessage, helpIndicator);
            var help = (string.IsNullOrEmpty(helpMessage) ? "" : $"{helpMessage}\n")
                       + $"Default: {defaultValueString}";
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
                            printHelp(label, help, helpIndicator);
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
        /// <param name="helpMessage"></param>
        /// <param name="answer"></param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool Ask<T>(string label, T? defaultValue, string helpMessage, out Answer<T> answer) where T : struct
        {
            var helpIndicator = """
                (!? => Show help, !! => Use default, !> => Suspend, Empty => Skip)
                """;
            printHeader(label, $"{defaultValue}", helpMessage, helpIndicator);
            var help = (string.IsNullOrEmpty(helpMessage) ? "" : $"{helpMessage}\n")
                       + $"Default: {defaultValue}";
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
                            printHelp(label, help, helpIndicator);
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
        /// <summary>
        /// Boolean input prompt
        /// </summary>
        /// <param name="label">Prompt label</param>
        /// <param name="answer"></param>
        /// <param name="trueHelpMessage"></param>
        /// <param name="falseHelpMessage"></param>
        /// <param name="defaultValue"</param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool AskBool(string label, bool defaultValue, string trueHelpMessage, string falseHelpMessage, out Answer<bool> answer)
        {
            var choices = new Collection<ChoiceDescription>();

            choices.Add(new ChoiceDescription("&True", trueHelpMessage));
            choices.Add(new ChoiceDescription("&False", falseHelpMessage));

            printHeader(label, "", $"{trueHelpMessage}(true) or {falseHelpMessage}(false)", showDefault: false);
            var res = _host.UI.PromptForChoice("", "", choices, defaultValue ? 0 : 1);
            switch (res)
            {
                case 0:
                    answer = new Answer<bool>(true);
                    return true;
                case 1:
                    answer = new Answer<bool>(false);
                    return true;
                default:
                    answer = new Answer<bool>(false);
                    return false;
            }

        }
        /// <summary>
        /// String input prompt
        /// </summary>
        /// <typeparam name="TEnum">inputed data type (Enum)</typeparam>
        /// <param name="label">Prompt label</param>
        /// <param name="answer"></param>
        /// <param name="helpMessage"></param>
        /// <param name="defaultValue"</param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool AskEnum<TEnum>(string label, TEnum defaultValue, string helpMessage, out Answer<TEnum> answer) where TEnum : System.Enum
        {
            var defaultValueString = $"{defaultValue:g} ({defaultValue:d})";
            printHeader(label, defaultValueString, helpMessage);
            var choices = new Collection<ChoiceDescription>();
            var defaultValueIndex = -1;
            var enumType = typeof(TEnum);

            var list = new List<TEnum>();
            var enumValues = Enum.GetValues(enumType);
            foreach (TEnum val in enumValues)
            {
                var num = $"{val:d}";
                var str = $"{val:g}";
                if (str == $"{defaultValue}")
                {
                    defaultValueIndex = choices.Count;
                }
                choices.Add(new ChoiceDescription($"&{str}", $"[{num}] {str}"));
                list.Add(val);
            }
            var res = _host.UI.PromptForChoice("", "", choices, defaultValueIndex);
            if (res >= 0 && res < list.Count)
            {
                answer = new Answer<TEnum>(list[res]);
                return true;
            }
            answer = new Answer<TEnum>(defaultValue, true);
            return false;
        }
        /// <summary>
        /// Password prompt
        /// </summary>
        /// <param name="caption">Header label</param>
        /// <param name="answer"></param>
        /// <param name="helpMessage"></param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        public bool AskPassword(string caption, string helpMessage, out Answer<string> answer)
        {
            printHeader(caption, "", helpMessage, showDefault: false);
            var label = "Password";
            var fd = new FieldDescription(label);
            fd.SetParameterType(typeof(SecureString));
            var fdc = new Collection<FieldDescription>() { fd };
            Dictionary<string, PSObject>? result = _host.UI.Prompt("", "", fdc);
            if (result != null && result.TryGetValue(label, out PSObject? pso))
            {
                if (pso == null || pso.BaseObject == null)
                {
                    answer = new Answer<string>(string.Empty, true);
                    return false;
                }
                if (pso.BaseObject is SecureString secureString)
                {
                    var password = Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(secureString));
                    secureString.Dispose();
                    answer = new Answer<string>(password ?? string.Empty);
                    return true;
                }
            }
            answer = new Answer<string>(string.Empty, true);
            return false;
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
