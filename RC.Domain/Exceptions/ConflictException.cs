using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Exceptions
{
    public class ConflictException(string message) : Exception(message)
    {
    }
}
