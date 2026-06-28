using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.DTOs;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Roles
{
    public interface IRolesRepository
    {
        Task<List<RoleViewModel>> GetAllRoles(CancellationToken cancellationToken);
        Task<RoleViewModel> GetRoleById(int roleId, CancellationToken cancellationToken);
        Task<List<Group>> GetEntitlmentsOfRole(int roleId, CancellationToken cancellationToken);
        Task<List<RoleViewModel>> UpdatePostStatus(int um03_id, int isPosted, int user_id, CancellationToken cancellationToken);
    }
}
