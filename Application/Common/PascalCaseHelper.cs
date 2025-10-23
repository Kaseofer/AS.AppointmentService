using AS.AppointmentService.Application.Dtos;
using AS.AppointmentService.Core.Entities;

namespace AS.AppointmentService.Application.Common
{
    public static class PascalCaseHelper
    {
        public static string ToPascalCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Select(word =>
                char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }


    }

    
}
