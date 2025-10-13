using AIInstructor.src.MenuItemler.Entity;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.MenuItemRoller.Entity
{
    public class MenuItemRol:BaseEntity
    {
        public MenuItem MenuItem { get; set; }
        public Rol Rol { get; set; }
    }
}
