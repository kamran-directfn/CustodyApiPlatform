using Asp.Versioning;
using Directfn.Custody.Api.Requests.Auth;
using Directfn.Custody.Api.Requests.User;
using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Authentication.TokenStore;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.DTOs.User;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Passwords;
using Directfn.Custody.ApiFramework.Repositories.User;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;

namespace Directfn.Custody.Api.Controllers
{
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    public sealed class UserController : CustodyControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthOptions _authOptions;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ITokenFingerprintService _tokenFingerprintService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IAuthTokenStore _authTokenStore;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILegacyPasswordService _legacyPasswordService;
        public UserController(IUserRepository userRepository, IJwtTokenService jwtTokenService, ITokenFingerprintService tokenFingerprintService, IRefreshTokenService refreshTokenService, IAuthTokenStore authTokenStore, ICurrentUserService currentUserService, ILegacyPasswordService legacyPasswordService, IOptions<AuthOptions> authOptions)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _tokenFingerprintService = tokenFingerprintService;
            _refreshTokenService = refreshTokenService;
            _authTokenStore = authTokenStore;
            _currentUserService = currentUserService;
            _legacyPasswordService = legacyPasswordService;
            _authOptions = authOptions.Value;
        }

       // [Authorize]
        [HttpPost("get-user")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<UserViewModel> data = await _userRepository.GetAllUserAsync(cancellationToken);

            return Success(data);
        }

        // [Authorize]
        [HttpPost("get-user-by-id")]
        public async Task<IActionResult> GetById(int UserId, CancellationToken cancellationToken)
        {
            UserViewModel data = await _userRepository.GetUserByIDAsync(UserId, cancellationToken);

            return Success(data);
        }

        // [Authorize]
        [HttpPost("verify-user-name")]
        public async Task<IActionResult> VerifyUserName(string userName, CancellationToken cancellationToken)
        {
             await _userRepository.VerifyUserNameAsync(userName, cancellationToken);

            var data = 1; 
            return Success(data);
        }

        [HttpPost("load-controls")]
        public async Task<IActionResult> LoadControls(CancellationToken cancellationToken)
        {
            //await _userRepository.VerifyUserNameAsync(userName, cancellationToken);

            var data = 1;
            return Success(data);
        }

        // [Authorize]
        [HttpPost("save-user")]
        public async Task<IActionResult> Add(UserRequest user)
        {
            UserViewModel data = new UserViewModel();
           // int userId = Int32.Parse(_currentUserService.UserId);
          //  string userName = _currentUserService.UserName.ToString();
            if (data.UM02_SUPERVISOR == 0)
            {
                data.UM02_SUPERVISOR = -1;
            }

            data.UM02_PASSWORD = _legacyPasswordService.EncryptLegacyPassword("default123");                //Convert.ToString(ConfigurationManager.AppSettings["Default_Password"]);
            data.UM02_FIRST_LOGIN = 1;

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
            // data.UM02_CREATED_BY = userId;

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

            await _userRepository.SaveUserAsync(data);

            return Success(new
            {
                LoggedOut = true
            });
        }

        [HttpPost("update-user")]
        public async Task<IActionResult> Update(UserRequest user)
        {
            UserViewModel data = new UserViewModel();
            // int userId = Int32.Parse(_currentUserService.UserId);
            //  string userName = _currentUserService.UserName.ToString();
            if (data.UM02_SUPERVISOR == 0)
            {
                data.UM02_SUPERVISOR = -1;
            }

            data.UM02_PASSWORD = "default123";//Convert.ToString(ConfigurationManager.AppSettings["Default_Password"]);
            data.UM02_FIRST_LOGIN = 1;

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
            // data.UM02_CREATED_BY = userId;
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

            await _userRepository.SaveUserAsync(data);

            return Success(new
            {
                LoggedOut = true
            });
        }

        [HttpPost("post-user")]
        public async Task<IActionResult> Post(int user_id, CancellationToken cancellationToken)
        {
            await _userRepository.UpdatePostStatus(user_id, cancellationToken);

            var data = 1;
            return Success(data);
        }

        [HttpPost("unpost-user")]
        public async Task<IActionResult> UnPost(int user_id, CancellationToken cancellationToken)
        {
            await _userRepository.UpdatePostStatus(user_id, cancellationToken);

            var data = 1;
            return Success(data);
        }

        [HttpPost("delete-user")]
        public async Task<IActionResult> Delete(int user_id, CancellationToken cancellationToken)
        {
            await _userRepository.Delete(user_id, cancellationToken);

            var data = 1;
            return Success(data);
        }
    }
}
