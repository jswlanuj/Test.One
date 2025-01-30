using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Test.One.Data;
using Test.One.Models;

namespace Test.One.Services
{
    public class UsersServices
    {
        private readonly ApplicationDbContext _dbContext;

        public UsersServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Users>> GetUsersList()
        {
            try
            {
                var query = $"CALL user_getUserDetails()";
                var data = await _dbContext.Users.FromSqlRaw(query).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                return new List<Users>();
            }
        }

        public async Task<int> AddUser(Users objUser, int userId)
        {
            try
            {
                var parameter = new List<MySqlParameter>();

                parameter.Add(new MySqlParameter("@username", objUser.Username));
                parameter.Add(new MySqlParameter("@passwordHash", objUser.PasswordHash));
                parameter.Add(new MySqlParameter("@email", objUser.Email));
                parameter.Add(new MySqlParameter("@firstName", objUser.FirstName));
                parameter.Add(new MySqlParameter("@lastName", objUser.LastName));
                parameter.Add(new MySqlParameter("@createdBy", userId));
                parameter.Add(new MySqlParameter("@isActive", objUser.IsActive));

                var query = $"CALL user_insertUser(@username, @passwordHash, @email, @firstName, @lastName, @createdBy, @isActive)";

                int result = await _dbContext.Database.ExecuteSqlRawAsync(query, parameter.ToArray());

                if (result == 0)
                {
                    throw new Exception("Data not inserted");
                }

                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> UpdateUser(Users objUser, int userId)
        {
            try
            {
                var parameter = new List<MySqlParameter>();

                parameter.Add(new MySqlParameter("@p_userId", objUser.UserId));
                parameter.Add(new MySqlParameter("@username", objUser.Username));
                parameter.Add(new MySqlParameter("@email", objUser.Email));
                parameter.Add(new MySqlParameter("@firstName", objUser.FirstName));
                parameter.Add(new MySqlParameter("@lastName", objUser.LastName));
                parameter.Add(new MySqlParameter("@isActive", objUser.IsActive));

                var query = $"CALL user_updateUsers(@p_userId,@username, @email, @firstName, @lastName, @isActive)";

                int result = await _dbContext.Database.ExecuteSqlRawAsync(query, parameter.ToArray());

                if (result == 0)
                {
                    throw new Exception("Data not updated");
                }

                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> DeleteUser(int userId)
        {
            try
            {
                var parameter = new List<MySqlParameter>();

                parameter.Add(new MySqlParameter("@p_id", userId));

                var query = $"CALL user_deleteUser(@p_id)";

                int result = await _dbContext.Database.ExecuteSqlRawAsync(query, parameter.ToArray());

                if (result == 0)
                {
                    throw new Exception("Data not deleted");
                }

                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> VerifyUser(int userId)
        {
            try
            {
                var parameter = new List<MySqlParameter>();

                parameter.Add(new MySqlParameter("@p_id", userId));

                var query = $"CALL user_verifyUser(@p_id)";

                int result = await _dbContext.Database.ExecuteSqlRawAsync(query, parameter.ToArray());

                if (result == 0)
                {
                    throw new Exception("Data not verified");
                }

                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

    }
}
