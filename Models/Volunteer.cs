using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteeringWpfApp.Models
{
    public class Volunteer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        public required DateTime DateOfBirth { get; set; }

        [MaxLength(11)]
        public required string Snils { get; set; }

        public required int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public Region Region { get; set; }

        [MaxLength(100)]
        public required string Email { get; set; }

        [MaxLength(11)]
        public required string Phone { get; set; }

        public required bool HasTransport { get; set; }

        public string? AdditionalInfo { get; set; }

        public string? PhotoFileName { get; set; }

        public string[]? AchievementsFileNames { get; set; }

        public required DateTime LastModifiedDateTime { get; set; }
    }
}
