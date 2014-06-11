using System.Collections.Generic;
using System.Linq;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{
    public class NoteService : BaseCRUDTrackeableService<Note>, INoteService
    {
        private readonly IIntKeyedRepository<Dvr> _dvrRepository;

        public NoteService(IIntKeyedRepository<Note> repository,
                           IIntKeyedRepository<Dvr> dvrRepository,
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            _dvrRepository = dvrRepository;
        }

        public IList<Note> GetAllByDevice(int deviceId)
        {
            return _repository.All().Where(x => x.Device.Id == deviceId).ToList();
        }

        public IList<Note> GetAllByDevice(int deviceId, int pageIndex, int pageSize, out int totalItems, out int totalPages)
        {
            var notesPage = GetLastPagedByDevice(deviceId, pageIndex, pageSize);

            totalItems = notesPage.TotalItems;
            totalPages = notesPage.TotalPages;

            return notesPage.ToList();
        }

        public Page<Note> GetLastPagedByDevice(int deviceId, int pageIndex, int pageSize)
        {
            var notes = _repository.All().Where(x => x.Device.Id == deviceId).OrderByDescending(x => x.Date);
            return notes.ToPage(pageIndex, pageSize);
        }

        public override void LogOperation(LogAction action, Note item)
        {
            var device = _dvrRepository.FindBy(item.Device.Id);
            var site = device.Site;
            var companyGrouping2 = site.CompanyGrouping2Level;
            var companyGrouping1 = companyGrouping2.CompanyGrouping1Level;
            
            _logService.Log(action, item.ToString(), companyGrouping1, companyGrouping2, site, device);
        }
    }
}
