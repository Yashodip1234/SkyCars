using SkyCars.Core.DomainEntity.CarMedia;
using SkyCars.Core.DomainEntity.User;
using SkyCars.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Data.Migration
{
    [MigrationInfo("2022-09-01 13:21:12", "CarMedia2 Schema")]
    
    public class CarMediaInitMigration : FluentMigrator.Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Create
                .Table(nameof(CarMedia)).WithDescription("User master") //can also pass custom name e.g. .Table("ProductMst")
                .WithColumn(nameof(CarMedia.Id)).AsInt32().PrimaryKey().NotNullable().Identity().WithColumnDescription("Autogenerated unique identifier")
                .WithColumn(nameof(CarMedia.UserId)).AsInt32().ForeignKey(nameof(User), nameof(User.Id)).Nullable().WithColumnDescription("User id")
                .WithColumn(nameof(CarMedia.MediaName)).AsAnsiString(100).NotNullable().WithColumnDescription("Media name")
                .WithColumn(nameof(CarMedia.MediaType)).AsAnsiString(100).NotNullable().WithColumnDescription("Media type")
                .WithColumn(nameof(CarMedia.MediaDescription)).AsAnsiString(100).NotNullable().WithColumnDescription("Media Description")
                .WithColumn(nameof(CarMedia.IsDefault)).AsBoolean().Nullable().WithColumnDescription("For default").WithDefaultValue(true)
                .WithColumn(nameof(CarMedia.IsDelete)).AsBoolean().Nullable().WithColumnDescription("For delete").WithDefaultValue(true);

            //Execute.Script("DbScriptMigration/UserDefaultRecored.sql");
            //Execute.Script("DbScriptMigration/logdbschema.sql");
        }
    }
}
