using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.MenuItemler.DTO;
using AIInstructor.src.MenuItemler.Service;
using AIInstructor.src.Shared.Attributes;
using AIInstructor.src.Shared.Controller;

namespace AIInstructor.src.MenuItemler.Controller
{
    public class MenuItemController:UIController
    {
        private readonly IMenuItemService menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            this.menuItemService = menuItemService;
        }

        [HttpGet]
        [Permission("MenuYonetimi.View")]
        public async Task<IEnumerable<MenuItemDto>> TumMenuItemlariniGetir()
        {
            return await this.menuItemService.GetAllAsync(include=>include.Include(e=>e.MenuItemRoller.Where(e=>!e.IsDeleted)).ThenInclude(e=>e.Rol));
        }

        [HttpGet("{id}")]
        [Permission("MenuYonetimi.View")]
        public async Task<MenuItemDto> GetById(Guid id)
        {
            return await this.menuItemService.GetByIdAsync(id, include => include.Include(e => e.MenuItemRoller).ThenInclude(e => e.Rol));
        }


        [HttpGet("MenuAgaciGetir")]
      
        public async Task<IEnumerable<MenuItemDto>> MenuAgaciGetir()
        {
            return await this.menuItemService.MenuAgaciGetir();
        }

        [HttpPost]
        [Permission("MenuYonetimi.Manage")]
        public async Task<ActionResult<MenuItemDto>> Create( MenuItemDto dto)
        {
            var createdMenuItem = await this.menuItemService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdMenuItem.Id }, createdMenuItem);
        }

        [HttpPut("{id}")]
        [Permission("MenuYonetimi.Manage")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MenuItemDto dto)
        {
            dto.Id= id;
            await this.menuItemService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Permission("MenuYonetimi.Manage")]
        public async Task<IActionResult> Delete(Guid id)
        {

            await this.menuItemService.DeleteAsync(id);
            return Ok();
        }

    }
}
