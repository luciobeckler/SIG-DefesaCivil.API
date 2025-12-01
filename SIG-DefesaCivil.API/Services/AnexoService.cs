using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;
using SIG_DefesaCivil.API.Services;

public class AnexoService
{
    private readonly GoogleDriveService _googleDriveService;
    private readonly DefesaCivilDbContext _context;
    private readonly int MaxFileSize = 5000000;

    public AnexoService(GoogleDriveService googleDriveService, DefesaCivilDbContext context)
    {
        _googleDriveService = googleDriveService;
        _context = context;
    }

    public async Task<List<IFormFile>?> SalvarAnexoAsync(List<IFormFile> arquivos, string entidadeId, string tipoEntidade)
    {
        if (arquivos != null && arquivos.Any())
        {
            foreach (var arquivo in arquivos)
            {
                if (arquivo.Length > MaxFileSize)
                    throw new ArgumentException("O arquivo excede o tamanho máximo permitido");

                var uploadResult = await _googleDriveService.UploadFileAsync(arquivo);

                var anexo = new Anexo
                {
                    NomeOriginal = arquivo.FileName,
                    UrlArmazenamento = uploadResult.WebViewLink,
                    IdArquivoExterno = uploadResult.FileId,
                    TipoConteudo = arquivo.ContentType,
                    TamanhoBytes = arquivo.Length,
                    EntidadeId = entidadeId,
                    TipoEntidade = tipoEntidade
                };
                _context.Anexos.Add(anexo);
            }
        }
        
        return arquivos;
    }

    public async Task RemoverAnexosAsync(string entidadeTipo, string eventoId, List<string> idsAnexosParaRemover)
    {
        if (idsAnexosParaRemover == null || !idsAnexosParaRemover.Any())
            return;

        var anexos = await _context.Anexos
            .Where(a => idsAnexosParaRemover.Contains(a.Id)
                     && a.EntidadeId == eventoId
                     && a.TipoEntidade == entidadeTipo)
            .ToListAsync();

        if (!anexos.Any())
            return;

        var tarefasDeExclusaoDrive = anexos
            .Select(anexo => _googleDriveService.DeleteFileAsync(anexo.IdArquivoExterno));

        try
        {
            await Task.WhenAll(tarefasDeExclusaoDrive);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao remover arquivos físicos no Drive.", ex);
        }

        _context.Anexos.RemoveRange(anexos);
        await _context.SaveChangesAsync();
    }
}