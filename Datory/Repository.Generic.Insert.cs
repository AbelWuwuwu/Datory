﻿using Datory.Utils;

namespace Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual int Insert(T dataInfo)
        {
            return RepositoryUtils.InsertObject(Database, TableName, TableColumns, dataInfo);
        }
    }
}
