using CamundaSoftwareProject.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CamundaSoftwareProject.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DatabaseController : ControllerBase
    {

        private readonly DatabaseContext _dbContext;

        public DatabaseController(DatabaseContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpPost("create-database", Name = "CreateDatabase")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> CrateDatabase([FromServices] DatabaseContext dbContext)
        {
            bool result = dbContext.Database.EnsureCreated();
            if (result) return Ok(true);
            return Ok(false);
        }

    }
}
