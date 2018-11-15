using blazor.jwttest.Server.Database;
using blazor.jwttest.Server.Database.Entities;
using blazor.jwttest.Server.Services.Exceptions;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace blazor.jwttest.Server.Services
{
  public class GenericDbAccess<viewT, dbT> where dbT : DbEntityBase
  {
    internal readonly EfDataContext _db;
    internal ILogger _logger;

    public GenericDbAccess(EfDataContext db)
    {
      _db = db;

      // Set up any global mapper mappings we have
      ApplyGlobalMappings();

      // Call into the virtual implementation (If there is one) in the derived class
      // to set custom mapster mappings for that class
      ApplyCustomMappings();
    }

    internal void ApplyGlobalMappings()
    {
      // Mapster Global Configuration
      // NOTE: These are commented out, as they are NOT used when dealing with a postgresql database, as postgres can
      // have columns that contain arrays natively (Great for things like roles and tags and such like).  Other DB's
      // such as SQLite and SQL Server however cannot handle arrays, so these mapster rules when in use will (or should)
      // transparently convert from array to delimeted string without you realizing.  All you need to do is to make sure
      // your view type has an array and your DbType has a string for the same named property.

      // The following two rules will serilize a string array to a token delimited string and back again
      //TypeAdapterConfig<string, string[]>.NewConfig().MapWith(str => str.Split(',', StringSplitOptions.None));
      //TypeAdapterConfig<string[], string>.NewConfig().MapWith(str => String.Join(',', str));

      // The following two rules will serilize an IEnumerable<int> to a token delimited string and back again
      //TypeAdapterConfig<string, int[]>.NewConfig().MapWith(str => str.ConvertToIntArray()); // Use string extension method
      //TypeAdapterConfig<int[], string>.NewConfig().MapWith(ienum => String.Join(',', ienum));
    }

    public virtual void ApplyCustomMappings()
    {
      // Empty definition in the base, but if provided in derived classes
      // will be called at class constructions so that mapster rules can be set up that are specific to the service
      // class being used.  A good example is in the Users Service, where passwords can be passed in for storage
      // but can not be retrieved.
    }

    internal List<viewT> FetchAll()
    {
      return _db.Set<dbT>().OrderBy(r => r.Id).ToList().Adapt<List<viewT>>();
    }

    internal viewT FetchSingle(int id)
    {
      dbT existingRecord = _db.Set<dbT>().Find(id);
      if (existingRecord == null)
      {
        throw new EntityNotFoundException(id, typeof(dbT));
      }

      return existingRecord.Adapt<viewT>(); ;
    }

    internal viewT FetchSinlgeUsingPredicate(Expression<Func<dbT, bool>> predicate)
    {
      dbT existingRecord = _db
          .Set<dbT>().FirstOrDefault(predicate);
      if (existingRecord == null)
      {
        throw new NoEntityFoundForPredicateException();
      }

      return existingRecord.Adapt<viewT>();
    }

    internal void Update(int id, viewT updatedEntity)
    {
      dbT existingRecord = _db.Set<dbT>().Find(id);
      if (existingRecord == null)
      {
        throw new EntityNotFoundException(id, typeof(dbT));
      }

      existingRecord = updatedEntity.Adapt(existingRecord);
      existingRecord.DateModified = DateTime.UtcNow;
      _db.SaveChanges();
    }

    internal viewT Add(viewT newEntity)
    {
      dbT entityToAdd = newEntity.Adapt<dbT>();

      entityToAdd.DateCreated = DateTime.UtcNow;

      _db.Set<dbT>().Add(entityToAdd);
      _db.SaveChanges();

      return entityToAdd.Adapt<viewT>();
    }

    internal void Delete(int id)
    {
      dbT existingRecord = _db.Set<dbT>().Find(id);
      if (existingRecord == null)
      {
        throw new EntityNotFoundException(id, typeof(dbT));
      }

      _db.Set<dbT>().Remove(existingRecord);
      _db.SaveChanges();
    }

  }
}

