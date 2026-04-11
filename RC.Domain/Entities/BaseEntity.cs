using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Entities
{
    public abstract class BaseEntity
    {
        public long Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }
}
