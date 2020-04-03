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
    public enum Genre
    {
        [Description("Fantasia")]
        Fantasy,
        [Description("Scifi")]
        ScienceFiction,
        [Description("Rakkaus")]
        Romance,
        [Description("Thrilleri")]
        Thriller,
        [Description("Mysteeri")]
        Mystery,
        [Description("Salapoliisi")]
        Detective,
        [Description("Dystopia")]
        Dystopia,
        [Description("Elämäkerta")]
        Biography,
        [Description("Sota")]
        War,
        [Description("Komedia")]
        Comedy,
        [Description("Draama")]
        Drama
    }
    public enum Education
    {
        [Description("Peruskoulu")]
        Primarychool,
        [Description("Lukio / keskiaste")]
        Highschool,
        [Description("Korkeakoulu")]
        University
    }
    [Display(Name = "Henkilö")]
    public class Person : IAuditable
    {
        public int Id { get; set; }
        [Display(Name = "Etunimi")]
        public string FirstName { get; set; }
        [Display(Name = "Sukunimi")]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - this.Birthday.Year;
                if (this.Birthday.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public Education Education { get; set; }
        public int? LuckyNumber { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
    }
    [NotMapped]
    [Display(Name = "Henkilö")]
    public class PersonDTO
    {
        [Hidden]
        public int Id { get; set; }
        [Hidden]
        public string Name
        {
            get
            {
                return $"{this.FirstName} {this.LastName}";
            }
        }   
        [Display(Name = "Etunimi"), StringLength(50,MinimumLength =2),Required]
        public string FirstName { get; set; }
        [Display(Name = "Sukunimi"), StringLength(50, MinimumLength = 2), Required]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Syntymäpäivä")]
        public DateTime Birthday { get; set; }
        [Display(Name = "Koulutus"), Required]
        public Education? Education { get; set; }
        
        [OptionsSource("ExampleDbLib.Book")]
        [Display(Name = "Suosikkikirja")]
        public int BookId { get; set; }
        
        [OptionsSource("ExampleDbLib.Movie")]
        [Display(Name = "Suosikkielokuva")]
        public int MovieId { get; set; }
        
        [Display(Name = "Onnennumero"), Range(1,10)]
        public int? LuckyNumber { get; set; }
    }
    public class PersonListDTO
    {
        [Display(Name = "")]
        public int Id { get; set; }
        [Display(Name = "Nimi")]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Syntymäpäivä")]
        [Required]
        public string Birthday { get; set; }
        [Display(Name = "Ikä")]
        public int Age { get; set; }
        [Display(Name = "Koulutus")]
        public string Education { get; set; }

        [OptionsSource("ExampleDbLib.Book")]
        [Display(Name = "Suosikkikirja")]
        public string BookName { get; set; }
        
        [OptionsSource("ExampleDbLib.Movie")]
        [Display(Name = "Suosikkielokuva")]
        public string MovieName { get; set; }
        
        [Display(Name = "Onnennumero")]
        public int? LuckyNumber { get; set; }
    }
}   
