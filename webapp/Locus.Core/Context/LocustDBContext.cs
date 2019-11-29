namespace Locus.Core.Context
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Locus.Core.Models;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class LocustDBContext : DbContext
    {
        public LocustDBContext()
            : base("name=LocustDBContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;

        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Requirement> Requirements { get; set; }
        public virtual DbSet<Step> Steps { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<TestCase> TestCases { get; set; }
        public virtual DbSet<TestProcedure> TestProcedures { get; set; }
        public virtual DbSet<TestScenario> TestScenarios { get; set; }
        public virtual DbSet<TestSuplemental> TestSuplementals { get; set; }
        public virtual DbSet<RequirementsTest> RequirementsTests { get; set; }
        public virtual DbSet<Test_Procedure_Test_Suplemental> test_procedure_test_suplemental { get; set; }
        public virtual DbSet<Test_ChangeLog> Test_ChangeLogs { get; set; }
        public virtual DbSet<ChangeLog> ChangeLogs { get; set; }
        //public virtual DbSet<Test_Tags> test_tags { get; set; }
        public virtual DbSet<Test_Tags> test_tags { get; set; }
        public virtual DbSet<Backup> Backups { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersProjects> UsersProjects { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<ExecutionGroup> ExecutionGroups { get; set; }
        public virtual DbSet<ExecutionTestEvidence> ExecutionTest { get; set; }
        public virtual DbSet<TestResult> TestResult { get; set; }
        public virtual DbSet<TestEnvironment> TestEnvironment { get; set; }
        public virtual DbSet<TestExecution> TestExecutions { get; set; }
        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<Runner> Runners { get; set; }
        public virtual DbSet<Scripts> Scripts { get; set; }
        public virtual DbSet<ScriptsGroup> ScriptsGroup { get; set; }
        public virtual DbSet<FilesDependencies> FilesDependencies { get; set; }




        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Requirements)
                .WithRequired(e => e.Project)
                .HasForeignKey(e => e.Project_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.tags)
                .WithOptional(e => e.Project)
                .HasForeignKey(e => e.Project_Id);

            modelBuilder.Entity<Requirement>()
                .HasOptional(e => e.RequirementsTest)
                .WithRequired(e => e.Requirement)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Requirement>()
              .HasOptional(e => e.RequirementsTest)
              .WithRequired(e => e.Requirement)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<RequirementsTest>()
            .HasKey(e => e.Requirement_Id);


            modelBuilder.Entity<ExecutionTestEvidence>()
            .HasKey(e => e.id);


            modelBuilder.Entity<ExecutionGroup>()
               .HasKey(e => e.Execution_Group_Id);

            modelBuilder.Entity<Tag>()
                .HasOptional(e => e.test_tags)
                .WithRequired(e => e.Tag);

            modelBuilder.Entity<Test_Tags>()
           .HasKey(e => e.Tag_Id);

            modelBuilder.Entity<Test_Procedure_Test_Suplemental>()
                .HasKey(e => e.Test_Suplemental_Id);

            modelBuilder.Entity<TestSuplemental>()
                .HasOptional(e => e.test_procedure_test_suplemental)
                .WithRequired(e => e.TestSuplemental)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Test_ChangeLog>()
                .HasKey(e => e.Change_Log_Id);

            modelBuilder.Entity<User>()
               .HasKey(e => e.Id);

            modelBuilder.Entity<UsersProjects>()
             .HasKey(e => e.Id);

            modelBuilder.Entity<Setting>()
           .HasKey(e => e.UserName);

            modelBuilder.Entity<TestEnvironment>()
            .HasKey(e => e.Id);

            modelBuilder.Entity<TestResult>()
            .HasKey(e => e.Test_Result_Id);


            modelBuilder.Entity<TestExecution>()
            .HasKey(e => e.Test_Execution_Id);

            modelBuilder.Entity<Attachment>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Runner>()
             .HasKey(e => e.Id);

            modelBuilder.Entity<Scripts>()
            .HasKey(e => e.Id);

            modelBuilder.Entity<ScriptsGroup>()
            .HasKey(e => e.Id);

            modelBuilder.Entity<FilesDependencies>()
                .HasKey(e => e.Id);
        }
    }
}
