namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pessoa : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.meuCuidado_Lembrete", "RelacionamentoIdosoProfissionalId", "dbo.meuCuidado_RelacionamentoIdosoProfissional");
            DropIndex("dbo.meuCuidado_Lembrete", new[] { "RelacionamentoIdosoProfissionalId" });
            CreateTable(
                "dbo.meuCuidado_Pessoa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.meuCuidado_Lembrete", "PessoaId", c => c.Int());
            CreateIndex("dbo.meuCuidado_Lembrete", "PessoaId");
            AddForeignKey("dbo.meuCuidado_Lembrete", "PessoaId", "dbo.meuCuidado_Pessoa", "Id");
            DropColumn("dbo.meuCuidado_Lembrete", "RelacionamentoIdosoProfissionalId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.meuCuidado_Lembrete", "RelacionamentoIdosoProfissionalId", c => c.Int(nullable: false));
            DropForeignKey("dbo.meuCuidado_Lembrete", "PessoaId", "dbo.meuCuidado_Pessoa");
            DropIndex("dbo.meuCuidado_Lembrete", new[] { "PessoaId" });
            DropColumn("dbo.meuCuidado_Lembrete", "PessoaId");
            DropTable("dbo.meuCuidado_Pessoa");
            CreateIndex("dbo.meuCuidado_Lembrete", "RelacionamentoIdosoProfissionalId");
            AddForeignKey("dbo.meuCuidado_Lembrete", "RelacionamentoIdosoProfissionalId", "dbo.meuCuidado_RelacionamentoIdosoProfissional", "Id", cascadeDelete: true);
        }
    }
}
