using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

using AIInstructor.src.Kullanicilar.DTO;
using AIInstructor.src.Kullanicilar.Service;
using AIInstructor.src.Shared.Attributes;
using AIInstructor.src.Shared.Controller;

namespace AIInstructor.src.Kullanicilar.Controller
{

    public class KullaniciController:UIController
    {
        private readonly IKullaniciService kullaniciService;
        
        public KullaniciController(IKullaniciService kullaniciService)
        {
            this.kullaniciService= kullaniciService;
           
        }


        [HttpGet]
        [Permission("KullaniciYonetimi.View")]
        public async Task<IEnumerable<KullaniciDTO>> TumKullanicilariGetir()
        {
          
            Log.Information("TumKullanicilariGetir metoduna girildi.");
           
            return await this.kullaniciService.GetAllAsync(q=>q.Include(x=>x.KullaniciKullaniciGruplar.Where(x=>!x.IsDeleted)).ThenInclude(a=>a.KullaniciGrup).ThenInclude(e=>e.KullaniciGrupRoller).ThenInclude(e=>e.Rol));
        }
    }
}
