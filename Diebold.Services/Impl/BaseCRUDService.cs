using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diebold.Domain.Contracts;
using Diebold.Services.Contracts;
using System.Linq.Dynamic;
using Diebold.Domain.Entities;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;
using Diebold.Domain.Contracts.Infrastructure;

namespace Diebold.Services.Impl
{
    public class BaseCRUDService<T> : BaseService, ICRUDService<T> where T : IntKeyedEntity
    {
        protected readonly IValidationProvider _validationProvider;
        protected readonly IIntKeyedRepository<T> _repository;
        protected readonly ILogService _logService;

        public BaseCRUDService(IIntKeyedRepository<T> repository, IUnitOfWork unitOfWork,
             IValidationProvider validationProvider, ILogService logService)
            : base(unitOfWork)
        {
            this._repository = repository;
            this._validationProvider = validationProvider;
            this._logService = logService;
        }

        //Overridable for logging context.
        public virtual void LogOperation(LogAction action, T item)
        {
            var description = item.ToString();
            var sourcePI = item.GetType().GetProperty("Name");

            if (sourcePI != null && sourcePI.GetValue(item, null) != null)
                description = sourcePI.GetValue(item, null).ToString();

            _logService.Log(action, description);
        }

        public virtual void Create(T item)
        {
            try
            {
                this._validationProvider.Validate(item);

                this._repository.Add(item);

                LogAction action;
                if (Enum.TryParse(item.GetType().Name + "Create", out action))
                {
                    LogOperation(action, item);
                }

                this._unitOfWork.Commit();
            }
            catch (Exception e)
            {
                this._unitOfWork.Rollback();

                var errorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                
                _logger.Debug(item.GetType() + " Local Create Error: " + errorMessage);

                throw new ServiceException(errorMessage, e);
            }
        }

        public virtual void Delete(int pk)
        {
            T itemToDelete = null;

            try
            {
                itemToDelete = this._repository.Load(pk);
                this._repository.Delete(itemToDelete);

                LogAction action;
                if (Enum.TryParse(itemToDelete.GetType().Name + "Delete", out action))
                {
                    LogOperation(action, itemToDelete);
                }

                this._unitOfWork.Commit();
            }
            catch (Exception e)
            {
                this._unitOfWork.Rollback();

                if (itemToDelete != null)
                {
                    string errorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;

                    _logger.Debug(itemToDelete.GetType() + " Local Delete Error: " + errorMessage);
                }

                throw new ServiceException("An error occurred while deleting element.", e);
            }
        }

        public virtual void Update(T item)
        {
            try
            {
                var persistentItem = this._repository.Load(item.Id);

                var destinationProperties = persistentItem.GetType().GetProperties();
                foreach (var destinationPI in destinationProperties)
                {
                    var sourcePI = item.GetType().GetProperty(destinationPI.Name);

                    destinationPI.SetValue(persistentItem,
                                           sourcePI.GetValue(item, null),
                                           null);
                }

                this._validationProvider.Validate(item);

                this._repository.Update(persistentItem);

                LogAction action;
                if (Enum.TryParse(persistentItem.GetType().Name + "Edit", out action))
                {
                    LogOperation(action, item);
                }
                
                this._unitOfWork.Commit();
            }
            catch (Exception e)
            {
                this._unitOfWork.Rollback();

                string errorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;

                _logger.Debug(item.GetType() + " Local Update Error: " + errorMessage);

                throw new ServiceException(errorMessage, e);
            }
        }

        public virtual void Update(IList<T> itemList)
        {
            try
            {
                foreach (var item in itemList)
                {
                    var persistentItem = this._repository.Load(item.Id);

                    var destinationProperties = persistentItem.GetType().GetProperties();
                    foreach (var destinationPI in destinationProperties)
                    {
                        var sourcePI = item.GetType().GetProperty(destinationPI.Name);

                        destinationPI.SetValue(persistentItem,
                                                sourcePI.GetValue(item, null),
                                                null);
                    }

                    this._validationProvider.Validate(item);

                    this._repository.Update(persistentItem);

                    LogAction action;
                    if (Enum.TryParse(persistentItem.GetType().Name + "Edit", out action))
                    {
                        LogOperation(action, item);
                    }
                }
                this._unitOfWork.Commit();
            }
            catch (Exception e)
            {
                this._unitOfWork.Rollback();

                string errorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;

                throw new ServiceException(errorMessage, e);
            }
        }

        public virtual IList<T> GetAll()
        {
            return this._repository.All().ToList();
        }

        public virtual IList<T> GetAll(int pageNumber, int pageSize, string sortBy, bool ascending, out int recordCount)
        {
            var query = _repository.All();

            recordCount = query.Count();

            var orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderBy(orderBy).ToList();
            //return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public virtual Page<T> GetPage(int pageNumber, int pageSize, string sortBy, bool @ascending)
        {
            var orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            return _repository.All().OrderBy(orderBy).ToPage(pageNumber, pageSize);
        }

        public virtual T Get(int pk)
        {
            return _repository.FindBy(pk);
        }

        public virtual IList<T> LookUp()
        {
            return this.GetAll();
        }
    }
}
