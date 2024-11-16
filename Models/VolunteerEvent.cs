using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteeringWpfApp.Models
{
    public class VolunteerEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required int VolunteerId { get; set; }
        [ForeignKey("VolunteerId")]
        public Volunteer Volunteer { get; set; }

        public required int EventId { get; set; }
        [ForeignKey("EventId")]
        public Event Event { get; set; }

        public required int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public required DateTime LastModifiedDateTime { get; set; }
    }
}
