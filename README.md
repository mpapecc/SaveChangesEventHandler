
# SaveChangesEventsHandler

## Description
This project wrapps Entity Framework Core (v8) SaveChanges and exposes interface with lifecycle methods so that user can define logic that will be excecuted before and after saving entity with **Added**, **Updated** or **Deleted** entity state to database with having referecne to that particular entity object. For updateing entity, reference is available for original and updated property values. 

## Usage
Add all requred infrastructure services to dependency container with **AddSaveChangesInfrastructure** extension method, only suported for .NET DI system.  Inherit **SaveChangesEventDbContext** into class that handles database connection (one that usually inherits DbContex). Override DbContext SaveChanges method to call **SaveChangesWithEventsDispatcher**, or you can create another method in your DbContext that calls SaveChangesWithEventsDispatcher and leave base SaveChanges implementation for usage when there is no need to call this logic. 

### Types

**SaveChangesEventDbContext -** inherits EF Core DbContext class. Requiers *ISaveChangesEventsDispatcher* as contructor dependecy.
**ISaveChangesEventsDispatcher-** interface responsible for calling after and before logic.
**ISaveChangesHandler-** generic inteface type which is used to define logic for a given entity. Dependencies can be injected via contructor. 
```csharp
public class SaveChangesHandler : ISaveChangesHandler<Entity>
{
	
	public SaveChangesHandler(SomeDependency someDependency)
	{
	}
	
    public void AfterNewPersisted(Entity entity)
    {
    }

    public void BeforeNewPersisted(Entityentity)
    {
    }

    public void AfterUpdate(Entity oldEntity, Entity newEntity)
    {
    }

    public void BeforeUpdate(Entity oldEntity, Entity newEntity)
    {
    }

    public void BeforeDelete(Contact entity)
    {
    }

    public void AfterDelete(Contact entity)
    {
    }
}
```
ISaveChangesHandler implementations are automatically detected and added on application start up into DI container. If any entity is added to DbContext in any of above mentioned three EntityState's Dispatcher will detect it and call SaveChangesHandler methods for that entity (**read Heads up**).

### Heads up !
When within ISaveChangesHandler method for certain type do not add new entity of that type to DbContext because it will throw StackOverflow error since it will cause loop (trigger handler => add entity to DbContext => trigger handler).
