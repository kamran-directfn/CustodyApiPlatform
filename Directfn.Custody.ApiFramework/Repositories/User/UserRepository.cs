using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;
using Directfn.Custody.ApiFramework.DTOs.User;
using Directfn.Custody.ApiFramework.Passwords;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.VisualBasic;
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
        private readonly IOracleDbManagerAsync _dbManager;

        public UserRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<LoginUserRecord?> GetUserForLoginAsync(string loginId, long rf48Code, CancellationToken cancellationToken)
        {
            List<OracleParameter> parameters = new() { new OracleParameter("pview", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }, new OracleParameter("P_login", OracleDbType.Varchar2) { Direction = ParameterDirection.Input, Value = loginId }, new OracleParameter("p_rf48_code", OracleDbType.Decimal) { Direction = ParameterDirection.Input, Value = rf48Code } };

            List<LoginUserRecord> users = await _dbManager.GetStoredProcedureRefCursorAsync<LoginUserRecord>("Pkg_UM02_USERS.User_Login", parameters, "pview", cancellationToken);

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

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_UM02_USERS.change_password_tokken", parameters, cancellationToken);
        }

        public async Task<List<UserViewModel>> GetAllUserAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.Direction = System.Data.ParameterDirection.Output;
            Pview.OracleDbType = OracleDbType.RefCursor;
            parameters.Add(Pview);

            var users = await _dbManager.GetStoredProcedureRefCursorAsync<UserViewModel>("Pkg_UM02_USERS.Get_Data", parameters, "pview", cancellationToken);

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
                lstParams.Add(p_um02_id);

                memberList = await _dbManager.GetStoredProcedureRefCursorAsync<MemberCode>("Pkg_UM09_USER_MEMBER.Get_Data", lstParams, "pview", cancellationToken);
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
                List<OracleParameter> lstParams = new List<OracleParameter>();

                OracleParameter Pview = new OracleParameter();
                Pview.ParameterName = "Pview";
                Pview.OracleDbType = OracleDbType.RefCursor;// System.Data.DbType.Int64;
                Pview.Direction = System.Data.ParameterDirection.Output;
                lstParams.Add(Pview);

                OracleParameter p_um02_id = new OracleParameter();
                p_um02_id.ParameterName = "p_um02_id";
                p_um02_id.Value = 0;
                p_um02_id.Direction = System.Data.ParameterDirection.Input;
                lstParams.Add(p_um02_id);

                memberList = await _dbManager.GetStoredProcedureRefCursorAsync<MemberCode>("Pkg_UM09_USER_MEMBER.Get_Data", lstParams, "pview", cancellationToken);
                memberList = memberList.Where(x => x.UM09_UM02_ID == id).ToList();
            }
            catch (Exception ex)
            {
            }

            return memberList;
        }

        public async Task<string> VerifyUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            string message = string.Empty;
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
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM02_USERS.User_Validtion", parameters);

                message = result.GetString("P_result");

            }
            catch (Exception ex)
            {
            }

            return message;

        }

        public async Task<int> SaveUserAsync(UserRequestModel user)
        {
            int new_user_id = 0;
            int userId = 1;//Int32.Parse(_currentUserService.UserId);
            string userName = "";//_currentUserService.UserName.ToString();
            UserViewModel data = new UserViewModel();
            try
            {

                if (data.UM02_SUPERVISOR == 0)
                {
                    data.UM02_SUPERVISOR = -1;
                }

                data.UM02_PASSWORD = "default123";// _legacyPasswordService.EncryptLegacyPassword("default123");                //Convert.ToString(ConfigurationManager.AppSettings["Default_Password"]);
                data.UM02_FIRST_LOGIN = 1;

                if (user.isLockAccount)
                {
                    data.UM02_LOCK_ACCOUNT = 1;
                }
                else
                {
                    data.UM02_LOCK_ACCOUNT = 0;
                }
                data.Modified_By_UserName = userName;
                data.Token_Expiry = DateTime.Now.AddHours(1);
                data.UM02_CREATED_BY = userId;

                data.UM02_LOGIN_ID = user.UM02_LOGIN_ID;
                data.UM02_NAME = user.UM02_NAME;
                data.UM02_LAST_NAME = user.UM02_LAST_NAME;
                data.UM02_FAX = user.UM02_FAX;
                data.UM02_MOBILE = user.UM02_MOBILE;
                data.UM04_UM03_ID = user.UM04_UM03_ID;
                data.UM02_REG_DATE = user.UM02_REG_DATE;
                data.UM02_EXPIRY_DATE = user.UM02_EXPIRY_DATE;
                data.UM02_JOB_TITLE = user.UM02_JOB_TITLE;
                data.lstMemberCode = user.lstMemberCode;
                data.isLockAccount = user.isLockAccount;
                data.UM02_IS_LDAP = user.UM02_IS_LDAP;
                data.isGeneratePassword = user.isGeneratePassword;
                data.UM02_IMAGE = user.UM02_IMAGE;
                data.Token = Guid.NewGuid().ToString();

                List<OracleParameter> parameters = new List<OracleParameter>();

                OracleParameter PUM02_IMAGE = new OracleParameter();
                PUM02_IMAGE.ParameterName = "PUM02_IMAGE";
                PUM02_IMAGE.Value = data.UM02_IMAGE;
                PUM02_IMAGE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_IMAGE);

                OracleParameter PUM02_LAST_FAILED_LOGIN = new OracleParameter();
                PUM02_LAST_FAILED_LOGIN.ParameterName = "PUM02_LAST_FAILED_LOGIN";
                PUM02_LAST_FAILED_LOGIN.Value = data.UM02_LAST_FAILED_LOGIN;
                PUM02_LAST_FAILED_LOGIN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LAST_FAILED_LOGIN);

                OracleParameter PUM02_PREF_LANGUAGE = new OracleParameter();
                PUM02_PREF_LANGUAGE.ParameterName = "PUM02_PREF_LANGUAGE";
                PUM02_PREF_LANGUAGE.Value = data.UM02_PREF_LANGUAGE;
                PUM02_PREF_LANGUAGE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_PREF_LANGUAGE);

                OracleParameter PUM02_FIRST_LOGIN = new OracleParameter();
                PUM02_FIRST_LOGIN.ParameterName = "PUM02_FIRST_LOGIN";
                PUM02_FIRST_LOGIN.Value = data.UM02_FIRST_LOGIN;
                PUM02_FIRST_LOGIN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_FIRST_LOGIN);

                OracleParameter PUM02_LOCK_ACCOUNT = new OracleParameter();
                PUM02_LOCK_ACCOUNT.ParameterName = "PUM02_LOCK_ACCOUNT";
                PUM02_LOCK_ACCOUNT.Value = data.UM02_LOCK_ACCOUNT;
                PUM02_LOCK_ACCOUNT.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LOCK_ACCOUNT);

                OracleParameter PUM02_REMARKS = new OracleParameter();
                PUM02_REMARKS.ParameterName = "PUM02_REMARKS";
                PUM02_REMARKS.Value = data.UM02_REMARKS;
                PUM02_REMARKS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_REMARKS);

                OracleParameter PUM02_APPROVAL_STATUS = new OracleParameter();
                PUM02_APPROVAL_STATUS.ParameterName = "PUM02_APPROVAL_STATUS";
                PUM02_APPROVAL_STATUS.Value = data.UM02_APPROVAL_STATUS;
                PUM02_APPROVAL_STATUS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_APPROVAL_STATUS);

                OracleParameter PUM02_AUTHORIZED_DATE = new OracleParameter();
                PUM02_AUTHORIZED_DATE.ParameterName = "PUM02_AUTHORIZED_DATE";
                PUM02_AUTHORIZED_DATE.Value = data.UM02_AUTHORIZED_DATE;
                PUM02_AUTHORIZED_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_AUTHORIZED_DATE);

                OracleParameter PUM02_AUTHORIZED_BY = new OracleParameter();
                PUM02_AUTHORIZED_BY.ParameterName = "PUM02_AUTHORIZED_BY";
                PUM02_AUTHORIZED_BY.Value = data.UM02_AUTHORIZED_BY;
                PUM02_AUTHORIZED_BY.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_AUTHORIZED_BY);

                OracleParameter PUM02_IP = new OracleParameter();
                PUM02_IP.ParameterName = "PUM02_IP";
                PUM02_IP.Value = data.UM02_IP;
                PUM02_IP.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_IP);

                OracleParameter PKey = new OracleParameter();
                PKey.ParameterName = "PKey";
                PKey.Size = 32767;
                PKey.Direction = System.Data.ParameterDirection.Output;
                parameters.Add(PKey);

                OracleParameter PUM02_NAME = new OracleParameter();
                PUM02_NAME.ParameterName = "PUM02_NAME";
                PUM02_NAME.Value = data.UM02_NAME;
                PUM02_NAME.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_NAME);

                OracleParameter PUM02_LOGIN_ID = new OracleParameter();
                PUM02_LOGIN_ID.ParameterName = "PUM02_LOGIN_ID";
                PUM02_LOGIN_ID.Value = data.UM02_LOGIN_ID;
                PUM02_LOGIN_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LOGIN_ID);

                OracleParameter PUM02_PASSWORD = new OracleParameter();
                PUM02_PASSWORD.ParameterName = "PUM02_PASSWORD";
                PUM02_PASSWORD.Value = data.UM02_PASSWORD;
                PUM02_PASSWORD.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_PASSWORD);

                OracleParameter PUM02_LAST_PWD_CHANGE = new OracleParameter();
                PUM02_LAST_PWD_CHANGE.ParameterName = "PUM02_LAST_PWD_CHANGE";
                PUM02_LAST_PWD_CHANGE.Value = data.UM02_LAST_PWD_CHANGE;
                PUM02_LAST_PWD_CHANGE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LAST_PWD_CHANGE);

                OracleParameter PUM02_SUPERVISOR = new OracleParameter();
                PUM02_SUPERVISOR.ParameterName = "PUM02_SUPERVISOR";
                PUM02_SUPERVISOR.Value = data.UM02_SUPERVISOR;
                PUM02_SUPERVISOR.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_SUPERVISOR);

                OracleParameter PUM02_IS_BRANCH_ADMIN = new OracleParameter();
                PUM02_IS_BRANCH_ADMIN.ParameterName = "PUM02_IS_BRANCH_ADMIN";
                PUM02_IS_BRANCH_ADMIN.Value = data.UM02_IS_BRANCH_ADMIN;
                PUM02_IS_BRANCH_ADMIN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_IS_BRANCH_ADMIN);

                OracleParameter PUM02_JOB_TITLE = new OracleParameter();
                PUM02_JOB_TITLE.ParameterName = "PUM02_JOB_TITLE";
                PUM02_JOB_TITLE.Value = data.UM02_JOB_TITLE;
                PUM02_JOB_TITLE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_JOB_TITLE);

                OracleParameter PUM02_REG_DATE = new OracleParameter();
                PUM02_REG_DATE.ParameterName = "PUM02_REG_DATE";
                PUM02_REG_DATE.Value = data.UM02_REG_DATE;
                PUM02_REG_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_REG_DATE);

                OracleParameter PUM02_EXPIRY_DATE = new OracleParameter();
                PUM02_EXPIRY_DATE.ParameterName = "PUM02_EXPIRY_DATE";
                PUM02_EXPIRY_DATE.Value = data.UM02_EXPIRY_DATE;
                PUM02_EXPIRY_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_EXPIRY_DATE);

                OracleParameter PUM02_BRANCH = new OracleParameter();
                PUM02_BRANCH.ParameterName = "PUM02_BRANCH";
                PUM02_BRANCH.Value = data.UM02_BRANCH;
                PUM02_BRANCH.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_BRANCH);

                OracleParameter PUM02_DEPARTMENT = new OracleParameter();
                PUM02_DEPARTMENT.ParameterName = "PUM02_DEPARTMENT";
                PUM02_DEPARTMENT.Value = data.UM02_DEPARTMENT;
                PUM02_DEPARTMENT.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_DEPARTMENT);

                OracleParameter PUM02_EMAIL = new OracleParameter();
                PUM02_EMAIL.ParameterName = "PUM02_EMAIL";
                PUM02_EMAIL.Value = data.UM02_EMAIL;
                PUM02_EMAIL.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_EMAIL);

                OracleParameter PUM02_MOBILE = new OracleParameter();
                PUM02_MOBILE.ParameterName = "PUM02_MOBILE";
                PUM02_MOBILE.Value = data.UM02_MOBILE;
                PUM02_MOBILE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_MOBILE);

                OracleParameter PUM02_FAX = new OracleParameter();
                PUM02_FAX.ParameterName = "PUM02_FAX";
                PUM02_FAX.Value = data.UM02_FAX;
                PUM02_FAX.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_FAX);

                OracleParameter PUM02_UM01_ID = new OracleParameter();
                PUM02_UM01_ID.ParameterName = "PUM02_UM01_ID";
                PUM02_UM01_ID.Value = 1;//data.UM02_UM01_ID;
                PUM02_UM01_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_UM01_ID);

                OracleParameter PUM02_TYPE = new OracleParameter();
                PUM02_TYPE.ParameterName = "PUM02_TYPE";
                PUM02_TYPE.Value = data.UM02_TYPE;
                PUM02_TYPE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_TYPE);

                OracleParameter PUM02_ASSIGNED_APPS = new OracleParameter();
                PUM02_ASSIGNED_APPS.ParameterName = "PUM02_ASSIGNED_APPS";
                PUM02_ASSIGNED_APPS.Value = data.UM02_ASSIGNED_APPS;
                PUM02_ASSIGNED_APPS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_ASSIGNED_APPS);

                OracleParameter PUM02_AUTHENTICATION_MODE = new OracleParameter();
                PUM02_AUTHENTICATION_MODE.ParameterName = "PUM02_AUTHENTICATION_MODE";
                PUM02_AUTHENTICATION_MODE.Value = data.UM02_AUTHENTICATION_MODE;
                PUM02_AUTHENTICATION_MODE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_AUTHENTICATION_MODE);

                OracleParameter PUM02_STATUS = new OracleParameter();
                PUM02_STATUS.ParameterName = "PUM02_STATUS";
                PUM02_STATUS.Value = data.UM02_STATUS;
                PUM02_STATUS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_STATUS);

                OracleParameter PUM02_LOCKED_DATE = new OracleParameter();
                PUM02_LOCKED_DATE.ParameterName = "PUM02_LOCKED_DATE";
                PUM02_LOCKED_DATE.Value = data.UM02_LOCKED_DATE;
                PUM02_LOCKED_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LOCKED_DATE);

                OracleParameter PUM02_LAST_LOGIN = new OracleParameter();
                PUM02_LAST_LOGIN.ParameterName = "PUM02_LAST_LOGIN";
                PUM02_LAST_LOGIN.Value = data.UM02_LAST_LOGIN;
                PUM02_LAST_LOGIN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LAST_LOGIN);

                OracleParameter PUM02_Edited_by = new OracleParameter();
                PUM02_Edited_by.ParameterName = "PUM02_Edited_by";
                PUM02_Edited_by.Value = data.UM02_CREATED_BY;
                PUM02_Edited_by.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_Edited_by);

                OracleParameter PUM02_TOKKEN = new OracleParameter();
                PUM02_TOKKEN.ParameterName = "PUM02_TOKKEN";
                PUM02_TOKKEN.Value = data.Token;
                PUM02_TOKKEN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_TOKKEN);

                OracleParameter PUM02_TOKKEN_STATUS = new OracleParameter();
                PUM02_TOKKEN_STATUS.ParameterName = "PUM02_TOKKEN_STATUS";
                PUM02_TOKKEN_STATUS.Value = 0;
                PUM02_TOKKEN_STATUS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_TOKKEN_STATUS);

                OracleParameter PUM02_TOKKEN_DATE = new OracleParameter();
                PUM02_TOKKEN_DATE.ParameterName = "PUM02_TOKKEN_DATE";
                PUM02_TOKKEN_DATE.Value = data.Token_Expiry;
                PUM02_TOKKEN_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_TOKKEN_DATE);

                OracleParameter PUM02_IS_LDAP = new OracleParameter();
                PUM02_IS_LDAP.ParameterName = "PUM02_IS_LDAP";
                PUM02_IS_LDAP.Value = data.UM02_IS_LDAP ? 1 : 0;
                PUM02_IS_LDAP.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_IS_LDAP);

                OracleParameter PUM02_LAST_NAME = new OracleParameter();
                PUM02_LAST_NAME.ParameterName = "PUM02_LAST_NAME";
                PUM02_LAST_NAME.Value = data.UM02_LAST_NAME;
                PUM02_LAST_NAME.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LAST_NAME);

                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM02_USERS.Add_Data", parameters);

                new_user_id = int.Parse(result.GetString("PKey"));
                AddUserRole(data, new_user_id);

                if (user.userPortfolioGroups != null && user.userPortfolioGroups.Count() > 0)
                {
                    foreach (var item in user.userPortfolioGroups)
                    {
                        if (item.GroupId > 0)
                        {
                            MemberCode code = new MemberCode();
                            code.UM09_RF48_ID = item.MemberId;
                            code.UM09_UM02_ID = new_user_id;
                            code.UM09_MODIFIED_BY = userId;
                            code.um09_um14_id = item.GroupId;
                            await SaveMemberCode(code);
                        }
                    }
                }
                else
                {
                    await SaveSadminPortfoliosEntries(new_user_id);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return new_user_id;
        }

        public async Task<string> UpdateUser(UserRequestModel user)
        {
            string message = string.Empty;
            UserViewModel data = new UserViewModel();
            int userId = 1;//Int32.Parse(_currentUserService.UserId);
            string userName = "";//_currentUserService.UserName.ToString();

            try
            {
                if (user.isLockAccount)
                {
                    data.UM02_LOCK_ACCOUNT = 1;
                }
                else
                {
                    data.UM02_LOCK_ACCOUNT = 0;
                }
                // data.Modified_By_UserName = userName;
                data.Token_Expiry = DateTime.Now.AddHours(1);
                data.UM02_MODIFIED_BY = userId;
                data.UM02_ID = user.UM02_ID;
                data.UM02_NAME = user.UM02_NAME;
                data.UM02_LAST_NAME = user.UM02_LAST_NAME;
                data.UM02_FAX = user.UM02_FAX;
                data.UM02_MOBILE = user.UM02_MOBILE;
                data.UM04_UM03_ID = user.UM04_UM03_ID;
                data.UM02_REG_DATE = user.UM02_REG_DATE;
                data.UM02_EXPIRY_DATE = user.UM02_EXPIRY_DATE;
                data.UM02_JOB_TITLE = user.UM02_JOB_TITLE;
                data.lstMemberCode = user.lstMemberCode;
                data.isLockAccount = user.isLockAccount;
                data.UM02_IS_LDAP = user.UM02_IS_LDAP;
                data.isGeneratePassword = user.isGeneratePassword;
                data.UM02_IMAGE = user.UM02_IMAGE;
                data.Token = Guid.NewGuid().ToString();

                List<OracleParameter> parameters = new List<OracleParameter>();

                OracleParameter PUM02_IMAGE = new OracleParameter();
                PUM02_IMAGE.ParameterName = "PUM02_IMAGE";
                PUM02_IMAGE.Value = data.UM02_IMAGE;
                PUM02_IMAGE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_IMAGE);

                OracleParameter PUM02_LAST_FAILED_LOGIN = new OracleParameter();
                PUM02_LAST_FAILED_LOGIN.ParameterName = "PUM02_LAST_FAILED_LOGIN";
                PUM02_LAST_FAILED_LOGIN.Value = data.UM02_LAST_FAILED_LOGIN;
                PUM02_LAST_FAILED_LOGIN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LAST_FAILED_LOGIN);

                OracleParameter PUM02_PREF_LANGUAGE = new OracleParameter();
                PUM02_PREF_LANGUAGE.ParameterName = "PUM02_PREF_LANGUAGE";
                PUM02_PREF_LANGUAGE.Value = data.UM02_PREF_LANGUAGE;
                PUM02_PREF_LANGUAGE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_PREF_LANGUAGE);

                OracleParameter PUM02_FIRST_LOGIN = new OracleParameter();
                PUM02_FIRST_LOGIN.ParameterName = "PUM02_FIRST_LOGIN";
                PUM02_FIRST_LOGIN.Value = data.UM02_FIRST_LOGIN;
                PUM02_FIRST_LOGIN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_FIRST_LOGIN);

                OracleParameter PUM02_LOCK_ACCOUNT = new OracleParameter();
                PUM02_LOCK_ACCOUNT.ParameterName = "PUM02_LOCK_ACCOUNT";
                PUM02_LOCK_ACCOUNT.Value = data.UM02_LOCK_ACCOUNT;
                PUM02_LOCK_ACCOUNT.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LOCK_ACCOUNT);

                OracleParameter PUM02_REMARKS = new OracleParameter();
                PUM02_REMARKS.ParameterName = "PUM02_REMARKS";
                PUM02_REMARKS.Value = data.UM02_REMARKS;
                PUM02_REMARKS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_REMARKS);

                OracleParameter PUM02_APPROVAL_STATUS = new OracleParameter();
                PUM02_APPROVAL_STATUS.ParameterName = "PUM02_APPROVAL_STATUS";
                PUM02_APPROVAL_STATUS.Value = data.UM02_APPROVAL_STATUS;
                PUM02_APPROVAL_STATUS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_APPROVAL_STATUS);

                OracleParameter PUM02_AUTHORIZED_DATE = new OracleParameter();
                PUM02_AUTHORIZED_DATE.ParameterName = "PUM02_AUTHORIZED_DATE";
                PUM02_AUTHORIZED_DATE.Value = data.UM02_AUTHORIZED_DATE;
                PUM02_AUTHORIZED_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_AUTHORIZED_DATE);

                OracleParameter PUM02_AUTHORIZED_BY = new OracleParameter();
                PUM02_AUTHORIZED_BY.ParameterName = "PUM02_AUTHORIZED_BY";
                PUM02_AUTHORIZED_BY.Value = data.UM02_AUTHORIZED_BY;
                PUM02_AUTHORIZED_BY.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_AUTHORIZED_BY);

                OracleParameter PUM02_IP = new OracleParameter();
                PUM02_IP.ParameterName = "PUM02_IP";
                PUM02_IP.Value = data.UM02_IP;
                PUM02_IP.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_IP);

                OracleParameter PUM02_ID = new OracleParameter();
                PUM02_ID.ParameterName = "PUM02_ID";
                PUM02_ID.Value = data.UM02_ID;
                PUM02_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_ID);

                OracleParameter PUM02_NAME = new OracleParameter();
                PUM02_NAME.ParameterName = "PUM02_NAME";
                PUM02_NAME.Value = data.UM02_NAME;
                PUM02_NAME.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_NAME);

                OracleParameter PUM02_LOGIN_ID = new OracleParameter();
                PUM02_LOGIN_ID.ParameterName = "PUM02_LOGIN_ID";
                PUM02_LOGIN_ID.Value = data.UM02_LOGIN_ID;
                PUM02_LOGIN_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LOGIN_ID);

                OracleParameter PUM02_PASSWORD = new OracleParameter();
                PUM02_PASSWORD.ParameterName = "PUM02_PASSWORD";
                PUM02_PASSWORD.Value = data.UM02_PASSWORD;
                PUM02_PASSWORD.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_PASSWORD);

                OracleParameter PUM02_LAST_PWD_CHANGE = new OracleParameter();
                PUM02_LAST_PWD_CHANGE.ParameterName = "PUM02_LAST_PWD_CHANGE";
                PUM02_LAST_PWD_CHANGE.Value = data.UM02_LAST_PWD_CHANGE;
                PUM02_LAST_PWD_CHANGE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LAST_PWD_CHANGE);

                OracleParameter PUM02_SUPERVISOR = new OracleParameter();
                PUM02_SUPERVISOR.ParameterName = "PUM02_SUPERVISOR";
                PUM02_SUPERVISOR.Value = data.UM02_SUPERVISOR;
                PUM02_SUPERVISOR.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_SUPERVISOR);

                OracleParameter PUM02_IS_BRANCH_ADMIN = new OracleParameter();
                PUM02_IS_BRANCH_ADMIN.ParameterName = "PUM02_IS_BRANCH_ADMIN";
                PUM02_IS_BRANCH_ADMIN.Value = data.UM02_IS_BRANCH_ADMIN;
                PUM02_IS_BRANCH_ADMIN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_IS_BRANCH_ADMIN);

                OracleParameter PUM02_JOB_TITLE = new OracleParameter();
                PUM02_JOB_TITLE.ParameterName = "PUM02_JOB_TITLE";
                PUM02_JOB_TITLE.Value = data.UM02_JOB_TITLE;
                PUM02_JOB_TITLE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_JOB_TITLE);

                OracleParameter PUM02_REG_DATE = new OracleParameter();
                PUM02_REG_DATE.ParameterName = "PUM02_REG_DATE";
                PUM02_REG_DATE.Value = data.UM02_REG_DATE;
                PUM02_REG_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_REG_DATE);

                OracleParameter PUM02_EXPIRY_DATE = new OracleParameter();
                PUM02_EXPIRY_DATE.ParameterName = "PUM02_EXPIRY_DATE";
                PUM02_EXPIRY_DATE.Value = data.UM02_EXPIRY_DATE;
                PUM02_EXPIRY_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_EXPIRY_DATE);

                OracleParameter PUM02_BRANCH = new OracleParameter();
                PUM02_BRANCH.ParameterName = "PUM02_BRANCH";
                PUM02_BRANCH.Value = data.UM02_BRANCH;
                PUM02_BRANCH.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_BRANCH);

                OracleParameter PUM02_DEPARTMENT = new OracleParameter();
                PUM02_DEPARTMENT.ParameterName = "PUM02_DEPARTMENT";
                PUM02_DEPARTMENT.Value = data.UM02_DEPARTMENT;
                PUM02_DEPARTMENT.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_DEPARTMENT);

                OracleParameter PUM02_EMAIL = new OracleParameter();
                PUM02_EMAIL.ParameterName = "PUM02_EMAIL";
                PUM02_EMAIL.Value = data.UM02_EMAIL;
                PUM02_EMAIL.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_EMAIL);

                OracleParameter PUM02_MOBILE = new OracleParameter();
                PUM02_MOBILE.ParameterName = "PUM02_MOBILE";
                PUM02_MOBILE.Value = data.UM02_MOBILE;
                PUM02_MOBILE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_MOBILE);

                OracleParameter PUM02_FAX = new OracleParameter();
                PUM02_FAX.ParameterName = "PUM02_FAX";
                PUM02_FAX.Value = data.UM02_FAX;
                PUM02_FAX.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_FAX);

                OracleParameter PUM02_UM01_ID = new OracleParameter();
                PUM02_UM01_ID.ParameterName = "PUM02_UM01_ID";
                PUM02_UM01_ID.Value = 1;//data.UM02_UM01_ID;
                PUM02_UM01_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_UM01_ID);

                OracleParameter PUM02_TYPE = new OracleParameter();
                PUM02_TYPE.ParameterName = "PUM02_TYPE";
                PUM02_TYPE.Value = data.UM02_TYPE;
                PUM02_TYPE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_TYPE);

                OracleParameter PUM02_ASSIGNED_APPS = new OracleParameter();
                PUM02_ASSIGNED_APPS.ParameterName = "PUM02_ASSIGNED_APPS";
                PUM02_ASSIGNED_APPS.Value = data.UM02_ASSIGNED_APPS;
                PUM02_ASSIGNED_APPS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_ASSIGNED_APPS);

                OracleParameter PUM02_AUTHENTICATION_MODE = new OracleParameter();
                PUM02_AUTHENTICATION_MODE.ParameterName = "PUM02_AUTHENTICATION_MODE";
                PUM02_AUTHENTICATION_MODE.Value = data.UM02_AUTHENTICATION_MODE;
                PUM02_AUTHENTICATION_MODE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_AUTHENTICATION_MODE);

                OracleParameter PUM02_STATUS = new OracleParameter();
                PUM02_STATUS.ParameterName = "PUM02_STATUS";
                PUM02_STATUS.Value = data.UM02_STATUS;
                PUM02_STATUS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_STATUS);

                OracleParameter PUM02_LOCKED_DATE = new OracleParameter();
                PUM02_LOCKED_DATE.ParameterName = "PUM02_LOCKED_DATE";
                PUM02_LOCKED_DATE.Value = data.UM02_LOCKED_DATE;
                PUM02_LOCKED_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LOCKED_DATE);

                OracleParameter PUM02_LAST_LOGIN = new OracleParameter();
                PUM02_LAST_LOGIN.ParameterName = "PUM02_LAST_LOGIN";
                PUM02_LAST_LOGIN.Value = data.UM02_LAST_LOGIN;
                PUM02_LAST_LOGIN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LAST_LOGIN);

                OracleParameter PUM02_Edited_by = new OracleParameter();
                PUM02_Edited_by.ParameterName = "PUM02_Edited_by";
                PUM02_Edited_by.Value = data.UM02_CREATED_BY;
                PUM02_Edited_by.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_Edited_by);

                OracleParameter PError = new OracleParameter();
                PError.ParameterName = "PError";
                PError.Direction = System.Data.ParameterDirection.Output;
                parameters.Add(PError);

                OracleParameter PUM02_TOKKEN = new OracleParameter();
                PUM02_TOKKEN.ParameterName = "PUM02_TOKKEN";
                PUM02_TOKKEN.Value = data.Token;
                PUM02_TOKKEN.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_TOKKEN);

                OracleParameter PUM02_TOKKEN_STATUS = new OracleParameter();
                PUM02_TOKKEN_STATUS.ParameterName = "PUM02_TOKKEN_STATUS";
                PUM02_TOKKEN_STATUS.Value = 0;
                PUM02_TOKKEN_STATUS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_TOKKEN_STATUS);

                OracleParameter PUM02_TOKKEN_DATE = new OracleParameter();
                PUM02_TOKKEN_DATE.ParameterName = "PUM02_TOKKEN_DATE";
                PUM02_TOKKEN_DATE.Value = data.Token_Expiry;
                PUM02_TOKKEN_DATE.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_TOKKEN_DATE);

                OracleParameter PUM02_IS_LDAP = new OracleParameter();
                PUM02_IS_LDAP.ParameterName = "PUM02_IS_LDAP";
                PUM02_IS_LDAP.Value = data.UM02_IS_LDAP ? 1 : 0;
                PUM02_IS_LDAP.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_IS_LDAP);

                OracleParameter PUM02_LAST_NAME = new OracleParameter();
                PUM02_LAST_NAME.ParameterName = "PUM02_LAST_NAME";
                PUM02_LAST_NAME.Value = data.UM02_LAST_NAME;
                PUM02_LAST_NAME.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_LAST_NAME);

                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM02_USERS.Edit_Data", parameters);

                EditUserRole(data);

                await DeleteMemberByUser(data.UM02_ID);
                if (user.userPortfolioGroups != null && user.userPortfolioGroups.Count() > 0)
                {
                    foreach (var item in user.userPortfolioGroups)
                    {
                        if (item.GroupId > 0)
                        {
                            MemberCode code = new MemberCode();
                            code.UM09_RF48_ID = item.MemberId;
                            code.UM09_UM02_ID = data.UM02_ID;
                            code.UM09_MODIFIED_BY = userId;
                            code.um09_um14_id = item.GroupId;
                            await SaveMemberCode(code);
                        }
                    }
                }
                else
                {
                    await SaveSadminPortfoliosEntries(data.UM02_ID);
                }

                message = result.GetString("PError");
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return message;
        }

        public async Task<List<UserViewModel>> UpdatePostStatus(int um02_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<UserViewModel> lst = new List<UserViewModel>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();

                OracleParameter PUM02_ID = new OracleParameter();
                PUM02_ID.ParameterName = "PUM02_ID";
                PUM02_ID.Value = um02_id;
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

                await _dbManager.ExecuteStoredProcedureAsync("Pkg_UM02_USERS.Update_User_Post_Status", parameters);

                lst = await GetAllUserAsync(cancellationToken);

            }
            catch (Exception ex)
            {
            }
            return lst;
        }

        public async Task<string> Delete(int um02_id, int user_id, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            try
            {
                string outParam = "";
                List<OracleParameter> parameters = new List<OracleParameter>();

                OracleParameter PKey = new OracleParameter();
                PKey.ParameterName = "PKey";
                PKey.Size = 32767;
                PKey.Direction = System.Data.ParameterDirection.Output;
                parameters.Add(PKey);

                OracleParameter PUM02_ID = new OracleParameter();
                PUM02_ID.ParameterName = "PUM02_ID";
                PUM02_ID.Value = um02_id;
                PUM02_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_ID);

                OracleParameter PUM02_Edited_by = new OracleParameter();
                PUM02_Edited_by.ParameterName = "PUM02_Edited_by";
                PUM02_Edited_by.Value = user_id;
                PUM02_Edited_by.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM02_Edited_by);

                OracleParameter PError = new OracleParameter();
                PError.ParameterName = "PError";
                PError.Direction = System.Data.ParameterDirection.Output;
                parameters.Add(PError);

                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_UM02_USERS.Delete_Data", parameters);

                message = result.GetString("PError");

            }
            catch (Exception ex)
            {
            }

            return message;
        }

        public async Task SaveMemberCode(MemberCode code)
        {
          
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();

                OracleParameter UM09_ID = new OracleParameter();
                UM09_ID.ParameterName = "UM09_ID";
                UM09_ID.Value = code.UM09_ID;
                UM09_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(UM09_ID);

                OracleParameter PUM09_UM02_ID = new OracleParameter();
                PUM09_UM02_ID.ParameterName = "PUM09_UM02_ID";
                PUM09_UM02_ID.Value = code.UM09_UM02_ID;
                PUM09_UM02_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM09_UM02_ID);

                OracleParameter PUM09_RF48_ID = new OracleParameter();
                PUM09_RF48_ID.ParameterName = "PUM09_RF48_ID";
                PUM09_RF48_ID.Value = code.UM09_RF48_ID;
                PUM09_RF48_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM09_RF48_ID);

                OracleParameter PUM09_STATUS = new OracleParameter();
                PUM09_STATUS.ParameterName = "PUM09_STATUS";
                PUM09_STATUS.Value = 2;
                PUM09_STATUS.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM09_STATUS);

                OracleParameter PUM09_Edited_by = new OracleParameter();
                PUM09_Edited_by.ParameterName = "PUM09_Edited_by";
                PUM09_Edited_by.Value = code.UM09_MODIFIED_BY;
                PUM09_Edited_by.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM09_Edited_by);

                OracleParameter PUM09_UM14_ID = new OracleParameter();
                PUM09_UM14_ID.ParameterName = "PUM09_UM14_ID";
                PUM09_UM14_ID.Value = code.um09_um14_id;
                PUM09_UM14_ID.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(PUM09_UM14_ID);

                await _dbManager.ExecuteStoredProcedureAsync("Pkg_UM09_USER_MEMBER.Add_Data", parameters);

                
            }
            catch (Exception ex)
            {
            }
            
        }

        public async Task DeleteMemberByUser(int um02_id)
        {
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                OracleParameter pum02_id = new OracleParameter();
                pum02_id.ParameterName = "pum02_id";
                pum02_id.Value = um02_id;
                pum02_id.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(pum02_id);

                await _dbManager.ExecuteStoredProcedureAsync("Pkg_UM09_USER_MEMBER.DeleteMemberByUser", parameters);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task SaveSadminPortfoliosEntries(int um02_Id)
        {
            
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                OracleParameter p_um02_id = new OracleParameter();
                p_um02_id.ParameterName = "p_um02_id";
                p_um02_id.OracleDbType = OracleDbType.Int32;
                p_um02_id.Value = um02_Id;
                p_um02_id.Direction = System.Data.ParameterDirection.Input;
                parameters.Add(p_um02_id);

                await _dbManager.ExecuteStoredProcedureAsync("pkg_portfolio_groups.mapp_sadmin_entries", parameters);
               
            }
            catch (Exception ex)
            {
            }

        }

        private async Task<bool> AddUserRole(UserViewModel user, int uM04_UM02_ID)
        {
            try
            {
                if (user != null && uM04_UM02_ID > 0)
                {
                    List<OracleParameter> parameters = new List<OracleParameter>();
                    
                    OracleParameter PKey = new OracleParameter();
                    PKey.ParameterName = "PKey";
                    PKey.Size = 32767;
                    PKey.Direction = System.Data.ParameterDirection.Output;
                    parameters.Add(PKey);

                    OracleParameter PUM04_UM02_ID = new OracleParameter();
                    PUM04_UM02_ID.ParameterName = "PUM04_UM02_ID";
                    PUM04_UM02_ID.OracleDbType = OracleDbType.Int32;
                    PUM04_UM02_ID.Value = uM04_UM02_ID;
                    PUM04_UM02_ID.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_UM02_ID);

                    OracleParameter PUM04_UM03_ID = new OracleParameter();
                    PUM04_UM03_ID.ParameterName = "PUM04_UM03_ID";
                    PUM04_UM03_ID.OracleDbType = OracleDbType.Int32;
                    PUM04_UM03_ID.Value = user.UM04_UM03_ID;
                    PUM04_UM03_ID.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_UM03_ID);

                    OracleParameter PUM04_STATUS = new OracleParameter();
                    PUM04_STATUS.ParameterName = "PUM04_STATUS";
                    PUM04_STATUS.OracleDbType = OracleDbType.Int32;
                    PUM04_STATUS.Value = 1;
                    PUM04_STATUS.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_STATUS);

                    OracleParameter PUM04_IP = new OracleParameter();
                    PUM04_IP.ParameterName = "PUM04_IP";
                    PUM04_IP.OracleDbType = OracleDbType.Varchar2;
                    PUM04_IP.Value = user.UM02_IP;
                    PUM04_IP.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_IP);

                    OracleParameter PUM04_Edited_by = new OracleParameter();
                    PUM04_Edited_by.ParameterName = "PUM04_Edited_by";
                    PUM04_Edited_by.OracleDbType = OracleDbType.Int32;
                    PUM04_Edited_by.Value = user.UM02_CREATED_BY;
                    PUM04_Edited_by.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_Edited_by);

                    await _dbManager.ExecuteStoredProcedureAsync("Pkg_UM04_USERS_ROLES.Add_Data", parameters);
                    
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> EditUserRole(UserViewModel user)
        {
            try
            {
                if (user != null)
                {
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    OracleParameter PKey = new OracleParameter();
                    PKey.ParameterName = "PKey";
                    PKey.Size = 32767;
                    PKey.Direction = System.Data.ParameterDirection.Output;
                    parameters.Add(PKey);

                    OracleParameter PUM04_UM02_ID = new OracleParameter();
                    PUM04_UM02_ID.ParameterName = "PUM04_UM02_ID";
                    PUM04_UM02_ID.OracleDbType = OracleDbType.Int32;
                    PUM04_UM02_ID.Value = user.UM02_ID;
                    PUM04_UM02_ID.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_UM02_ID);

                    OracleParameter PUM04_UM03_ID = new OracleParameter();
                    PUM04_UM03_ID.ParameterName = "PUM04_UM03_ID";
                    PUM04_UM03_ID.OracleDbType = OracleDbType.Int32;
                    PUM04_UM03_ID.Value = user.UM04_UM03_ID;
                    PUM04_UM03_ID.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_UM03_ID);

                    OracleParameter PUM04_STATUS = new OracleParameter();
                    PUM04_STATUS.ParameterName = "PUM04_STATUS";
                    PUM04_STATUS.OracleDbType = OracleDbType.Int32;
                    PUM04_STATUS.Value = 1;
                    PUM04_STATUS.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_STATUS);

                    OracleParameter PUM04_IP = new OracleParameter();
                    PUM04_IP.ParameterName = "PUM04_IP";
                    PUM04_IP.OracleDbType = OracleDbType.Varchar2;
                    PUM04_IP.Value = user.UM02_IP;
                    PUM04_IP.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_IP);

                    OracleParameter PUM04_Edited_by = new OracleParameter();
                    PUM04_Edited_by.ParameterName = "PUM04_Edited_by";
                    PUM04_Edited_by.OracleDbType = OracleDbType.Int32;
                    PUM04_Edited_by.Value = user.UM02_CREATED_BY;
                    PUM04_Edited_by.Direction = System.Data.ParameterDirection.Input;
                    parameters.Add(PUM04_Edited_by);

                    await _dbManager.ExecuteStoredProcedureAsync("Pkg_UM04_USERS_ROLES.Edit_Data", parameters);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<UserEntitlementRecord>> GetUserEntitlementsAsync(long userId, CancellationToken cancellationToken)
        {
            var parameters = new List<OracleParameter>
                    {
                        new("Pview", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        },
                        new("p_user_Id", OracleDbType.Decimal)
                        {
                            Direction = ParameterDirection.Input,
                            Value = userId
                        }
                    };

            var entitlements = await _dbManager.GetStoredProcedureRefCursorAsync<UserEntitlementRecord>(
                "Get_User_Entitlments",
                parameters,
                "Pview",
                cancellationToken);

            return entitlements;
        }
    }
}
