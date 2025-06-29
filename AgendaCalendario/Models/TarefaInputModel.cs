using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models
{
    public class TarefaInputModel
    {
        public int? Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        [Required]
        public DateTime Data { get; set; }

        public TipoRecorrencia Recorrencia { get; set; }

        public DateTime? DataFimRecorrencia { get; set; }

        public List<int> CategoriasIds { get; set; } = new();
        
        public int UtilizadorId { get; set; }
    }
}