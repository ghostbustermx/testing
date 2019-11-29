using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("User")]
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string UserName { get; set; }

        [Required]
        [StringLength(150)]
        public string Role { get; set; }

        [Required]
        [StringLength(150)]
        public string FirstName { get; set; }


        [StringLength(150)]
        public string Division { get; set; }

        [StringLength(150)]
        public string JobTitle { get; set; }

        [StringLength(150)]
        public string Gender { get; set; }

        [StringLength(150)]
        public string Department { get; set; }

        [StringLength(150)]
        public string Alias { get; set; }

        public DateTime? HireDate { get; set; }

        [StringLength(150)]
        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
        public string PhotoUrl { get; set; }

        public bool IsActive { get; set; }



    }
}
