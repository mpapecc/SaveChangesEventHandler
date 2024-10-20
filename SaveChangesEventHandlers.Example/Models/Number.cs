using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SaveChangesEventHandlers.Example.Models
{
    public class Number:BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Value { get; set; }
        public Guid ContactId { get;set; }
        [JsonIgnore]
        public Contact? Contact { get; set; }
    }
}
