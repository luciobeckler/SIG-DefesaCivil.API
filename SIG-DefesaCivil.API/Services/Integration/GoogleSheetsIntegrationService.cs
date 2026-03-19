using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using SIG_DefesaCivil.API.Models.Ocorrencia;
using System.Text.RegularExpressions;

namespace SIG_DefesaCivil.API.Services.Integration
{
    public class GoogleSheetsIntegrationService
    {
        private readonly SheetsService _sheetsService;
        private readonly string _spreadsheetId = "1BdcJnzJ5xODrPmSy8z-NRn7FhMQxB317RT0hGthseJ4";

        public GoogleSheetsIntegrationService(SheetsService sheetsService)
        {
            _sheetsService = sheetsService;
        }

        public async Task<int?> InserirOcorrenciaAsync(Ocorrencia ocorrencia)
        {
            try
            {
                string sheetName = GetSheetName(ocorrencia.Protocolo);
                await EnsureSheetExistsAsync(sheetName);

                var objectList = SetObjectList(ocorrencia);
                var valueRange = new ValueRange { Values = new List<IList<object>> { objectList } };

                var range = GetRangeForInsert(sheetName, objectList.Count);

                var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadsheetId, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                var response = await appendRequest.ExecuteAsync();
                var updatedRange = response.Updates?.UpdatedRange;

                return getUpdatedRow(updatedRange);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao integrar com Sheets: {ex.Message}");
                return null;
            }
        }
        public async Task<int?> AtualizarOcorrenciaAsync(Ocorrencia ocorrencia)
        {
            try
            {
                string sheetName = GetSheetName(ocorrencia.Protocolo);
                await EnsureSheetExistsAsync(sheetName);

                // Se a ocorrência já não tiver linha definida na base de dados, força a busca
                if (ocorrencia.LinhaExcel == null || ocorrencia.LinhaExcel <= 0)
                {
                    ocorrencia.LinhaExcel = await EncontrarLinhaPorProtocoloAsync(ocorrencia.Protocolo, sheetName);
                    if (ocorrencia.LinhaExcel == null)
                    {
                        Console.WriteLine($"Falha crítica: Protocolo {ocorrencia.Protocolo} não existe na folha.");
                        return null;
                    }
                }
                else
                {
                    // Validação de Integridade: Verifica se a linha guardada ainda está correta
                    var checkRange = $"'{sheetName}'!A{ocorrencia.LinhaExcel.Value}";
                    var getRequest = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, checkRange);
                    var getResponse = await getRequest.ExecuteAsync();

                    var currentValues = getResponse.Values;
                    string valorEncontrado = (currentValues != null && currentValues.Count > 0 && currentValues[0].Count > 0)
                                             ? currentValues[0][0].ToString()
                                             : string.Empty;

                    if (valorEncontrado != ocorrencia.Protocolo)
                    {
                        Console.WriteLine($"Inconsistência: Linha {ocorrencia.LinhaExcel.Value} contém '{valorEncontrado}'. Reavaliando posição...");

                        // Fallback: Varredura completa na coluna A para reencontrar o protocolo
                        ocorrencia.LinhaExcel = await EncontrarLinhaPorProtocoloAsync(ocorrencia.Protocolo, sheetName);

                        if (ocorrencia.LinhaExcel == null)
                        {
                            Console.WriteLine($"Falha crítica: O protocolo {ocorrencia.Protocolo} foi apagado manualmente da folha.");
                            return null;
                        }
                    }
                }

                // Prossegue com a atualização na linha (agora garantida como correta)
                var objectList = SetObjectList(ocorrencia);
                var valueRange = new ValueRange { Values = new List<IList<object>> { objectList } };

                var updateRange = GetRangeForUpdate(sheetName, objectList.Count, ocorrencia.LinhaExcel.Value);

                var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, updateRange);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                var updateResponse = await updateRequest.ExecuteAsync();

                if (updateResponse.UpdatedRows > 0)
                {
                    // Retorna o número da linha. O seu Controller DEVE atualizar a base de dados SQL com este número.
                    return ocorrencia.LinhaExcel;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar ocorrência no Sheets: {ex.Message}");
                return null;
            }
        }

        // Método auxiliar para fazer a varredura da Coluna A
        private async Task<int?> EncontrarLinhaPorProtocoloAsync(string protocolo, string sheetName)
        {
            var searchRange = $"'{sheetName}'!A:A";
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, searchRange);
            var response = await request.ExecuteAsync();

            if (response.Values == null) return null;

            for (int i = 0; i < response.Values.Count; i++)
            {
                if (response.Values[i].Count > 0 && response.Values[i][0]?.ToString() == protocolo)
                {
                    return i + 1; // A API do Google Sheets utiliza índices baseados em 1
                }
            }

            return null;
        }

        private string GetSheetName(string protocolo)
        {
            var parts = protocolo.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[0], out int year))
            {
                return $"OCORRENCIAS_{year}";
            }

            throw new FormatException($"Formato de protocolo inválido: {protocolo}. Esperado YYYY-X.");
        }

        // 2. Método para garantir que a aba do ano exista
        private async Task EnsureSheetExistsAsync(string sheetName)
        {
            var spreadsheet = await _sheetsService.Spreadsheets.Get(_spreadsheetId).ExecuteAsync();
            bool exists = spreadsheet.Sheets.Any(s => s.Properties.Title == sheetName);

            if (!exists)
            {
                int newSheetId = new Random().Next(1, 1000000000);

                // 1. Comando para criar a aba e congelar a primeira linha
                var addSheetRequest = new Request
                {
                    AddSheet = new AddSheetRequest
                    {
                        Properties = new SheetProperties
                        {
                            SheetId = newSheetId,
                            Title = sheetName,
                            GridProperties = new GridProperties
                            {
                                FrozenRowCount = 1
                            }
                        }
                    }
                };

                // Nomes literais das colunas (devem estar na exata mesma ordem do SetObjectList)
                string[] colunas = {
                    "N_DA_VISTORIA", "DATA_SOLICITACAO", "HORARIO", "NOME DO SOLICITANTE",
                    "CPF/IDENTIDADE","ENDEREÇO","N°","BAIRRO","TELEFONE","TIPIFICACAO_OCORRENCIA",
                    "ABERTURA DA VISTORIA","DATA DA VISTORIA","VISTORIADORES","GRAU_RISCO","NOTIFICAÇÃO",
                    "INTERDIÇÃO","STATUS_VISTORIA","STATUS_RELATORIO","DATA_RELATORIO","OBSERVAÇÃO",
                };

                var headerValues = new List<CellData>();

                foreach (var col in colunas)
                {
                    headerValues.Add(new CellData
                    {
                        UserEnteredValue = new ExtendedValue { StringValue = col },
                        UserEnteredFormat = new CellFormat
                        {
                            TextFormat = new TextFormat { Bold = true },
                            BackgroundColor = new Color { Red = 0.9f, Green = 0.9f, Blue = 0.9f } // Cinza claro
                        }
                    });
                }

                // 2. Comando para injetar os dados e a formatação na Linha 0 (A1:T1)
                var headerRequest = new Request
                {
                    UpdateCells = new UpdateCellsRequest
                    {
                        Start = new GridCoordinate { SheetId = newSheetId, RowIndex = 0, ColumnIndex = 0 },
                        Rows = new List<RowData> { new RowData { Values = headerValues } },
                        Fields = "userEnteredValue,userEnteredFormat(textFormat,backgroundColor)"
                    }
                };

                var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
                {
                    // Empacota tudo em uma única chamada de rede
                    Requests = new List<Request> { addSheetRequest, headerRequest }
                };

                await _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, _spreadsheetId).ExecuteAsync();
            }
        }


        private int? getUpdatedRow(string range)
        {

            if (!string.IsNullOrEmpty(range))
            {
                var match = Regex.Match(range, @"[A-Z]+(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int rowIndex))
                {
                    return rowIndex;
                }
            }

            Console.WriteLine("Ocorrência inserida, mas o Google não retornou o Range válido.");
            return null;
        }


        // Algoritmo real de conversão de índice para letra de coluna (suporta AA, AB, etc.)
        private string GetColumnName(int index)
        {
            const int dividend = 26;
            string columnName = string.Empty;
            int modifier;

            while (index > 0)
            {
                modifier = (index - 1) % dividend;
                columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
                index = (index - modifier) / dividend;
            }
            return columnName;
        }

        private string GetRangeForInsert(string sheetName, int size)
        {
            string endColumn = GetColumnName(size);
            return $"'{sheetName}'!A:{endColumn}";
        }

        private string GetRangeForUpdate(string sheetName, int size, int rowIndex)
        {
            string endColumn = GetColumnName(size);
            return $"'{sheetName}'!A{rowIndex}:{endColumn}{rowIndex}";
        }

        private static List<object> SetObjectList(Ocorrencia ocorrencia)
        {
            return new List<object>()
            {
                FormatValue(ocorrencia.Protocolo),
                FormatValue(ocorrencia.Campos.DataEHoraDoOcorrido?.Date),
                FormatValue(ocorrencia.Campos.DataEHoraDoOcorrido?.TimeOfDay),
                FormatValue(ocorrencia.Campos.Solicitante?.Nome),
                FormatValue(ocorrencia.Campos.Solicitante?.CPF),
                FormatValue(ocorrencia.Campos.Localizacao?.Rua),
                FormatValue(ocorrencia.Campos.Localizacao?.Numero),
                FormatValue(ocorrencia.Campos.Localizacao?.Bairro),
                FormatValue(ocorrencia.Campos.Solicitante?.Telefone),
                FormatValue(ocorrencia.Campos.TipificacaoDaOcorrencia),
                FormatValue(ocorrencia.Responsavel?.UserName),
                FormatValue(ocorrencia.Campos?.DataEHoraInicioAtendimento?.Date),
                FormatValue(ocorrencia.Responsavel?.UserName),
                FormatValue(ocorrencia.Campos.GrauDeRisco),
                FormatValue("Sim"), //Notificacao?
                FormatValue("Não"), //Interdição?
                FormatValue(ocorrencia.Etapa?.Nome),
                FormatValue("Concluido"), //Status relatório
                FormatValue(ocorrencia.Campos.DataEHoraTerminoAtendimento),
                FormatValue("Observação")
            };
        }

        private static string FormatValue(object? v)
        {
            if (v == null) return string.Empty;
            if (v is string s) return s;
            if (v is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
            if (v is TimeSpan ts) return ts.ToString(@"hh\:mm\:ss");
            if (v is System.Collections.IEnumerable seq && !(v is string))
            {
                var items = seq.Cast<object?>().Select(x => x?.ToString() ?? string.Empty);
                return string.Join(", ", items);
            }
            return v.ToString() ?? string.Empty;
        }
    }
}