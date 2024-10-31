using Models.Entity;

namespace WebMVC.ViewModels
{
    public class RolesViewModel
    {
        public RolesViewModel()
        {
            RoleList = [];
        }
        public ApplicationUser User { get; set; }
        public List<RoleSelection> RoleList { get; set; }

    }

    public class RoleSelection
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
