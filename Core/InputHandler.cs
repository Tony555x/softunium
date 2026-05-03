using System;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Core;

public static class InputHandler
{
    // Returns true if the input was fully handled (e.g. info was shown)
    // Returns false if it should try to execute the option
    public static bool Handle(string input, List<Option> options, out Option selectedOption)
    {
        selectedOption = null;
        if (string.IsNullOrWhiteSpace(input)) return true;

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bool isInfoRequest = false;
        string targetString = input.Trim();

        if (parts.Length > 0 && parts[0].Equals("п", StringComparison.OrdinalIgnoreCase))
        {
            isInfoRequest = true;
            if (parts.Length > 1)
            {
                // Reconstruct the rest of the string
                targetString = string.Join(' ', parts.Skip(1));
            }
            else
            {
                // Just 'п' with nothing else
                return true;
            }
        }

        // Try to match by ID or Text
        Option matchedOption = null;
        if (int.TryParse(targetString, out int id))
        {
            matchedOption = options.FirstOrDefault(o => o.Id == id);
        }
        else
        {
            matchedOption = options.FirstOrDefault(o => o.Text.Equals(targetString, StringComparison.OrdinalIgnoreCase));
        }

        if (matchedOption != null)
        {
            if (isInfoRequest)
            {
                Console.WriteLine();
                Console.WriteLine($"Информация за [{matchedOption.Text}]: {matchedOption.Info}");
                Console.WriteLine("Натиснете Enter за продължаване...");
                Console.ReadLine();
                return true; // Handled info request
            }
            else
            {
                selectedOption = matchedOption;
                return false; // Option selected, execute it
            }
        }
        
        return true; // Input not recognized, ignore and loop
    }
}
