namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AtualizaDocumento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documento", "Descricao", c => c.String());
            AddColumn("dbo.Documento", "TipoUsuario", c => c.Int(nullable: false));
            AddColumn("dbo.Documento", "DataUpload", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documento", "DataUpload");
            DropColumn("dbo.Documento", "TipoUsuario");
            DropColumn("dbo.Documento", "Descricao");
        }
    }
}
