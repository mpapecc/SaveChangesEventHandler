namespace SaveChangesEventHandlers.Core.Abstraction
{
    public interface ISaveChangesHandler<in T> : ISaveChangesHandlerKey where T : class
    {
        void BeforeNewPersisted(T entity);
        void AfterNewPersisted(T entity);
        void BeforeUpdate(T oldEntity, T newEntity);
        void AfterUpdate(T oldEntity, T newEntity);
        void BeforeDelete(T entity);
        void AfterDelete(T entity);
    }
}
