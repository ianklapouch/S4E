using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TesteS4E.Models
{
    public class Empresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Teste")]
        public string Nome { get; set; }
        [Required]
        [StringLength(14)]
        public string Cnpj { get; set; }
    }

}