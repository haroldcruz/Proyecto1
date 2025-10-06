namespace SistemaAcademicoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class evaluacion : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Evaluacion", "Observaciones", c => c.String(nullable: false));
            AlterColumn("dbo.Evaluacion", "TipoParticipacion", c => c.String(nullable: false));
            AlterColumn("dbo.Evaluacion", "Estado", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Evaluacion", "Estado", c => c.String());
            AlterColumn("dbo.Evaluacion", "TipoParticipacion", c => c.String());
            AlterColumn("dbo.Evaluacion", "Observaciones", c => c.String());
        }
    }
}
