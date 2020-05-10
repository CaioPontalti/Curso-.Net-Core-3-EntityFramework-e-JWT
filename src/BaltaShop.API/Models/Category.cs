using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BaltaShop.API.Models
{
    public class Category
    {
        [Key]
        [Column(TypeName = "int")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Este campo deve ter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve ter entre 3 e 60 caracteres")]
        [Column(TypeName = "varchar(200)")]
        public string Title { get; set; }
    }
}

