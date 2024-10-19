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


        public async Task<List<ContactDto>?> GetAll(List<Guid> tags, string? firstName = null,  string? lastName = null)
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

            var result = await query.OrderBy(c => c.FirstName).ToListAsync();


            return _customMap.MapList<ContactDto, Contact>(result);
        }

        public async Task<ContactDetailDto?> GetById(Guid contactId)
        {
            var record = await _contactRepository.GetById(contactId)
                .Include(c => c.Emails)
                .Include(c => c.Numbers)
                .Include(c => c.ContactTags)
                .ThenInclude(ct => ct.Tag).FirstOrDefaultAsync();

            return record == null ? null: _customMap.Map<ContactDetailDto,Contact>(record);
        }


        public async Task<ContactDetailDto?> Update(Guid contactId, ContactDetailDto record)
        {
            if (contactId != record.Id)
                return null;

            await UpdateContactEmails(contactId, record);
            await UpdateContactNumbers(contactId, record);
            await UpdateContactTags(contactId, record);
            record.Emails.Clear();
            record.Numbers.Clear();
            record.ContactTags.Clear();

            var data = _customMap.Map<Contact,ContactDetailDto>(record);
            return await _contactRepository.UpdateAsync(data) ? record: null;
        }

        private async Task UpdateContactEmails(Guid id, ContactDetailDto record)
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
                    emailsForAdd.Add(email);
                }
            }

            if (emailsForDelete.Any())
            {
                await _emailRepository.DeleteRangeAsync(emailsForDelete);
            }
            if (emailsForUpdate.Any())
            {
                await _emailRepository.UpdateRangeAsync(emailsForUpdate);
            }
            if (emailsForAdd.Any())
            {
                await _emailRepository.UpdateRangeAsync(emailsForAdd);
            }
        }

        private async Task UpdateContactNumbers(Guid id, ContactDetailDto record)
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
                    emailsForAdd.Add(number);
                }
            }

            if (numbersForDelete.Any())
            {
                await _numberRepository.DeleteRangeAsync(numbersForDelete);
            }
            if (emailsForUpdate.Any())
            {
                await _numberRepository.UpdateRangeAsync(emailsForUpdate);
            }
            if (emailsForAdd.Any())
            {
                await _numberRepository.UpdateRangeAsync(emailsForAdd);
            }
        }

        private async Task UpdateContactTags(Guid id, ContactDetailDto record)
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
                await _contactTagRepository.DeleteRangeAsync(contactTagsForDelete);
            }

            if (contactTagsForAdd.Any())
            {

                await _contactTagRepository.AddRangeAsync(contactTagsForAdd);
            }
        }

    }
}
