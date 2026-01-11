using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services;

public class AnexoService
{
    private readonly GoogleDriveService _googleDriveService;
    private readonly DefesaCivilDbContext _context;
    private readonly long MaxFileSize = 5 * 1024 * 1024;

    public AnexoService(GoogleDriveService googleDriveService, DefesaCivilDbContext context)
    {
        _googleDriveService = googleDriveService;
        _context = context;
    }

    public async Task<List<Anexo>> SalvarAnexoAsync(List<IFormFile> arquivos, string entidadeId, string tipoEntidade)
    {
        var anexosSalvos = new List<Anexo>();

        if (arquivos != null && arquivos.Any())
        {
            foreach (var arquivo in arquivos)
            {
                if (arquivo.Length > MaxFileSize)
                    throw new ArgumentException($"O arquivo {arquivo.FileName} excede o tamanho máximo permitido de 5MB.");

                // 1. Upload no Google Drive
                var uploadResult = await _googleDriveService.UploadFileAsync(arquivo);

                // 2. Criação do Objeto
                var anexo = new Anexo
                {
                    Id = Guid.NewGuid().ToString(), // Garante ID se o banco não gerar
                    NomeOriginal = arquivo.FileName,
                    UrlArmazenamento = uploadResult.WebViewLink,
                    IdArquivoExterno = uploadResult.FileId,
                    TipoConteudo = arquivo.ContentType,
                    TamanhoBytes = arquivo.Length,
                    EntidadeId = entidadeId,
                    TipoEntidade = tipoEntidade,
                    DataUpload = DateTime.UtcNow // Boa prática ter data
                };

                anexosSalvos.Add(anexo);
                _context.Anexos.Add(anexo);
            }

            await _context.SaveChangesAsync();
        }

        return anexosSalvos;
    }

    public async Task RemoverAnexosAsync(string entidadeTipo, string entidadeId, List<string> idsAnexosParaRemover)
    {
        if (idsAnexosParaRemover == null || !idsAnexosParaRemover.Any())
            return;

        var anexos = await _context.Anexos
            .Where(a => idsAnexosParaRemover.Contains(a.Id)
                      && a.EntidadeId == entidadeId
                      && a.TipoEntidade == entidadeTipo)
            .ToListAsync();

        if (!anexos.Any())
            return;

        // Deleta do Drive
        var tarefasDeExclusaoDrive = anexos
            .Where(x => !string.IsNullOrEmpty(x.IdArquivoExterno))
            .Select(anexo => _googleDriveService.DeleteFileAsync(anexo.IdArquivoExterno));

        try
        {
            await Task.WhenAll(tarefasDeExclusaoDrive);
        }
        catch (Exception)
        {
            // Logar erro, mas prosseguir para remover do banco para evitar inconsistência
            // _logger.LogError("Erro ao deletar do drive", ex);
        }

        // Deleta do Banco
        _context.Anexos.RemoveRange(anexos);
        await _context.SaveChangesAsync();
    }
}