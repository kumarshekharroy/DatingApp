using System.Collections.Generic;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController:ControllerBase
    {
        private readonly DataContext _DataContext;
        public ValuesController(DataContext dataContext)
        {
            _DataContext=dataContext;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _DataContext.Values.ToListAsync());
        }

        [HttpGet("{id}")]
        public async  Task<IActionResult> Get(int id)
        {
            return  Ok(await _DataContext.Values.FirstOrDefaultAsync(x=>x.Id==id));
        }
         
        public void Post([FromBody]string value)
        {
            
        }

        [HttpPut("{id}")]
        public void Put(int id,[FromBody]string value)
        {
            
        }
    }
}