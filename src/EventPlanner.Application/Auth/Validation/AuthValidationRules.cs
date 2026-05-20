using System.Net.Mail;

namespace EventPlanner.Application.Auth.Validation;

internal static class AuthValidationRules
{
    public static bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);

            return mailAddress.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
