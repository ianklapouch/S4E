using System.Text.RegularExpressions;

namespace TesteS4E.Utils
{
    public static class CnpjUtils
    {
        public static string AdicionaMascara(string cnpj)
        {
            if (!string.IsNullOrEmpty(cnpj) && cnpj.Length == 14)
            {
                return Regex.Replace(cnpj, @"(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})", "$1.$2.$3/$4-$5");
            }

            return cnpj;
        }
        public static string RemoveMascara(string cnpj)
        {
            if (!string.IsNullOrEmpty(cnpj) && cnpj.Length == 18)
            {
                return Regex.Replace(cnpj, @"[\./-]", "");
            }
            return cnpj;
        }
    }
}