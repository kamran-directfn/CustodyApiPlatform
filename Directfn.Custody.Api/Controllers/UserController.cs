using Asp.Versioning;
using Directfn.Custody.Api.Requests.Auth;
using Directfn.Custody.Api.Requests.User;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Authentication.TokenStore;
using Directfn.Custody.ApiFramework.Common.DTOs;
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
    [Authorize]
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

        
        [AuditAction("GET_USER")]
        [HttpPost("get-user")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<UserViewModel> data = await _userRepository.GetAllUserAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_USER_BY_ID")]
        [HttpPost("get-user-by-id")]
        public async Task<IActionResult> GetById(int UserId, CancellationToken cancellationToken)
        {
            UserViewModel data = await _userRepository.GetUserByIDAsync(UserId, cancellationToken);

            return Success(data);
        }

      
        [AuditAction("VERIFY_USER_NAME")]
        [HttpPost("verify-user-name")]
        public async Task<IActionResult> VerifyUserName(string userName, CancellationToken cancellationToken)
        {
            var data = await _userRepository.VerifyUserNameAsync(userName, cancellationToken);
            
            return Success(data);
        }

        [AuditAction("SAVE_USER")]
        [HttpPost("save-user")]
        public async Task<IActionResult> Add(UserRequestModel user)
        {
            int userId = 1;//Int32.Parse(_currentUserService.UserId);
            string userName = "";//_currentUserService.UserName.ToString();
                        
            user.UM02_ID = await _userRepository.SaveUserAsync(user);
            //new_user_id > 0
            
            return Success(user);
        }

        [AuditAction("UPDATE_USER")]
        [HttpPost("update-user")]
        public async Task<IActionResult> Update(UserRequestModel user)
        {
            await _userRepository.UpdateUser(user);

            return Success(user);
        }

        [AuditAction("POST_USER")]
        [HttpPost("post-user")]
        public async Task<IActionResult> Post(int um02_id, int isPosted, CancellationToken cancellationToken)
        {
            int user_id = 1;
            var data = await _userRepository.UpdatePostStatus( um02_id, isPosted, user_id, cancellationToken);
         
            return Success(data);
        }

        [AuditAction("UNPOST_USER")]
        [HttpPost("unpost-user")]
        public async Task<IActionResult> UnPost(int um02_id, int isPosted, CancellationToken cancellationToken)
        {
            int user_id = 1;
            var data = await _userRepository.UpdatePostStatus(um02_id, isPosted, user_id, cancellationToken);
             
            return Success(data);
        }

        [AuditAction("DELETE_USER")]
        [HttpPost("delete-user")]
        public async Task<IActionResult> Delete(int um02_id, CancellationToken cancellationToken)
        {
             int user_id = 1;
            var data = await _userRepository.Delete(um02_id, user_id, cancellationToken);

           
            return Success(data);
        }
    }
}
