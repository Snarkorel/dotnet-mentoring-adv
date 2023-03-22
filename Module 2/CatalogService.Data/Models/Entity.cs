using System.ComponentModel.DataAnnotations;

namespace CatalogService.Data.Models
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
