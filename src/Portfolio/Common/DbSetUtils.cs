using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Common;

public static class DbSetUtils {
  public static void AddOrUpdate<T>(this DbSet<T> dbSet, T newEntity, Expression<Func<T, bool>> onConflict, Action<T> doUpdate = null) where T : class {
    var entity = dbSet.FirstOrDefault(onConflict);
    if (entity == null) {
      dbSet.Add(newEntity);
    } else if (doUpdate != null) {
      doUpdate(entity);
      dbSet.Update(entity);
    }
  }

  public static List<Dictionary<string, object>> FromSqlRaw(this DbContext dbContext, string sql) {
    var entities = new List<Dictionary<string, object?>>();
    using (var cmd = dbContext.Database.GetDbConnection().CreateCommand()) {
      cmd.CommandText = sql;
      if (cmd.Connection.State.Equals(ConnectionState.Closed)) { cmd.Connection.Open(); }
      using (var reader = cmd.ExecuteReader()) {
        while (reader.Read()) {
          var entity = new Dictionary<string, object>();
          for (var i = 0; i < reader.FieldCount; ++i) {
            // convert DateTime to DateOnly if column type is 'date'
            var value = reader.GetDataTypeName(i) == "date" ? DateOnly.FromDateTime(reader.GetDateTime(i)) : reader.GetValue(i);
            entity.Add(reader.GetName(i), reader.IsDBNull(i) ? null : value);
          }
          entities.Add(entity);
        }
      }
      if (cmd.Connection.State.Equals(ConnectionState.Open)) { cmd.Connection.Close(); }
    }
    return entities;
  }
}