using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIInstructor.src.Shared.Controller
{
    [Authorize(Policy = "UIPolicy")]
    [Route("ui/[controller]")]
    [ApiController]
    public abstract class UIController : ControllerBase
    {
    }
}
