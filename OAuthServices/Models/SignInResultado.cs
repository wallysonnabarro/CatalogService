namespace OAuthServices.Models
{
    public class SignInResultado
    {
        private static readonly SignInResultado _success = new SignInResultado { Succeeded = true };
        private static readonly SignInResultado _failed = new SignInResultado();
        private static readonly SignInResultado _lockedOut = new SignInResultado { IsLockedOut = true };
        private static readonly SignInResultado _notAllowed = new SignInResultado { IsNotAllowed = true };
        private static readonly SignInResultado _twoFactorRequired = new SignInResultado { RequiresTwoFactor = true };
        private static readonly SignInResultado _EmailConfirmRequired = new SignInResultado { RequeredEmailConfirm = true };


        public bool Succeeded { get; protected set; }
        public bool IsLockedOut { get; protected set; }
        public bool IsNotAllowed { get; protected set; }
        public bool RequiresTwoFactor { get; protected set; }
        public bool RequeredEmailConfirm { get; protected set; }
        public static SignInResultado Success => _success;

        public static SignInResultado Failed => _failed;

        public static SignInResultado LockedOut => _lockedOut;

        public static SignInResultado NotAllowed => _notAllowed;

        public static SignInResultado TwoFactorRequired => _twoFactorRequired;
        public static SignInResultado EmailConfirmRequired => _EmailConfirmRequired;

        public override string ToString()
        {
            return IsLockedOut ? "Lockedout" :
                      IsNotAllowed ? "NotAllowed" :
                   RequiresTwoFactor ? "RequiresTwoFactor" :
                   RequeredEmailConfirm ? "EmailConfirmRequired" :
                   Succeeded ? "Succeeded" : "Failed";
        }

    }
}
