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
            var sql = @"SELECT 
                            Code, 
                            Descricao, 
                            Milimetragem, 
                            MOQ, 
                            Estoque, 
                            MetragemWIP 
                        FROM rolos 
                        ORDER BY Code";
            return await con.QueryAsync<Rolo>(sql);
        }

        public async Task<int> InsertAsync(Rolo r)
        {
            using var con = GetConn();
            var sql = @"INSERT INTO rolos 
                            (Code, Descricao, Milimetragem, MOQ, Estoque, MetragemWIP)
                        VALUES 
                            (@Code, @Descricao, @Milimetragem, @MOQ, @Estoque, @MetragemWIP)";
            return await con.ExecuteAsync(sql, r);
        }

        public async Task<int> UpdateAsync(Rolo r)
        {
            using var con = GetConn();
            var sql = @"UPDATE rolos SET
                            Descricao = @Descricao,
                            Milimetragem = @Milimetragem,
                            MOQ = @MOQ,
                            Estoque = @Estoque,
                            MetragemWIP = @MetragemWIP
                        WHERE Code = @Code";
            return await con.ExecuteAsync(sql, r);
        }

        public async Task<int> DeleteAsync(string code)
        {
            using var con = GetConn();
            var sql = "DELETE FROM rolos WHERE Code = @code";
            return await con.ExecuteAsync(sql, new { code });
        }
    }
}
