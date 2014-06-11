using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Transactions;
using Diebold.Domain.Exceptions;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{
    public class RoleService : BaseCRUDTrackeableService<Role>, IRoleService
    {
        public RoleService(IRoleRepository repository,
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
        {

        }

        public override void Create(Role item)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    this._validationProvider.Validate(item);

                    this._repository.Add(item);

                    LogOperation(LogAction.RoleCreate, item);

                    this._unitOfWork.Commit();
                }
                catch (Exception E)
                {
                    this._unitOfWork.Rollback();

                    throw new ServiceException("Role Create Failed", E);
                }

                //execute another task here. 
                //if failed, catch and throw service exception. 
                //throw new ServiceException("fail segunda tran");

                scope.Complete();
            }

            //base.Create(item);
        }

        public override void Update(Role item)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    this._validationProvider.Validate(item);

                    this._repository.Update(item);

                    LogOperation(LogAction.RoleCreate, item);

                    this._unitOfWork.Commit();
                }
                catch (Exception E)
                {
                    this._unitOfWork.Rollback();

                    throw new ServiceException("Role Update Failed", E);
                }

                //execute another task here. 
                //if failed, catch and throw service exception. 
                //throw new ServiceException("fail segunda tran");

                scope.Complete();
            }            
        }

        public IList<string> GetAllActions()
        {
            return Enum.GetNames(typeof (Domain.Entities.Action)).ToList();
        }

        public IList<string> GetActionsByRole(string roleName)
        {
            var role = _repository.FindBy(r => r.Name == roleName);

            IList<string> retList = new List<string>(role.Actions.Count);

            foreach (var action in role.Actions)
            {
                retList.Add(Enum.GetName(typeof (Diebold.Domain.Entities.Action), action));
            }

            return retList;
        }

        public bool RoleHasAction(string roleName, Domain.Entities.Action action)
        {
            return (_repository.FindBy(r => r.Name == roleName && r.Actions.Contains(action)) != null);
        }

        public Page<Role> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

            if (!string.IsNullOrEmpty(whereCondition))
            {
                query = query.Where(x => x.Name.Contains(whereCondition));
            }

            var orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            return query.OrderBy(orderBy).ToPage(pageNumber, pageSize);
        }

        public int GetUsersCountWithThisRole(int roleId)
        {
            return _repository.All().Count(x => x.DeletedKey == null && x.Id == roleId);
        }

        public override void Delete(int pk)
        {
            Role itemToDelete = null;

            try
            {
                itemToDelete = _repository.Load(pk);
                _repository.Delete(itemToDelete);

                LogAction action;
                if (Enum.TryParse(itemToDelete.GetType().Name + "Delete", out action))
                    _logService.Log(action);

                _unitOfWork.Commit();
            }
            catch (RepositoryException e)
            {
                _unitOfWork.Rollback();
                throw new ServiceException(e.Message, e);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();

                if (itemToDelete != null)
                    _logger.Debug(itemToDelete.GetType() + " Local Delete Error: " + e.Message);

                throw new ServiceException("An error occurred while deleting the element.", e);
            }
        }

        public void SetRolePortlets(int RoleId, IList<RolePortlets> RolePortlets)
        {           
            var Role = _repository.Load(RoleId);

            IList<int> itemsToDelete = new List<int>();
            foreach (var RolePortlet in Role.RolePortlets)
            {
                if (!RolePortlets.Contains(RolePortlet))
                    itemsToDelete.Add(RolePortlet.Id);
            }

            foreach (int RolePortletId in itemsToDelete)
                Role.RolePortlets.Remove(new RolePortlets { Id = RolePortletId });

            RolePortlets.ToList().Where(x => x.Id == 0).ToList().ForEach(y =>
            {
                Role.RolePortlets.Add(y);
            });
        }

    }
}
