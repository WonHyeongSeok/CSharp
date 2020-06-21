#if DEBUG
//#define LATENCY_CHECK
#endif
using NHibernate.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using NHibernate;
using NHibernate.Criterion;
using LuciferShareLib;
using System;
using DataLayer.Entities;
using LuciferPacket;

namespace DataLayer
{

    public class RepositoryFactory<T>
        where T : class
    {
        static RepositoryFactory<T> _instance = null;
        static RepositoryFactory()
        { // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        }

        public static RepositoryFactory<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositoryFactory<T>();
                }
                return _instance;
            }
        }

        protected RepositoryFactory()
        {
        }

        public Repository<T> Pop(ISession session)
        {
            var rep = PoolManager.Instance.GetObject<Repository<T>>();
            rep.SetSession(session);
            return rep;
        }

        public Repository<T> Pop()
        {
            return PoolManager.Instance.GetObject<Repository<T>>();
        }
    }


    //public class Repository<T> : IIntKeyedRepository<T>, ILongKeyedRepository<T>, IStringKeyedRepository<T>, IDisposable
    //    where T : class
    public class Repository<T> : IPoolableObject where T : class
    {
        protected ISession _session;

        private IPoolManager poolManager;
        public bool isUsed { get; set; }

        public int PoolCount { get { return 1; } }

#if LATENCY_CHECK
        DateTime startTime = default(DateTime);
        string action = "";
#endif
        public void Dispose()
        {
#if LATENCY_CHECK
            if (default(DateTime) != startTime) 
            {
                DalmutiLog.Statistics( "Repository_latency("+action+"):"+ typeof(T) + ":" + (DateTime.Now - startTime).TotalMilliseconds.ToString());
                startTime = default(DateTime);
                action = "";
            }
#endif
            if (this.poolManager != null) { this.poolManager.ReturnObject(this); }
            GC.SuppressFinalize(this);
        }

        void IPoolableObject.Reset()
        {
        }

        void IPoolableObject.SetPoolManager(IPoolManager poolManager)
        {
            this.poolManager = poolManager;
        }

        public virtual void Clear()
        {
        }

        public virtual void Init()
        {
        }

        ~Repository()
        {
            Dispose();
        }


        public Repository(ISession session)
        {
            _session = session;
        }
        public Repository()
        {
        }

        public void SetSession(ISession session)
        {
#if LATENCY_CHECK
            startTime = DateTime.Now;
#endif
            _session = session;
        }

        //public T FindBy(long uId, int mId)
        //{
        //    var knight = _session.CreateCriteria(typeof(T))
        //        .Add(Restrictions.Eq("uId", uId))
        //        .Add(Restrictions.Eq("mId", mId))
        //        .UniqueResult<T>();
        //    return knight;
        //}

        #region IIntKeyedRepository<T> Members
        //
        public T FindBy(object id)
        {
#if LATENCY_CHECK
            action = "FindBy";
#endif
            return _session.Get<T>(id);
        }

        #endregion

        #region IRepository<T> Members

        public bool Add(T entity)
        {
#if LATENCY_CHECK
            action = "Add";
#endif
            _session.Save(entity);
            return true;
        }

        public bool Add(System.Collections.Generic.IEnumerable<T> items)
        {
#if LATENCY_CHECK
            action = "Add";
#endif
            foreach (T item in items)
            {
                _session.Save(item);
            }
            return true;
        }

        public bool Update(T entity)
        {
#if LATENCY_CHECK
            action = "Update";
#endif
            _session.Update(entity);
            return true;
        }

        public bool Delete(T entity)
        {
#if LATENCY_CHECK
            action = "Delete";
#endif
            _session.Delete(entity);
            //            SimpleObjectPool<T>.Instance.Push(entity); 외부에서 push
            return true;
        }


        public void DeleteById(System.Linq.Expressions.Expression<System.Func<T, bool>> expression)
        {
#if LATENCY_CHECK
            action = "Delete";
#endif
            foreach (var entity in All().Where(expression))
            {
                _session.Delete(entity);
            }
        }

        public bool Delete(System.Collections.Generic.IEnumerable<T> entities)
        {
#if LATENCY_CHECK
            action = "Delete";
#endif
            foreach (T entity in entities)
            {
                _session.Delete(entity);

            }
            return true;
        }

        #endregion

        #region IReadOnlyRepository<T> Members

        public IQueryable<T> All()
        {
            return _session.Query<T>();
        }

        //public IQueryable<T> TestQueryTop(System.Linq.Expressions.Expression<System.Func<T, bool>> expression, int topCount)
        //{
        //    var queryable = _session.Query<T>()..Where(expression).AsQueryable();
        //    return queryable;
        //}

        public T FindBy(System.Linq.Expressions.Expression<System.Func<T, bool>> expression)
        {
#if LATENCY_CHECK
            action = "FindBy";
#endif
            var queryable = FilterBy(expression);
            if (queryable.Count() == 0)
                return default(T);
            if (queryable.Count() > 1)
            {
                string expressionString = queryable.Expression.ToString();
               // DalmutiLog.Warn(String.Format("Repository.FindBy() Error. Count is {0}. Expression is \"{1}\"", queryable.Count(), expressionString));
            }

            return queryable.First();
        }

        public IQueryable<T> FilterBy(System.Linq.Expressions.Expression<System.Func<T, bool>> expression1, System.Linq.Expressions.Expression<System.Func<T, int>> expression2 = null)
        {
#if LATENCY_CHECK
            action = "FilterBy";
#endif
            IQueryable<T> queryable;
            if (expression1 == null)
            {
                queryable = All().AsQueryable();
            }
            else
            {
                queryable = All().Where(expression1).AsQueryable();

                if (expression2 != null)
                    return queryable.OrderBy(expression2);
            }

            return queryable;
        }

        #endregion

        public T FindByName(string name, string id)
        {
#if LATENCY_CHECK
            action = "FindByName";
#endif
            return _session.CreateCriteria(typeof(T))
                            .Add(Restrictions.Eq(name, id))
                            .UniqueResult<T>();
        }

        /*
        protected void InsertEntity<V>(V entity, RedisEntityState<V> redisEntityState, List<IRedisableObject> dbUpdateEntityList) where V : T, IRedisableObject
        {
            dbUpdateEntityList?.Add(entity);
            redisEntityState.SetBackupState();

            redisEntityState.SetNone(eDBEntityState.NeedInsert);
            Add(entity);
            redisEntityState.SetCurrentInRedis(entity);
        }
        */

        //protected void UpdateEntity<V>(V entity, RedisEntityState<V> redisEntityState, List<IRedisableObject> dbUpdateEntityList) where V : T, IRedisableObject
        //{
        //    dbUpdateEntityList?.Add(entity);
        //    switch (redisEntityState.GetDBState())
        //    {
        //        case eDBState.NeedInsert:
        //        case eDBState.Deleted:
        //            Add(entity);
        //            break;
        //        case eDBState.None:
        //            Update(entity);
        //            break;
        //    }
        //    redisEntityState.SetDBUpdate();
        //    redisEntityState.SetCurrentInRedis(entity);
        //}

        //protected void DeleteEntity<V>(V entity, RedisEntityState<V> redisEntityState, List<IRedisableObject> dbUpdateEntityList) where V : T, IRedisableObject
        //{
        //    dbUpdateEntityList?.Add(entity);
        //    switch (redisEntityState.GetDBState())
        //    {
        //        case eDBState.None:

        //            Delete(entity);
        //            break;
        //        default:
        //            break;
        //    }

        //    redisEntityState.SetDBDelete();
        //    redisEntityState.SetDeleteInRedis(entity);
        //}

        //public bool DBUpdate<V>(V entity, DBEntityState dbEntityState, List<IRedisableObject> dbUpdateEntityList = null) where V : T, IRedisableObject
        //{
        //    RedisEntityState<V> redisEntityState = dbEntityState as RedisEntityState<V>;

        //    if (false == dbEntityState.IsModified())
        //        return false;

        //    dbEntityState.SetDBVersion();

        //    switch (dbEntityState.GetEntityState())
        //    {
        //        case eEntityState.OnlyRedisUse:
        //        case eEntityState.FirstMemoryLoad:
        //            return false;
        //        case eEntityState.OnlyRedisDelete:
        //            redisEntityState.SetDBDelete();
        //            DeleteEntity(entity, redisEntityState, dbUpdateEntityList);
        //            break;
        //        case eEntityState.None:
        //            UpdateEntity(entity, redisEntityState, dbUpdateEntityList);
        //            break;
        //        case eEntityState.Delete:
        //            DeleteEntity(entity, redisEntityState, dbUpdateEntityList);
        //            break;
        //    }

        //    return true;
        //}



    }
}
