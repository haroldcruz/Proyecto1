namespace SistemaAcademicoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Docente", "Password", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Docente", "Nombre", c => c.String(nullable: false));
            AlterColumn("dbo.Docente", "Correo", c => c.String(nullable: false));
            DropColumn("dbo.Docente", "Apellido");
            DropColumn("dbo.Docente", "Usuario");
            DropColumn("dbo.Docente", "PasswordHash");
            DropColumn("dbo.Docente", "IntentosFallidos");
            DropColumn("dbo.Docente", "FechaBloqueo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Docente", "FechaBloqueo", c => c.DateTime());
            AddColumn("dbo.Docente", "IntentosFallidos", c => c.Int(nullable: false));
            AddColumn("dbo.Docente", "PasswordHash", c => c.String());
            AddColumn("dbo.Docente", "Usuario", c => c.String());
            AddColumn("dbo.Docente", "Apellido", c => c.String());
            AlterColumn("dbo.Docente", "Correo", c => c.String());
            AlterColumn("dbo.Docente", "Nombre", c => c.String());
            DropColumn("dbo.Docente", "Password");
        }
    }
}
