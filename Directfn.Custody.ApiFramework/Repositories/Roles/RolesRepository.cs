using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.DTOs;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Roles
{
    public sealed class RolesRepository : IRolesRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public RolesRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<RoleViewModel>> GetAllRoles(CancellationToken cancellationToken)
        {
           
                List<RoleViewModel> lstRoles = new List<RoleViewModel>();
                OracleParameter Pview = new OracleParameter();
                Pview.ParameterName = "Pview";
                Pview.OracleDbType = OracleDbType.RefCursor;// System.Data.DbType.Int64;
                Pview.Direction = System.Data.ParameterDirection.Output;
                List<OracleParameter> lstParams = new List<OracleParameter>();
                lstParams.Add(Pview);
            lstRoles =  await _dbManager.GetStoredProcedureRefCursorAsync<RoleViewModel>("Pkg_UM03_ROLES.Get_Data", lstParams, "pview", cancellationToken);

            
                lstRoles = lstRoles.Select(x =>
                {
                    x.isPosted = false;
                    return x;
                }).ToList();
               
            
            return lstRoles;
        }

        public async Task<List<Group>> GetEntitlmentsOfRole(int roleId, CancellationToken cancellationToken)
        {
            List<int> Entitlment_Ids = new List<int>();
            //GenericResponse response = new GenericResponse();
            List<UserRoleEntitlements> lstEntitlments = new List<UserRoleEntitlements>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstEntitlments = await GetEntitlements(cancellationToken);

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;// System.Data.DbType.Int64;
            Pview.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(Pview);

            OracleParameter p_role_id = new OracleParameter();
            p_role_id.ParameterName = "p_role_id";
            p_role_id.Direction = System.Data.ParameterDirection.Input;
            p_role_id.Value = roleId;
            p_role_id.OracleDbType = OracleDbType.Double;
            lstParams.Add(p_role_id);

            var dt = await _dbManager.GetStoredProcedureResultAsync("Get_EntitlmentIds_Of_Role", lstParams, cancellationToken);

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ///////// check if this entitlment exist in the list of entitlments and make it isChecke true
                    int entitlmentId = 0;
                    entitlmentId = Convert.ToInt32(dt.Rows[i]["entitlment_Id"]);
                    var isExist = lstEntitlments.Where(x => x.Entitlement_Id == entitlmentId).FirstOrDefault();
                    if (isExist != null)
                    {
                        isExist.isChecked = true;
                    }
                }
            }

            List<RolesEntitlements> entitlments = new List<RolesEntitlements>();

            //lstEntitlments = (List<UserRoleEntitlements>)ent_data;
            entitlments = GenerateEntitlments(lstEntitlments);

            entitlments = entitlments.Select(c =>
            {
                if (c.items != null && c.items.Count > 0)
                {
                    c.items = c.items.Select(x => { x.expanded = true; return x; }).ToList();
                }
                c.expanded = true;
                return c;
            }).ToList();
            List<Group> en = new List<Group>();
            en = lstEntitlments.GroupBy(c => c.group_name).Select(m => new Group()
            {
                Name = m.Key,
                JsSelector = m.Key.Replace(" ", "_").Replace("-", "_").Replace("/", "_"),
                screens = m.GroupBy(x => x.screen_name).Select(v => new Screen()
                {
                    Name = v.Key,
                    JsSelector = v.Key.Replace(" ", "_").Replace("-", "_").Replace("/", "_"),
                    screenEntitlements = v.Select(x => new ScreenEntitlement()
                    {
                        Text = x.entitlement,
                        Id = x.Entitlement_Id,
                        IsSelected = x.isChecked
                    }).ToList()
                }).ToList()
            }).ToList();

            return en;
        }

        public async Task<List<UserRoleEntitlements>> GetEntitlements(CancellationToken cancellationToken)
        {
            List<UserRoleEntitlements> lstEntitlements = new List<UserRoleEntitlements>();
            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;// System.Data.DbType.Int64;
            Pview.Direction = System.Data.ParameterDirection.Output;
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(Pview);
            lstEntitlements = await _dbManager.GetStoredProcedureRefCursorAsync<UserRoleEntitlements>("get_entitlements", lstParams, "pview", cancellationToken);
            //new DBManager().GetStoredProcedureResult<UserEntitlements>("get_entitlements", lstParams).ToList();
            return lstEntitlements;

        }
        public async Task<RoleViewModel> GetRoleById(int roleId, CancellationToken cancellationToken)
        {
            List<RoleViewModel> roleData = new List<RoleViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();
            
            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;// System.Data.DbType.Int64;
            Pview.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(Pview);
            
            OracleParameter P_UM03_ID = new OracleParameter();
            P_UM03_ID.ParameterName = "P_UM03_ID";
            P_UM03_ID.Direction = System.Data.ParameterDirection.Input;
            P_UM03_ID.Value = roleId;
            lstParams.Add(P_UM03_ID);

            roleData = await _dbManager.GetStoredProcedureRefCursorAsync<RoleViewModel>("Pkg_UM03_ROLES.Get_ID_Data", lstParams, "pview", cancellationToken);


            //List<UserRoleEntitlements> ent_data = await GetEntitlmentsOfRole(roleId, cancellationToken);

            //List<UserRoleEntitlements> lstEntitlments = new List<UserRoleEntitlements>();
            //List<RolesEntitlements> entitlments = new List<RolesEntitlements>();

            //lstEntitlments = (List<UserRoleEntitlements>)ent_data;
            //entitlments = GenerateEntitlments(lstEntitlments);

            


            return roleData.FirstOrDefault();
        }

        public async Task<List<RoleViewModel>> UpdatePostStatus(int um03_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<RoleViewModel> lst = new List<RoleViewModel>();

            List<OracleParameter> parameters = new List<OracleParameter>();

            OracleParameter PUM02_ID = new OracleParameter();
            PUM02_ID.ParameterName = "PUM02_ID";
            PUM02_ID.Value = um03_id;
            PUM02_ID.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_ID);

            OracleParameter PUM02_IS_POSTED = new OracleParameter();
            PUM02_IS_POSTED.ParameterName = "PUM02_IS_POSTED";
            PUM02_IS_POSTED.Value = isPosted;
            PUM02_IS_POSTED.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_IS_POSTED);

            OracleParameter p_Modified_By = new OracleParameter();
            p_Modified_By.ParameterName = "p_Modified_By";
            p_Modified_By.Value = user_id;
            p_Modified_By.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(p_Modified_By);

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_UM03_ROLES.Update_Role_Post_Status", parameters);

            lst = await GetAllRoles(cancellationToken);


            return lst;
        }

        public List<RolesEntitlements> GenerateEntitlments(List<UserRoleEntitlements> entitlments)
        {
            List<RolesEntitlements> lstEntitlements = new List<RolesEntitlements>();
            List<string> screens = entitlments.Select(x => x.screen_name).Distinct().ToList();
            foreach (var item in screens)
            {
                RolesEntitlements entitlementParent = new RolesEntitlements();
                entitlementParent.id = null;
                entitlementParent.label = item;
                entitlementParent.isChecked = false;
                var childEntitlements = entitlments.Where(x => x.screen_name == item).ToList();
                ///// get if tab structure exist in this screen
                var get_Sections_Of_Screen = entitlments.Where(x => x.screen_name == item && x.parent_controller == item).Select(x => x.section).Distinct().ToList();
                if (get_Sections_Of_Screen != null && get_Sections_Of_Screen.Count > 0)
                {

                    entitlementParent.items = new List<RolesEntitlements>();


                    foreach (var nested_Section_Names in get_Sections_Of_Screen)
                    {
                        RolesEntitlements entitlement_Section = new RolesEntitlements();
                        entitlement_Section.label = nested_Section_Names;
                        entitlement_Section.isChecked = false;
                        entitlement_Section.id = null;
                        entitlement_Section.items = new List<RolesEntitlements>();
                        var section_Entitlments = entitlments.Where(x => x.section == nested_Section_Names).ToList();
                        foreach (var section_Entitlment in section_Entitlments)
                        {
                            RolesEntitlements childEntitlement_Section = new RolesEntitlements();
                            childEntitlement_Section.label = section_Entitlment.entitlement;
                            childEntitlement_Section.id = Convert.ToString(section_Entitlment.Entitlement_Id);
                            if (section_Entitlment.isChecked)
                            {
                                childEntitlement_Section.isChecked = true;

                            }
                            else
                            {
                                childEntitlement_Section.isChecked = false;
                            }
                            entitlement_Section.items.Add(childEntitlement_Section);
                        }
                        entitlementParent.items.Add(entitlement_Section);
                    }
                    lstEntitlements.Add(entitlementParent);
                }

                else
                {
                    entitlementParent.items = new List<RolesEntitlements>();
                    foreach (var itemChild in childEntitlements)
                    {

                        RolesEntitlements childEntitlement = new RolesEntitlements();
                        childEntitlement.id = Convert.ToString(itemChild.Entitlement_Id);
                        childEntitlement.label = itemChild.entitlement;
                        if (itemChild.isChecked == true)
                        {
                            childEntitlement.isChecked = true;
                        }
                        else
                        {
                            childEntitlement.isChecked = false;
                        }
                        entitlementParent.items.Add(childEntitlement);
                    }

                    lstEntitlements.Add(entitlementParent);
                }
            }
            return lstEntitlements;

        }
    }
}
