using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteeringWpfApp.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        public required DateTime StartDateTime { get; set; }

        public required DateTime EndDateTime { get; set; }

        public required int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public Region Region { get; set; }

        [MaxLength(100)]
        public required string Location { get; set; }

        public string? AdditionalInfo { get; set; }

        public required DateTime LastModifiedDateTime { get; set; }
    }
}