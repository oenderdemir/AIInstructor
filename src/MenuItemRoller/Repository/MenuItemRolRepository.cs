using AutoMapper;
using AIInstructor.src.Context;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.MenuItemRoller.Repository
{
    public class MenuItemRolRepository:BaseRepository<MenuItemRol>, IMenuItemRolRepository
    {
        public MenuItemRolRepository(VTSDbContext context, IMapper mapper):base(context, mapper) 
        {
            
        }  
    }
}
