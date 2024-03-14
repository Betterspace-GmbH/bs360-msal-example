using AspNetExample.Clients;
using Microsoft.AspNetCore.Mvc;

namespace AspNetExample.Controllers
{
    [ApiController]
    [Route("/api")]
    public class TokenController(TokenService _tokenService) : ControllerBase
    {
        [HttpGet("token")]
        public async Task<IActionResult> GetB2CToken()
        {
            return Ok(await _tokenService.GetAccessTokenAsync());
        }
    }
}
