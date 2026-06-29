using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
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
            lstRoles = await _dbManager.GetStoredProcedureRefCursorAsync<RoleViewModel>("Pkg_UM03_ROLES.Get_Data", lstParams, "pview", cancellationToken);


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

            //List<RolesEntitlements> entitlments = new List<RolesEntitlements>();

            //entitlments = GenerateEntitlments(lstEntitlments);

            //entitlments = entitlments.Select(c =>
            //{
            //    if (c.items != null && c.items.Count > 0)
            //    {
            //        c.items = c.items.Select(x => { x.expanded = true; return x; }).ToList();
            //    }
            //    c.expanded = true;
            //    return c;
            //}).ToList();

            List<Group> en = new List<Group>();

            en = MapEntitlements(lstEntitlments);

            return en;
        }

        public List<Group> MapEntitlements(List<UserRoleEntitlements> lstEntitlments)
        {
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

        public async Task<string> DeleteRoles(int um03_id, int user_id, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            string outParam = "";
            List<OracleParameter> parameters = new List<OracleParameter>();

            OracleParameter PKey = new OracleParameter();
            PKey.ParameterName = "PKey";
            PKey.Size = 32767;
            PKey.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(PKey);

            OracleParameter PUM03_ID = new OracleParameter();
            PUM03_ID.ParameterName = "PUM03_ID";
            PUM03_ID.Value = um03_id;
            PUM03_ID.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM03_ID);

            OracleParameter PUM03_Edited_by = new OracleParameter();
            PUM03_Edited_by.ParameterName = "PUM03_Edited_by";
            PUM03_Edited_by.Value = user_id;
            PUM03_Edited_by.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM03_Edited_by);

            OracleParameter PError = new OracleParameter();
            PError.ParameterName = "PError";
            PError.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(PError);

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM03_ROLES.Delete_Data", parameters);

            message = result.GetString("PError");

            return message;
        }

        public async Task<RoleViewModel> AddRoles(RoleViewModel role, List<string> entilments, CancellationToken cancellationToken)
        {
            role.Entitlments = new List<UserRoleEntitlements>();
            List<string> lstEntitlements = entilments;

            foreach (var item in lstEntitlements)
            {
                role.Entitlments.Add(new UserRoleEntitlements() { Entitlement_Id = Convert.ToInt32(item) });
            }
            role.UM03_STATUS = 0;
            // role.UM03_CREATED_BY = User.Id;


            List<OracleParameter> lstParams = new List<OracleParameter>();

            OracleParameter PKey = new OracleParameter();
            PKey.ParameterName = "PKey";
            PKey.Size = 32767;
            PKey.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(PKey);

            OracleParameter PUM03_PARENT_ID = new OracleParameter();
            PUM03_PARENT_ID.ParameterName = "PUM03_PARENT_ID";
            PUM03_PARENT_ID.Direction = System.Data.ParameterDirection.Input;
            PUM03_PARENT_ID.Value = role.UM03_PARENT_ID;
            lstParams.Add(PUM03_PARENT_ID);

            OracleParameter PUM03_NAME = new OracleParameter();
            PUM03_NAME.ParameterName = "PUM03_NAME";
            PUM03_NAME.Direction = System.Data.ParameterDirection.Input;
            PUM03_NAME.Value = role.UM03_NAME;
            lstParams.Add(PUM03_NAME);

            OracleParameter PUM03_NAME_SEC = new OracleParameter();
            PUM03_NAME_SEC.ParameterName = "PUM03_NAME_SEC";
            PUM03_NAME_SEC.Direction = System.Data.ParameterDirection.Input;
            PUM03_NAME_SEC.Value = role.UM03_NAME_SEC;
            lstParams.Add(PUM03_NAME_SEC);

            OracleParameter PUM03_STATUS = new OracleParameter();
            PUM03_STATUS.ParameterName = "PUM03_STATUS";
            PUM03_STATUS.Direction = System.Data.ParameterDirection.Input;
            PUM03_STATUS.Value = role.UM03_STATUS;
            lstParams.Add(PUM03_STATUS);

            OracleParameter PUM03_IP = new OracleParameter();
            PUM03_IP.ParameterName = "PUM03_IP";
            PUM03_IP.Direction = System.Data.ParameterDirection.Input;
            PUM03_IP.Value = role.UM03_IP;
            lstParams.Add(PUM03_IP);

            OracleParameter PUM03_Created_by = new OracleParameter();
            PUM03_Created_by.ParameterName = "PUM03_Created_by";
            PUM03_Created_by.Direction = System.Data.ParameterDirection.Input;
            PUM03_Created_by.Value = role.UM03_CREATED_BY;
            lstParams.Add(PUM03_Created_by);

            OracleParameter PUM03_Edited_by = new OracleParameter();
            PUM03_Edited_by.ParameterName = "PUM03_Edited_by";
            PUM03_Edited_by.Direction = System.Data.ParameterDirection.Input;
            PUM03_Edited_by.Value = null;
            lstParams.Add(PUM03_Edited_by);

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM03_ROLES.Add_Data", lstParams);

            role.UM03_ID = Convert.ToInt32(result.GetString("PKey"));

            if (role.Entitlments != null && role.Entitlments.Count > 0 && role.UM03_ID > 0)
            {
                foreach (var item in role.Entitlments)
                {
                    List<OracleParameter> entitlmentsParams = new List<OracleParameter>();


                    //OracleParameter PKey = new OracleParameter();
                    //PKey.ParameterName = "PKey";
                    //PKey.Size = 32767;
                    //PKey.Direction = System.Data.ParameterDirection.Output;
                    entitlmentsParams.Add(PKey);


                    OracleParameter PUM08_UM03_ID = new OracleParameter();
                    PUM08_UM03_ID.ParameterName = "PUM08_UM03_ID";
                    PUM08_UM03_ID.Direction = System.Data.ParameterDirection.Input;
                    PUM08_UM03_ID.Value = role.UM03_ID;
                    entitlmentsParams.Add(PUM08_UM03_ID);

                    OracleParameter PUM08_UM07_ID = new OracleParameter();
                    PUM08_UM07_ID.ParameterName = "PUM08_UM07_ID";
                    PUM08_UM07_ID.Direction = System.Data.ParameterDirection.Input;
                    PUM08_UM07_ID.Value = item.Entitlement_Id;
                    entitlmentsParams.Add(PUM08_UM07_ID);

                    OracleParameter PUM08_STATUS = new OracleParameter();
                    PUM08_STATUS.ParameterName = "PUM08_STATUS";
                    PUM08_STATUS.Direction = System.Data.ParameterDirection.Input;
                    PUM08_STATUS.Value = 1;
                    entitlmentsParams.Add(PUM08_STATUS);

                    OracleParameter PUM08_IP = new OracleParameter();
                    PUM08_IP.ParameterName = "PUM08_IP";
                    PUM08_IP.Direction = System.Data.ParameterDirection.Input;
                    PUM08_IP.Value = role.UM03_IP;
                    entitlmentsParams.Add(PUM08_IP);

                    OracleParameter PUM08_Edited_by = new OracleParameter();
                    PUM08_Edited_by.ParameterName = "PUM08_Edited_by";
                    PUM08_Edited_by.Direction = System.Data.ParameterDirection.Input;
                    PUM08_Edited_by.Value = role.UM03_MODIFIED_BY;
                    entitlmentsParams.Add(PUM08_Edited_by);

                    StoredProcedureResult result2 = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM08_ROLES_ENTITLMENTS.Add_Data", entitlmentsParams);
                }
            }


            return role;
        }

        public async Task<RoleViewModel> UpdateRole(RoleViewModel role, List<string> entilments, CancellationToken cancellationToken)
        {
            role.Entitlments = new List<UserRoleEntitlements>();
            List<string> lstEntitlements = entilments;

            foreach (var item in lstEntitlements)
            {
                role.Entitlments.Add(new UserRoleEntitlements() { Entitlement_Id = Convert.ToInt32(item) });
            }
            role.UM03_STATUS = 0;
            // role.UM03_CREATED_BY = User.Id;


            List<OracleParameter> lstParams = new List<OracleParameter>();

            OracleParameter PUM03_ID = new OracleParameter();
            PUM03_ID.ParameterName = "PUM03_ID";
            PUM03_ID.Direction = System.Data.ParameterDirection.Input;
            PUM03_ID.Value = role.UM03_PARENT_ID;
            lstParams.Add(PUM03_ID);

            OracleParameter PUM03_PARENT_ID = new OracleParameter();
            PUM03_PARENT_ID.ParameterName = "PUM03_PARENT_ID";
            PUM03_PARENT_ID.Direction = System.Data.ParameterDirection.Input;
            PUM03_PARENT_ID.Value = role.UM03_PARENT_ID;
            lstParams.Add(PUM03_PARENT_ID);

            OracleParameter PUM03_NAME = new OracleParameter();
            PUM03_NAME.ParameterName = "PUM03_NAME";
            PUM03_NAME.Direction = System.Data.ParameterDirection.Input;
            PUM03_NAME.Value = role.UM03_NAME;
            lstParams.Add(PUM03_NAME);

            OracleParameter PUM03_NAME_SEC = new OracleParameter();
            PUM03_NAME_SEC.ParameterName = "PUM03_NAME_SEC";
            PUM03_NAME_SEC.Direction = System.Data.ParameterDirection.Input;
            PUM03_NAME_SEC.Value = role.UM03_NAME_SEC;
            lstParams.Add(PUM03_NAME_SEC);

            OracleParameter PUM03_STATUS = new OracleParameter();
            PUM03_STATUS.ParameterName = "PUM03_STATUS";
            PUM03_STATUS.Direction = System.Data.ParameterDirection.Input;
            PUM03_STATUS.Value = role.UM03_STATUS;
            lstParams.Add(PUM03_STATUS);

            OracleParameter PUM03_IP = new OracleParameter();
            PUM03_IP.ParameterName = "PUM03_IP";
            PUM03_IP.Direction = System.Data.ParameterDirection.Input;
            PUM03_IP.Value = role.UM03_IP;
            lstParams.Add(PUM03_IP);

            OracleParameter PUM03_Edited_by = new OracleParameter();
            PUM03_Edited_by.ParameterName = "PUM03_Edited_by";
            PUM03_Edited_by.Direction = System.Data.ParameterDirection.Input;
            PUM03_Edited_by.Value = role.UM03_MODIFIED_BY; 
            lstParams.Add(PUM03_Edited_by);

            OracleParameter PError = new OracleParameter();
            PError.ParameterName = "PError";
            PError.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(PError);

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM03_ROLES.Add_Data", lstParams);


            if (role.Entitlments != null && role.Entitlments.Count > 0 && role.UM03_ID > 0)
            {

                List<OracleParameter> deleteEntitlmentsParams = new List<OracleParameter>();

                OracleParameter PUM08_UM03_ID = new OracleParameter();
                PUM08_UM03_ID.ParameterName = "PUM08_UM03_ID";
                PUM08_UM03_ID.Direction = System.Data.ParameterDirection.Input;
                PUM08_UM03_ID.Value = role.UM03_ID;
                deleteEntitlmentsParams.Add(PUM08_UM03_ID);

                OracleParameter PUM08_Edited_by = new OracleParameter();
                PUM08_Edited_by.ParameterName = "PUM08_Edited_by";
                PUM08_Edited_by.Direction = System.Data.ParameterDirection.Input;
                PUM08_Edited_by.Value = role.UM03_MODIFIED_BY;
                deleteEntitlmentsParams.Add(PUM08_Edited_by);

                await _dbManager.ExecuteStoredProcedureAsync("Pkg_UM08_ROLES_ENTITLMENTS.Delete_Data", deleteEntitlmentsParams);

                foreach (var item in role.Entitlments)
                {
                    List<OracleParameter> entitlmentsParams = new List<OracleParameter>();


                    OracleParameter PKey = new OracleParameter();
                    PKey.ParameterName = "PKey";
                    PKey.Size = 32767;
                    PKey.Direction = System.Data.ParameterDirection.Output;
                    entitlmentsParams.Add(PKey);


                    //OracleParameter PUM08_UM03_ID = new OracleParameter();
                    //PUM08_UM03_ID.ParameterName = "PUM08_UM03_ID";
                    //PUM08_UM03_ID.Direction = System.Data.ParameterDirection.Input;
                    //PUM08_UM03_ID.Value = role.UM03_ID;
                    entitlmentsParams.Add(PUM08_UM03_ID);

                    OracleParameter PUM08_UM07_ID = new OracleParameter();
                    PUM08_UM07_ID.ParameterName = "PUM08_UM07_ID";
                    PUM08_UM07_ID.Direction = System.Data.ParameterDirection.Input;
                    PUM08_UM07_ID.Value = item.Entitlement_Id;
                    entitlmentsParams.Add(PUM08_UM07_ID);

                    OracleParameter PUM08_STATUS = new OracleParameter();
                    PUM08_STATUS.ParameterName = "PUM08_STATUS";
                    PUM08_STATUS.Direction = System.Data.ParameterDirection.Input;
                    PUM08_STATUS.Value = 1;
                    entitlmentsParams.Add(PUM08_STATUS);

                    OracleParameter PUM08_IP = new OracleParameter();
                    PUM08_IP.ParameterName = "PUM08_IP";
                    PUM08_IP.Direction = System.Data.ParameterDirection.Input;
                    PUM08_IP.Value = role.UM03_IP;
                    entitlmentsParams.Add(PUM08_IP);

                    //OracleParameter PUM08_Edited_by = new OracleParameter();
                    //PUM08_Edited_by.ParameterName = "PUM08_Edited_by";
                    //PUM08_Edited_by.Direction = System.Data.ParameterDirection.Input;
                    //PUM08_Edited_by.Value = role.UM03_MODIFIED_BY;
                    entitlmentsParams.Add(PUM08_Edited_by);

                    StoredProcedureResult result2 = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM08_ROLES_ENTITLMENTS.Add_Data", lstParams);
                }
            }


            return role;
        }
    }
}
