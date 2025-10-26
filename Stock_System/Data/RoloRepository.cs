using Dapper;
using EstoqueRolos.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EstoqueRolos.Data
{
    public class RoloRepository
    {
        private readonly string _connStr;
        private static readonly SemaphoreSlim _dbSemaphore = new(1, 1);

        public RoloRepository()
        {
            _connStr = $"Data Source={App.DatabaseFilePath}";
        }

        private IDbConnection GetConn()
        {
            return new SqliteConnection(_connStr);
        }

        public async Task<IEnumerable<Rolo>> GetAllAsync()
        {
            await _dbSemaphore.WaitAsync();
            try
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
                var result = await con.QueryAsync<Rolo>(sql);
                return result;
            }
            finally
            {
                _dbSemaphore.Release();
            }
        }

        public async Task<int> InsertAsync(Rolo r)
        {
            await _dbSemaphore.WaitAsync();
            try
            {
                using var con = GetConn();
                var sql = @"INSERT INTO rolos 
                                (Code, Descricao, Milimetragem, MOQ, Estoque, MetragemWIP)
                            VALUES 
                                (@Code, @Descricao, @Milimetragem, @MOQ, @Estoque, @MetragemWIP)";
                var res = await con.ExecuteAsync(sql, r);
                return res;
            }
            finally
            {
                _dbSemaphore.Release();
            }
        }

        public async Task<int> UpdateAsync(Rolo r)
        {
            await _dbSemaphore.WaitAsync();
            try
            {
                using var con = GetConn();
                var sql = @"UPDATE rolos SET
                                Descricao = @Descricao,
                                Milimetragem = @Milimetragem,
                                MOQ = @MOQ,
                                Estoque = @Estoque,
                                MetragemWIP = @MetragemWIP
                            WHERE Code = @Code";
                var res = await con.ExecuteAsync(sql, r);
                return res;
            }
            finally
            {
                _dbSemaphore.Release();
            }
        }

        public async Task<int> DeleteAsync(string code)
        {
            await _dbSemaphore.WaitAsync();
            try
            {
                using var con = GetConn();
                var sql = "DELETE FROM rolos WHERE Code = @code";
                var res = await con.ExecuteAsync(sql, new { code });
                return res;
            }
            finally
            {
                _dbSemaphore.Release();
            }
        }
    }
}
