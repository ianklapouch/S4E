using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesteS4E.Models
{
    public class Associado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Nome { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [StringLength(11)]
        public string Cpf { get; set; }
        [Required]
        public DateTime DataNascimento { get; set; }
    }
}