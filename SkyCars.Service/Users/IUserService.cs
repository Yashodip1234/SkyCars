using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkyCars.Core;
using SkyCars.Core.DomainEntity.User;
using SkyCars.Core.DomainEntity.Grid;

namespace SkyCars.Services.Users
{
    /// <summary>
    /// System Setting service interface
    /// </summary>
    public partial interface IUserService
    {
        #region Methods
        Task<IPagedList<User>> GetAllAsync(GridRequestModel objGrid);
        Task<IList<User>> GetByIdsAsync(IList<int> ids);
        Task<User> GetByIdAsync(int Id);
        Task InsertAsync(User User, int UserId, string Username);
        Task UpdateAsync(User User, int UserId, string Username);
        Task UpdateAsync(IList<User> UserList, int UserId, string Username);
        Task<bool> IsNameExist(string UserName, int Id);
        #endregion
    }
}