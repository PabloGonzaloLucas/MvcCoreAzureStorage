using Azure.Data.Tables;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageTables
    {
        private TableClient tableClient;

        public ServiceStorageTables(TableServiceClient table)
        {
            this.tableClient = table.GetTableClient("clientes");
        }

        public async Task CreateClientAsync(int id, string nombre,
            string empresa, int edad, int salario)
        {
            Cliente client = new Cliente
            {
                IdCliente = id,
                Nombre = nombre,
                Empresa = empresa,
                Edad = edad,
                Salario = salario
            };
            await this.tableClient.AddEntityAsync<Cliente>(client);
        }

        public async Task<Cliente> FindClienteAsync(string partitionKey,
            string rowKey)
        {
            Cliente cliente = await this.tableClient.GetEntityAsync<Cliente>
                (partitionKey, rowKey);
            return cliente;
        }

        public async Task DeleteClienteAsync(string partitionKey, string rowKey)
        {
            await this.tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<List<Cliente>> GetClientesAsync()
        {
            List<Cliente> clientes = new List<Cliente>();
            var query = this.tableClient.QueryAsync<Cliente>(filter: "");
            await foreach (var item in query)
            {
                clientes.Add(item);
            }
            return clientes;
        }

        public async Task<List<Cliente>> GetClientesEmpresaAsync(string empresa)
        {
            string filtro = "Salario gt 250000 and Salario lt 150000";
            var query = this.tableClient.QueryAsync<Cliente>(filter: filtro);
            var query2 = this.tableClient.Query<Cliente>(x => x.Empresa == empresa);
            return query2.ToList();
        }
    }
}
