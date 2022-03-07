using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooSimple_Database.Migrations
{
    [Migration(20220303)]
    public class FirstMigration : Migration
    {
        public override void Up()
        {
            Create.Table("Accounts")
                .WithColumn("AccountId").AsString().PrimaryKey().Identity()
                .WithColumn("AccountTypeId").AsInt64()
                .WithColumn("Mask").AsString()
                .WithColumn("Name").AsString()
                .WithColumn("NickName").AsString()
                .WithColumn("CurrentBalance").AsDecimal(18, 2)
                .WithColumn("AvailableBalance").AsDecimal(18, 2)
                .WithColumn("AccessToken").AsString()
                .WithColumn("UserAccountId").AsString()
                .WithColumn("LastUpdated").AsDateTime()
                .WithColumn("UseForBudgeting").AsBoolean()
                .WithColumn("ReloginRequired").AsBoolean();
        }

        public override void Down()
        {
        }
    }
}
