using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using DataLayer.Entities;

namespace DataLayer.Repositories
{
    internal class UserGameRecodeRepository : RepositoryBase, IUserGameRecodeRepository
    {

        public UserGameRecodeRepository(IDbTransaction transaction)
       : base(transaction)
        {
        }
        public void Add(UserGameRecordEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("No Account Entity");


            var sql = String.Format("INSERT INTO lucifer_community.user_game_record" +
                  " (`uId`, `zoneId`, `roomUid`, `regDate`, `updateDate`)" +
                  " VALUES('{0}', '{1}', '{2}', '{3}', '{4}');",
                  entity.uId, entity.zoneId, entity.roomUid, entity.regDate, entity.updateDate);


            entity.uId = Connection.ExecuteScalar<long>(
                 sql,
                transaction: Transaction
            );
        }

        public IEnumerable<UserGameRecordEntity> All()
        {
            return Connection.Query<UserGameRecordEntity>(
         "SELECT * FROM lucifer_community.user_game_record"
         );
        }

        public void Delete(long uId)
        {
            var sql = String.Format("DELETE lucifer_community.user_game_record WHERE uId = '{0}';", uId);

            Connection.Execute(
           sql,
            transaction: Transaction
        );
        }

        public void Delete(UserGameRecordEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("No room Entity");

            Delete(entity.roomUid);
        }

        public UserGameRecordEntity Find(long uId)
        {

            var sql = String.Format("SELECT * FROM lucifer_community.user_game_record WHERE uId = '{0}';", uId);

            return Connection.Query<UserGameRecordEntity>(
                  sql,
                  transaction: Transaction
              ).FirstOrDefault();     
        }


        public void Update(UserGameRecordEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("No UserGameRecordEntity Entity");

            var sql = String.Format("UPDATE lucifer_community.user_game_record SET " +
             " `uId` = {0}, `zoneId`= {1}, `roomUid`= {2}, `regDate`= '{3}', " +
             "`updateDate`= '{4}' WHERE (`uId` = {0});",
             entity.uId, entity.zoneId, entity.roomUid, entity.regDate, entity.updateDate);


            Connection.Execute(
               sql,
                transaction: Transaction
            );
        }


    }
}
