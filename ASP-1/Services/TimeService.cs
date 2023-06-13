namespace ASP_1.Services
{
    public class TimeService
    {
        public DateTime GetMoment()
        {
            return DateTime.Now.ToLocalTime();
        }
    }
}
