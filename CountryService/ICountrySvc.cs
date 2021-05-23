using ModelService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CountryService
{
    public interface ICountrySvc
    {
        Task<List<CountryModel>> GetCountriesAsync();
    }
}
