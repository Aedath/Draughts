namespace Draughts.Access.Models
{
    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
    }
}