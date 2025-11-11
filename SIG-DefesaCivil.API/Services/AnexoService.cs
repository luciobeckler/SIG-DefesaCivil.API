using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.Models;
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

    public async Task<Anexo> SalvarAnexoAsync(IFormFile arquivo, string entidadeId, string tipoEntidade)
    {
        // 1. Validações (tamanho, tipo, etc. - como antes)
        // ... (if arquivo.Length > MaxFileSize...) ...
        if (arquivo.Length > MaxFileSize)
            throw new ArgumentException("O arquivo excede o tamanho máximo permitido");

        // 2. Salva no Google Drive
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

        return anexo;
    }

    public async Task ExcluirAnexoAsync(string anexoId)
    {
        var anexo = await _context.Anexos.FindAsync(anexoId);
        if (anexo == null) return;

        await _googleDriveService.DeleteFileAsync(anexo.IdArquivoExterno);

        _context.Anexos.Remove(anexo);
    }
}