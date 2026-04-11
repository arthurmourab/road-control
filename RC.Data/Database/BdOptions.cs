using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Database
{
    public class BdOptions
    {
        public string? Username {  get; set; }
        public string? Password { get; set; }
        public string? Server { get; set; }
        public string? Database {  get; set; }
        public string? ConnectionString { get; set; }
    }
}
