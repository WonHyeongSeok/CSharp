/*  Written by Tim Schreiber
    StackOverflow user 'sakir' is incorrectly claiming that they wrote this code in the following answer: 
        http://stackoverflow.com/questions/31298235/dapper-and-unit-of-work-pattern/31636037
    
    They have never in any way contributed to this code, and the false attribution has been reported to StackOverflow. */

using DataLayer;
using DataLayer.Repositories;
using System;

namespace DataLayer
{
    public interface IUnitOfWork : IDisposable
    {
        //IAccountRepository AccountRepository { get; }
        //IUserRepository UserRepository { get; }

        void Commit();
    }
}
