using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;

namespace Diebold.Services.Contracts
{
    public interface INoteService : ICRUDTrackeableService<Note>
    {
        IList<Note> GetAllByDevice(int deviceId);
        IList<Note> GetAllByDevice(int deviceId, int pageIndex, int recordsCount, out int totalItems, out int totalPages);
        Page<Note> GetLastPagedByDevice(int deviceId, int pageIndex, int recordsCount);
    }
}