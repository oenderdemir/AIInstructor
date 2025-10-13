using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.MenuItemler.Entity
{
    public class MenuItem:BaseEntity
    {
        public string? Label { get; set; }
        public string? Icon { get; set; }
        public string? RouterLink { get; set; }
        public string? QueryParams { get; set; }
        public MenuItem? Parent { get; set; }
        public int MenuOrder { get; set; } = 0;

        public ICollection<MenuItemRol> MenuItemRoller { get; set; } = new List<MenuItemRol>();
    }
}
