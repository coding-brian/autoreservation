using Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class CoachRepository : ICoachRepository
    {
        private string _connectString = "Server=ec2-35-175-17-88.compute-1.amazonaws.com;Port=5432;Username=rahavmxtnzzjeu;Password=75c3a0f809d9c9ba1d3d666519d66b23854daaa88f25400c84de1ddd1bd4471a;Database=d39cq48n58q1vi;SslMode=Require;TrustServerCertificate=True;";

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
                            name varchar(200) NOT NULL,
                            imageUrl text NOT NULL,
                            createdate bigint NOT NULL
                            )
                            ";

                    using (var command = new NpgsqlCommand(sql, conn))
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

                var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                foreach (var coach in coaches)
                {

                    var valueString = "(" + "'" + coach.Name + "'" + "," + "'" + coach.ImageUrl + "'" + "," + time.ToString() + ")";
                    values.Add(valueString);
                }

                string sql = @$"INSERT INTO Coach(name,imageUrl,createdate) VALUES {string.Join(",", values)}";

                using (var command = new NpgsqlCommand(sql, conn))
                {

                    result = await command.ExecuteNonQueryAsync();
                }
                conn.Close();
            }

            return result > 0;
        }

        public async Task<bool> UpdateDate(List<CoachDTO> coaches)
        {
            int result = default;

            using (var conn = new NpgsqlConnection(_connectString))
            {
                conn.Open();

                foreach (var coach in coaches)
                {
                    string sql = @$"UPDATE COACH set name='{coach.Name}',ImageUrl='{coach.ImageUrl}'  where id={coach.id}";

                    using (var command = new NpgsqlCommand(sql, conn))
                    {
                        result = await command.ExecuteNonQueryAsync();
                    }
                }
                conn.Close();
            }

            return result > 0;
        }

        public async Task<List<CoachDTO>> SelectCoaches()
        {
            string sql = @$"SELECT * FROM Coach ";

            var result = new List<CoachDTO>();

            using (var conn = new NpgsqlConnection(_connectString))
            {
                conn.Open();

                using (var command = new NpgsqlCommand(sql, conn))
                {
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        var coachDTO = new CoachDTO();
                        coachDTO.id = reader.GetInt32(0);
                        coachDTO.Name = reader.GetString(1);
                        coachDTO.ImageUrl = reader.GetString(2);
                        result.Add(coachDTO);
                    }

                }
                conn.Close();
            }

            return result;
        }
    }
}
