using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.KullaniciGruplar.DTO;
using AIInstructor.src.KullaniciGruplar.Service;
using AIInstructor.src.MenuItemler.DTO;
using AIInstructor.src.Shared.Attributes;
using AIInstructor.src.Shared.Controller;

namespace AIInstructor.src.KullaniciGruplar.Controller
{
    public class KullaniciGrupController:UIController
    {
        private readonly IKullaniciGrupService kullaniciGrupService;

        public KullaniciGrupController(IKullaniciGrupService kullaniciGrupService)
        {
            this.kullaniciGrupService = kullaniciGrupService;
        }

        [HttpGet]
        [Permission("KullaniciGrupYonetimi.View")]
        public async Task<IEnumerable<KullaniciGrupDTO>> TumKUllaniciGruplariniGetir()
        {
            int a = 0;
            
            return await this.kullaniciGrupService.GetAllAsync(include => include.Include(e => e.KullaniciGrupRoller.Where(e => !e.IsDeleted)).ThenInclude(e => e.Rol));
        }


        [HttpGet("{id}")]
        [Permission("KullaniciGrupYonetimi.View")]
        public async Task<KullaniciGrupDTO> GetById(Guid id)
        {
            return await this.kullaniciGrupService.GetByIdAsync(id, include => include.Include(e => e.KullaniciGrupRoller).ThenInclude(e => e.Rol));
        }

        [HttpPost]
        [Permission("KullaniciGrupYonetimi.Manage")]
        public async Task<ActionResult<KullaniciGrupDTO>> KullaniciGrupEkle(KullaniciGrupDTO yeniGrup)
        {
            var createdKullaniciGrup = await this.kullaniciGrupService.AddAsync(yeniGrup);
            return CreatedAtAction(nameof(GetById), new { id = createdKullaniciGrup.Id }, createdKullaniciGrup);
        }

        [HttpPut("{id}")]
        [Permission("KullaniciGrupYonetimi.Manage")]
        public async Task<IActionResult> Update(Guid id, [FromBody] KullaniciGrupDTO dto)
        {
            dto.Id = id;
            await this.kullaniciGrupService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Permission("MenuYonetimi.Manage")]
        public async Task<IActionResult> Delete(Guid id)
        {

            await this.kullaniciGrupService.DeleteAsync(id);
            return Ok();
        }

    }
}
