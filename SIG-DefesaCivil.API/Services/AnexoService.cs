using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services;

public class AnexoService
{
    private readonly GoogleDriveService _googleDriveService;
    private readonly DefesaCivilDbContext _context;
    private readonly long MaxFileSize = 5 * 1024 * 1024;

    // Lista de tipos permitidos para segurança
    private readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "application/pdf" };

    public AnexoService(GoogleDriveService googleDriveService, DefesaCivilDbContext context)
    {
        _googleDriveService = googleDriveService;
        _context = context;
    }

    public async Task<List<Anexo>> SalvarAnexosEmLoteAsync(List<IFormFile> arquivos, string entidadeId, string tipoEntidade)
    {
        var anexosParaSalvar = new List<Anexo>();

        // 1. Validação Prévia (Fail Fast)
        foreach (var arquivo in arquivos)
        {
            if (arquivo.Length > MaxFileSize)
                throw new ArgumentException($"O arquivo {arquivo.FileName} excede 5MB.");

            if (!AllowedMimeTypes.Contains(arquivo.ContentType))
                throw new ArgumentException($"Tipo de arquivo não permitido: {arquivo.ContentType}");
        }

        // 2. Upload Paralelo para o Google Drive
        // Criamos uma lista de Tasks, mas não rodamos o await ainda
        var uploadTasks = arquivos.Select(async arquivo =>
        {
            var result = await _googleDriveService.UploadFileAsync(arquivo);
            return new Anexo
            {
                Id = Guid.NewGuid().ToString(),
                NomeOriginal = arquivo.FileName,
                UrlArmazenamento = result.WebViewLink,
                IdArquivoExterno = result.FileId,
                TipoConteudo = arquivo.ContentType,
                TamanhoBytes = arquivo.Length,
                EntidadeId = entidadeId,
                TipoEntidade = tipoEntidade,
                DataUpload = DateTime.UtcNow
            };
        });

        try
        {
            // Executa todos os uploads simultaneamente e aguarda
            var resultados = await Task.WhenAll(uploadTasks);
            anexosParaSalvar.AddRange(resultados);

            // 3. Salvar metadados no Banco de Dados
            await _context.Anexos.AddRangeAsync(anexosParaSalvar);
            await _context.SaveChangesAsync();

            return anexosParaSalvar;
        }
        catch (Exception ex)
        {
            // ROLLBACK MANUAL: Se der erro no banco ou em algum upload, 
            // precisamos apagar os arquivos que porventura subiram para o Drive.
            var idsParaApagar = anexosParaSalvar.Select(a => a.IdArquivoExterno).Where(id => !string.IsNullOrEmpty(id));
            foreach (var idDrive in idsParaApagar)
            {
                await _googleDriveService.DeleteFileAsync(idDrive); // Fire and forget ou await seguro
            }
            throw new Exception("Falha ao salvar anexos. Operação cancelada.", ex);
        }
    }

    public async Task RemoverAnexosAsync(string entidadeTipo, string entidadeId, List<string> idsAnexosParaRemover)
    {
        if (idsAnexosParaRemover == null || !idsAnexosParaRemover.Any()) return;

        var anexos = await _context.Anexos
            .Where(a => idsAnexosParaRemover.Contains(a.Id)
                     && a.EntidadeId == entidadeId
                     && a.TipoEntidade == entidadeTipo)
            .ToListAsync();

        if (!anexos.Any()) return;

        // Estratégia Segura: 
        // 1. Remove do Banco PRIMEIRO. Se falhar aqui, o arquivo fica "falso" no drive, mas o sistema não quebra.
        // Se remover do Drive primeiro e o banco falhar, o usuário vê o anexo na tela mas o link não abre (pior UX).

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Anexos.RemoveRange(anexos);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw; // Não continua para deletar do Drive
        }

        // 2. Agora que saiu do banco, remove do Drive (Operação de limpeza)
        // Usamos Task.WhenAll para ser rápido
        var tasksDelecao = anexos
            .Where(x => !string.IsNullOrEmpty(x.IdArquivoExterno))
            .Select(a => _googleDriveService.DeleteFileAsync(a.IdArquivoExterno));

        await Task.WhenAll(tasksDelecao);
    }
}