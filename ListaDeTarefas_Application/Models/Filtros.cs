namespace ListaDeTarefas_Application.Models
{
    public class Filtros
    {
        public Filtros(string filtroString)
        {
            // Define um valor padrão para a string de filtro, caso seja nula ou vazia.
            FiltroString = filtroString ?? "todos-todos-todos";

            // Divide a string de filtro em partes.
            string[] filtros = FiltroString.Split('-');

            // Inicializa as propriedades com os valores dos filtros ou com "todos" se o filtro não existir.
            CategoriaId = filtros.Length > 0 && !string.IsNullOrEmpty(filtros[0]) ? filtros[0] : "todos";
            Vencimento = filtros.Length > 1 && !string.IsNullOrEmpty(filtros[1]) ? filtros[1] : "todos";
            StatusId = filtros.Length > 2 && !string.IsNullOrEmpty(filtros[2]) ? filtros[2] : "todos";
        }

        public string FiltroString { get; set; }
        public string CategoriaId { get; set; }
        public string StatusId { get; set; }
        public string Vencimento { get; set; }


        public bool TemCategoria => CategoriaId.ToLower() != "todos";
        public bool TemVencimento => Vencimento.ToLower() != "todos";
        public bool TemStatus => StatusId.ToLower() != "todos";

        public static Dictionary<string, string> VencimentoValoresFiltro =>
            new Dictionary<string, string>
            {
                {"futuro", "Futuro"},
                {"passado", "Passado"},
                {"hoje", "Hoje"}
            };

        public bool EPassado => Vencimento.ToLower() == "passado";
        public bool EFuturo => Vencimento.ToLower() == "futuro";
        public bool EHoje => Vencimento.ToLower() == "hoje";
    }
}
