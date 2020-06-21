using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using DataLayer.Entities;

namespace DataLayer.Repositories
{
    internal class UserRepository : RepositoryBase, IUserRepository
    {

        public UserRepository(IDbTransaction transaction)
       : base(transaction)
        {
        }
        public void Add(UserEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("No Account Entity");


            var sql = String.Format("INSERT INTO lucifer_community.user" +
                " (`uId`, `userName`, `characterId`, `itemId`, `userLevel`, `exp`, `chao`, `isVIP`, `gold`, `cash`, `deckSkin`,'lastLogout','regDate','updateDate',)" +
                " VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}','{11}','{12}','{13}');",
                entity.uId, entity.userName, entity.characterId, entity.itemId, entity.userLevel,
                entity.exp, entity.chao, entity.isVIP, entity.gold, entity.cash, entity.deckSkin, entity.lastLogout, entity.regDate, entity.updateDate, entity.playingState);

            entity.uId = Connection.ExecuteScalar<long>(
                 sql,
                transaction: Transaction
            );
        }

        public IEnumerable<UserEntity> All()
        {
            return Connection.Query<UserEntity>(
         "SELECT * FROM lucifer_community.user"
         );
        }

        public void Delete(long uId)
        {
            var sql = String.Format("DELETE lucifer_community.user WHERE uId = '{0}';", uId);


            Connection.Execute(
           sql,
            transaction: Transaction
        );
        }

        public void Delete(UserEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Delete(entity.uId);
        }

        public UserEntity Find(long uId)
        {

            var sql = String.Format("SELECT * FROM lucifer_community.user WHERE uId = '{0}';", uId);
;
            return Connection.Query<UserEntity>(
                  sql,
                  transaction: Transaction
              ).FirstOrDefault();     
        }

        public UserEntity FindByName(string name)
        {
            var sql = String.Format("SELECT * FROM lucifer_community.user WHERE userName = '{0}';", name);
            ;
            return Connection.Query<UserEntity>(
                  sql,
                  transaction: Transaction
              ).FirstOrDefault();
        }

        public void Update(UserEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("No Account Entity");

            var sql = String.Format("UPDATE lucifer_community.user SET " +
               " `uId` = {0}, `userName`= '{1}', `characterId`= {2}, `itemId`= {3}, " +
               "`userLevel`= {4}, `exp`= {5}, `chao`= {6}, `isVIP` = {7}, `gold` = {8}, `cash` = {9}, `deckSkin` = {10}, `lastLogout` = '{11}', `regDate` = '{12}', `updateDate` = '{13}', `playingState`  = '{14}' " +
               "WHERE (`uId` = {0})",
               entity.uId, entity.userName, entity.characterId, entity.itemId, entity.userLevel,
               entity.exp, entity.chao, entity.isVIP, entity.gold, entity.cash, entity.deckSkin, entity.lastLogout, entity.regDate, entity.updateDate, entity.playingState);


            Connection.Execute(
               sql,
                transaction: Transaction
            );
        }


    }
}
