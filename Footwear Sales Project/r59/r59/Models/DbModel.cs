using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace r59.Models
{
    public enum ProductType { Mens=1, Womens, Kids}
    public class Product 
    {
        public int ProductId { get; set; }

        public string CreatedBy { get; set; } = default!;

        [Required , StringLength(40)]
        public string ProductName { get; set; } = default!;
        [Required , EnumDataType(typeof(ProductType))]
        public ProductType ProductType { get; set; }
        [Required , Column (TypeName ="money")]
        public decimal? Price { get; set; }
        [Required, Column(TypeName = "date")]
        public DateTime? MfgDate { get; set; }
        [Required, StringLength(40)]
        public string Picture { get; set; } = default!;
        public bool InStock     { get; set; }
        public virtual ICollection<Sale> Sales { get; set; } = [];
    }
    public class Sale
    { 
    public int SaleId { get; set; }
        [Required, StringLength(40)]
        public string SellerName    { get; set; }= default!;
        [Required, Column(TypeName = "date")]
        public DateTime? Date   { get; set; }
        public int Quantity     { get; set; }
        //fk
        [Required , ForeignKey ("Product")]
        public int ProductId { get; set; }
        //navigation
        public virtual Product? Product { get; set; }
    }
   
}
