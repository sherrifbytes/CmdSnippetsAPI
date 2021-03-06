using System.Collections.Generic;
using CmdSnippetsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using CmdSnippetsAPI.Data;
using AutoMapper;
using CmdSnippetsAPI.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace CmdSnippetsAPI.Controllers
{
    // api/commands
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICmdSnippetsRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICmdSnippetsRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
                
        // GET api/commands
        [HttpGet]
        public ActionResult <IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var cmdItems = _repo.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(cmdItems));
        }

        // GET api/commands/{id}
        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult <CommandReadDto> GetCommandById(int id)
        {
            var cmdItem = _repo.GetCommandById(id);
            if (cmdItem != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(cmdItem));
            }
            return NotFound();
        }

        // POST api/commands
        [HttpPost]
        public ActionResult <CommandReadDto> CreateCommand(CommandCreateDto cmdCreateDto)
        {
            var cmdModel = _mapper.Map<Command>(cmdCreateDto);
                _repo.CreateCommand(cmdModel);
                _repo.SaveChanges();

            var cmdReadDto = _mapper.Map<CommandReadDto>(cmdModel);

            return CreatedAtRoute(nameof(GetCommandById), new {Id = cmdReadDto.Id}, cmdReadDto);
        }

        // PUT api/commands/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto cmdUpdateDto)
        {
            var cmdModelFromRepo = _repo.GetCommandById(id);
            if(cmdModelFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(cmdUpdateDto, cmdModelFromRepo); 
            /* This acts as an auto update through mapping and comparing 
            source and destination, all we need now is to flush/save changes

            For maintaining a seperate interface from implimentation
            we could still call an update method */
            _repo.UpdateCommand(cmdModelFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }

        // PATCH api/commands/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            var cmdModelFromRepo = _repo.GetCommandById(id);
            if (cmdModelFromRepo == null)
            {
                return NotFound();
            }
            var cmdToPatch = _mapper.Map<CommandUpdateDto>(cmdModelFromRepo);
            patchDoc.ApplyTo(cmdToPatch, ModelState);

            if (!TryValidateModel(cmdToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(cmdToPatch, cmdModelFromRepo);
            _repo.UpdateCommand(cmdModelFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }

        // DELETE api/commads/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var cmdModelFromRepo = _repo.GetCommandById(id);
            if (cmdModelFromRepo == null)
            {
                return NotFound();
            }

            _repo.DeleteCommand(cmdModelFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }

    }
}