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

    [Display(Name = "Movie")]
    public class Movie : IProps, IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Director { get; set; }
        public int PublishYear { get; set; }
        public Genre Genre { get; set; }
        public bool OscarWinner { get; set; }
        public string Comments { get; set; }
        public List<EnumDesc> GetPropEnums(object obj, ExampleDbContext context)
        {
            var list = context.Movies.ToList();
            var retval = list.Select(x => new EnumDesc() { text = $"{x.Director}: {x.Name}", value = x.Id })
             .ToList();

            return retval;
        }

    }
    [NotMapped]
    [Display(Name = "Elokuva")]
    public class MovieDTO
    {
        [Hidden]
        public int Id { get; set; }
        [Display(Name = "Elokuvan nimi")]
        [StringLength(50), Required]
        public string Name { get; set; }
        [Display(Name = "Ohjaaja")]
        [StringLength(50)]
        public string Director { get; set; }
        [Display(Name = "Julkaistu", Description = "Julkaisuvuosi")]
        [OptionsSource("ExampleDbLib.PublishYear")]
        public int PublishYear { get; set; }
        [Display(Name = "Lajityyppi"), Required]
        public Genre? Genre { get; set; }
        [Display(Name = "Oscarvoittaja")]
        public bool OscarWinner { get; set; }
        [Display(Name = "Kuvaus")]
        public string Comments { get; set; }

    }
    public class MovieListDTO
    {
        [Display(Name = "")]
        public int Id { get; set; }
        [Display(Name = "Nimi")]
        public string Name { get; set; }
        [Display(Name = "Ohjaaja")]
        public string Director { get; set; }
        [Display(Name = "Julkaistu")]
        public int PublishYear { get; set; }
        [Display(Name = "Lajityyppi")]
        public string Genre { get; set; }
        [Display(Name = "Kuvaus")]
        public string Comments { get; set; }
    }

}