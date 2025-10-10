namespace SistemaAcademicoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarNotaEnTarea : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tarea", "Nota", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tarea", "Nota");
        }
    }
}
