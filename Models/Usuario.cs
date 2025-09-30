using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextParkAPI.Models
{
    [Table("TB_NEXTPARK_USUARIO")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Required]
        [Column("NR_EMAIL")]
        [MaxLength(100)]
        public string NrEmail { get; set; } = string.Empty;

        public ICollection<Login> Logins { get; set; } = new List<Login>();
    }
}
