using Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class CoachRepository : ICoachRepository
    {
        private string _connectString = "Host=ec2-35-175-17-88.compute-1.amazonaws.com;Port=5432;Username=rahavmxtnzzjeu;Password=75c3a0f809d9c9ba1d3d666519d66b23854daaa88f25400c84de1ddd1bd4471a;Database=d39cq48n58q1vi;SslMode=Require;";

        public async Task CreateTable()
        {
            try 
            {
                using (var conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();

                    string sql = @"
                            create table Coach(
                            id SERIAL PRIMARY KEY,
                            name nvarchar(200) NOT NULL,
                            imageUrl text NOT NULL,
                            createdate bigint NOT NULL
                            )
                            ";

                    using (var command = new NpgsqlCommand(sql))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    conn.Close();
                }
            } 
            catch (Exception e) 
            {
                throw e;
            }
        }

        public async Task<bool> InsertCoachData(List<CoachDTO> coaches)
        {
            int result = default;

            using (var conn = new NpgsqlConnection(_connectString))
            {
                conn.Open();

                List<string> values = new List<string>();

                var time = new DateTimeOffset().ToUnixTimeMilliseconds();

                foreach (var coach in coaches)
                {

                    var valueString = "(" + coach.Name + "," + coach.ImageUrl + "," + time.ToString() + ")";
                    values.Add(valueString);
                }

                string sql = @$"INSERT INTO Coach(`name`,`imageUrl`,`createdate`) VALUES {string.Join(",", values)}";

                using (var command = new NpgsqlCommand(sql))
                {
                    result=await command.ExecuteNonQueryAsync();
                }
                conn.Close();
            }

            return result > 0;
        }
    }
}
