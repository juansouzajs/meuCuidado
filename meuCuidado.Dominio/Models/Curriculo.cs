using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Dominio.Models
{
    public class Curriculo
    {
        public int Id { get; set; }

        public Guid IdentificadorUnico { get; set; }

        public string Nome { get; set; }

        public int AnosExperiencia { get; set; }

        // ESCOLARIDADE
        public string EscolaridadeNivel { get; set; }

        public string EscolaridadeNome { get; set; }

        // JSONS
        public string CursosJson { get; set; }

        public string ExperienciasJson { get; set; }

        public string RedesSociaisJson { get; set; }

        // AVALIAÇÕES
        public double AvaliacaoMedia { get; set; }

        public int NumeroAvaliacoes { get; set; }

        // USUÁRIO
        public TipoUsuario TipoUsuario { get; set; }

        public int UsuarioId { get; set; }

        [NotMapped]
        public List<string> Cursos
        {
            get => string.IsNullOrEmpty(CursosJson)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(CursosJson);

            set => CursosJson =
                JsonConvert.SerializeObject(value ?? new List<string>());
        }

        [NotMapped]
        public List<string> Experiencias
        {
            get => string.IsNullOrEmpty(ExperienciasJson)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(ExperienciasJson);

            set => ExperienciasJson =
                JsonConvert.SerializeObject(value ?? new List<string>());
        }

        [NotMapped]
        public List<string> RedesSociais
        {
            get => string.IsNullOrEmpty(RedesSociaisJson)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(RedesSociaisJson);

            set => RedesSociaisJson =
                JsonConvert.SerializeObject(value ?? new List<string>());
        }
    }
}