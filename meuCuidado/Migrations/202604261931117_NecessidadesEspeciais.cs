namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NecessidadesEspeciais : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.meuCuidado_Idoso", "DescricaoNecessidadesEspeciais", c => c.String());
            AddColumn("dbo.meuCuidado_Tutor", "DescricaoNecessidadesEspeciais", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.meuCuidado_Tutor", "DescricaoNecessidadesEspeciais");
            DropColumn("dbo.meuCuidado_Idoso", "DescricaoNecessidadesEspeciais");
        }
    }
}
