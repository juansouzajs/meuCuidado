namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEtapaAcessoUsuario : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.meuCuidado_Lembrete", "MedicamentoId");
            RenameColumn(table: "dbo.meuCuidado_Lembrete", name: "Medicamento_Id", newName: "MedicamentoId");
            RenameIndex(table: "dbo.meuCuidado_Lembrete", name: "IX_Medicamento_Id", newName: "IX_MedicamentoId");
            AddColumn("dbo.meuCuidado_CuidadorDeIdoso", "EtapaAcesso", c => c.Int(nullable: false));
            AddColumn("dbo.meuCuidado_Fisioterapeuta", "EtapaAcesso", c => c.Int(nullable: false));
            AddColumn("dbo.meuCuidado_Idoso", "EtapaAcesso", c => c.Int(nullable: false));
            AddColumn("dbo.meuCuidado_Medico", "EtapaAcesso", c => c.Int(nullable: false));
            AddColumn("dbo.meuCuidado_Tutor", "EtapaAcesso", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.meuCuidado_Tutor", "EtapaAcesso");
            DropColumn("dbo.meuCuidado_Medico", "EtapaAcesso");
            DropColumn("dbo.meuCuidado_Idoso", "EtapaAcesso");
            DropColumn("dbo.meuCuidado_Fisioterapeuta", "EtapaAcesso");
            DropColumn("dbo.meuCuidado_CuidadorDeIdoso", "EtapaAcesso");
            RenameIndex(table: "dbo.meuCuidado_Lembrete", name: "IX_MedicamentoId", newName: "IX_Medicamento_Id");
            RenameColumn(table: "dbo.meuCuidado_Lembrete", name: "MedicamentoId", newName: "Medicamento_Id");
            AddColumn("dbo.meuCuidado_Lembrete", "MedicamentoId", c => c.Int());
        }
    }
}
