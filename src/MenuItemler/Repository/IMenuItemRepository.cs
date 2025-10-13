using AIInstructor.src.MenuItemler.Entity;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.MenuItemler.Repository
{
    public interface IMenuItemRepository : IBaseRepository<MenuItem>
    {
        void RemoveMenuItemRollerRangeAsync(IEnumerable<MenuItemRol> menuItemRoller);
    }


}
