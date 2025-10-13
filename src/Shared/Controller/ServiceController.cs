using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIInstructor.src.Shared.Controller
{
    [Authorize(Policy = "ServicePolicy")]
    [Route("service/[controller]")]
    [ApiController]
    public abstract class ServiceController : ControllerBase
    {
    }
}
