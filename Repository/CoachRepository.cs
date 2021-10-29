using Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task CreateCoaChTimeTable()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();

                    string sql = @"
                                CREATE TABLE CoachTime
                                (
                                  seqno SERIAL PRIMARY KEY,
                                  starttime bigint NOT NULL,
                                  endtime bigint NOT NULL,
                                  coachid int NOT NULL
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

        public async Task CreateUserCoachTimeTable()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();

                    string sql = @"
                                CREATE TABLE UserCoachTime
                                (
                                  seqno SERIAL PRIMARY KEY,
                                  userId varchar(200) NOT NULL,
                                  coachTiemNo int NOT NULL
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
                    string sql = @$"UPDATE COACH set name='{coach.Name}',ImageUrl='{coach.ImageUrl}'  where id={coach.Id}";

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
                        coachDTO.Id = reader.GetInt32(0);
                        coachDTO.Name = reader.GetString(1);
                        coachDTO.ImageUrl = reader.GetString(2);
                        result.Add(coachDTO);
                    }

                }
                conn.Close();
            }

            return result;
        }

        public async Task<List<CoachTimeDTO>> SelectCoachesTime(int id)
        {
            string sql = @$"SELECT * FROM CoachTime where coachid={id} ";

            var result = new List<CoachTimeDTO>();

            using (var conn = new NpgsqlConnection(_connectString))
            {
                conn.Open();

                using (var command = new NpgsqlCommand(sql, conn))
                {
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        var coachTime = new CoachTimeDTO();
                        coachTime.StartTime = reader.GetFieldValue<long>(1);
                        coachTime.EndTime = reader.GetFieldValue<long>(2);
                        coachTime.CoachId = reader.GetFieldValue<long>(3);
                        result.Add(coachTime);
                    }
                }
                conn.Close();
            }

            return result;
        }

        public async Task<int> InsertCoachTime(CoachDTO coach)
        {
            object result = default;

            using (var conn = new NpgsqlConnection(_connectString))
            {
                conn.Open();

                var valueString = "(" + "'" + coach.StartTime.ToUnixTimeMilliseconds() + "'" + "," + "'" + coach.EndTime.ToUnixTimeMilliseconds() + "'" + "," + coach.Id + ")";

                string sql = @$"INSERT INTO coachtime(starttime,endtime,coachid) VALUES {valueString} RETURNING seqno";

                using (var command = new NpgsqlCommand(sql, conn))
                {

                    result = await command.ExecuteScalarAsync();
                }
                conn.Close();
            }

            return Convert.ToInt32(result);
        }

        public async Task<bool> InsertUserCoachTime(UserCoachTimeDTO userCoachTimeDTO)
        {
            int result = default;

            using (var conn = new NpgsqlConnection(_connectString))
            {
                conn.Open();

                var valueString = "(" + "'" + userCoachTimeDTO.userId + "'" + "," + "'" + userCoachTimeDTO.coachTiemNo + "'" + ")";

                string sql = @$"INSERT INTO UserCoachTime(userId,coachTiemNo) VALUES {valueString}";

                using (var command = new NpgsqlCommand(sql, conn))
                {

                    result = await command.ExecuteNonQueryAsync();
                }
                conn.Close();
            }

            return result > 0;
        }

        public async Task<List<CoachUserTimeDTO>> SelectUserCoachTime(string userId)
        {
            try
            {
                string sql = @$"Select co.name,ct.starttime,ct.endtime from  UserCoachTime uc
                            join CoachTime ct
                            on uc.coachTiemNo=ct.seqno
                            join Coach co 
                            on ct.coachid=co.id
                            where uc.userId='{userId}'";

                var result = new List<CoachUserTimeDTO>();

                using (var conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();

                    using (var command = new NpgsqlCommand(sql, conn))
                    {
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            var coachUserTime = new CoachUserTimeDTO();
                            coachUserTime.Name = reader.GetFieldValue<string>(0);
                            coachUserTime.StartTime = reader.GetFieldValue<long>(1);
                            coachUserTime.EndTime = reader.GetFieldValue<long>(2);
                            result.Add(coachUserTime);
                        }
                    }
                    conn.Close();
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> DeleteUserCoachTime(string userId, int coachId)
        {
            int result = default;
            try
            {
                using (var conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();

                    string sql = @$"
with aaa AS(select coachtiemno from usercoachtime where userid='{userId}' and coachtiemno in (select seqno FROM coachtime where coachid='{coachId}'))

delete from coachtime where seqno in (select coachtiemno from aaa);

delete from usercoachtime WHERE  userid = '{userId}' and coachtiemno in (select seqno FROM coachtime where coachid='{coachId}');


";

                    using (var command = new NpgsqlCommand(sql, conn))
                    {

                        result = await command.ExecuteNonQueryAsync();
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return result > 0;
        }
    }
}
