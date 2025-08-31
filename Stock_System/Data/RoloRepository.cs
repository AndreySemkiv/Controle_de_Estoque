using Dapper;
using EstoqueRolos.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstoqueRolos.Data
{
    public class RoloRepository
    {
        private readonly string _connStr;

        public RoloRepository()
        {
            _connStr = App.Configuration.GetConnectionString("MySqlConn")!;
        }

        private MySqlConnection GetConn() => new MySqlConnection(_connStr);

        public async Task<IEnumerable<Rolo>> GetAllAsync()
        {
            using var con = GetConn();
            return await con.QueryAsync<Rolo>("SELECT Id, IdRolo, Milimetragem, MetragemDisponivel, WIP FROM rolos ORDER BY Id DESC");
        }

        public async Task<int> InsertAsync(Rolo r)
        {
            using var con = GetConn();
            var sql = @"INSERT INTO rolos (IdRolo, Milimetragem, MetragemDisponivel, WIP)
                        VALUES (@IdRolo, @Milimetragem, @MetragemDisponivel, @WIP);
                        SELECT LAST_INSERT_ID();";
            return await con.ExecuteScalarAsync<int>(sql, r);
        }

        public async Task<int> UpdateAsync(Rolo r)
        {
            using var con = GetConn();
            var sql = @"UPDATE rolos SET
                        IdRolo=@IdRolo,
                        Milimetragem=@Milimetragem,
                        MetragemDisponivel=@MetragemDisponivel,
                        WIP=@WIP
                        WHERE Id=@Id";
            return await con.ExecuteAsync(sql, r);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var con = GetConn();
            return await con.ExecuteAsync("DELETE FROM rolos WHERE Id=@id", new { id });
        }
    }
}
