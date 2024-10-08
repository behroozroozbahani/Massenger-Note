﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PortalCore.Application.Common.Identity;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Common.Token;
using PortalCore.Common.Extensions;
using PortalCore.Domain.Entities.Identity;
using PortalCore.Persistence.Context;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PortalCore.Persistence.Services.Identity
{
    public class ApplicationUserManager :
        UserManager<User>,
        IApplicationUserManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IApplicationDbContext _uow;
        private readonly IUsedPasswordsService _usedPasswordsService;
        private readonly IdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationUserManager> _logger;
        private readonly IOptions<IdentityOptions> _optionsAccessor;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IServiceProvider _services;
        private readonly DbSet<User> _users;
        private readonly DbSet<Role> _roles;
        private readonly IApplicationUserStore _userStore;
        private readonly IEnumerable<IUserValidator<User>> _userValidators;
        private User _currentUserInScope;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mapperConfiguration;

        public ApplicationUserManager(
            IApplicationUserStore store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<ApplicationUserManager> logger,
            IHttpContextAccessor contextAccessor,
            IApplicationDbContext uow,
            IUsedPasswordsService usedPasswordsService,
            IMapper mapper)
            : base(
                (UserStore<User, Role, ApplicationDbContext, Guid, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>)store,
                  optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userStore = store ?? throw new ArgumentNullException(nameof(_userStore));
            _optionsAccessor = optionsAccessor ?? throw new ArgumentNullException(nameof(_optionsAccessor));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(_passwordHasher));
            _userValidators = userValidators ?? throw new ArgumentNullException(nameof(_userValidators));
            _passwordValidators = passwordValidators ?? throw new ArgumentNullException(nameof(_passwordValidators));
            _keyNormalizer = keyNormalizer ?? throw new ArgumentNullException(nameof(_keyNormalizer));
            _errors = errors ?? throw new ArgumentNullException(nameof(_errors));
            _services = services ?? throw new ArgumentNullException(nameof(_services));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(_contextAccessor));
            _uow = uow ?? throw new ArgumentNullException(nameof(_uow));
            _usedPasswordsService = usedPasswordsService ?? throw new ArgumentNullException(nameof(_usedPasswordsService));
            _mapper = mapper;
            _mapperConfiguration = mapper.ConfigurationProvider;
            _users = uow.Set<User>();
            _roles = uow.Set<Role>();
        }

        #region BaseClass

        string IApplicationUserManager.CreateTwoFactorRecoveryCode()
        {
            return base.CreateTwoFactorRecoveryCode();
        }

        Task<PasswordVerificationResult> IApplicationUserManager.VerifyPasswordAsync(IUserPasswordStore<User> store, User user, string password)
        {
            return base.VerifyPasswordAsync(store, user, password);
        }

        public override async Task<IdentityResult> CreateAsync(User user)
        {
            var result = await base.CreateAsync(user);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            return result;
        }

        public override async Task<IdentityResult> CreateAsync(User user, string password)
        {
            var result = await base.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            return result;
        }

        public override async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            //user.SerialNumber = Guid.NewGuid().ToString("N");// To force other logins to expire.
            var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            return result;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            //user.SerialNumber = Guid.NewGuid().ToString("N");// To force other logins to expire.
            var result = await base.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            return result;
        }

        #endregion

        #region CustomMethods

        public User FindById(Guid userId)
        {
            return _users.Find(userId);
        }

        public Task<User> FindByIdIncludeUserRolesAsync(Guid userId)
        {
            return _users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == userId);
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return Users.ToListAsync();
        }

        public User GetCurrentUser()
        {
            if (_currentUserInScope != null)
            {
                return _currentUserInScope;
            }

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return null;
            }

            var userId = Guid.Parse(currentUserId);
            return _currentUserInScope = FindById(userId);
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return _currentUserInScope ??
                (_currentUserInScope = await GetUserAsync(_contextAccessor.HttpContext.User));
        }

        public string GetCurrentUserId()
        {
            return _contextAccessor.HttpContext.User.Identity.GetUserId();
        }

        public Guid? CurrentUserId
        {
            get
            {
                var userId = _contextAccessor.HttpContext.User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                return !Guid.TryParse(userId, out Guid result) ? (Guid?)null : result;
            }
        }

        IPasswordHasher<User> IApplicationUserManager.PasswordHasher { get => base.PasswordHasher; set => base.PasswordHasher = value; }

        IList<IUserValidator<User>> IApplicationUserManager.UserValidators => base.UserValidators;

        IList<IPasswordValidator<User>> IApplicationUserManager.PasswordValidators => base.PasswordValidators;

        IQueryable<User> IApplicationUserManager.Users => base.Users;

        public string GetCurrentUserName()
        {
            return _contextAccessor.HttpContext.User.Identity.GetUserName();
        }

        public async Task<bool> HasPasswordAsync(Guid userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            return user?.PasswordHash != null;
        }

        public async Task<bool> HasPhoneNumberAsync(Guid userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            return user?.PhoneNumber != null;
        }

        //public async Task<byte[]> GetEmailImageAsync(Guid? userId)
        //{
        //    if (userId == null)
        //        return "?".TextToImage(new TextToImageOptions());

        //    var user = await FindByIdAsync(userId.Value.ToString());
        //    if (user == null)
        //        return "?".TextToImage(new TextToImageOptions());

        //    if (!user.IsEmailPublic)
        //        return "?".TextToImage(new TextToImageOptions());

        //    return user.Email.TextToImage(new TextToImageOptions());
        //}

        //public async Task<PagedUsersListViewModel> GetPagedUsersListAsync(SearchUsersViewModel model, int pageNumber)
        //{
        //    var skipRecords = pageNumber * model.MaxNumberOfRows;
        //    var query = _users.Include(x => x.Roles).AsNoTracking();

        //    if (!model.ShowAllUsers)
        //    {
        //        query = query.Where(x => x.IsActive == model.UserIsActive);
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.TextToFind))
        //    {
        //        model.TextToFind = model.TextToFind.ApplyCorrectYeKe();

        //        if (model.IsPartOfEmail)
        //        {
        //            query = query.Where(x => x.Email.Contains(model.TextToFind));
        //        }

        //        if (model.IsUserId)
        //        {
        //            if (Guid.TryParse(model.TextToFind, out Guid userId))
        //            {
        //                query = query.Where(x => x.Id == userId);
        //            }
        //        }

        //        if (model.IsPartOfName)
        //        {
        //            query = query.Where(x => x.FirstName.Contains(model.TextToFind));
        //        }

        //        if (model.IsPartOfLastName)
        //        {
        //            query = query.Where(x => x.LastName.Contains(model.TextToFind));
        //        }

        //        if (model.IsPartOfUserName)
        //        {
        //            query = query.Where(x => x.UserName.Contains(model.TextToFind));
        //        }

        //        if (model.IsPartOfLocation)
        //        {
        //            query = query.Where(x => x.Location.Contains(model.TextToFind));
        //        }
        //    }

        //    if (model.HasEmailConfirmed)
        //    {
        //        query = query.Where(x => x.EmailConfirmed);
        //    }

        //    if (model.UserIsLockedOut)
        //    {
        //        query = query.Where(x => x.LockoutEnd != null);
        //    }

        //    if (model.HasTwoFactorEnabled)
        //    {
        //        query = query.Where(x => x.TwoFactorEnabled);
        //    }

        //    query = query.OrderBy(x => x.Id);
        //    return new PagedUsersListViewModel
        //    {
        //        Paging =
        //        {
        //            TotalItems = await query.CountAsync()
        //        },
        //        Users = await query.Skip(skipRecords).Take(model.MaxNumberOfRows).ToListAsync(),
        //        Roles = await _roles.ToListAsync()
        //    };
        //}

        //public async Task<PagedUsersListViewModel> GetPagedUsersListAsync(
        //    int pageNumber, int recordsPerPage,
        //    string sortByField, SortOrder sortOrder,
        //    bool showAllUsers)
        //{
        //    var skipRecords = pageNumber * recordsPerPage;
        //    var query = _users.Include(x => x.Roles).AsNoTracking();

        //    if (!showAllUsers)
        //    {
        //        query = query.Where(x => x.IsActive);
        //    }

        //    switch (sortByField)
        //    {
        //        case nameof(User.Id):
        //            query = sortOrder == SortOrder.Descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
        //            break;
        //        default:
        //            query = sortOrder == SortOrder.Descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
        //            break;
        //    }

        //    return new PagedUsersListViewModel
        //    {
        //        Paging =
        //        {
        //            TotalItems = await query.CountAsync()
        //        },
        //        Users = await query.Skip(skipRecords).Take(recordsPerPage).ToListAsync(),
        //        Roles = await _roles.ToListAsync()
        //    };
        //}

        public async Task<IdentityResult> UpdateUserAndSecurityStampAsync(Guid userId, Action<User> action)
        {
            var user = await FindByIdIncludeUserRolesAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "کاربر مورد نظر یافت نشد."
                });
            }

            action(user);

            var result = await UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            return await UpdateSecurityStampAsync(user);
        }

        public async Task<IdentityResult> AddOrUpdateUserRolesAsync(Guid userId, IList<Guid> selectedRoleIds, Action<User> action = null)
        {
            var user = await FindByIdIncludeUserRolesAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "کاربر مورد نظر یافت نشد."
                });
            }

            var currentUserRoleIds = user.Roles.Select(x => x.RoleId).ToList();

            if (selectedRoleIds == null)
            {
                selectedRoleIds = new List<Guid>();
            }

            var newRolesToAdd = selectedRoleIds.Except(currentUserRoleIds).ToList();
            foreach (var roleId in newRolesToAdd)
            {
                user.Roles.Add(new UserRole { RoleId = roleId, UserId = user.Id });
            }

            var removedRoles = currentUserRoleIds.Except(selectedRoleIds).ToList();
            foreach (var roleId in removedRoles)
            {
                var userRole = user.Roles.SingleOrDefault(ur => ur.RoleId == roleId);
                if (userRole != null)
                {
                    user.Roles.Remove(userRole);
                }
            }

            user.SerialNumber = Guid.NewGuid().ToString("N");

            action?.Invoke(user);

            var result = await UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            return await UpdateSecurityStampAsync(user);
        }

        Task<IdentityResult> IApplicationUserManager.UpdatePasswordHash(User user, string newPassword, bool validatePassword)
        {
            return base.UpdatePasswordHash(user, newPassword, validatePassword);
        }

        #endregion

        #region Jwt

        public async Task<string> GetSerialNumberAsync(Guid userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            return user.SerialNumber;
        }

        public async Task UpdateUserLastActivityDateAsync(Guid userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            if (user.LastLoggedIn != null)
            {
                var updateLastActivityDate = TimeSpan.FromMinutes(2);
                var currentUtc = DateTimeOffset.UtcNow;
                var timeElapsed = currentUtc.Subtract(user.LastLoggedIn.Value);
                if (timeElapsed < updateLastActivityDate)
                {
                    return;
                }
            }
            user.LastLoggedIn = DateTimeOffset.UtcNow;
            await _uow.SaveChangesAsync();
        }

        #endregion

        public async Task<JwtUserInfo?> GetJwtUserInfoAsync(string mobileNumber)
        {
            return await _users
                .Where(p => p.UserName == mobileNumber)
                .ProjectTo<JwtUserInfo?>(_mapperConfiguration)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IdentityResult> CreateByMobileNumberAsync(string mobileNumber)
        {
            mobileNumber = mobileNumber.FixMobileNumber();

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = mobileNumber,
                MobileNumber = mobileNumber,
                PhoneNumber = mobileNumber,
                PhoneNumberConfirmed = true,
                SerialNumber = Guid.NewGuid().ToString("N"),
            };

            var result = await base.CreateAsync(user, mobileNumber);
            return result;
        }
    }
}