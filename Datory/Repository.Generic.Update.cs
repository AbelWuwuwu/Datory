﻿using System;
using Datory.Utils;
using SqlKata;

namespace Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual bool Update(T dataInfo)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return false;

            if (!Utilities.IsGuid(dataInfo.Guid))
            {
                dataInfo.Guid = Utilities.GetGuid();
            }
            dataInfo.LastModifiedDate = DateTime.Now;

            var query = Q.Where(nameof(Entity.Id), dataInfo.Id);

            foreach (var tableColumn in TableColumns)
            {
                if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id))) continue;

                var value = tableColumn.IsExtend
                    ? Utilities.JsonSerialize(dataInfo.ToDictionary(dataInfo.GetColumnNames()))
                    : dataInfo.Get(tableColumn.AttributeName);

                query.Set(tableColumn.AttributeName, value);
            }

            return RepositoryUtils.UpdateAll(Database, TableName, query) > 0;
        }

        public virtual bool Update(T dataInfo, params string[] columnNames)
        {
            if (dataInfo.Id > 0)
            {
                var query = Q.Where(nameof(Entity.Id), dataInfo.Id);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id))) continue;
                    query.Set(columnName, ReflectionUtils.GetValue(dataInfo, columnName));
                }

                //var values = RepositoryUtils.ObjToDict(dataInfo, columnNames, nameof(IEntity.Id));
                //foreach (var value in values)
                //{
                //    query.Set(value.Key, value.Value);
                //}
                return RepositoryUtils.UpdateAll(Database, TableName, query) > 0;
            }
            if (Utilities.IsGuid(dataInfo.Guid))
            {
                var query = Q.Where(nameof(Entity.Guid), dataInfo.Guid);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id)) ||
                        Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Guid))) continue;
                    query.Set(columnName, ReflectionUtils.GetValue(dataInfo, columnName));
                }

                //var values = RepositoryUtils.ObjToDict(dataInfo, columnNames, nameof(IEntity.Id), nameof(IEntity.Guid));
                //foreach (var value in values)
                //{
                //    query.Set(value.Key, value.Value);
                //}

                return RepositoryUtils.UpdateAll(Database, TableName, query) > 0;
            }

            return false;
        }

        public virtual int Update(Query query)
        {
            return RepositoryUtils.UpdateAll(Database, TableName, query);
        }

        public virtual int Increment(Query query, int num = 1)
        {
            return RepositoryUtils.IncrementAll(Database, TableName, query, num);
        }

        public virtual int Decrement(Query query, int num = 1)
        {
            return RepositoryUtils.DecrementAll(Database, TableName, query, num);
        }
    }
}
