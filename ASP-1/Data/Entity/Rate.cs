﻿namespace ASP_1.Data.Entity
{
    public class Rate
    {
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } = 0;
    }
}
