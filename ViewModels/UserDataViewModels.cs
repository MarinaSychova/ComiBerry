namespace ComiBerry.ViewModels
{
    public class BasicUserData
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public string? Bio { get; set; }
    }

    public class VIEWMyAccountViewModel
    {
        public required BasicUserData BasicUserDataPart { get; set; }

        public required string Email { get; set; }
    }

    public class VIEWViewAuthorViewModel
    {
        public required BasicUserData BasicUserDataPart { get; set; }

        public ICollection<Series>? Series { get; set; }
    }

    public class ADMINGetUsersViewModel
    {
        public List<User>? Users { get; set; }

        public string? Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public required string Role { get; set; }
    }

    public class ADMINViewProfileViewModel
    {
        public required VIEWMyAccountViewModel ExtendedUserDataPart { get; set; }

        public required string Role { get; set; }

        public ICollection<Series>? Series { get; set; }
    }
}
