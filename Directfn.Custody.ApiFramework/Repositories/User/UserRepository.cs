using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.DTOs.User;
using Oracle.ManagedDataAccess.Client;
using Serilog;
using System.Data;
using System.Drawing;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace Directfn.Custody.ApiFramework.Repositories.User
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IOracleDbManagerAsync _dbManagerAsync;
        private readonly IOracleDbManager _dbManagers;

        public UserRepository(IOracleDbManagerAsync dbManagerAsync, IOracleDbManager dbManagers)
        {
            _dbManagerAsync = dbManagerAsync;
            _dbManagers = dbManagers;
        }

        public async Task<LoginUserRecord?> GetUserForLoginAsync(string loginId, long rf48Code, CancellationToken cancellationToken)
        {
            List<OracleParameter> parameters = new() { new OracleParameter("pview", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }, new OracleParameter("P_login", OracleDbType.Varchar2) { Direction = ParameterDirection.Input, Value = loginId }, new OracleParameter("p_rf48_code", OracleDbType.Decimal) { Direction = ParameterDirection.Input, Value = rf48Code } };

            List<LoginUserRecord> users = await _dbManagerAsync.GetStoredProcedureRefCursorAsync<LoginUserRecord>("Pkg_UM02_USERS.User_Login", parameters, "pview", cancellationToken);

            return users.FirstOrDefault();
        }

        public async Task ChangeFirstLoginPasswordAsync(long userId, string encryptedPassword, CancellationToken cancellationToken)
        {
            var parameters = new List<OracleParameter>
            {
                new("p_um02_id", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Input,
                    Value = userId
                },
                new("p_password", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = encryptedPassword
                }
             };

            await _dbManagerAsync.ExecuteStoredProcedureAsync("Pkg_UM02_USERS.change_password_tokken", parameters, cancellationToken);
        }

        public async Task<List<UserViewModel>> GetAllUserAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.Direction = System.Data.ParameterDirection.Output;
            Pview.OracleDbType = OracleDbType.RefCursor;
            parameters.Add(Pview);


            var users =  await _dbManagerAsync.GetStoredProcedureRefCursorAsync<UserViewModel>("Pkg_UM02_USERS.Get_Data",parameters, "pview", cancellationToken);


            return users;
        }

        public async Task<UserViewModel> GetUserByIDAsync(int userId, CancellationToken cancellationToken)
        {
            List<UserViewModel> lstUsers = await GetAllUserAsync(cancellationToken);
            UserViewModel selectedUser = new UserViewModel();
            selectedUser = lstUsers.Where(x => x.UM02_ID == userId).FirstOrDefault();
            selectedUser.ConfirmPassword = selectedUser.UM02_PASSWORD;
            selectedUser.memeberCode = await GetMemberCodeByUserId(userId, cancellationToken);
            selectedUser.memberCodes = await GetMembers(userId, cancellationToken);

            return selectedUser;
        }

        public async Task<int[]> GetMemberCodeByUserId(int id, CancellationToken cancellationToken)
        {
            List<MemberCode> memberList = new List<MemberCode>();
            int[] memberArr = new int[memberList.Count];
            try
            {
                OracleParameter Pview = new OracleParameter();
                Pview.ParameterName = "Pview";
                Pview.OracleDbType = OracleDbType.RefCursor;// System.Data.DbType.Int64;
                Pview.Direction = System.Data.ParameterDirection.Output;
                List<OracleParameter> lstParams = new List<OracleParameter>();
                lstParams.Add(Pview);

                OracleParameter p_um02_id = new OracleParameter();
                p_um02_id.ParameterName = "p_um02_id";
                p_um02_id.Value = 0;
                p_um02_id.Direction = System.Data.ParameterDirection.Input;
                //lstParams.Add(Pview);
                lstParams.Add(p_um02_id);
               
                memberList = await _dbManagerAsync.GetStoredProcedureRefCursorAsync<MemberCode>("Pkg_UM09_USER_MEMBER.Get_Data", lstParams, "pview", cancellationToken);
                memberList = memberList.Where(x => x.UM09_UM02_ID == id).ToList();
                memberArr = new int[memberList.Count];
                if (memberList.Count > 0)
                {
                    for (int i = 0; i < memberList.Count; i++)
                    {
                        memberArr[i] = memberList[i].UM09_RF48_ID;
                    }
                }
            }
            catch (Exception ex)
            {
               
            }

            return memberArr;

        }

        public async Task<List<MemberCode>> GetMembers(int id, CancellationToken cancellationToken)
        {
            List<MemberCode> memberList = new List<MemberCode>();

            try
            {
                OracleParameter Pview = new OracleParameter();
                Pview.ParameterName = "Pview";
                Pview.OracleDbType = OracleDbType.RefCursor;// System.Data.DbType.Int64;
                Pview.Direction = System.Data.ParameterDirection.Output;
                List<OracleParameter> lstParams = new List<OracleParameter>();
                lstParams.Add(Pview);

                OracleParameter p_um02_id = new OracleParameter();
                p_um02_id.ParameterName = "p_um02_id";
                p_um02_id.Value = 0;
                p_um02_id.Direction = System.Data.ParameterDirection.Input;
                //lstParams.Add(Pview);
                lstParams.Add(p_um02_id);
                //memberList = new DBManager().GetStoredProcedureResult<DTO.MemberCode>("Pkg_UM09_USER_MEMBER.Get_Data", lstParams).ToList();
                memberList = await _dbManagerAsync.GetStoredProcedureRefCursorAsync<MemberCode>("Pkg_UM09_USER_MEMBER.Get_Data", lstParams, "pview", cancellationToken);
                memberList = memberList.Where(x => x.UM09_UM02_ID == id).ToList();

            }
            catch (Exception ex)
            {
                //log.Error(ex);
            }

            return memberList;

        }

        public async Task VerifyUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            //GenericResponse response = new GenericResponse();
            try
            {
                string outParam = "";
                List<OracleParameter> parameters = new List<OracleParameter>();

                OracleParameter P_result = new OracleParameter();
                P_result.ParameterName = "P_result";
                P_result.Size = 32767;
                P_result.Direction = System.Data.ParameterDirection.Output;
                parameters.Add(P_result);

                OracleParameter P_login = new OracleParameter();
                P_login.ParameterName = "P_login";
                P_login.Value = userName;
                P_login.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(P_login);
                var x = await _dbManagerAsync.ExecuteStoredProcedureAsync("Pkg_UM02_USERS.User_Validtion", parameters);

                //response = new DBManager().ExecuteStoredProcedure("Pkg_UM02_USERS.User_Validtion", parameters);
                //Dictionary<string, string> _parametersVal = (Dictionary<string, string>)response.responseObject;
                //if (_parametersVal != null && _parametersVal.Count > 0)
                //{
                //    var result = _parametersVal.Where(x => x.Key.ToLower() == "p_result").FirstOrDefault();
                //    if (!string.IsNullOrEmpty(result.Value))
                //    {
                //        outParam = Convert.ToString(result.Value);
                //    }

                //}
                //if (outParam != null && outParam != "")
                //{
                //    if (outParam.ToLower() == "valid")
                //    {
                //        //response.status = Enumerations.Status.Success;
                //    }
                //    else
                //    {
                //        //response.status = Enumerations.Status.Failure;
                //        //response.feedBack = userName + " is already occupied, Please try with some other username.";
                //    }


                //}
            }
            catch (Exception ex)
            {
                
            }
            


        }

        public async Task SaveUserAsync(UserViewModel user)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            OracleParameter PUM02_IMAGE = new OracleParameter();
            PUM02_IMAGE.ParameterName = "PUM02_IMAGE";
            PUM02_IMAGE.Value = user.UM02_IMAGE;
            PUM02_IMAGE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_IMAGE);

            OracleParameter PUM02_LAST_FAILED_LOGIN = new OracleParameter();
            PUM02_LAST_FAILED_LOGIN.ParameterName = "PUM02_LAST_FAILED_LOGIN";
            PUM02_LAST_FAILED_LOGIN.Value = user.UM02_LAST_FAILED_LOGIN;
            PUM02_LAST_FAILED_LOGIN.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_LAST_FAILED_LOGIN);

            OracleParameter PUM02_PREF_LANGUAGE = new OracleParameter();
            PUM02_PREF_LANGUAGE.ParameterName = "PUM02_PREF_LANGUAGE";
            PUM02_PREF_LANGUAGE.Value = user.UM02_PREF_LANGUAGE;
            PUM02_PREF_LANGUAGE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_PREF_LANGUAGE);

            OracleParameter PUM02_FIRST_LOGIN = new OracleParameter();
            PUM02_FIRST_LOGIN.ParameterName = "PUM02_FIRST_LOGIN";
            PUM02_FIRST_LOGIN.Value = user.UM02_FIRST_LOGIN;
            PUM02_FIRST_LOGIN.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_FIRST_LOGIN);

            OracleParameter PUM02_LOCK_ACCOUNT = new OracleParameter();
            PUM02_LOCK_ACCOUNT.ParameterName = "PUM02_LOCK_ACCOUNT";
            PUM02_LOCK_ACCOUNT.Value = user.UM02_LOCK_ACCOUNT;
            PUM02_LOCK_ACCOUNT.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_LOCK_ACCOUNT);

            OracleParameter PUM02_REMARKS = new OracleParameter();
            PUM02_REMARKS.ParameterName = "PUM02_REMARKS";
            PUM02_REMARKS.Value = user.UM02_REMARKS;
            PUM02_REMARKS.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_REMARKS);

            OracleParameter PUM02_APPROVAL_STATUS = new OracleParameter();
            PUM02_APPROVAL_STATUS.ParameterName = "PUM02_APPROVAL_STATUS";
            PUM02_APPROVAL_STATUS.Value = user.UM02_APPROVAL_STATUS;
            PUM02_APPROVAL_STATUS.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_APPROVAL_STATUS);

            OracleParameter PUM02_AUTHORIZED_DATE = new OracleParameter();
            PUM02_AUTHORIZED_DATE.ParameterName = "PUM02_AUTHORIZED_DATE";
            PUM02_AUTHORIZED_DATE.Value = user.UM02_AUTHORIZED_DATE;
            PUM02_AUTHORIZED_DATE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_AUTHORIZED_DATE);

            OracleParameter PUM02_AUTHORIZED_BY = new OracleParameter();
            PUM02_AUTHORIZED_BY.ParameterName = "PUM02_AUTHORIZED_BY";
            PUM02_AUTHORIZED_BY.Value = user.UM02_AUTHORIZED_BY;
            PUM02_AUTHORIZED_BY.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_AUTHORIZED_BY);

            OracleParameter PUM02_IP = new OracleParameter();
            PUM02_IP.ParameterName = "PUM02_IP";
            PUM02_IP.Value = user.UM02_IP;
            PUM02_IP.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_IP);

            OracleParameter PKey = new OracleParameter();
            PKey.ParameterName = "PKey";
            PKey.Size = 32767;
            PKey.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(PKey);

            OracleParameter PUM02_NAME = new OracleParameter();
            PUM02_NAME.ParameterName = "PUM02_NAME";
            PUM02_NAME.Value = user.UM02_NAME;
            PUM02_NAME.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_NAME);

            OracleParameter PUM02_LOGIN_ID = new OracleParameter();
            PUM02_LOGIN_ID.ParameterName = "PUM02_LOGIN_ID";
            PUM02_LOGIN_ID.Value = user.UM02_LOGIN_ID;
            PUM02_LOGIN_ID.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_LOGIN_ID);

            OracleParameter PUM02_PASSWORD = new OracleParameter();
            PUM02_PASSWORD.ParameterName = "PUM02_PASSWORD";
            PUM02_PASSWORD.Value = user.UM02_PASSWORD;
            PUM02_PASSWORD.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_PASSWORD);

            OracleParameter PUM02_LAST_PWD_CHANGE = new OracleParameter();
            PUM02_LAST_PWD_CHANGE.ParameterName = "PUM02_LAST_PWD_CHANGE";
            PUM02_LAST_PWD_CHANGE.Value = user.UM02_LAST_PWD_CHANGE;
            PUM02_LAST_PWD_CHANGE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_LAST_PWD_CHANGE);

            OracleParameter PUM02_SUPERVISOR = new OracleParameter();
            PUM02_SUPERVISOR.ParameterName = "PUM02_SUPERVISOR";
            PUM02_SUPERVISOR.Value = user.UM02_SUPERVISOR;
            PUM02_SUPERVISOR.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_SUPERVISOR);

            OracleParameter PUM02_IS_BRANCH_ADMIN = new OracleParameter();
            PUM02_IS_BRANCH_ADMIN.ParameterName = "PUM02_IS_BRANCH_ADMIN";
            PUM02_IS_BRANCH_ADMIN.Value = user.UM02_IS_BRANCH_ADMIN;
            PUM02_IS_BRANCH_ADMIN.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_IS_BRANCH_ADMIN);

            OracleParameter PUM02_JOB_TITLE = new OracleParameter();
            PUM02_JOB_TITLE.ParameterName = "PUM02_JOB_TITLE";
            PUM02_JOB_TITLE.Value = user.UM02_JOB_TITLE;
            PUM02_JOB_TITLE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_JOB_TITLE);

            OracleParameter PUM02_REG_DATE = new OracleParameter();
            PUM02_REG_DATE.ParameterName = "PUM02_REG_DATE";
            PUM02_REG_DATE.Value = user.UM02_REG_DATE;
            PUM02_REG_DATE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_REG_DATE);

            OracleParameter PUM02_EXPIRY_DATE = new OracleParameter();
            PUM02_EXPIRY_DATE.ParameterName = "PUM02_EXPIRY_DATE";
            PUM02_EXPIRY_DATE.Value = user.UM02_EXPIRY_DATE;
            PUM02_EXPIRY_DATE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_EXPIRY_DATE);

            OracleParameter PUM02_BRANCH = new OracleParameter();
            PUM02_BRANCH.ParameterName = "PUM02_BRANCH";
            PUM02_BRANCH.Value = user.UM02_BRANCH;
            PUM02_BRANCH.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_BRANCH);

            OracleParameter PUM02_DEPARTMENT = new OracleParameter();
            PUM02_DEPARTMENT.ParameterName = "PUM02_DEPARTMENT";
            PUM02_DEPARTMENT.Value = user.UM02_DEPARTMENT;
            PUM02_DEPARTMENT.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_DEPARTMENT);

            OracleParameter PUM02_EMAIL = new OracleParameter();
            PUM02_EMAIL.ParameterName = "PUM02_EMAIL";
            PUM02_EMAIL.Value = user.UM02_EMAIL;
            PUM02_EMAIL.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_EMAIL);

            OracleParameter PUM02_MOBILE = new OracleParameter();
            PUM02_MOBILE.ParameterName = "PUM02_MOBILE";
            PUM02_MOBILE.Value = user.UM02_MOBILE;
            PUM02_MOBILE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_MOBILE);

            OracleParameter PUM02_FAX = new OracleParameter();
            PUM02_FAX.ParameterName = "PUM02_FAX";
            PUM02_FAX.Value = user.UM02_FAX;
            PUM02_FAX.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_FAX);

            OracleParameter PUM02_UM01_ID = new OracleParameter();
            PUM02_UM01_ID.ParameterName = "PUM02_UM01_ID";
            PUM02_UM01_ID.Value = user.UM02_UM01_ID;
            PUM02_UM01_ID.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_UM01_ID);

            OracleParameter PUM02_TYPE = new OracleParameter();
            PUM02_TYPE.ParameterName = "PUM02_TYPE";
            PUM02_TYPE.Value = user.UM02_TYPE;
            PUM02_TYPE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_TYPE);

            OracleParameter PUM02_ASSIGNED_APPS = new OracleParameter();
            PUM02_ASSIGNED_APPS.ParameterName = "PUM02_ASSIGNED_APPS";
            PUM02_ASSIGNED_APPS.Value = user.UM02_ASSIGNED_APPS;
            PUM02_ASSIGNED_APPS.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_ASSIGNED_APPS);

            OracleParameter PUM02_AUTHENTICATION_MODE = new OracleParameter();
            PUM02_AUTHENTICATION_MODE.ParameterName = "PUM02_AUTHENTICATION_MODE";
            PUM02_AUTHENTICATION_MODE.Value = user.UM02_AUTHENTICATION_MODE;
            PUM02_AUTHENTICATION_MODE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_AUTHENTICATION_MODE);

            OracleParameter PUM02_STATUS = new OracleParameter();
            PUM02_STATUS.ParameterName = "PUM02_STATUS";
            PUM02_STATUS.Value = user.UM02_STATUS;
            PUM02_STATUS.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_STATUS);

            OracleParameter PUM02_LOCKED_DATE = new OracleParameter();
            PUM02_LOCKED_DATE.ParameterName = "PUM02_LOCKED_DATE";
            PUM02_LOCKED_DATE.Value = user.UM02_LOCKED_DATE;
            PUM02_LOCKED_DATE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_LOCKED_DATE);

            OracleParameter PUM02_LAST_LOGIN = new OracleParameter();
            PUM02_LAST_LOGIN.ParameterName = "PUM02_LAST_LOGIN";
            PUM02_LAST_LOGIN.Value = user.UM02_LAST_LOGIN;
            PUM02_LAST_LOGIN.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_LAST_LOGIN);

            OracleParameter PUM02_Edited_by = new OracleParameter();
            PUM02_Edited_by.ParameterName = "PUM02_Edited_by";
            PUM02_Edited_by.Value = user.UM02_CREATED_BY;
            PUM02_Edited_by.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_Edited_by);

            OracleParameter PUM02_TOKKEN = new OracleParameter();
            PUM02_TOKKEN.ParameterName = "PUM02_TOKKEN";
            PUM02_TOKKEN.Value = user.Token;
            PUM02_TOKKEN.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_TOKKEN);

            OracleParameter PUM02_TOKKEN_STATUS = new OracleParameter();
            PUM02_TOKKEN_STATUS.ParameterName = "PUM02_TOKKEN_STATUS";
            PUM02_TOKKEN_STATUS.Value = 0;
            PUM02_TOKKEN_STATUS.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_TOKKEN_STATUS);

            OracleParameter PUM02_TOKKEN_DATE = new OracleParameter();
            PUM02_TOKKEN_DATE.ParameterName = "PUM02_TOKKEN_DATE";
            PUM02_TOKKEN_DATE.Value = user.Token_Expiry;
            PUM02_TOKKEN_DATE.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_TOKKEN_DATE);

            OracleParameter PUM02_IS_LDAP = new OracleParameter();
            PUM02_IS_LDAP.ParameterName = "PUM02_IS_LDAP";
            PUM02_IS_LDAP.Value = user.UM02_IS_LDAP ? 1 : 0;
            PUM02_IS_LDAP.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_IS_LDAP);

            OracleParameter PUM02_LAST_NAME = new OracleParameter();
            PUM02_LAST_NAME.ParameterName = "PUM02_LAST_NAME";
            PUM02_LAST_NAME.Value = user.UM02_LAST_NAME;
            PUM02_LAST_NAME.Direction = System.Data.ParameterDirection.Input;
            parameters.Add(PUM02_LAST_NAME);

           var x = await _dbManagerAsync.ExecuteStoredProcedureAsync("Pkg_UM02_USERS.Add_Data", parameters);

            //response.status = Enumerations.Status.Success;

        }

        public Task UpdatePostStatus(int user_id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int user_id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
