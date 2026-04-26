namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AtualizaCurriculo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Curriculo", "TipoUsuario", c => c.Int(nullable: false));
            AddColumn("dbo.Curriculo", "UsuarioId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Curriculo", "UsuarioId");
            DropColumn("dbo.Curriculo", "TipoUsuario");
        }
    }
}
