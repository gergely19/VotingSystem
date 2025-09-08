using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Models
{
    public class Vote
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OptionId { get; set; }

        [ForeignKey("OptionId")]
        public virtual Option? Option { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
