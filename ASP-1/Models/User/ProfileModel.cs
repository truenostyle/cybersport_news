using ASP_1.Data.Entity;

namespace ASP_1.Models.User
{
    public class ProfileModel
    {
        public Guid Id { get; set; }
        public String Login { get; set; } = null!;
       
        public String Email { get; set; } = null!;
        public bool IsEmailPublic { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public String RealName { get; set; } = null!;
        public bool IsRealNamePublic { get; set; }
        public String Avatar { get; set; } = null!;
        public DateTime RegisterDt { get; set; }
        public bool IsDtPublic { get; set; }


        public bool IsPersonal { get; set; }

        public bool IsModerator { get; set; }
        public String Description { get; set; } = null!;

        
        public ProfileModel(Data.Entity.User user)
        {
            var thisProps = this.GetType().GetProperties();
            foreach ( var prop in user.GetType().GetProperties())
            {
                var thisProp = thisProps.FirstOrDefault(p => p.Name == prop.Name && p.PropertyType.IsAssignableFrom(prop.PropertyType));
                thisProp?.SetValue(this, prop.GetValue(user));
               
                
            }
            this.IsEmailConfirmed = user.EmailCode is null;
        }
    }
}
