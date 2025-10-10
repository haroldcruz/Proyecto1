namespace SistemaAcademicoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPorcentajePonderacionToTarea : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tarea", "PorcentajePonderacion", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Evaluacion", "Observaciones", c => c.String());
            AlterColumn("dbo.Evaluacion", "TipoParticipacion", c => c.String());
            AlterColumn("dbo.Evaluacion", "Estado", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Evaluacion", "Estado", c => c.String(nullable: false));
            AlterColumn("dbo.Evaluacion", "TipoParticipacion", c => c.String(nullable: false));
            AlterColumn("dbo.Evaluacion", "Observaciones", c => c.String(nullable: false));
            DropColumn("dbo.Tarea", "PorcentajePonderacion");
        }
    }
}
