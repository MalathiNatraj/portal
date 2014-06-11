using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Diebold.Domain.Entities
{
    public class DeviceStatus : TrackeableEntity
    {
        public virtual string Name { get; set; }
        public virtual DataType DataType { get; set; }
        public virtual string Value { get; set; }
        public virtual bool IsCollection { get; set; }
    }

    public enum DataType
    {
        String,
        Long,
        Boolean,
        Date,
        Object,
        Dictionary,
        Integer
    }


    public static class ListX
    {
        public static IList<Diebold.Domain.Entities.DeviceStatus> FilterDuplicates<DeviceStatus>(this IList<Diebold.Domain.Entities.DeviceStatus> list)
        {
            IList<Diebold.Domain.Entities.DeviceStatus> filteredList = new List<Diebold.Domain.Entities.DeviceStatus>();
            list.ToList().ForEach(x =>
            {
                if ((filteredList.Where(y => y.Name.Equals(x.Name)).Count()) == 0)
                {
                    filteredList.Add(x);
                }
            });
            return filteredList;
        }
    }
}
