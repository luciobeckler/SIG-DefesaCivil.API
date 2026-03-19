using Google.Apis.Drive.v3;
using Google.Apis.Upload;

namespace SIG_DefesaCivil.API.Services.Integration
{
    public class GoogleDriveUploadResult
    {
        public string FileId { get; set; }
        public string WebViewLink { get; set; }
    }

    public class GoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly string _folderId;
        private readonly ILogger<GoogleDriveService> _logger;

        // O DriveService já vem autenticado via Injeção de Dependência
        public GoogleDriveService(DriveService driveService, IConfiguration configuration, ILogger<GoogleDriveService> logger)
        {
            _driveService = driveService;
            _logger = logger;
            _folderId = configuration["GoogleDrive:FolderId"];
        }

        public async Task<GoogleDriveUploadResult> UploadFileAsync(IFormFile file)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = $"{Guid.NewGuid()}_{file.FileName}",
                Parents = new[] { _folderId }
            };

            FilesResource.CreateMediaUpload request;

            using (var stream = file.OpenReadStream())
            {
                request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id, webViewLink";

                // Realiza o upload
                var progress = await request.UploadAsync();

                if (progress.Status == UploadStatus.Failed)
                {
                    _logger.LogError(progress.Exception, "Falha no upload para o Google Drive: {FileName}", file.FileName);
                    throw progress.Exception;
                }
            }

            var fileResult = request.ResponseBody;

            if (fileResult == null)
            {
                throw new Exception("O Google Drive não retornou metadados após o upload.");
            }

            // Define permissão pública (se for requisito do negócio)
            await TornarPublicoAsync(fileResult.Id);

            return new GoogleDriveUploadResult
            {
                FileId = fileResult.Id,
                WebViewLink = fileResult.WebViewLink
            };
        }

        public async Task DeleteFileAsync(string fileId)
        {
            if (string.IsNullOrEmpty(fileId)) return;

            try
            {
                await _driveService.Files.Delete(fileId).ExecuteAsync();
                _logger.LogInformation("Anexo removido do Google Drive: {FileId}", fileId);
            }
            catch (Exception ex)
            {
                // Loga como Warning para não poluir o log de erros graves, já que o anexo pode já não existir
                _logger.LogWarning(ex, "Erro não bloqueante ao excluir anexo do Google Drive: {FileId}", fileId);
            }
        }

        private async Task TornarPublicoAsync(string fileId)
        {
            try
            {
                var permission = new Google.Apis.Drive.v3.Data.Permission()
                {
                    Type = "anyone",
                    Role = "reader"
                };
                await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Não foi possível tornar o anexo {FileId} público.", fileId);
                // Não lançamos throw aqui para não cancelar o processo todo só por causa da permissão
            }
        }
    }
}