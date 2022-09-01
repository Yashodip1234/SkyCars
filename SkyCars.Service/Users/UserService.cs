using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SkyCars.Core;
using SkyCars.Core.DomainEntity.User;
using SkyCars.Core.DomainEntity.Grid;
using SkyCars.Data;
using SkyCars.Services.Users;

namespace SkyCars.Services.Users
{
    public partial class UserService : IUserService
    {
        #region Fields

        private readonly IRepository<User> _UserRepository;

        #endregion

        #region Ctor

        public UserService(IRepository<User> UserRepository)
        {
            _UserRepository = UserRepository;
        }

        #endregion

        #region Methods
        public virtual async Task<IPagedList<User>> GetAllAsync(GridRequestModel objGrid)
        {
            var query = from d in _UserRepository.Table
                        where !d.IsDelete
                        select new User()
                        {
                            Id = d.Id,
                            UserName = d.UserName,
                            FirstName = d.FirstName,
                            LastName = d.LastName,
                            Email = d.Email,
                            ContactNumber = d.ContactNumber,
                            CreatedDate = d.CreatedDate
                        };

            return await _UserRepository.GetAllPagedAsync(objGrid, query);

            //return await _UserRepository.GetAllPagedAsync(objGrid);
        }

        public virtual async Task<User> GetByIdAsync(int Id)
        {
            return await _UserRepository.GetByIdAsync(Id);
        }
        public virtual async Task<IList<User>> GetByIdsAsync(IList<int> ids)
        {
            return await _UserRepository.GetByIdsAsync(ids);
        }

        public virtual async Task InsertAsync(User User, int UserId, string Username)
        {
            User.CreatedDate = System.DateTime.Now;
            await _UserRepository.InsertAsync(User);
        }
        public virtual async Task UpdateAsync(User User, int UserId, string Username)
        {
            User.UpdatedDate = System.DateTime.Now;
            await _UserRepository.UpdateAsync(User);
        }

        public virtual async Task UpdateAsync(IList<User> UserList, int UserId, string Username)
        {
            if (UserList.Count == 0)
                throw new ArgumentNullException(nameof(UserList));

            await _UserRepository.UpdateAsync(UserList);
        }

        public virtual async Task<bool> IsNameExist(string UserName, int Id)
        {
            var result = await _UserRepository.GetAllAsync(query =>
            {
                return query.Where(x => x.UserName == UserName && x.Id != Id);
            });
            return result.Count > 0;
        }
        #endregion
    }
}