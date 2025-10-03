namespace SistemaAcademicoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCorreoDocenteToCurso : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Curso", "CorreoDocente", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Curso", "CorreoDocente");
        }
    }
}
