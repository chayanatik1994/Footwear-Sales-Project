using r59.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace r59.ViewModels
{
    public class ProductEditModel 
    {
        public int ProductId { get; set; }
        [Required, StringLength(40)]
        public string ProductName { get; set; } = default!;
        [Required, EnumDataType(typeof(ProductType))]
        public ProductType ProductType { get; set; }
        [Required, Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Required, Column(TypeName = "date")]
        public DateTime? MfgDate { get; set; }
        
        public IFormFile? Picture { get; set; } = default!;
        public bool InStock { get; set; }
        public virtual List<Sale> Sales { get; set; } = [];
    }
}
