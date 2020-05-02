using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ServerlessMoedas.Data;
using ServerlessMoedas.Models;

namespace ServerlessMoedas
{
    public class MoedasQueueTrigger
    {
        private MoedasContext _context;

        public MoedasQueueTrigger(MoedasContext context)
        {
            _context = context;
        }

        [FunctionName("MoedasQueueTrigger")]
        public void Run([QueueTrigger("queue-cotacoes", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            var cotacao =
                JsonSerializer.Deserialize<Cotacao>(myQueueItem);

            if (!String.IsNullOrWhiteSpace(cotacao.Sigla) &&
                cotacao.Valor.HasValue && cotacao.Valor > 0)
            {
                Cotacao dadosCotacao = _context.Cotacoes
                            .Where(c => c.Sigla == cotacao.Sigla)
                            .FirstOrDefault();
                if (dadosCotacao == null)
                {
                    dadosCotacao = new Cotacao();
                    dadosCotacao.Sigla = cotacao.Sigla;
                    _context.Add(dadosCotacao);
                }

                dadosCotacao.UltimaCotacao = DateTime.Now;
                dadosCotacao.Valor = cotacao.Valor;
                _context.SaveChanges();

                log.LogInformation($"MoedasQueueTrigger: {myQueueItem}");
            }
            else
                log.LogError($"MoedasQueueTrigger - Erro validação: {myQueueItem}");
        }
    }
}