using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Entities
{
    public class Organization : BaseEntity
    {
        public string Name {  get; set; }
        public string Document {  get; set; }
        public bool IsActive { get; set; }
    }
}
