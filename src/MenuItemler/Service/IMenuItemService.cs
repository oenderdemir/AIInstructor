using AIInstructor.src.MenuItemler.DTO;
using AIInstructor.src.MenuItemler.Entity;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Service;

namespace AIInstructor.src.MenuItemler.Service
{
    public interface IMenuItemService : IBaseService<MenuItemDto, MenuItem>
    {
        Task<IEnumerable<MenuItemDto>> MenuAgaciGetir();
        

    }
}
