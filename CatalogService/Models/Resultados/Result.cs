namespace CatalogService.Models.Resultados
{
    public class Result<T>
    {
        private static readonly Result<T> _success = new Result<T> { Succeeded = true };
        private ICollection<Errors> _errors = new List<Errors>();
        private T? _dados { get; set; }


        public ICollection<Errors> Errors => _errors;
        public bool Succeeded { get; protected set; }
        public static Result<T> Success => _success;
        public T Dados => _dados!;

        /// <summary>
        /// Retorno os dados com sucesso
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        public static Result<T> Sucesso(T dados)
        {
            var result = new Result<T> { Succeeded = true };

            if (dados != null)
            {
                result._dados = dados;
            }
            return result;
        }

        /// <summary>
        /// Retorna uma ICollection<Errors> personalizado
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static Result<T> Failed(Errors errors)
        {
            var result = new Result<T> { Succeeded = false };

            if (errors != null)
            {
                result._errors.Add(errors);
            }

            return result;
        }

        public override string ToString()
        {
            if (Errors != null && Errors.Any())
            {
                return string.Join(Environment.NewLine, Errors.Select(e => e.Description));
            }
            return Succeeded ? "Operação realizada com sucesso." : "Erro desconhecido.";
        }
    }
}
