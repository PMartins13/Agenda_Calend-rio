namespace AgendaCalendario.Models
{
    /// <summary>
    /// DTO usado para editar categorias.
    /// </summary>
    public class CategoriaDto
    {
        /// <summary>
        /// Identificador da categoria.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da categoria.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Cor da categoria (em formato hexadecimal).
        /// </summary>
        public string Cor { get; set; }
    }
}