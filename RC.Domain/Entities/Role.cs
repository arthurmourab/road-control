using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public static class Roles
        {
            public const string SystemAdmin = "SystemAdmin";
            public const string OrganizationAdmin = "OrganizationAdmin";
            public const string Driver = "Driver";
            public const string GasStationAttendant = "GasStationAttendant";

            // Perfis autorizados a registrar abastecimentos
            public const string FuelingManagers = $"{Driver},{OrganizationAdmin},{SystemAdmin}";

            // Perfis autorizados a cadastrar usuários
            public const string UserManagers = $"{OrganizationAdmin},{SystemAdmin}";
        }
    }
}
