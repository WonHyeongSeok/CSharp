/*  Written by Tim Schreiber
    StackOverflow user 'sakir' is incorrectly claiming that they wrote this code in the following answer: 
        http://stackoverflow.com/questions/31298235/dapper-and-unit-of-work-pattern/31636037
    
    They have never in any way contributed to this code, and the false attribution has been reported to StackOverflow. */

using DataLayer.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace DataLayer
{
    public partial class UnitOfWork : IUnitOfWork
    {
     
        private IAccountRepository _accountRepository;
        private IUserRepository _userRepository;
        private IRoomRepository _roomRepository;
        private IUserGameRecodeRepository _userGameRecodeRepository;
        private IUserItemRepository _userItemRepository;
        private IUserMailRepository _userMailRepository;
      


        public IAccountRepository AccountRepository
        {
            get { return _accountRepository ?? (_accountRepository = new AccountRepository(_transaction)); }
        }

        public IUserRepository UserRepository
        {
            get { return _userRepository ?? (_userRepository = new UserRepository(_transaction)); }
        }

        public IRoomRepository RoomRepository
        {
            get { return _roomRepository ?? (_roomRepository = new RoomRepository(_transaction)); }
        }

        public IUserGameRecodeRepository UserGameRecodeRepository
        {
            get { return _userGameRecodeRepository ?? (_userGameRecodeRepository = new UserGameRecodeRepository(_transaction)); }
        }

        public IUserItemRepository UserItemRepository
        {
            get { return _userItemRepository ?? (_userItemRepository = new UserItemRepository(_transaction)); }
        }

        public IUserMailRepository UserMailRepository
        {
            get { return _userMailRepository ?? (_userMailRepository = new UserMailRepository(_transaction)); }
        }

    }
}
