﻿namespace SaveChangesEventHandlers.Core.Abstraction
{
    public interface ISaveChangesHandler<in T>: ISaveChangesHandlerKey where T : class,IEntity
    {
        void BeforeNewPersisted(T entity);
        void AfterNewPersisted(T entity);
        void BeforeUpdate(T oldEntity, T newEntity);
        void AfterUpdate(T oldEntity, T newEntity);
    }
}