using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.MenuItemler.DTO
{
    public class MenuItemDto: BaseRDBMSDto
    {

        public string? Label { get; set; }
        public string? Icon { get; set; }
        public string? RouterLink { get; set; }
        public string? QueryParams { get; set; }
        public Guid? ParentId { get; set; }
        public int MenuOrder { get; set; } = 0;

        public List<RolDto>? Roles { get; set; } = new();

        public List<MenuItemDto>? Items { get; set; } 


    }
}
