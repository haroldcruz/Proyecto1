using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SistemaAcademicoMVC.Models
{
    /// <summary>
    /// El contexto principal para Entity Framework.
    /// Maneja la conexión a la base de datos y expone los DbSet para cada entidad del sistema académico.
    /// </summary>
    public class SistemaAcademicoMVCContext : DbContext
    {
        // El nombre "SistemaAcademicoMVCContext" debe coincidir con el de la cadena de conexión en Web.config.
        public SistemaAcademicoMVCContext() : base("name=SistemaAcademicoMVCContext")
        {
            // Puedes agregar configuración adicional aquí si lo necesitas.
        }

        // DbSet para los docentes registrados en el sistema.
        public DbSet<Docente> Docentes { get; set; }

        // DbSet para los estudiantes registrados.
        public DbSet<Estudiante> Estudiantes { get; set; }

        // DbSet para los cuatrimestres disponibles.
        public DbSet<Cuatrimestre> Cuatrimestres { get; set; }

        // DbSet para los cursos ofertados.
        public DbSet<Curso> Cursos { get; set; }

        // DbSet para la relación cursos-cuatrimestre.
        public DbSet<CursosPorCuatrimestre> CursosPorCuatrimestre { get; set; }

        // DbSet para las matrículas de estudiantes en cursos/cuatrimestres.
        public DbSet<Matricula> Matriculas { get; set; }

        // DbSet para las evaluaciones académicas.
        public DbSet<Evaluacion> Evaluaciones { get; set; }

        // DbSet para los registros de participación.
        public DbSet<Participacion> Participaciones { get; set; }

        // DbSet para las tareas asignadas.
        public DbSet<Tarea> Tareas { get; set; }

        /// <summary>
        /// Configuración de convenciones y relaciones personalizadas.
        /// Puedes definir reglas específicas, claves foráneas, restricciones, etc.
        /// </summary>
        /// <param name="modelBuilder">El constructor de modelos de EF</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Elimina la convención de pluralización de nombres de tablas.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Aquí puedes agregar reglas personalizadas, por ejemplo:
            // modelBuilder.Entity<Estudiante>().HasIndex(e => e.Identificacion).IsUnique();

            // Si quieres definir relaciones o restricciones específicas, hazlo aquí.
        }
    }
}