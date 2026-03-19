using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services.Integration;

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

    public async Task<List<Anexo>> SalvarAnexosEmLoteAsync(List<AnexoUploadDTO> fileData, string entidadeId, ETiposEntidades tipoEntidade)
    {
        var anexosParaSalvar = new List<Anexo>();

        // 1. Validação Prévia (Fail Fast)
        foreach (var item in fileData)
        {
            // CRÍTICA APLICADA: Tem que acessar a propriedade .Anexo
            if (item == null)
                throw new ArgumentException("Um dos itens não contém um anexo válido.");

            if (item.Anexo.Length > MaxFileSize)
                throw new ArgumentException($"O anexo {item.Anexo.FileName} excede 5MB.");

            if (!AllowedMimeTypes.Contains(item.Anexo.ContentType))
                throw new ArgumentException($"Tipo de anexo não permitido: {item.Anexo.ContentType}");
        }

        // 2. Upload Paralelo para o Google Drive
        var uploadTasks = fileData.Select(async item =>
        {
            // CRÍTICA APLICADA: Passar o IFormFile para o Drive
            var result = await _googleDriveService.UploadFileAsync(item.Anexo);

            return new Anexo
            {
                Id = Guid.NewGuid().ToString(),
                UrlArmazenamento = result.WebViewLink,
                EntidadeId = entidadeId,
                NomeOriginal = item.Anexo.FileName,
                TamanhoBytes = item.Anexo.Length,
                TipoConteudo = item.Anexo.ContentType,
                IdAnexoExterno = result.FileId,
                TipoEntidade = tipoEntidade,
                DataUpload = DateTime.UtcNow,
                Localizacao =
                {
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
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
            var idsParaApagar = anexosParaSalvar.Select(a => a.IdAnexoExterno).Where(id => !string.IsNullOrEmpty(id));
            foreach (var idDrive in idsParaApagar)
            {
                await _googleDriveService.DeleteFileAsync(idDrive);
            }
            throw new Exception("Falha ao salvar anexos. Operação cancelada.", ex);
        }
    }

    public async Task RemoverAnexosAsync(List<string> idsAnexosParaRemover)
    {
        if (idsAnexosParaRemover.IsNullOrEmpty()) return;

        var anexos = await _context.Anexos
            .Where(a => idsAnexosParaRemover.Contains(a.Id))
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
            .Where(x => !string.IsNullOrEmpty(x.IdAnexoExterno))
            .Select(a => _googleDriveService.DeleteFileAsync(a.IdAnexoExterno));

        await Task.WhenAll(tasksDelecao);
    }
}