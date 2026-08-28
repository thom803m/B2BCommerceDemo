using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Services.Helpers
{
    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Today => DateTime.UtcNow.Date;
    }
}
