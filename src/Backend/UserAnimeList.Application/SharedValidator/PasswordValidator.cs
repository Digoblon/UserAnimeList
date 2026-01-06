using FluentValidation;
using FluentValidation.Validators;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.SharedValidator;

public class PasswordValidator<T> : PropertyValidator<T,string>
{
    public override bool IsValid(ValidationContext<T> context, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessagesException.PASSWORD_EMPTY);

            return false;
        }

        if(!VerifyPasswordStrength(password, context))
            return false;
        
        return true;
    }

    public override string Name =>  "PasswordValidator";

    protected override string GetDefaultMessageTemplate(string errorCode) => "{ErrorMessage}";

    private bool VerifyPasswordStrength(string password,ValidationContext<T> context)
    {
        int charCount = 0;
        
        if (password.Length < 8 && password.Length > 50)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessagesException.INVALID_PASSWORD);

            return false;
        }

        if (password.All(char.IsDigit))
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessagesException.INVALID_PASSWORD);
            
            return false;
        }

        if (HasDigit(password))
        {
            charCount++;
        }
        
        if (HasSymbol(password))
        {
            charCount++;
        }
        
        if (HasUppercase(password))
        {
            charCount++;
        }
        
        if (HasLowercase(password))
        {
            charCount++;
        }

        if (charCount <= 2)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessagesException.INVALID_PASSWORD);
            
            return false;
        }
        
        return true;
    }
    
    private static bool HasUppercase(string password)
        => password.Any(char.IsUpper);

    private static bool HasLowercase(string password)
        => password.Any(char.IsLower);

    private static bool HasDigit(string password)
        => password.Any(char.IsDigit);

    private static bool HasSymbol(string password)
        => password.Any(ch => !char.IsLetterOrDigit(ch));
}