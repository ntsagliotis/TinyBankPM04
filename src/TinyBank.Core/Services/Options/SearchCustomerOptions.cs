using System;
using System.Collections.Generic;

namespace TinyBank.Core.Services.Options
{
    public class SearchAccountOptions
    {
        public Guid? CustomerId { get; set; }
        public string AccountId { get; set; }
        public int? MaxResults { get; set; }
        public bool? TrackResults { get; set; }
        public int? Skip { get; set; }

        public SearchAccountOptions()
        {
        }
    }
}
