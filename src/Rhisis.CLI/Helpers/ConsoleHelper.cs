using System;
using System.Text;

namespace Rhisis.CLI.Helpers
{
    /// <summary>
    /// Extends the basic console with helper methods.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Reads a string value from the console.
        /// </summary>
        /// <param name="defaultValue">Default value return if input is empty</param>
        /// <returns></returns>
        public static string ReadStringOrDefault(string defaultValue = null)
        {
            string value = Console.ReadLine();

            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>
        /// Reads an integer from the console or return the default value if read value is not a number.
        /// </summary>
        /// <param name="defaultValue">Default value</param>
        /// <returns></returns>
        public static int ReadIntegerOrDefault(int defaultValue = 0)
        {
            string value = Console.ReadLine();

            if (!string.IsNullOrEmpty(value))
            {
                return int.TryParse(value, out int result) ? result : defaultValue;
            }

            return defaultValue;
        }
        
        /// <summary>
        /// Displays an enum as a numbered list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DisplayEnum<T>()
        {
            string[] providerNames = Enum.GetNames(typeof(T));
            int[] providerValues = (int[])Enum.GetValues(typeof(T));

            for (int i = 1; i < providerNames.Length; i++)
                Console.WriteLine($"{providerValues[i]}. {providerNames[i]}");
        }

        /// <summary>
        /// Reads an enum from the console.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum ReadEnum<TEnum>() where TEnum : struct
        {
            var value = default(TEnum);
            int[] providerValues = (int[])Enum.GetValues(typeof(TEnum));
            string selectedProvider = Console.ReadLine();

            if (int.TryParse(selectedProvider, out int selectedProviderValue) && selectedProviderValue > 0 && selectedProviderValue < providerValues.Length)
                value = (TEnum)(object)selectedProviderValue;
            else if (Enum.TryParse(selectedProvider, true, out TEnum provider))
                value = provider;

            return value;
        }

        /// <summary>
        /// Reads a password from the console input.
        /// </summary>
        /// <param name="passwordCharacter">Password character used to hide input characters</param>
        /// <returns></returns>
        public static string ReadPassword(string passwordCharacter = "*")
        {
            var password = new StringBuilder();

            do
            {
                ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                if (keyinfo.Key != ConsoleKey.Backspace && keyinfo.Key != ConsoleKey.Enter)
                {
                    password.Append(keyinfo.KeyChar);
                    Console.Write(passwordCharacter);
                }
                else
                {
                    if (keyinfo.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Remove((password.Length - 1), 1);
                        Console.Write("\b \b");
                    }
                    else if (keyinfo.Key == ConsoleKey.Enter)
                    {
                        Console.Write(Environment.NewLine);
                        break;
                    }
                }
            } while (true);
            
            return password.ToString();
        }

        /// <summary>
        /// Ask a questions with a yes/no answer.
        /// </summary>
        /// <param name="confirmationMessage">Confirmation message</param>
        /// <returns></returns>
        public static bool AskConfirmation(string confirmationMessage)
        {
            Console.Write($"{confirmationMessage} (y/n): ");
            string response = Console.ReadLine();

            return response.Equals("y", StringComparison.OrdinalIgnoreCase) 
                || response.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }
    }
}
