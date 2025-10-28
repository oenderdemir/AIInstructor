using Microsoft.EntityFrameworkCore;
using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.KullaniciKullaniciGruplar.Entitiy;
using AIInstructor.src.Kullanicilar.Entity;
using AIInstructor.src.MenuItemler.Entity;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Roller.Entity;

namespace AIInstructor.src.Context
{
    public static class VTSDbContextExtensions
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VTSDbContext>();
         

            var configuratioon = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Migration'lar uygulanmamışsa uygula
            await context.Database.MigrateAsync();

            // Örnek: Roller tablosunu doldur
            if (!context.Set<Rol>().Any())
            {
                context.Set<Rol>().AddRange(new List<Rol>
                {
                    new Rol { Ad = "Admin", Domain = "KullaniciTipi", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "UIUser", Domain = "KullaniciTipi", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "ServiceUser", Domain = "KullaniciTipi", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "View", Domain = "KullaniciYonetimi", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "Manage", Domain = "KullaniciYonetimi", CreatedAt = DateTime.UtcNow },

                    new Rol { Ad = "View", Domain = "KullaniciGrupYonetimi", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "Manage", Domain = "KullaniciGrupYonetimi", CreatedAt = DateTime.UtcNow },

                    new Rol { Ad = "View", Domain = "RolYonetimi", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "Manage", Domain = "RolYonetimi", CreatedAt = DateTime.UtcNow },

                    new Rol { Ad = "View", Domain = "MenuYonetimi", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "Manage", Domain = "MenuYonetimi", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "AIKisi", Domain = "SystemRole", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "AIInstructor", Domain = "SystemRole", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "DersYetkilisi", Domain = "SystemRole", CreatedAt = DateTime.UtcNow },
                    new Rol { Ad = "Ogrenci", Domain = "SystemRole", CreatedAt = DateTime.UtcNow },
                });

                await context.SaveChangesAsync();
            }

            if (!context.Set<Kullanici>().Any())
            {
                context.Set<Kullanici>().AddRange(new List<Kullanici>
                {
                    new Kullanici { Ad="admin",Soyad="kullanicisi",TCNO="23868366202",KullaniciAdi="admin",Email="admin@test.test",AvatarPath=" ",
                    ParolaHash = "4DFF4EA340F0A823F15D3F4F01AB62EAE0E5DA579CCB851F8DB9DFE84C58B2B37B89903A740E1EE172DA793A6E79D560E5F7F9BD058A12A280433ED6FA46510A"
                   , Durum = "Aktif", IsDeleted=false},
                    new Kullanici { Ad="test",Soyad="kullanicisi",TCNO="13868366208",KullaniciAdi="test",Email="test@test.test",AvatarPath=" ",
                    ParolaHash = "4DFF4EA340F0A823F15D3F4F01AB62EAE0E5DA579CCB851F8DB9DFE84C58B2B37B89903A740E1EE172DA793A6E79D560E5F7F9BD058A12A280433ED6FA46510A"
                   , Durum = "Aktif", IsDeleted=false}

                });

                await context.SaveChangesAsync();
            }

            if (!context.Set<KullaniciGrup>().Any())
            {
                context.Set<KullaniciGrup>().AddRange(new List<KullaniciGrup>
                {
                    new KullaniciGrup { Ad="Yöneticiler",IsDeleted=false},
                    new KullaniciGrup { Ad="Standart Kullanicilar",IsDeleted=false}

                });

                await context.SaveChangesAsync();
            }

            if (!context.Set<KullaniciKullaniciGrup>().Any())
            {

                Kullanici cuce = await context.Set<Kullanici>().FirstOrDefaultAsync(e => e.KullaniciAdi.Equals("admin"));
                KullaniciGrup yoneticiler = await context.Set<KullaniciGrup>().FirstOrDefaultAsync(e => e.Ad.Equals("Yöneticiler"));

                Kullanici test = await context.Set<Kullanici>().FirstOrDefaultAsync(e => e.KullaniciAdi.Equals("test"));
                KullaniciGrup stadndartKullanicilar = await context.Set<KullaniciGrup>().FirstOrDefaultAsync(e => e.Ad.Equals("Standart Kullanicilar"));

                context.Set<KullaniciKullaniciGrup>().AddRange(new List<KullaniciKullaniciGrup>
                {
                    new KullaniciKullaniciGrup { Kullanici=cuce,KullaniciGrup=yoneticiler},
                    new KullaniciKullaniciGrup { Kullanici=test,KullaniciGrup=stadndartKullanicilar},

                });

                await context.SaveChangesAsync();
            }

            if (!context.Set<KullaniciKullaniciGrup>().Any())
            {

                Kullanici cuce = await context.Set<Kullanici>().FirstOrDefaultAsync(e => e.KullaniciAdi.Equals("cuce"));
                KullaniciGrup yoneticiler = await context.Set<KullaniciGrup>().FirstOrDefaultAsync(e => e.Ad.Equals("Yöneticiler"));

                context.Set<KullaniciKullaniciGrup>().AddRange(new List<KullaniciKullaniciGrup>
                {
                    new KullaniciKullaniciGrup { Kullanici=cuce,KullaniciGrup=yoneticiler}
                });

                await context.SaveChangesAsync();
            }

            if (!context.Set<KullaniciGrupRol>().Any())
            {
                KullaniciGrup yoneticiler = await context.Set<KullaniciGrup>().FirstOrDefaultAsync(e => e.Ad.Equals("Yöneticiler"));
                KullaniciGrup stadndartKullanicilar = await context.Set<KullaniciGrup>().FirstOrDefaultAsync(e => e.Ad.Equals("Standart Kullanicilar"));
                var roller = await context.Set<Rol>().Where(e => !e.IsDeleted).ToListAsync();
                foreach (var rol in roller)
                {
                    context.Set<KullaniciGrupRol>().AddAsync(new KullaniciGrupRol()
                    {
                        Rol = rol,
                        KullaniciGrup = yoneticiler
                    });
                    if (rol.Ad == "View" || rol.Ad == "UIUser")
                    {
                        context.Set<KullaniciGrupRol>().AddAsync(new KullaniciGrupRol()
                        {
                            Rol = rol,
                            KullaniciGrup = stadndartKullanicilar
                        });
                    }
                }


                await context.SaveChangesAsync();
            }

            if (!context.Set<MenuItem>().Any())
            {
                var YonetimPaneli = new MenuItem { Label = "Yönetim Paneli", Icon = "fa-solid fa-people-arrows", RouterLink = "", QueryParams = null, IsDeleted = false, MenuOrder = 100 };
                var RolYonetimiMenu = new MenuItem { Label = "Rol Yönetimi", Icon = "fa-solid fa-drum-steelpan", RouterLink = "roller", QueryParams = null, IsDeleted = false, MenuOrder = 1, Parent = YonetimPaneli };
                var KulalniciGrupYonetimiMenu = new MenuItem { Label = "Kullanici Grup Yönetimi", Icon = "fa-solid fa-people-roof", RouterLink = "kullanici-gruplar", QueryParams = null, IsDeleted = false, MenuOrder = 2, Parent = YonetimPaneli };
                var KulalniciYonetimiMenu = new MenuItem { Label = "Kullanici Yönetimi", Icon = "fa-regular fa-user", RouterLink = "kullanicilar", QueryParams = null, IsDeleted = false, MenuOrder = 3, Parent = YonetimPaneli };
                var menuYonetimi = new MenuItem { Label = "Menü Yönetimi", Icon = "fa-solid fa-gopuram", RouterLink = "menuler", QueryParams = null, IsDeleted = false, MenuOrder = 4, Parent = YonetimPaneli };

                context.Set<MenuItem>().AddRange(new List<MenuItem>
                {
                    YonetimPaneli,
                    RolYonetimiMenu,
                    KulalniciGrupYonetimiMenu,
                    KulalniciYonetimiMenu,
                    menuYonetimi

                });

            



                await context.SaveChangesAsync();
            }

            if (!context.Set<MenuItemRol>().Any())
            {


                var menuRolYonetimi = await context.Set<MenuItem>().FirstOrDefaultAsync(e => e.Label.Equals("Rol Yönetimi"));
                var menuKullaniciGrupYonetimi = await context.Set<MenuItem>().FirstOrDefaultAsync(e => e.Label.Equals("Kullanici Grup Yönetimi"));
                var menuKullaniciYonetimi = await context.Set<MenuItem>().FirstOrDefaultAsync(e => e.Label.Equals("Kullanici Yönetimi"));
                var menuMenuYonetimi = await context.Set<MenuItem>().FirstOrDefaultAsync(e => e.Label.Equals("Menü Yönetimi"));

                var rolRolYonetimi = await context.Set<Rol>().FirstOrDefaultAsync(e => e.Domain.Equals("RolYonetimi") && e.Ad.Equals("View"));
                var rolKullaniciGrupYonetimi = await context.Set<Rol>().FirstOrDefaultAsync(e => e.Domain.Equals("KullaniciGrupYonetimi") && e.Ad.Equals("View"));
                var rolKullaniciYonetimi = await context.Set<Rol>().FirstOrDefaultAsync(e => e.Domain.Equals("KullaniciYonetimi") && e.Ad.Equals("View"));
                var rolMenuYonetimi = await context.Set<Rol>().FirstOrDefaultAsync(e => e.Domain.Equals("MenuYonetimi") && e.Ad.Equals("View"));

                context.Set<MenuItemRol>().AddRange(new List<MenuItemRol>
                {
                    new MenuItemRol{Rol=rolRolYonetimi,MenuItem=menuRolYonetimi},
                    new MenuItemRol{Rol=rolKullaniciGrupYonetimi,MenuItem=menuKullaniciGrupYonetimi},
                    new MenuItemRol{Rol=rolKullaniciYonetimi,MenuItem=menuKullaniciYonetimi},
                    new MenuItemRol{Rol=rolMenuYonetimi,MenuItem=menuMenuYonetimi},

                });



                await context.SaveChangesAsync();
            }

         
        }
    }

}
