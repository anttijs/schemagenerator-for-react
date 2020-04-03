using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace ExampleDbLib
{
    

    [Display(Name = "Kirja")]
    public class Book : IProps, IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Writer { get; set; }
        public int PublishYear { get; set; }
        public Genre Genre { get; set; }
        public string Comments { get; set; }
        public List<EnumDesc> GetPropEnums(object obj, ExampleDbContext context)
        {
            var list = context.Books.ToList();
            var retval = list.Select(x => new EnumDesc() { text = $"{x.Writer}: {x.Name}", value = x.Id }).ToList();

            return retval;
        }

    }
    [NotMapped]
    [Display(Name = "Kirja")]
    public class BookDTO
    {
        [Hidden]
        public int Id { get; set; }
        [Display(Name = "Kirjan nimi"), Required]
        [StringLength(50, MinimumLength=2)]
        public string Name { get; set; }
        [Display(Name = "Kirjailija"), Required]
        public string Writer { get; set; }
        [Display(Name = "Julkaistu")]
        [OptionsSource("ExampleDbLib.PublishYear")]
        public int PublishYear { get; set; }
        [Display(Name = "Lajityyppi"),Required]
        public Genre? Genre { get; set; }
        [Display(Name = "Kuvaus")]
        public string Comments { get; set; }
    }
    public class BookListDTO
    {
        [Display(Name = "")]
        public int Id { get; set; }
        [Display(Name = "Nimi")]
        public string Name { get; set; }
        [Display(Name = "Kirjailija")]
        public string Writer { get; set; }
        [Display(Name = "Julkaistu")]
        public int PublishYear { get; set; }
        [Display(Name = "Lajityyppi")]
        public string Genre { get; set; }
        [Display(Name = "Kuvaus")]
        public string Comments { get; set; }
    }
}   
