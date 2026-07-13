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
            public const string GasStationAdmin = "GasStationAdmin";

            // Perfis autorizados a registrar abastecimentos
            public const string FuelingManagers = $"{Driver},{OrganizationAdmin},{SystemAdmin}";

            // Perfis autorizados a cadastrar/gerenciar usuários (cada um restrito
            // à própria alçada — matriz detalhada no UserService)
            public const string UserManagers = $"{OrganizationAdmin},{GasStationAdmin},{SystemAdmin}";

            // Perfis autorizados a gerenciar a frota (veículos) — GasStationAdmin
            // NÃO participa: postos não gerenciam veículos das organizações
            public const string FleetManagers = $"{OrganizationAdmin},{SystemAdmin}";

            // Perfis que consultam os postos disponíveis para a própria organização
            public const string GasStationViewers = $"{Driver},{OrganizationAdmin},{SystemAdmin}";

            // Perfis que consultam abastecimentos: os gestores + o frentista
            // (este último vê apenas os que confirmou com o próprio código)
            public const string FuelingViewers = $"{Driver},{OrganizationAdmin},{SystemAdmin},{GasStationAttendant}";
        }
    }
}
