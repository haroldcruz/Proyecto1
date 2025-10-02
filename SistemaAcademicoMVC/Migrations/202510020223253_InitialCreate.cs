namespace SistemaAcademicoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cuatrimestre",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Año = c.Int(nullable: false),
                        Numero = c.Int(nullable: false),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CursosPorCuatrimestre",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CuatrimestreId = c.Int(nullable: false),
                        CursoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cuatrimestre", t => t.CuatrimestreId, cascadeDelete: true)
                .ForeignKey("dbo.Curso", t => t.CursoId, cascadeDelete: true)
                .Index(t => t.CuatrimestreId)
                .Index(t => t.CursoId);
            
            CreateTable(
                "dbo.Curso",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Codigo = c.String(),
                        Creditos = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Docente",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Apellido = c.String(),
                        Usuario = c.String(),
                        PasswordHash = c.String(),
                        Correo = c.String(),
                        IntentosFallidos = c.Int(nullable: false),
                        FechaBloqueo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Estudiante",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Apellidos = c.String(),
                        Identificacion = c.String(),
                        FechaNacimiento = c.DateTime(nullable: false),
                        Provincia = c.String(),
                        Canton = c.String(),
                        Distrito = c.String(),
                        Correo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Matricula",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstudianteId = c.Int(nullable: false),
                        CuatrimestreId = c.Int(nullable: false),
                        CursoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cuatrimestre", t => t.CuatrimestreId, cascadeDelete: true)
                .ForeignKey("dbo.Curso", t => t.CursoId, cascadeDelete: true)
                .ForeignKey("dbo.Estudiante", t => t.EstudianteId, cascadeDelete: true)
                .Index(t => t.EstudianteId)
                .Index(t => t.CuatrimestreId)
                .Index(t => t.CursoId);
            
            CreateTable(
                "dbo.Evaluacion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatriculaId = c.Int(nullable: false),
                        NotaNumerica = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Observaciones = c.String(),
                        TipoParticipacion = c.String(),
                        Estado = c.String(),
                        Fecha = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matricula", t => t.MatriculaId, cascadeDelete: true)
                .Index(t => t.MatriculaId);
            
            CreateTable(
                "dbo.Participacion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatriculaId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        Asistio = c.Boolean(nullable: false),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matricula", t => t.MatriculaId, cascadeDelete: true)
                .Index(t => t.MatriculaId);
            
            CreateTable(
                "dbo.Tarea",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatriculaId = c.Int(nullable: false),
                        Descripcion = c.String(),
                        FechaEntrega = c.DateTime(nullable: false),
                        Entregada = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matricula", t => t.MatriculaId, cascadeDelete: true)
                .Index(t => t.MatriculaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tarea", "MatriculaId", "dbo.Matricula");
            DropForeignKey("dbo.Participacion", "MatriculaId", "dbo.Matricula");
            DropForeignKey("dbo.Evaluacion", "MatriculaId", "dbo.Matricula");
            DropForeignKey("dbo.Matricula", "EstudianteId", "dbo.Estudiante");
            DropForeignKey("dbo.Matricula", "CursoId", "dbo.Curso");
            DropForeignKey("dbo.Matricula", "CuatrimestreId", "dbo.Cuatrimestre");
            DropForeignKey("dbo.CursosPorCuatrimestre", "CursoId", "dbo.Curso");
            DropForeignKey("dbo.CursosPorCuatrimestre", "CuatrimestreId", "dbo.Cuatrimestre");
            DropIndex("dbo.Tarea", new[] { "MatriculaId" });
            DropIndex("dbo.Participacion", new[] { "MatriculaId" });
            DropIndex("dbo.Evaluacion", new[] { "MatriculaId" });
            DropIndex("dbo.Matricula", new[] { "CursoId" });
            DropIndex("dbo.Matricula", new[] { "CuatrimestreId" });
            DropIndex("dbo.Matricula", new[] { "EstudianteId" });
            DropIndex("dbo.CursosPorCuatrimestre", new[] { "CursoId" });
            DropIndex("dbo.CursosPorCuatrimestre", new[] { "CuatrimestreId" });
            DropTable("dbo.Tarea");
            DropTable("dbo.Participacion");
            DropTable("dbo.Evaluacion");
            DropTable("dbo.Matricula");
            DropTable("dbo.Estudiante");
            DropTable("dbo.Docente");
            DropTable("dbo.Curso");
            DropTable("dbo.CursosPorCuatrimestre");
            DropTable("dbo.Cuatrimestre");
        }
    }
}
