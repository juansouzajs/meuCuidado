namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class escolaridade : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Curriculo", "EscolaridadeNivel", c => c.String());
            AddColumn("dbo.Curriculo", "EscolaridadeNome", c => c.String());
            DropColumn("dbo.Curriculo", "Escolaridade");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Curriculo", "Escolaridade", c => c.String());
            DropColumn("dbo.Curriculo", "EscolaridadeNome");
            DropColumn("dbo.Curriculo", "EscolaridadeNivel");
        }
    }
}
