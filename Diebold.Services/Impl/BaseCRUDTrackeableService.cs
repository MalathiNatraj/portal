using System;
using System.Collections.Generic;
using System.Linq;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using System.Linq.Dynamic;
using Diebold.Domain.Entities;
using Diebold.Services.Infrastructure;
using Diebold.Services.Exceptions;

namespace Diebold.Services.Impl
{
    public class BaseCRUDTrackeableService<T> : BaseCRUDService<T>, ICRUDTrackeableService<T> where T : TrackeableEntity
    {
        //new protected readonly ITrackeableEntityRepository<T> _repository; 

        public BaseCRUDTrackeableService(IIntKeyedRepository<T> repository, IUnitOfWork unitOfWork,
            IValidationProvider validationProvider, ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            //this._repository = repository;
        }

        public override void Create(T item)
        {
            item.Enable();

            base.Create(item);
        }

        public override void Delete(int pk)
        {
            //Logical delete.
            T itemToDelete = null;

            try
            {
                itemToDelete = _repository.Load(pk);
                itemToDelete.LogicalDelete();

                _repository.Update(itemToDelete);

                LogAction action;
                if (Enum.TryParse(itemToDelete.GetType().Name + "Delete", out action))
                {
                    LogOperation(action, itemToDelete);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();

                if (itemToDelete != null)
                    _logger.Debug(itemToDelete.GetType() + " Local Delete Error: " + e.Message);

                throw new ServiceException("An error occurred while deleting the element.", e);
            }
        }

        public override T Get(int pk)
        {
            return _repository.FindBy(x => x.Id == pk && x.DeletedKey == null);
           // return base.Get(pk);
        }

        public override IList<T> GetAll()
        {
            return _repository.All().Where(x => x.DeletedKey == null).ToList();
        }

        public override IList<T> GetAll(int pageNumber, int pageSize, string sortBy, bool ascending, out int recordCount)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

            recordCount = query.Count();

            string orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            return query.OrderBy(orderBy).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public virtual void Enable(int pk)
        {
            T entityToEnable = null;

            try
            {
                entityToEnable = _repository.Load(pk);
                entityToEnable.Enable();
                _repository.Update(entityToEnable);

                LogAction action;
                if (Enum.TryParse(entityToEnable.GetType().Name + "Enable", out action))
                {
                    LogOperation(action, entityToEnable);
                }

                _unitOfWork.Commit();
            }
            catch(Exception e)
            {
                _unitOfWork.Rollback();

                if (entityToEnable != null)
                    _logger.Debug(entityToEnable.GetType() + " Local Enable Error: " + e.Message);

                throw new ServiceException("An error occurred while enabling the element.", e);
            }
        }

        public virtual void Disable(int pk)
        {
            T entityToDisable = null;

            try
            {
                entityToDisable = _repository.Load(pk);
                entityToDisable.Disable();
                _repository.Update(entityToDisable);

                LogAction action;
                if (Enum.TryParse(entityToDisable.GetType().Name + "Disable", out action))
                {
                    LogOperation(action, entityToDisable);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();

                if (entityToDisable != null)
                    _logger.Debug(entityToDisable.GetType() + " Local Disable Error: " + e.Message);

                throw new ServiceException("An error occurred while disabling the element.", e);
            }
        }

        /*
        public override IList<T> LookUp()
        {
            return _repository.All().Where(x => (x.IsDeleted == false && x.IsDisabled = false)).ToList();
        }
        */

        public IList<T> GetAllEnabled()
        {
            return _repository.All().Where(x => x.DeletedKey == null && x.IsDisabled == false).ToList();
        }
    }
}
