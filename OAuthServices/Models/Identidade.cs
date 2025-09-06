using OAuthServices.Models.Resultados;
using System.Globalization;

namespace OAuthServices.Models
{
    public class Identidade
    {
        private static readonly Identidade _success = new Identidade { Succeeded = true };

        private Errors _errors = new Errors();


        public bool Succeeded { get; protected set; }
        public Errors? Error => _errors;
        public static Identidade Success => _success;

        public static Identidade Failed(Errors errors)
        {
            var result = new Identidade { Succeeded = false };
            if (errors != null)
            {
                result._errors = errors;
            }
            return result;
        }

        public override string ToString()
        {
            return Succeeded ?
            "Succeeded" :
                   string.Format(CultureInfo.InvariantCulture, "{0} : {1}", "Failed", string.Join(",", Error!.Code));
        }
    }
}
