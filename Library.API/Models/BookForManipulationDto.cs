using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
    public abstract class BookForManipulationDto
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [MaxLength(100, ErrorMessage = "O tamanho do título não pode ser maior que 100 caracteres")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "O tamanho da descrição não pode ser maior que 500 caracteres")]
        public virtual string Description { get; set; }
    }
}
