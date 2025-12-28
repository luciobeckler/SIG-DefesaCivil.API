using SIG_DefesaCivil.API.Services;

namespace SIG_DefesaCivil.API.Workers
{
    public class AutomacaoMovimentacaoWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutomacaoMovimentacaoWorker> _logger;

        public AutomacaoMovimentacaoWorker(IServiceProvider serviceProvider, ILogger<AutomacaoMovimentacaoWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de Movimentação Automática Iniciado.");

            // Loop infinito que roda enquanto a aplicação estiver de pé
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Verificando ocorrências vencidas...");

                    // Cria um escopo para poder usar o Entity Framework e Services
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var ocorrenciaService = scope.ServiceProvider.GetRequiredService<OcorrenciaService>();

                        await ocorrenciaService.ProcessarMovimentacoesAutomaticas();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar verificação automática.");
                }

                // Define o intervalo de verificação.
                // Exemplo: Verifica a cada 1 hora (TimeSpan.FromHours(1)) ou a cada 10 minutos.
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}

