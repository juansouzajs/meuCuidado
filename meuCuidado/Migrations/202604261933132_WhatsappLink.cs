namespace meuCuidado.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WhatsappLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.meuCuidado_CuidadorDeIdoso", "LinkWhatsapp", c => c.String());
            AddColumn("dbo.meuCuidado_Fisioterapeuta", "LinkWhatsapp", c => c.String());
            AddColumn("dbo.meuCuidado_Idoso", "LinkWhatsapp", c => c.String());
            AddColumn("dbo.meuCuidado_Medico", "LinkWhatsapp", c => c.String());
            AddColumn("dbo.meuCuidado_Tutor", "LinkWhatsapp", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.meuCuidado_Tutor", "LinkWhatsapp");
            DropColumn("dbo.meuCuidado_Medico", "LinkWhatsapp");
            DropColumn("dbo.meuCuidado_Idoso", "LinkWhatsapp");
            DropColumn("dbo.meuCuidado_Fisioterapeuta", "LinkWhatsapp");
            DropColumn("dbo.meuCuidado_CuidadorDeIdoso", "LinkWhatsapp");
        }
    }
}
