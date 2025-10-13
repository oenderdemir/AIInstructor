
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AIInstructor.src.KullaniciKullaniciGruplar.DTO;
using AIInstructor.src.KullaniciKullaniciGruplar.Service;
using AIInstructor.src.Kullanicilar.DTO;
using AIInstructor.src.Shared.Controller;

namespace AIInstructor.src.KullaniciKullaniciGruplar.Controller
{
    public class KullaniciKullaniciGruplarController : UIController
    {
      
        private readonly IKullaniciKullaniciGrupService kullaniciKullaniciGrupService;

        public KullaniciKullaniciGruplarController( IKullaniciKullaniciGrupService kullaniciKullaniciGrupService)
        {
         
            this.kullaniciKullaniciGrupService = kullaniciKullaniciGrupService;
        }

        [HttpGet]
        public async Task<IEnumerable<KullaniciKullaniciGrupDTO>> GetAll()
        {
            Log.Information("GetAll Metoduna girildi.");
            var r= await kullaniciKullaniciGrupService.GetAllAsync();
            return r;

            //int a = 0;
        }
    }
}
