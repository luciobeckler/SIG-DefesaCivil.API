// /Services/Storage/GoogleDriveService.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SIG_DefesaCivil.API.Services
{
    // Objeto de retorno (pode estar em um arquivo DTO separado)
    public class GoogleDriveUploadResult
    {
        public string FileId { get; set; }
        public string WebViewLink { get; set; } // Link para visualização
    }

    public class GoogleDriveService
    {
        private readonly string _folderId;
        private readonly DriveService _driveService;

        public GoogleDriveService(IConfiguration configuration)
        {
            _folderId = configuration["GoogleDrive:FolderId"];
            _driveService = Authenticate(configuration);
        }

        private DriveService Authenticate(IConfiguration configuration)
        {
            var clientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets
            {
                ClientId = configuration["GoogleDrive:ClientId"],
                ClientSecret = configuration["GoogleDrive:ClientSecret"]
            };

            var token = new TokenResponse
            {
                RefreshToken = configuration["GoogleDrive:RefreshToken"]
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.Scope.Drive }
            });

            var credential = new UserCredential(flow, "user", token);

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SIG-DefesaCivil-API"
            });
        }

        public async Task<GoogleDriveUploadResult> UploadFileAsync(IFormFile file)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = $"{Guid.NewGuid()}_{file.FileName}", // Garante nome único
                Parents = new[] { _folderId } // Define a pasta de destino
            };

            FilesResource.CreateMediaUpload request;
            Google.Apis.Upload.IUploadProgress progress;

            using (var stream = file.OpenReadStream())
            {
                request = _driveService.Files.Create(
                    fileMetadata, stream, file.ContentType);
                request.Fields = "id, webViewLink";
                progress = await request.UploadAsync();
            }

            if (progress.Status == Google.Apis.Upload.UploadStatus.Failed)
            {
                throw progress.Exception;
            }

            var fileResult = request.ResponseBody;
            if (fileResult == null)
            {
                throw new Exception("Falha no upload para o Google Drive: Upload concluído, mas nenhum metadado foi retornado.");
            }

            // Tornar o arquivo público para qualquer pessoa com o link
            var permission = new Google.Apis.Drive.v3.Data.Permission()
            {
                Type = "anyone",
                Role = "reader"
            };
            await _driveService.Permissions.Create(permission, fileResult.Id).ExecuteAsync();

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
            }
            catch (Exception ex)
            {
                // Logar o erro (ex), mas não parar a execução
                Console.WriteLine($"Erro ao excluir arquivo do Google Drive: {ex.Message}");
            }
        }
    }
}