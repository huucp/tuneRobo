using user;

namespace TuneRoboWPF.Utility
{
    public class UserProfile
    {
        public ulong UserID { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string AvatarURL { get; set; }

        public UserProfile(ProfileReply profile)
        {
            UserID = profile.user_id;
            DisplayName = profile.display_name;
            Email = profile.email;
            AvatarURL = profile.avatar_url;
        }
        public UserProfile(SigninReply profile)
        {
            UserID = profile.user_id;
            DisplayName = profile.display_name;
            Email = profile.email;
            AvatarURL = profile.avatar_url;
        }        
    }
}
