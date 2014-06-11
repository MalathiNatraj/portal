using System;
using System.Collections.Generic;
using System.Linq;
using Diebold.Domain.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts.Infrastructure;
using NHibernate.Transform;

namespace Diebold.DAO.NH.Repositories
{
    public class AlertInfoRepository : BaseIntKeyedRepository<AlertInfo>, IAlertInfoRepository
    {
        public AlertInfoRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        
        public IList<ResultsReport> GetAlertsForReport(IList<string> alarmTypes, IList<int> userIds, string deviceStatus,
                                                       string dateType, DateTime dateFrom, DateTime dateTo,
                                                       IList<int> deviceIds, int? pageIndex, int? pageSize,
                                                       string sortBy, bool ascending, bool groupLevelSelected, out int rowCount)
        {
            if (groupLevelSelected && deviceIds.Count == 0)
            {
                rowCount = 0;
                return new List<ResultsReport>();
            }

            var query =
                @"
                        SELECT 
                            RowNum, TotalCount, Date, DateOk, Area, Site, DeviceName, AlertDescription, ResolvedBy,
                            LastNoteBy, DVRType, CurrentStatus
                        FROM (
	                            Select
	                              ROW_NUMBER()Over(Order by ai.groupId Asc) As RowNum,
	                              Count(*) OVER() AS [TotalCount],
	                              min(DateOccur) as Date, 
	                              max(DateOccur) as DateOk, 
	                              min(cg1.Name) as Area,
	                              min(s.Name) as Site,
	                              min(d.name) as DeviceName,
	                              min(ac.alarmType) +          
                                  case min(ac.alarmType)
                                  when 'VideoLoss' Then ' - Camera ' + min(cast(ai.ElementIdentifier as varchar))
                                  when 'DriveTemperature' Then ' - Drive ' + min(cast(ai.ElementIdentifier as varchar))
                                  when 'RaidStatus' Then ' - Raid ' + min(cast(ai.ElementIdentifier as varchar))
                                  when 'SMART' Then ' - Drive ' + min(cast(ai.ElementIdentifier as varchar))
                                  else '' end as AlertDescription,                                            	                              
                                  min(case when (ra.deviceId is not null) then (rau.FirstName + ' ' + rau.LastName) else '' end) as ResolvedBy,
	                              max(nu.FirstName + ' ' + nu.LastName) as LastNoteBy,
	                              min(dvr.deviceType) as DVRType,
	                              min(case when (cast(IsdeviceOK as varchar) = '1') then 'Ok' else ac.severity end) as CurrentStatus,  
	                              max(groupId) as groupId
	                              from AlertInfo ai
                                  inner join Device d on d.id = ai.deviceId  
	                              inner join Dvr dvr on dvr.id = ai.deviceId  
	  
	                              left join  (select deviceId, alarmConfigurationId, userId  from ResolvedAlert ra
	                              inner join (SELECT max(id) as Id  from ResolvedAlert group by deviceId, alarmconfigurationId) as items
	                              on items.id = ra.id) ra on ra.deviceId = ai.deviceId
	  
	                              left join (select deviceId, userId from Note n
	                               inner join (SELECT max(id) as Id  from Note 
	                               group by deviceId) as items
	                               on items.id = n.id) ns on ns.deviceId = ai.deviceId
	   
	                              left join [User] rau on rau.id = ra.userId
	                              left join [User] nu on nu.id = ns.userId
	                              inner join AlarmConfiguration ac on ac.id = ai.alarmConfigurationid
	                              inner join Gateway g on g.id = dvr.gatewayId
	                              inner join [Site] s on s.id = dvr.SiteId
	                              inner join CompanyGrouping2Level cg1 on cg1.id = s.CompanyGrouping2LevelId 
                                  where ai.dateOccur between :dateFrom and DATEADD(d, 1, :dateTo) {filterConditional}
                                  group by ai.groupId ) AS Result   
                              WHERE :pageIndex IS NULL OR :pageSize IS NULL OR RowNum BETWEEN (:pageIndex - 1) * :pageSize + 1 AND :pageIndex * :pageSize  
                              order by {sortExpression}";

            query = query.Replace("{sortExpression}", sortBy + (ascending ? " asc" : " desc"));

            var filterConditial = string.Empty;

            if (deviceIds.Count > 0)
               filterConditial += " and ai.deviceId in (:deviceIds) ";

            if (deviceStatus != "AllDevices" && deviceStatus != "")
            //if (deviceStatus != "AllDevices")
                filterConditial += " and d.deletedKey " + ((deviceStatus == "ActiveDevices") ? " is null" : " is not null");

            if (alarmTypes.Count > 0)
                filterConditial += " and ac.alarmType in (:alarmTypes) ";

            if (userIds.Count > 0)
                filterConditial += " and (ra.userId in (:userIds) or ns.userId in (:userIds)) ";

            query = query.Replace("{filterConditional}", filterConditial);

            var sqlQuery = Session.CreateSQLQuery(query)
                .SetParameter("pageIndex", pageIndex)
                .SetParameter("pageSize", pageSize)
                .SetParameter("dateFrom", dateFrom)
                .SetParameter("dateTo", dateTo);

            if (deviceIds.Count > 0) 
                sqlQuery.SetParameterList("deviceIds", deviceIds);
            
            if (alarmTypes.Count > 0)
                sqlQuery.SetParameterList("alarmTypes", alarmTypes);

            if (userIds.Count > 0)
                sqlQuery.SetParameterList("userIds", userIds);

            sqlQuery.SetResultTransformer(Transformers.AliasToBean(typeof (ResultsReport)));

            var result = sqlQuery.List<ResultsReport>();
            rowCount = result.Count > 0 ? (int)result.First().TotalCount : 0;
            return result;
        }
    }
}
