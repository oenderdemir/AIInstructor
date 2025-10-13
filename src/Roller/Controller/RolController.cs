using Microsoft.AspNetCore.Mvc;
using AIInstructor.src.MenuItemler.DTO;
using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Roller.Service;
using AIInstructor.src.Shared.Attributes;
using AIInstructor.src.Shared.Controller;

namespace AIInstructor.src.Roller.Controller
{
    public class RolController:UIController
    {
        private readonly IRolService rolService;
        public RolController(IRolService rolService)
        {
            this.rolService = rolService;
        }

        [HttpGet("ViewRolleriniGetir")]
        [Permission("RolYonetimi.View")]
        public async Task<IEnumerable<RolDto>> ViewRolleriniGeti()
        {
            return await rolService.ViewRolleriniGetir();
        }

        [HttpGet]
        [Permission("RolYonetimi.View")]
        public async Task<IEnumerable<RolDto>> TumRolleriGetir()
        {
            return await rolService.GetAllAsync();
        }



    }
}
