using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Carrinho.Core.Entities
{
    [Table("Product")]
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string SmallImagePath { get; set; }
        public string LargeImagePath { get; set; }
        public decimal Price { get; set; }
    }
}
