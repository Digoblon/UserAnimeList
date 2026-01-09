using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.SharedValidator;

public partial class UserNameValidator<T> : PropertyValidator<T,string>
{
    private static readonly Regex UserNameRegex = MyRegex();
    
    public override bool IsValid(ValidationContext<T> context, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessagesException.NAME_EMPTY);

            return false;
        }

        if (!UserNameRegex.IsMatch(userName))
        {
            context.MessageFormatter.AppendArgument(
                "ErrorMessage",
                ResourceMessagesException.NAME_INVALID_FORMAT
            );

            return false;
        }
        
        return true;
    }

    public override string Name  => "UserNameValidator";
    
    protected override string GetDefaultMessageTemplate(string errorCode) => "{ErrorMessage}";
    
    [GeneratedRegex("^[a-zA-Z0-9_-]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}