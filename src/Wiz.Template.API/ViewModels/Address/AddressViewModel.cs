namespace Wiz.Template.API.ViewModels.Address
{
    public class AddressViewModel
    {
        public AddressViewModel() { }

        public AddressViewModel(int id, string cEP, string logradouro, string complemento, string bairro, string localidade, string uF)
        {
            Id = id;
            CEP = cEP;
            Logradouro = logradouro;
            Complemento = complemento;
            Bairro = bairro;
            Localidade = localidade;
            UF = uF;
        }

        public int Id { get; set; }
        public string CEP { get; set; }
        public string Logradouro { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Localidade { get; set; }
        public string UF { get; set; }
    }
        
}
