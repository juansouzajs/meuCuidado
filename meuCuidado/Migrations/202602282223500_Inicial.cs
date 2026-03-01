namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.meuCuidado_Avaliacao",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentificadorUnico = c.Guid(nullable: false),
                        RelacionamentoIdosoProfissionalId = c.Int(nullable: false),
                        Nota = c.Int(nullable: false),
                        Comentario = c.String(),
                        RelacionamentoIdosoProfissional_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", t => t.RelacionamentoIdosoProfissional_Id)
                .ForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", t => t.RelacionamentoIdosoProfissionalId)
                .Index(t => t.RelacionamentoIdosoProfissionalId)
                .Index(t => t.RelacionamentoIdosoProfissional_Id);
            
            CreateTable(
                "dbo.meuCuidado_RelacionamentoIdosoProfissional",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentificadorUnico = c.Guid(nullable: false),
                        CuidadorId = c.Int(),
                        FisioterapeutaId = c.Int(),
                        IdosoId = c.Int(),
                        TutorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.meuCuidado_CuidadorDeIdoso", t => t.CuidadorId)
                .ForeignKey("dbo.meuCuidado_Fisioterapeuta", t => t.FisioterapeutaId)
                .ForeignKey("dbo.meuCuidado_Idoso", t => t.IdosoId)
                .ForeignKey("dbo.meuCuidado_Tutor", t => t.TutorId)
                .Index(t => t.CuidadorId)
                .Index(t => t.FisioterapeutaId)
                .Index(t => t.IdosoId)
                .Index(t => t.TutorId);
            
            CreateTable(
                "dbo.meuCuidado_CuidadorDeIdoso",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentificadorUnico = c.Guid(nullable: false),
                        Nome = c.String(nullable: false),
                        CPF = c.String(),
                        Genero = c.String(),
                        Endereco = c.String(nullable: false),
                        Telefone = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Senha = c.String(),
                        DataCadasto = c.DateTime(nullable: false),
                        DataAtualizacao = c.DateTime(),
                        UltimoLogin = c.DateTime(),
                        RelacionamentoIdosoProfissional_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", t => t.RelacionamentoIdosoProfissional_Id)
                .Index(t => t.RelacionamentoIdosoProfissional_Id);
            
            CreateTable(
                "dbo.meuCuidado_Fisioterapeuta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentificadorUnico = c.Guid(nullable: false),
                        Nome = c.String(nullable: false),
                        CPF = c.String(),
                        Genero = c.String(),
                        Endereco = c.String(nullable: false),
                        Telefone = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Senha = c.String(),
                        DataCadasto = c.DateTime(nullable: false),
                        DataAtualizacao = c.DateTime(),
                        UltimoLogin = c.DateTime(),
                        RelacionamentoIdosoProfissional_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", t => t.RelacionamentoIdosoProfissional_Id)
                .Index(t => t.RelacionamentoIdosoProfissional_Id);
            
            CreateTable(
                "dbo.meuCuidado_Idoso",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataNascimento = c.DateTime(nullable: false),
                        NecessidadesEspeciais = c.Boolean(nullable: false),
                        TutorId = c.Int(nullable: false),
                        IdentificadorUnico = c.Guid(nullable: false),
                        Nome = c.String(nullable: false),
                        CPF = c.String(),
                        Genero = c.String(),
                        Endereco = c.String(nullable: false),
                        Telefone = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Senha = c.String(),
                        DataCadasto = c.DateTime(nullable: false),
                        DataAtualizacao = c.DateTime(),
                        UltimoLogin = c.DateTime(),
                        RelacionamentoIdosoProfissional_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", t => t.RelacionamentoIdosoProfissional_Id)
                .ForeignKey("dbo.meuCuidado_Tutor", t => t.TutorId)
                .Index(t => t.TutorId)
                .Index(t => t.RelacionamentoIdosoProfissional_Id);
            
            CreateTable(
                "dbo.meuCuidado_Medico",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdosoId = c.Int(nullable: false),
                        IdentificadorUnico = c.Guid(nullable: false),
                        Nome = c.String(nullable: false),
                        CPF = c.String(),
                        Genero = c.String(),
                        Endereco = c.String(nullable: false),
                        Telefone = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Senha = c.String(),
                        DataCadasto = c.DateTime(nullable: false),
                        DataAtualizacao = c.DateTime(),
                        UltimoLogin = c.DateTime(),
                        Tutor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.meuCuidado_Idoso", t => t.IdosoId, cascadeDelete: true)
                .ForeignKey("dbo.meuCuidado_Tutor", t => t.Tutor_Id)
                .Index(t => t.IdosoId)
                .Index(t => t.Tutor_Id);
            
            CreateTable(
                "dbo.meuCuidado_Tutor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelacaoComIdoso = c.String(nullable: false),
                        NecessidadesEspeciais = c.Boolean(nullable: false),
                        IdentificadorUnico = c.Guid(nullable: false),
                        Nome = c.String(nullable: false),
                        CPF = c.String(),
                        Genero = c.String(),
                        Endereco = c.String(nullable: false),
                        Telefone = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Senha = c.String(),
                        DataCadasto = c.DateTime(nullable: false),
                        DataAtualizacao = c.DateTime(),
                        UltimoLogin = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Curriculo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentificadorUnico = c.Guid(nullable: false),
                        Nome = c.String(),
                        AnosExperiencia = c.Int(nullable: false),
                        Escolaridade = c.String(),
                        AvaliacaoMedia = c.Double(nullable: false),
                        NumeroAvaliacoes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Documento",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TipoDocumento = c.Int(nullable: false),
                        Extensao = c.Int(nullable: false),
                        Caminho = c.String(),
                        UsuarioId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.meuCuidado_Lembrete",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentificadorUnico = c.Guid(nullable: false),
                        RelacionamentoIdosoProfissionalId = c.Int(nullable: false),
                        MedicamentoId = c.Int(),
                        Descricao = c.String(),
                        DataHoraPrimeiroAlerta = c.DateTime(),
                        DataHora = c.DateTime(nullable: false),
                        Repete = c.Boolean(nullable: false),
                        Medicamento_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.meuCuidado_Medicamento", t => t.Medicamento_Id)
                .ForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", t => t.RelacionamentoIdosoProfissionalId, cascadeDelete: true)
                .Index(t => t.RelacionamentoIdosoProfissionalId)
                .Index(t => t.Medicamento_Id);
            
            CreateTable(
                "dbo.meuCuidado_Medicamento",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentificadorUnico = c.Guid(nullable: false),
                        Nome = c.String(nullable: false),
                        Dosagem = c.String(nullable: false),
                        FormaFarmaceutica = c.String(),
                        DuracaoEmDias = c.Int(nullable: false),
                        Observacoes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.meuCuidado_Lembrete", "RelacionamentoIdosoProfissionalId", "dbo.meuCuidado_RelacionamentoIdosoProfissional");
            DropForeignKey("dbo.meuCuidado_Lembrete", "Medicamento_Id", "dbo.meuCuidado_Medicamento");
            DropForeignKey("dbo.meuCuidado_Avaliacao", "RelacionamentoIdosoProfissionalId", "dbo.meuCuidado_RelacionamentoIdosoProfissional");
            DropForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", "TutorId", "dbo.meuCuidado_Tutor");
            DropForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", "IdosoId", "dbo.meuCuidado_Idoso");
            DropForeignKey("dbo.meuCuidado_Medico", "Tutor_Id", "dbo.meuCuidado_Tutor");
            DropForeignKey("dbo.meuCuidado_Idoso", "TutorId", "dbo.meuCuidado_Tutor");
            DropForeignKey("dbo.meuCuidado_Idoso", "RelacionamentoIdosoProfissional_Id", "dbo.meuCuidado_RelacionamentoIdosoProfissional");
            DropForeignKey("dbo.meuCuidado_Medico", "IdosoId", "dbo.meuCuidado_Idoso");
            DropForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", "FisioterapeutaId", "dbo.meuCuidado_Fisioterapeuta");
            DropForeignKey("dbo.meuCuidado_Fisioterapeuta", "RelacionamentoIdosoProfissional_Id", "dbo.meuCuidado_RelacionamentoIdosoProfissional");
            DropForeignKey("dbo.meuCuidado_RelacionamentoIdosoProfissional", "CuidadorId", "dbo.meuCuidado_CuidadorDeIdoso");
            DropForeignKey("dbo.meuCuidado_CuidadorDeIdoso", "RelacionamentoIdosoProfissional_Id", "dbo.meuCuidado_RelacionamentoIdosoProfissional");
            DropForeignKey("dbo.meuCuidado_Avaliacao", "RelacionamentoIdosoProfissional_Id", "dbo.meuCuidado_RelacionamentoIdosoProfissional");
            DropIndex("dbo.meuCuidado_Lembrete", new[] { "Medicamento_Id" });
            DropIndex("dbo.meuCuidado_Lembrete", new[] { "RelacionamentoIdosoProfissionalId" });
            DropIndex("dbo.meuCuidado_Medico", new[] { "Tutor_Id" });
            DropIndex("dbo.meuCuidado_Medico", new[] { "IdosoId" });
            DropIndex("dbo.meuCuidado_Idoso", new[] { "RelacionamentoIdosoProfissional_Id" });
            DropIndex("dbo.meuCuidado_Idoso", new[] { "TutorId" });
            DropIndex("dbo.meuCuidado_Fisioterapeuta", new[] { "RelacionamentoIdosoProfissional_Id" });
            DropIndex("dbo.meuCuidado_CuidadorDeIdoso", new[] { "RelacionamentoIdosoProfissional_Id" });
            DropIndex("dbo.meuCuidado_RelacionamentoIdosoProfissional", new[] { "TutorId" });
            DropIndex("dbo.meuCuidado_RelacionamentoIdosoProfissional", new[] { "IdosoId" });
            DropIndex("dbo.meuCuidado_RelacionamentoIdosoProfissional", new[] { "FisioterapeutaId" });
            DropIndex("dbo.meuCuidado_RelacionamentoIdosoProfissional", new[] { "CuidadorId" });
            DropIndex("dbo.meuCuidado_Avaliacao", new[] { "RelacionamentoIdosoProfissional_Id" });
            DropIndex("dbo.meuCuidado_Avaliacao", new[] { "RelacionamentoIdosoProfissionalId" });
            DropTable("dbo.meuCuidado_Medicamento");
            DropTable("dbo.meuCuidado_Lembrete");
            DropTable("dbo.Documento");
            DropTable("dbo.Curriculo");
            DropTable("dbo.meuCuidado_Tutor");
            DropTable("dbo.meuCuidado_Medico");
            DropTable("dbo.meuCuidado_Idoso");
            DropTable("dbo.meuCuidado_Fisioterapeuta");
            DropTable("dbo.meuCuidado_CuidadorDeIdoso");
            DropTable("dbo.meuCuidado_RelacionamentoIdosoProfissional");
            DropTable("dbo.meuCuidado_Avaliacao");
        }
    }
}
