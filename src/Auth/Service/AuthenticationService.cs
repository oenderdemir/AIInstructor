using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using AIInstructor.src.Auth.DTO;
using AIInstructor.src.KullaniciGrupRoller.Repository;
using AIInstructor.src.KullaniciKullaniciGruplar.Repository;
using AIInstructor.src.Kullanicilar.Repository;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Shared.CurrentUser.Service;
using AIInstructor.src.Shared.RDBMS.Service;

namespace AIInstructor.src.Auth.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJWTTokenService tokenService;
        private readonly IKullaniciRepository kullaniciRepository;
        private readonly IKullaniciKullaniciGrupRepository kullaniciKullaniciGrupRepository;
        private readonly IKullaniciGrupRolRepository kullaniciGrupRolRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IHashService hashService;

        public AuthenticationService(IKullaniciRepository kullaniciRepository,
            IKullaniciKullaniciGrupRepository kullaniciKullaniciGrupRepository,
            IKullaniciGrupRolRepository kullaniciGrupRolRepository,
            IJWTTokenService tokenService,
            ICurrentUserService currentUserService,
            IHashService hashService)
        {
            this.kullaniciRepository = kullaniciRepository;
            this.kullaniciKullaniciGrupRepository = kullaniciKullaniciGrupRepository;
            this.tokenService = tokenService;
            this.hashService = hashService;
            this.kullaniciGrupRolRepository = kullaniciGrupRolRepository;
            this.currentUserService = currentUserService;

        }

        public async Task<LoginResponseDTO> ChangePassword(ChangePasswordRequestDTO model)
        {
            if(model.NewPassword!=model.NewPassword2)
            {
                throw new Exception("Yeni Parolalar Tutmuyor");
            }

            var userName=currentUserService.GetCurrentUsername();
            var user = await this.kullaniciRepository.GetByKullaniciAdiAsync(userName);
            var hashedCurrentPassword = await this.hashService.ComputeHash(model.CurrentPassword);
            if(user.Parola!=hashedCurrentPassword)
            {
                throw new Exception("Hatali Parola");
            }

            var newHashedPassword = await this.hashService.ComputeHash(model.NewPassword);

            user.Parola=newHashedPassword;
            user.Status=enumKullaniciStatus.Standart;

            await this.kullaniciRepository.SaveChangesAsync();
            return await LogoutAsync();


        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            LoginResponseDTO response = new();
            if (string.IsNullOrEmpty(request.KullaniciAdi) || string.IsNullOrEmpty(request.Parola))
            {
                throw new ArgumentNullException(nameof(request));
            }
           
            var user = await this.kullaniciRepository.GetByKullaniciAdiAsync(request.KullaniciAdi);


            var hashedPassword = await this.hashService.ComputeHash(request.Parola);
            if (user == null)
            {
                response.AuthenticateResult = false;
                response.AuthToken = string.Empty;
            }
            else if (hashedPassword != user.Parola)
            {
                response.AuthenticateResult = false;
                response.AuthToken = string.Empty;
            }
            else
            {
                var kullaniciGruplar = await this.kullaniciKullaniciGrupRepository.Where(e => e.Kullanici == user,e=>e.Include(x=>x.KullaniciGrup))
                    .Select(e=>e.KullaniciGrup).ToListAsync();

                List<Rol> roles = new List<Rol>();

                foreach (var kullaniciGrup in kullaniciGruplar)
                {
                    var roller=await this.kullaniciGrupRolRepository.Where(e => e.KullaniciGrup == kullaniciGrup, q => q.Include(x => x.Rol))
                                                              .Select(e=>e.Rol).ToListAsync();
                    roles.AddRange(roller);
                }

               
                //userRoles.ForEach(ur => roles.Add("roleismi"));
                var generatedTokenInformation = await tokenService.GenerateToken(new GenerateTokenRequest { KullaniciAdi = user.KullaniciAdi, Ad = user.Ad, Soyad = user.Soyad, EMail = user.Email, Roller = roles });

                response.AccessTokenExpireDate = DateTime.UtcNow.AddHours(8);
                response.AuthenticateResult = true;
                response.AuthToken = generatedTokenInformation.Token;
                response.KullaniciStatus = user.Status;

            }


            return await Task.FromResult(response);
        }

        public async Task<LoginResponseDTO> LogoutAsync()
        {
            LoginResponseDTO response = new();
            response.AccessTokenExpireDate = DateTime.UtcNow.AddHours(8);
            response.AuthenticateResult = false;
            response.AuthToken = "";
            return await Task.FromResult(response);
        }
    }
}
