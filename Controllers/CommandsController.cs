using System.Collections.Generic;
using CmdSnippetsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using CmdSnippetsAPI.Data;
using CmdSnippetsAPI.Profiles;
using AutoMapper;
using CmdSnippetsAPI.DTOs;

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
            // return Ok(cmdItems);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(cmdItems));
        }

        //GET api/commands/{id}
        [HttpGet("{id}")]
        public ActionResult <CommandReadDto> GetCommandById(int id)
        {
            var cmdItem = _repo.GetCommandById(id);
            if (cmdItem != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(cmdItem));
            }
            return NotFound();
        }
        
    }
}