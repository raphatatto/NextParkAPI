using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextParkAPI.Models
{
    [Table("TB_NEXTPARK_LOGIN")]
    public class Login
    {
        [Key]
        [Column("ID_LOGIN")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdLogin { get; set; }

        [Required]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("NR_EMAIL")]
        [MaxLength(100)]
        public string NrEmail { get; set; } = string.Empty;

        [Required]
        [Column("DS_SENHA")]
        [MaxLength(255)]
        public string DsSenha { get; set; } = string.Empty;

        public Usuario? Usuario { get; set; }
    }
}
