namespace SistemaAcademicoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoginLockoutFieldsToDocente : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Docente", "FailedLoginAttempts", c => c.Int(nullable: false));
            AddColumn("dbo.Docente", "LockoutEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Docente", "LockoutEnd");
            DropColumn("dbo.Docente", "FailedLoginAttempts");
        }
    }
}
