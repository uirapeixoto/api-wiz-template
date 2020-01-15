using Bogus;
using Wiz.Template.Domain.Models.Services;

namespace Wiz.Template.Integration.Tests.Mocks
{
    public static class ViaCEPMock
    {
        public static Faker<ViaCEP> ViaCEPModelFaker =>
            new Faker<ViaCEP>()
            .CustomInstantiator(x => new ViaCEP
            (
                cep: x.Address.ZipCode(),
                logradouro: x.Address.StreetAddress(),
                localidade: x.Address.StreetName(),
                bairro: x.Address.Locale,
                complemento: x.Address.StreetName(),
                uf: x.Address.CountryCode(Bogus.DataSets.Iso3166Format.Alpha2)
            ));
    }
}
