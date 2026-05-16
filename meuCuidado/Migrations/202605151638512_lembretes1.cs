namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lembretes1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.meuCuidado_Lembrete", "UsuarioId", c => c.Int(nullable: false));
            AddColumn("dbo.meuCuidado_Lembrete", "TipoUsuario", c => c.Int(nullable: false));
            AddColumn("dbo.meuCuidado_Pessoa", "UsuarioId", c => c.Int());
            AddColumn("dbo.meuCuidado_Pessoa", "TipoUsuario", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.meuCuidado_Pessoa", "TipoUsuario");
            DropColumn("dbo.meuCuidado_Pessoa", "UsuarioId");
            DropColumn("dbo.meuCuidado_Lembrete", "TipoUsuario");
            DropColumn("dbo.meuCuidado_Lembrete", "UsuarioId");
        }
    }
}
