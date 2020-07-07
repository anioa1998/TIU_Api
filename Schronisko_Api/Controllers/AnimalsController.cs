using Schronisko_Api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Schronisko_Api.DTOs;
using Microsoft.EntityFrameworkCore.Query;
using SQLitePCL;
using System.IO;
using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Schronisko_Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Schronisko_Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private ShelterDbContext _context;
        private FileService _fileService; 
        public AnimalsController(ShelterDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        #region GET
    
        [HttpGet]
        [EnableCors("developerska")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<List<GetAllAnimalsDTO>> GetAllAnimals()
        {
            List<Animal> animals = _context.Animals.ToList();
            GetAllAnimalsDTO temporaryDTOObject;
            List<GetAllAnimalsDTO> resultList = new List<GetAllAnimalsDTO>();
            //Animal to GetAnimalsDTO mapping
            /* Zazwyczaj korzysta się z automappera definiowanego w osobnych klasach, gdzie określa się, które pole/właściwość z klasy
                źródłowej ma być przypisane do pola/właściwości klasy docelowej. */
            foreach (var animal in animals)
            {
                temporaryDTOObject = new GetAllAnimalsDTO();
                temporaryDTOObject.Id = animal.Id;
                temporaryDTOObject.Name = animal.Name;
                temporaryDTOObject.Age = animal.Age;
                temporaryDTOObject.Photo = animal.Photo;
                temporaryDTOObject.Reference = animal.Reference;
                resultList.Add(temporaryDTOObject);
            }


            return Ok(resultList);
        }
        [HttpGet("{id}"), Authorize]
        [EnableCors("developerska")]
        public ActionResult<GetAnimalDTO> GetAnimal(int id)
        {
            /*Żeby nie pobierać z bazy 2 razy tych samych danych typu name, id, age itd. można przekazać obiekt FromBody z przepisanymi danymi
            z wybranego obiektu i uzupełnić jedynie o brakujące*/
            try
            {
                GetAnimalDTO resultObject = new GetAnimalDTO();
                DetailVolunteerDTO detailVolunteerDTO = new DetailVolunteerDTO();
                var animal = _context.Animals.Include("Status")
                                       .Include("Volunteer")
                                       .Where(a => a.Id == id)
                                       .Single();
                //Animal to GetAnimalDTO mapper
                resultObject.Id = animal.Id;
                resultObject.Name = animal.Name;
                resultObject.Age = animal.Age;
                resultObject.Photo = animal.Photo;
                resultObject.Reference = animal.Reference;
                resultObject.Status = animal.Status.Description;
                

                detailVolunteerDTO.Name = animal.Volunteer.Name;
                detailVolunteerDTO.Surname = animal.Volunteer.Surname;
                detailVolunteerDTO.Age = animal.Volunteer.Age;
                detailVolunteerDTO.Phone = animal.Volunteer.Phone;

                resultObject.VolunteerDTO = detailVolunteerDTO;

                return Ok(resultObject);
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpGet("status"), Authorize(Roles ="Admin")]
        [EnableCors("developerska")]
        public ActionResult<List<string>> GetAllStatus()
        {
            try
            {
                List<string> statusList = _context.Status.Select(s => s.Description)
                                                         .ToList();
                return Ok(statusList);
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpGet("shortvolunteer"), Authorize(Roles = "Admin")]
        [EnableCors("developerska")]
        public ActionResult<List<ShortVolunteerDTO>> GetVolunteerList()
        {
            try
            { 
                List<Volunteer> allVolunteerList = _context.Volunteers.ToList();
                ShortVolunteerDTO temporaryDTOObject = null;
                List<ShortVolunteerDTO> resultList = new List<ShortVolunteerDTO>();

                //Volunteer to ShortVolunteer mapping
                
                foreach(var volunteer in allVolunteerList)
                {
                    temporaryDTOObject = new ShortVolunteerDTO();
                    temporaryDTOObject.Id = volunteer.Id;
                    temporaryDTOObject.Name = volunteer.Name;
                    temporaryDTOObject.Surname = volunteer.Surname;
                    temporaryDTOObject.Age = volunteer.Age;
                    resultList.Add(temporaryDTOObject);
                }
                return Ok(resultList);
            }
            catch
            {
                return NotFound();
            }
        }

        

        #endregion
        [HttpPost]
        [EnableCors("developerska"), Authorize(Roles = "Admin")]
        public ActionResult CreateAnimal([FromBody] CreateAnimalDTO createNewAnimal)
        {
            try
            {
                Animal newAnimalObject = new Animal();

                //CreateAnimalDTO to Animal mapping
                newAnimalObject.Name = createNewAnimal.Name;
                newAnimalObject.Age = createNewAnimal.Age;
                newAnimalObject.Reference = createNewAnimal.Reference;
                newAnimalObject.Photo = createNewAnimal.Photo;
                //photo - skopiować fizycznyh obiekt z podanej ścieżki do określonego miejsca na serwerze
                //na sztywno można ustawić assets do aplikacji - docelowo miejsce na serwerze, żeby pobierało zawsze z chmury;
                //zapisanie w bazie ścieżki do pliku zapisanego na serwerze;
                newAnimalObject.Status = _context.Status.Where(s => s.Description == createNewAnimal.Status)
                                                        .Single();
                newAnimalObject.Volunteer = _context.Volunteers.Where(v => v.Id == createNewAnimal.VolunteerDTO.Id)
                                                               .Single();

                _context.Animals.Add(newAnimalObject);
                _context.SaveChanges();
                return StatusCode(201);
            }
            catch
            {
                return StatusCode(404);
            }
        }
        [HttpPost("upload"), DisableRequestSizeLimit, Authorize(Roles = "Admin")]
        [EnableCors("developerska")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                string dbPath = await _fileService.SaveFileAsync(file);

                if (!string.IsNullOrEmpty(dbPath))
                { 
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        [EnableCors("developerska")]
        public ActionResult UpdateAnimal(int id, [FromBody] UpdateAnimalDTO updatedAnimal)
        {
            if (!ModelState.IsValid || null == updatedAnimal)
            {
                return NoContent();
            }
            

            try
            {
                Animal selectedAnimal = _context.Animals.Where(a => a.Id == id)
                                                        .Single();
                selectedAnimal.Name = updatedAnimal.Name;
                selectedAnimal.Age = updatedAnimal.Age;
                selectedAnimal.Reference = updatedAnimal.Reference;
                selectedAnimal.Photo = selectedAnimal.Photo;
                selectedAnimal.Status = _context.Status.Where(s => s.Description == updatedAnimal.Status)
                                                       .Single();
                selectedAnimal.Volunteer = _context.Volunteers.Where(v => v.Id == updatedAnimal.VolunteerDTO.Id)
                                                              .Single();

                _context.Update(selectedAnimal);
                _context.SaveChanges();
                return Ok();
            }
            catch
            {
                return NotFound();
            }

        }
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        [EnableCors("developerska")]
        public ActionResult DeleteAnimal(int id)
        {
            try
            {
                Animal animalToDelete = _context.Animals.Where(a => a.Id == id)
                                                        .Single();
                

                var pathToDelete = Path.Combine(@"D:\Semestr VI\TIU\Zad1\Zad1App\src", animalToDelete.Photo);
                _context.Animals.Remove(animalToDelete);
                _context.SaveChanges();
                _fileService.DeleteSelectedFile(pathToDelete);
                return Ok();
            } 
            catch
            {
                return NotFound();
            }
        }
    }
}