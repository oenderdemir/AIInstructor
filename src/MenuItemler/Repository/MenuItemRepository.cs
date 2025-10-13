using AutoMapper;
using AIInstructor.src.Context;
using AIInstructor.src.MenuItemler.Entity;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.MenuItemler.Repository
{
    public class MenuItemRepository:BaseRepository<MenuItem>,IMenuItemRepository
    {
        private readonly VTSDbContext context;
        public MenuItemRepository(VTSDbContext context, IMapper mapper) :
            base(context, mapper)
        {
            this.context = context;
        }

        public void RemoveMenuItemRollerRangeAsync(IEnumerable<MenuItemRol> menuItemRoller)
        {
            this.context.MenuItemRoller.RemoveRange(menuItemRoller);
        }
    }
}
