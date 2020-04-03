using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ExampleDbLib;

namespace SchemaGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().MigrateDatabase().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ExampleDbContext>();
                    context.Database.Migrate();
                    
                    if (context.People.Count() == 5 &&
                        context.Books.Count() == 8 &&
                        context.Movies.Count() == 8)
                        return webHost;

                    context.Database.ExecuteSqlRaw("DELETE FROM People");
                    context.Database.ExecuteSqlRaw("DELETE FROM Movies");
                    context.Database.ExecuteSqlRaw("DELETE FROM Books");

                    var tirana = new Book
                    {
                        Comments = "Finlandia-voittajan toinen teos",
                        Genre = Genre.Drama,
                        PublishYear = 2016,
                        Name = "Tiranan sydän",
                        Writer = "Pajtim Statovci"
                    };
                    context.Books.Add(tirana);

                    var veljekset = new Book
                    {
                        Comments = "Suomalaisen kirjallisuuden klassikko",
                        Genre = Genre.Romance,
                        PublishYear = 1870,
                        Name = "Seitsemän veljestä",
                        Writer = "Aleksis Kivi"
                    };
                    context.Books.Add(veljekset);

                    var alkemisti = new Book
                    {
                        Comments = "Tavoitelkaa unelmianne",
                        Genre = Genre.Fantasy,
                        PublishYear = 1988,
                        Name = "Alkemisti",
                        Writer = "Paulo Coelho"
                    };
                    context.Books.Add(alkemisti);

                    var vallankumous = new Book
                    {
                        Comments = "Satiiri Neuvostoliiton vallankumouksesta",
                        Genre = Genre.Dystopia,
                        PublishYear = 1945,
                        Name = "Eläinten vallankumous",
                        Writer = "Gerge Orwell"
                    };
                    context.Books.Add(vallankumous);

                    var idiootti = new Book
                    {
                        Comments = "Hyvän tekeminen ei ole helppoa",
                        Genre = Genre.Romance,
                        PublishYear = 1869,
                        Name = "Idiootti",
                        Writer = "Fjodor Dostojevski"
                    };
                    context.Books.Add(idiootti);

                    var oikeusjuttu = new Book
                    {
                        Comments = "Vaikealukuinen, mutta kirjan epätoivoista tunnelmaa et voi unohtaa",
                        Genre = Genre.Dystopia,
                        PublishYear = 1925,
                        Name = "Oikeusjuttu",
                        Writer = "Franz Kafka"
                    };
                    context.Books.Add(oikeusjuttu);

                    var kherra = new Book
                    {
                        Comments = "Englantilaiset koulupojat muuttuvat vähitellen villi-ihmisiksi autiolla saarella",
                        Genre = Genre.Dystopia,
                        PublishYear = 1954,
                        Name = "Kärpästen herra",
                        Writer = "William Golding"
                    };
                    context.Books.Add(kherra);

                    var everstinna = new Book
                    {
                        Comments = "Virkistävän erilainen näkökulma sota-aikaan.",
                        Genre = Genre.Biography,
                        PublishYear = 2017,
                        Name = "Everstinna",
                        Writer = "Rosa Liksom"
                    };
                    context.Books.Add(everstinna);

                    var matrix = new Movie
                    {
                        Comments = "Ehkä elämmekin simulaatiossa",
                        Director = "Wachowskin sisarukset",
                        Genre = Genre.ScienceFiction,
                        PublishYear = 1999,
                        Name = "Matrix"
                    };
                    context.Movies.Add(matrix);

                    var tuntematon = new Movie
                    {
                        Comments = "Se alkuperäinen tuntematon",
                        Director = "Väinö Linna",
                        Genre = Genre.War,
                        PublishYear = 1954,
                        Name = "Tuntematon sotilas"
                    };
                    context.Movies.Add(tuntematon);

                    var kane = new Movie
                    {
                        Comments = "Valittu useasti maailman parhaaksi elokuvaksi",
                        Director = "Orson Welles",
                        Genre = Genre.Biography,
                        PublishYear = 1941,
                        Name = "Citizen Kane"
                    };
                    context.Movies.Add(kane);

                    var nykyaika = new Movie
                    {
                        Comments = "Pienen ihmisen puolella nykyaikaa vastaan",
                        Director = "Charles Chaplin",
                        Genre = Genre.Comedy,
                        PublishYear = 1936,
                        Name = "Nykyaika"
                    };
                    context.Movies.Add(nykyaika);

                    var hai = new Movie
                    {
                        Comments = "Kauhuelokuvien klassikko",
                        Director = "Steven Spielberg",
                        Genre = Genre.Thriller,
                        PublishYear = 1975,
                        Name = "Tappajahai"
                    };
                    context.Movies.Add(hai);

                    var titanic = new Movie
                    {
                        Comments = "Yksi kaikkien aikojen katsotuimpia elokuvia",
                        Director = "James Cameron",
                        Genre = Genre.Romance,
                        PublishYear = 1997,
                        Name = "Titanic"
                    };
                    context.Movies.Add(titanic);

                    var pedro = new Movie
                    {
                        Comments = "Puhuminen kannattaa",
                        Director = "Pedro Almodóvar",
                        Genre = Genre.Romance,
                        PublishYear = 2002,
                        Name = "Puhu hänelle"
                    };
                    context.Movies.Add(pedro);

                    var forrest = new Movie
                    {
                        Comments = "Hyväsydäminen, mutta ei kovin älykäs Forrest onnistuu elämässä",
                        Director = "Robert Zemeckis",
                        Genre = Genre.Drama,
                        PublishYear = 1994,
                        Name = "Forrest Gump"
                    };
                    context.Movies.Add(forrest);

                    var x = new Person
                    {
                        Birthday = new DateTime(1969, 1, 16),
                        Education = Education.University,
                        FirstName = "Antti",
                        LastName = "Sumimoto",
                        LuckyNumber = 17,
                        Movie = pedro,
                        Book = kherra
                    };
                    context.People.Add(x);
                    var y = new Person
                    {
                        Birthday = new DateTime(2000, 5, 29),
                        Education = Education.Highschool,
                        FirstName = "Aino",
                        LastName = "Marttila",
                        LuckyNumber = 34,
                        Movie = titanic,
                        Book = everstinna
                    };
                    context.People.Add(y);
                    var z = new Person
                    {
                        Birthday = new DateTime(2002, 1, 17),
                        Education = Education.Primarychool,
                        FirstName = "Juho",
                        LastName = "Kuusimaa",
                        LuckyNumber = 45,
                        Movie = matrix,
                        Book = vallankumous
                    };
                    context.People.Add(z);
                    var v = new Person
                    {
                        Birthday = new DateTime(1970, 1, 14),
                        Education = Education.University,
                        FirstName = "Virpi",
                        LastName = "Virta",
                        LuckyNumber = 10,
                        Movie = tuntematon,
                        Book = veljekset
                    };
                    context.People.Add(v);

                    var w = new Person
                    {
                        Birthday = new DateTime(1952, 6, 25),
                        Education = Education.University,
                        FirstName = "Eli",
                        LastName = "Lake",
                        LuckyNumber = 10,
                        Movie = forrest,
                        Book = alkemisti
                    };
                    context.People.Add(w);
                    context.SaveChanges();

                }
                catch (Exception)
                {

                }
            }
            return webHost;
        }
    }
}