using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }

        // Organização à qual o usuário pertence (opcional — SystemAdmin não pertence a nenhuma)
        public long? OrganizationId { get; set; }
        public Organization Organization { get; set; }

        // Posto ao qual o usuário pertence (opcional — papéis de posto:
        // GasStationAdmin e GasStationAttendant). Um usuário nunca tem organização E posto.
        public long? GasStationId { get; set; }
        public GasStation? GasStation { get; set; }
    }
}
