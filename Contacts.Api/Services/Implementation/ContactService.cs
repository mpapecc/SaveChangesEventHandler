using Autofac.Core;
using Contacts.Api.Dtos;
using Contacts.Api.Models;
using Contacts.Api.Repositories.Abstraction;
using Contacts.Api.Services.Abstraction;
using Contacts.Api.Utils.Expressions;
using Contacts.Api.Utils.Mappings;
using Contacts.Api.Utils.Mappings.Abstraction;
using Contacts.Api.Utils.Mappings.Implementation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Contacts.Api.Services.Implementation
{
    public class ContactService : IContactService
    {
        private readonly IBaseRepository<Email> _emailRepository;
        private readonly IBaseRepository<Number> _numberRepository;
        private readonly IBaseRepository<Contact> _contactRepository;
        private readonly IBaseRepository<ContactTag> _contactTagRepository;
        private readonly ICustomMap _customMap;

        public ContactService(
            IBaseRepository<Contact> contactRepository,
            IBaseRepository<Email> emailRepository,
            IBaseRepository<Number> numberRepository,
            IBaseRepository<ContactTag> contactTagRepository,
            ICustomMap customMap
            )
        {
            _contactRepository = contactRepository;
            _emailRepository = emailRepository;
            _numberRepository = numberRepository;
            _contactTagRepository = contactTagRepository;
            _customMap = customMap;
        }


        public List<ContactDto>? GetAll(List<Guid> tags, string? firstName = null,  string? lastName = null)
        {
            var query = _contactRepository.GetAll();
            if (firstName != null)
            {
                query = query.Where(ContactExpressions.ContactFirstNameQuery(firstName));
            }

            if (lastName != null)
            {
                query = query.Where(ContactExpressions.ContactLastNameQuery(lastName));
            }

            if (tags.Any())
            {
                tags.ForEach(tag => query = query.Where(ContactExpressions.ContactContainsTagQuery(tag)));
            }

            var result = query.OrderBy(c => c.FirstName).ToList();


            return _customMap.MapList<ContactDto, Contact>(result);
        }

        public ContactDetailDto? GetById(Guid contactId)
        {
            var record = _contactRepository.GetById(contactId)
                .Include(c => c.Emails)
                .Include(c => c.Numbers)
                .Include(c => c.ContactTags)
                .ThenInclude(ct => ct.Tag).FirstOrDefault();

            return _customMap.Map<ContactDetailDto,Contact>(record);
        }


        public ContactDetailDto? Update(Guid contactId, ContactDetailDto record)
        {
            if (contactId != record.Id)
                return null;

            UpdateContactEmails(contactId, record);
            UpdateContactNumbers(contactId, record);
            UpdateContactTags(contactId, record);

            record.Emails.Clear();
            record.Numbers.Clear();
            record.ContactTags.Clear();

            var data = _customMap.Map<Contact,ContactDetailDto>(record);
            _contactRepository.Update(data);
            _contactRepository.Commit();
            return record;
        }

        private void UpdateContactEmails(Guid id, ContactDetailDto record)
        {
            List<Email> allContactEmails = _emailRepository.GetAll().Where(e => e.ContactId == id).ToList();

            List<Email> emailsForDelete = new List<Email>();
            List<Email> emailsForUpdate = new List<Email>();
            List<Email> emailsForAdd = new List<Email>();

            int i = -1;
            foreach (Email email in allContactEmails)
            {
                i++;
                var emailRecord = record.Emails.Where(e => e.Id == email.Id);
                if (!emailRecord.Any())
                {
                    emailsForDelete.Add(email);
                    continue;
                }
                email.Value = emailRecord.First().Value;
                emailsForUpdate.Add(email);
            }

            foreach (Email email in record.Emails)
            {
                if (!allContactEmails.Where(e => e.Id == email.Id).Any())
                {
                    email.ContactId = id;
                    emailsForAdd.Add(email);
                }
            }

            if (emailsForDelete.Any())
            {
                 _emailRepository.DeleteRange(emailsForDelete);
            }
            if (emailsForUpdate.Any())
            {
                 _emailRepository.UpdateRange(emailsForUpdate);
            }
            if (emailsForAdd.Any())
            {
                 _emailRepository.UpdateRange(emailsForAdd);
            }
        }

        private void UpdateContactNumbers(Guid id, ContactDetailDto record)
        {
            List<Number> allContactNumbers = _numberRepository.GetAll().Where(e => e.ContactId == id).ToList();

            List<Number> numbersForDelete = new List<Number>();
            List<Number> emailsForUpdate = new List<Number>();
            List<Number> emailsForAdd = new List<Number>();

            int i = -1;
            foreach (Number number in allContactNumbers)
            {
                i++;
                var numberRecord = record.Numbers.Where(e => e.Id == number.Id);
                if (!numberRecord.Any())
                {
                    numbersForDelete.Add(number);
                    continue;
                }
                number.Value = numberRecord.First().Value;
                emailsForUpdate.Add(number);
            }

            foreach (Number number in record.Numbers)
            {
                if (!allContactNumbers.Where(e => e.Id == number.Id).Any())
                {
                    number.ContactId = id;
                    emailsForAdd.Add(number);
                }
            }

            if (numbersForDelete.Any())
            {
                 _numberRepository.DeleteRange(numbersForDelete);
            }
            if (emailsForUpdate.Any())
            {
                 _numberRepository.UpdateRange(emailsForUpdate);
            }
            if (emailsForAdd.Any())
            {
                 _numberRepository.UpdateRange(emailsForAdd);
            }
        }

        private void UpdateContactTags(Guid id, ContactDetailDto record)
        {
            List<ContactTag> allContactTags = _contactTagRepository.GetAll().Where(ct => ct.ContactId == id).ToList();
            List<ContactTag> contactTagsForDelete = new List<ContactTag>();
            List<ContactTag> contactTagsForAdd = new List<ContactTag>();

            foreach (var contactTag in allContactTags)
            {
                var contactTagRecord = record.ContactTags.Where(ct => ct.TagId == contactTag.TagId);
                if (!contactTagRecord.Any())
                {
                    contactTagsForDelete.Add(contactTag);
                }
            }

            foreach (ContactTag contactTag in record.ContactTags)
            {
                if (!allContactTags.Where(ct => ct.TagId == contactTag.TagId).Any())
                {
                    contactTag.ContactId = id;
                    contactTagsForAdd.Add(contactTag);
                }
            }

            if (contactTagsForDelete.Any())
            {
                 _contactTagRepository.DeleteRange(contactTagsForDelete);
            }

            if (contactTagsForAdd.Any())
            {

                 _contactTagRepository.AddRange(contactTagsForAdd);
            }
        }

    }
}
