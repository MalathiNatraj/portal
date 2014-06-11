using System;

namespace Diebold.Domain.Entities
{
    public class ResultsReport : IntKeyedEntity
    {
        public int TotalAlarm { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateOk { get; set; }
        public bool IsDeviceOk { get; set; }
        public string Area { get; set; }
        public string Site { get; set; }
        public string DeviceName { get; set; }
        public string AlertDescription { get; set; }
        public string ResolvedBy { get; set; }
        public string CurrentStatus { get; set; }
        public string LastNoteBy { get; set; }
        public string DVRType { get; set; }

        public Int64 RowNum { get; set; }
        public Int64 TotalCount { get; set; }

    }
}
