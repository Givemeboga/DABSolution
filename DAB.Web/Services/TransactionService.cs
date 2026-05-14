using System.Net.Http.Json;
using DAB.Web.Models;

namespace DAB.Web.Services
{
    public class TransactionService
    {
        private readonly HttpClient _http;

        public TransactionService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("API");
        }

        public async Task<List<Transaction>> GetAll()
        {
            return await _http.GetFromJsonAsync<List<Transaction>>("api/transactions") ?? new List<Transaction>();
        }

        public async Task Retrait(Transaction t)
        {
            await _http.PostAsJsonAsync("api/transactions/retrait", t);
        }

        public async Task Transfert(Transaction t)
        {
            await _http.PostAsJsonAsync("api/transactions/transfert", t);
        }
    }
}
