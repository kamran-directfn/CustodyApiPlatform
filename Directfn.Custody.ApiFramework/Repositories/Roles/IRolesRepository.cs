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
        Task<List<UserRoleEntitlements>> GetEntitlements(CancellationToken cancellationToken);
        List<RolesEntitlements> GenerateEntitlments(List<UserRoleEntitlements> entitlments);
        List<Group> MapEntitlements(List<UserRoleEntitlements> lstEntitlments);

        Task<string> DeleteRoles(int um03_id, int user_id, CancellationToken cancellationToken);

        Task<RoleViewModel> AddRoles(RoleViewModel role, List<string> entilments, CancellationToken cancellationToken);
        Task<RoleViewModel> UpdateRole(RoleViewModel role, List<string> entilments, CancellationToken cancellationToken);
    }
}
