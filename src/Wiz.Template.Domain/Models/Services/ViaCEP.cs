using System.Runtime.Serialization;

namespace Wiz.Template.Domain.Models.Services
{
    [DataContract(Name = "endereco")]
    public class ViaCEP
    {
        public ViaCEP() { }

        public ViaCEP(string cep, string logradouro, string complemento, string bairro, string localidade, string uf)
        {
            CEP = cep;
            Logradouro = logradouro;
            Complemento = complemento;
            Bairro = bairro;
            Localidade = localidade;
            UF = uf;
        }
        [DataMember(Name = "cep")]
        public string CEP { get; set; }
        [DataMember(Name = "logradouro")]
        public string Logradouro { get; set; }
        [DataMember(Name = "complemento")]
        public string Complemento { get; set; }
        [DataMember(Name = "bairro")]
        public string Bairro { get; set; }
        [DataMember(Name = "localidade")]
        public string Localidade { get; set; }
        [DataMember(Name = "uf")]
        public string UF { get; set; }
    }
}
