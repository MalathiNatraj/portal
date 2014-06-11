using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Diebold.Migrations.Migrations
{
    [Migration(2012072501)]
    public class ChangeThresholdAndOperator : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"UPDATE  AlarmConfiguration SET Threshold = 'passed' where AlarmType = 'SMART'");
            Execute.Sql(@"UPDATE  AlarmConfiguration SET Threshold = 'clean' where AlarmType = 'RaidStatus'");

            Execute.Sql(@"update  AlarmConfiguration set Operator = 'NotEquals' where AlarmType = 'SMART'");
            Execute.Sql(@"update  AlarmConfiguration set Operator = 'NotEquals' where AlarmType = 'RaidStatus'");
        }

        public override void Down()
        {
            Execute.Sql(@"UPDATE  AlarmConfiguration SET Threshold = 'fail' where AlarmType = 'SMART'");
            Execute.Sql(@"UPDATE  AlarmConfiguration SET Threshold = 'fail' where AlarmType = 'RaidStatus'");

            Execute.Sql(@"update  AlarmConfiguration set Operator = 'Equals' where AlarmType = 'SMART'");
            Execute.Sql(@"update  AlarmConfiguration set Operator = 'Equals' where AlarmType = 'RaidStatus'");
        }
    }
}
