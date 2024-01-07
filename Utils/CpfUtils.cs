using System.Text.RegularExpressions;

namespace TesteS4E.Utils
{
    public static class CpfUtils
    {
        public static string AdicionaMascara(string cpf)
        {
            if (!string.IsNullOrEmpty(cpf) && cpf.Length == 11)
            {
                return Regex.Replace(cpf, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4");
            }
            return cpf;
        }
        public static string RemoveMascara(string cpf)
        {
            if (!string.IsNullOrEmpty(cpf) && cpf.Length == 14)
            {
                return Regex.Replace(cpf, @"[\.-]", "");
            }
            return cpf;
        }
    }
}
