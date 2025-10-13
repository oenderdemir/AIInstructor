using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.MenuItemler.DTO;
using AIInstructor.src.MenuItemler.Entity;
using AIInstructor.src.MenuItemler.Repository;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.MenuItemRoller.Repository;
using AIInstructor.src.Shared.RDBMS.Service;
using AIInstructor.src.Shared.Repository;

namespace AIInstructor.src.MenuItemler.Service
{
    public class MenuItemService:BaseService<MenuItemDto,MenuItem>,IMenuItemService
    {
        private readonly IMenuItemRepository menuItemRepository;
        private readonly IRolRepository rolRepository;
        private readonly IMenuItemRolRepository menuItemRolRepository;
        private readonly IMapper mapper;
        public MenuItemService(IMenuItemRepository menuItemRepository
            ,IRolRepository rolRepository
            , IMenuItemRolRepository menuItemRolRepository
            , IMapper mapper):base(menuItemRepository,mapper) 
        {
            this.menuItemRepository= menuItemRepository;    
            this.rolRepository= rolRepository;
            this.menuItemRolRepository= menuItemRolRepository;
            this.mapper=mapper; 

        }



        public async Task<IEnumerable<MenuItemDto>> MenuAgaciGetir()
        {
            List<MenuItemDto> result = new List<MenuItemDto>();
            var menuItemler = await this.menuItemRepository.GetAllAsync(
                        q=>q.Include(e=>e.MenuItemRoller.Where(e=>!e.IsDeleted)).ThenInclude(e=>e.Rol));

            // parentIds null olanlari al

            var kokler = menuItemler.Where(e => e.Parent == null).OrderBy(e=>e.MenuOrder).ToList();
            foreach (var kok in kokler)
            {
                var kokDto=mapper.Map<MenuItem, MenuItemDto>(kok);
                var yapraklar = menuItemler.Where(e => e.Parent == kok).OrderBy(e => e.MenuOrder).ToList();
                if(yapraklar.Count>0)
                {
                    kokDto.Items = new List<MenuItemDto>();
                }
                foreach(var yaprak in yapraklar)
                {
                    kokDto.Items.Add(mapper.Map<MenuItem,MenuItemDto>(yaprak));
                }

                result.Add(kokDto);
                
            }
            return result;
        }

        public override async Task<MenuItemDto> AddAsync(MenuItemDto dto)
        {
            var menuItem = _mapper.Map<MenuItem>(dto);

            if(dto.ParentId!=null)
            {
                menuItem.Parent = await this.menuItemRepository.GetByIdAsync((Guid)dto.ParentId);
            }
            // Gelen Rolleri işle
            menuItem.MenuItemRoller = new List<MenuItemRol>();

            foreach (var roleDto in dto.Roles)
            {
                var rol = await this.rolRepository.GetByIdAsync(roleDto.Id.Value);
                menuItem.MenuItemRoller.Add(new MenuItemRol
                {
                    MenuItem = menuItem,
                    Rol = rol
                });
            }

            await _repository.AddAsync(menuItem);
            await _repository.SaveChangesAsync();

            return _mapper.Map<MenuItemDto>(menuItem);
        }

        public override async Task<MenuItemDto> UpdateAsync(MenuItemDto dto)
        {
            var menuItem = await _repository.GetByIdAsync(dto.Id.Value, q => q.Include(x => x.MenuItemRoller));

            if (menuItem == null)
                throw new Exception("MenuItem bulunamadı!");

            // 1. Alanları güncelle
            menuItem.Label = dto.Label;
            menuItem.Icon = dto.Icon;
            menuItem.RouterLink = dto.RouterLink;
            menuItem.QueryParams = dto.QueryParams;
            menuItem.MenuOrder = dto.MenuOrder;
            if (dto.ParentId != null)
            {
                menuItem.Parent = await this.menuItemRepository.GetByIdAsync((Guid)dto.ParentId);
            }

            // 2. Mevcut MenuItemRoller temizle
            if (menuItem.MenuItemRoller != null && menuItem.MenuItemRoller.Any())
            {
                this.menuItemRepository.RemoveMenuItemRollerRangeAsync(menuItem.MenuItemRoller);
               
               
            }

            // 3. Yeni roller ekle
            if (dto.Roles != null && dto.Roles.Any())
            {
                menuItem.MenuItemRoller = new List<MenuItemRol>();
                var roleIds=dto.Roles.Select(e=>e.Id).Distinct().ToList(); 
                foreach (var roleId in roleIds)
                {
                    var rol = await this.rolRepository.GetByIdAsync(roleId.Value);
                    menuItem.MenuItemRoller.Add(new MenuItemRol
                    {
                        MenuItem = menuItem,
                        Rol = rol
                    });
                }
            }

            _repository.Update(menuItem);
            await _repository.SaveChangesAsync();
            return dto;
        }




    }
}
