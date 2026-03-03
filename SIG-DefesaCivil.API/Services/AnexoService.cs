using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services;

public class AnexoService
{
    private readonly GoogleDriveService _googleDriveService;
    private readonly DefesaCivilDbContext _context;
    private readonly long MaxFileSize = 5 * 1024 * 1024;

    private readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "application/pdf" };

    public AnexoService(GoogleDriveService googleDriveService, DefesaCivilDbContext context)
    {
        _googleDriveService = googleDriveService;
        _context = context;
    }

    public async Task<List<Anexo>> SalvarAnexosEmLoteAsync(List<ArquivoUploadDTO> fileData, string entidadeId, ETiposEntidades tipoEntidade)
    {
        var anexosParaSalvar = new List<Anexo>();

        // 1. Validação Prévia (Fail Fast)
        foreach (var item in fileData)
        {
            // CRÍTICA APLICADA: Tem que acessar a propriedade .Arquivo
            if (item == null)
                throw new ArgumentException("Um dos itens não contém um arquivo válido.");

            if (item.Arquivo.Length > MaxFileSize)
                throw new ArgumentException($"O arquivo {item.Arquivo.FileName} excede 5MB.");

            if (!AllowedMimeTypes.Contains(item.Arquivo.ContentType))
                throw new ArgumentException($"Tipo de arquivo não permitido: {item.Arquivo.ContentType}");
        }

        // 2. Upload Paralelo para o Google Drive
        var uploadTasks = fileData.Select(async item =>
        {
            // CRÍTICA APLICADA: Passar o IFormFile para o Drive
            var result = await _googleDriveService.UploadFileAsync(item.Arquivo);

            return new Anexo
            {
                Id = Guid.NewGuid().ToString(),
                NomeOriginal = item.Arquivo.FileName,
                UrlArmazenamento = result.WebViewLink,
                IdArquivoExterno = result.FileId,
                TipoConteudo = item.Arquivo.ContentType,
                TamanhoBytes = item.Arquivo.Length,
                TipoEntidade = tipoEntidade,
                DataUpload = DateTime.UtcNow,
                Localizacao =
                {
                    Longitude = item.Localizacao.Longitude,
                    Latitude = item.Localizacao.Latitude
                },
                DataHoraCaptura = item.DataHoraCaptura
            };
        });

        try
        {
            var resultados = await Task.WhenAll(uploadTasks);
            anexosParaSalvar.AddRange(resultados);

            // 3. Salvar metadados no Banco de Dados
            await _context.Anexos.AddRangeAsync(anexosParaSalvar);
            await _context.SaveChangesAsync();

            return anexosParaSalvar;
        }
        catch (Exception ex)
        {
            var idsParaApagar = anexosParaSalvar.Select(a => a.IdArquivoExterno).Where(id => !string.IsNullOrEmpty(id));
            foreach (var idDrive in idsParaApagar)
            {
                await _googleDriveService.DeleteFileAsync(idDrive);
            }
            throw new Exception("Falha ao salvar anexos. Operação cancelada.", ex);
        }
    }

    public async Task RemoverAnexosAsync(ETiposEntidades entidadeTipo, string entidadeId, List<string> idsAnexosParaRemover)
    {
        // ... (Mantido exatamente igual ao seu código, a lógica de remoção está correta)
        if (idsAnexosParaRemover == null || !idsAnexosParaRemover.Any()) return;

        var anexos = await _context.Anexos
            .Where(a => idsAnexosParaRemover.Contains(a.Id)
                     && a.EntidadeId == entidadeId
                     && a.TipoEntidade == entidadeTipo)
            .ToListAsync();

        if (!anexos.Any()) return;

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
            throw;
        }

        var tasksDelecao = anexos
            .Where(x => !string.IsNullOrEmpty(x.IdArquivoExterno))
            .Select(a => _googleDriveService.DeleteFileAsync(a.IdArquivoExterno));

        await Task.WhenAll(tasksDelecao);
    }
}