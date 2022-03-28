using JetBrains.Annotations;

namespace AuthFlow.Infrastructure
{
    [UsedImplicitly]
    public class PasswordValidator : IPasswordValidator
    {
        public (bool, string) Validate(string pass)
        {
            (bool, string) Result = (true, null);

            if (string.IsNullOrEmpty(pass))
            {
                Result = (false, "Please enter a password");
            }
            else if(pass.Length < 6)
            {
                Result = (false, "Must be at least 6 characters long");
            }

            return Result;
        }
    }
}
