using SaveChangesEventHandlers.Example.Models;
using System.Linq.Expressions;

namespace SaveChangesEventHandlers.Example.Utils.Expressions
{
    public static class ContactExpressions
    {

        public static Expression<Func<Contact, bool>> ContactContainsTagQuery(Guid tag)
        {

            return c => c.ContactTags.Any(ct => ct.TagId == tag);
        }

        public static Expression<Func<Contact, bool>> ContactFirstNameQuery(string firstName)
        {

            return contact => contact.FirstName.Contains(firstName);
        }

        public static Expression<Func<Contact, bool>> ContactLastNameQuery(string lastName)
        {

            return contact => contact.LastName.Contains(lastName);
        }
    }
}
