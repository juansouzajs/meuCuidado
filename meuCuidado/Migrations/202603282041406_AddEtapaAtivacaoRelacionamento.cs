namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEtapaAtivacaoRelacionamento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.meuCuidado_RelacionamentoIdosoProfissional", "EtapaAtivacao", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.meuCuidado_RelacionamentoIdosoProfissional", "EtapaAtivacao");
        }
    }
}
