using System.Threading.Tasks;
using Database;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphQL
{
    [Route("graphql")]
    [ApiController]
    public class GraphQLController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GraphQLController(ApplicationDbContext db) => _db = db;

        [Authorize]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
        var inputs = query.Variables.ToInputs();

        var schema = new Schema
        {
            Query = new AuthorQuery(_db)
        };

        var result = await new DocumentExecuter().ExecuteAsync(_ =>
        {
            _.Schema = schema;
            _.Query = query.Query;
            _.OperationName = query.OperationName;
            _.Inputs = inputs;
        });

        if(result.Errors?.Count > 0)
        {
            return BadRequest();
        }

        return Ok(result);
        }
    }
}