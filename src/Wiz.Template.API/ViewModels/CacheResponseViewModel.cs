using System;

namespace Wiz.Template.API.ViewModels
{
    public class CacheResponseViewModel
    {
        public TimeSpan? Expiration { get; set; } 
        public object Data { get; set; }
    }
}
