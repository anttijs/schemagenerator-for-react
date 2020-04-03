using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExampleDbLib;
using System.Text.Json;

namespace SchemaGenerator.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CRUDController : ControllerBase
    {
        private readonly ExampleDbContext _context;
        private readonly IMapper _mapper;

        public CRUDController(ExampleDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ReplyData>> GetPeople()
        {
            try
            {
                var response = new ReplyData();
                var data = await _context.People
                    .Include(e => e.Book)
                    .Include(e => e.Movie)
                    .ToListAsync();
                response.data = _mapper.Map<List<Person>, List<PersonListDTO>>(data);
                PersonListDTO p = new PersonListDTO();
                response.schema = new Schema(p, _context);
                return response;
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load data");
            }
        }
        [HttpGet]
        public async Task<ActionResult<ReplyData>> GetMovies()
        {
            try
            { 
                var response = new ReplyData();
                var data = await _context.Movies.ToListAsync();
                response.data = _mapper.Map<List<Movie>, List<MovieListDTO>>(data);
                MovieListDTO p = new MovieListDTO();
                response.schema = new Schema(p, _context);
                return response;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load data");
            }
        }
        [HttpGet]
        public async Task<ActionResult<ReplyData>> GetBooks()
        {
            try
            {
            var response = new ReplyData();
            var data = await _context.Books.ToListAsync();
            response.data = _mapper.Map<List<Book>, List<BookListDTO>>(data);
            BookListDTO p = new BookListDTO();
            response.schema = new Schema(p, _context);
            return response;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load data");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReplyData>> GetPerson(int id)
        {
            try
            { 
            var obj = new PersonDTO
            {
                Id = -1
            };

            if (id != -1)
            {
                    var p = await _context.People.Where(x => x.Id == id).FirstOrDefaultAsync();
                    obj = _mapper.Map<PersonDTO>(p);
            }
            if (obj == null && id != -1)
            {
                return NotFound();
            }
            var response = new ReplyData
            {
                data = obj,
                schema = new Schema(obj, _context)
            };
            return response;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load data");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReplyData>> GetMovie(int id)
        {
            try
            { 
            var obj = new MovieDTO
            {
                Id = -1
            };

            if (id != -1)
            {
                obj = await _context.Movies.Where(x => x.Id == id).ProjectTo<MovieDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            }
            if (obj == null && id != -1)
            {
                return NotFound();
            }
            var response = new ReplyData
            {
                data = obj,
                schema = new Schema(obj, _context)
            };
            return response;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load data");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReplyData>> GetBook(int id)
        {
            try
            { 
            var obj = new BookDTO
            {
                Id = -1
            };

            if (id != -1)
            {
                obj = await _context.Books.Where(x => x.Id == id).ProjectTo<BookDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            }
            var response = new ReplyData
            {
                data = obj,
                schema = new Schema(obj, _context)
            };
            return response;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load data");
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> PutPerson(int id, PersonDTO Person)
        {
            try
            { 
            if (id != Person.Id)
            {
                return BadRequest("Wrong id");
            }
            var oldPerson = await _context.People.FindAsync(id);
            if (oldPerson==null)
            {
                return BadRequest($"Could not find data for {Person.Name}");
            }

            _mapper.Map<PersonDTO, Person>(Person, oldPerson);
            await _context.SaveChangesAsync();
            return $"Successfully updated information about person {Person.FirstName} {Person.LastName}";
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update data");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> PutBook(int id, BookDTO Book)
        {
            try
            { 
            if (id != Book.Id)
            {
                return BadRequest("Wrong id");
            }
            var oldBook = await _context.Books.FindAsync(id);
            if (oldBook == null)
            {
                return BadRequest($"Could not find data for {Book.Name}");
            }
            _mapper.Map<BookDTO, Book>(Book, oldBook);
            await _context.SaveChangesAsync();
            return $"Successfully updated information about Book {Book.Name}";
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update data");
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> PutMovie(int id, MovieDTO Movie)
        {
            try
            {
            if (id != Movie.Id)
            {
                return BadRequest("Wrong id");
            }
            var oldMovie = await _context.Movies.FindAsync(id);
            if (oldMovie == null)
            {
                return BadRequest($"Could not find data for {Movie.Name}");
            }
            _mapper.Map<MovieDTO, Movie>(Movie, oldMovie);
            await _context.SaveChangesAsync();
            return $"Successfully updated information about Movie {Movie.Name}";
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update data");
            }
        }
        [HttpPost]
        public async Task<ActionResult<string>> PostPerson(PersonDTO personDTO)
        {
            try
            { 
            Person person = new Person();
            var tmp = person.Id;
            _mapper.Map<PersonDTO, Person>(personDTO, person);
            person.Id = tmp;
            _context.People.Add(person);
            await _context.SaveChangesAsync();
            return $"Successfully added information about person {person.FirstName} {person.LastName}";
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add data");
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> PostBook(BookDTO BookDTO)
        {
            try
            { 
            Book Book = new Book();
            var tmp = Book.Id;
            _mapper.Map<BookDTO, Book>(BookDTO, Book);
            Book.Id = tmp;
            _context.Books.Add(Book);
            await _context.SaveChangesAsync();
            return $"Successfully added information about Book {Book.Name}";
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add data");
            }
        }
        [HttpPost]
        public async Task<ActionResult<string>> PostMovie(MovieDTO MovieDTO)
        {
            try
            { 
            Movie Movie = new Movie();
            var tmp = Movie.Id;
            _mapper.Map<MovieDTO, Movie>(MovieDTO, Movie);
            Movie.Id = tmp;
            _context.Movies.Add(Movie);
            await _context.SaveChangesAsync();
            return $"Successfully added information about Movie {Movie.Name}";
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add data");
            }
        }
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeletePerson(int id)
        {
            try
            { 
            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.People.Remove(person);
            await _context.SaveChangesAsync();
            return $"Successfully deleted information about person {person.FirstName} {person.LastName}";
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete data");
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteMovie(int id)
        {
            try
            { 
            var Movie = await _context.Movies.FindAsync(id);
            if (Movie == null)
            {
                return NotFound();
            }
            var isUsed = await _context.People.Where(e => e.MovieId == id).AnyAsync();
            if (isUsed == true)
            {
                return BadRequest("Cannot delete. The movie is referenced in a Person entity.");
            }
            _context.Movies.Remove(Movie);
            await _context.SaveChangesAsync();
            return $"Successfully deleted information about Movie {Movie.Name}";
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete data");
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteBook(int id)
        {
            var Book = await _context.Books.FindAsync(id);
            if (Book == null)
            {
                return NotFound();
            }
            var isUsed = await _context.People.Where(e => e.BookId == id).AnyAsync();
            if (isUsed == true)
            {
                return BadRequest("Cannot delete. The book is referenced in a Person entity.");
            }
            _context.Books.Remove(Book);
            await _context.SaveChangesAsync();
            return $"Successfully deleted information about Book {Book.Name}";
        }
    }
}
