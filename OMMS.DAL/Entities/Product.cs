﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMMS.DAL.Entities
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Thumbnail { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
    }
}
