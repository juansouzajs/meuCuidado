namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurriculoUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Curriculo", "CursosJson", c => c.String());
            AddColumn("dbo.Curriculo", "ExperienciasJson", c => c.String());
            AddColumn("dbo.Curriculo", "RedesSociaisJson", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Curriculo", "RedesSociaisJson");
            DropColumn("dbo.Curriculo", "ExperienciasJson");
            DropColumn("dbo.Curriculo", "CursosJson");
        }
    }
}
