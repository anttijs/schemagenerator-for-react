using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleDbLib;
using System.Globalization;

namespace SchemaGenerator.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DateTime, string>().ConvertUsing(new DateToStringConverter());
            CreateMap<string, DateTime>().ConvertUsing(new StringToDateConverter());
            CreateMap<Enum, string>().ConvertUsing(e => e.GetDescription());

            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<Person, PersonListDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, BookListDTO>();

            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<Movie, MovieListDTO>();

        }
    }
    public class DateToStringConverter : ITypeConverter<DateTime, string>
    {
        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            if (source == DateTime.MinValue)
                return "";
            CultureInfo culture = CultureInfo.CreateSpecificCulture("fi-FI");
            return source.ToString("d", culture);
        }
    }
    public class StringToDateConverter : ITypeConverter<string, DateTime>
    {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            return DateTime.Parse(source);
        }
    }

}
