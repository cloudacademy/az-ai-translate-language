using System.Text;
using Azure.Security.KeyVault.Secrets;
using Azure.AI.Translation.Document;

bool AsynchronousTranslateDocument = true;
string targetLanguage = "de";
string endpoint = "https://b00ktranslator.cognitiveservices.azure.com/";

// Get Translator secret from Key Vault
string keyVaultUrl = "https://talk-talk-kv.vault.azure.net/";
string secretName = "booktranslator-key1";
var keyVaultManager = new KeyVaultManager(keyVaultUrl);
KeyVaultSecret resourceKey = await keyVaultManager.GetSecretAsync(secretName);
string apiKey = resourceKey.Value;

if (AsynchronousTranslateDocument)
{
    string sourceContainerUrl = @"https://b00ks.blob.core.windows.net/fostermother";
    string targetContainerUrl = @"https://b00ks.blob.core.windows.net/fostermotherde";
    DocTranslationManager docTranslationManager = new DocTranslationManager(endpoint, sourceContainerUrl, targetContainerUrl,  targetLanguage);
    await docTranslationManager.TranslateDocumentAsync(apiKey);
}
else
{
    SingleDocumentTranslationClient client = new SingleDocumentTranslationClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));

    try
    {
        string filePath = @"D:\CloudAcademy\AI-102\FosterMother\FM_Chapter_1.txt";
        using Stream fileStream = File.OpenRead(filePath);

        /* Content types supported for translation are:
            application/pdf: PDF documents
            application/vnd.openxmlformats-officedocument.wordprocessingml.document: Microsoft Word documents (.docx)
            application/vnd.openxmlformats-officedocument.spreadsheetml.sheet: Microsoft Excel documents (.xlsx)
            application/vnd.openxmlformats-officedocument.presentationml.presentation: Microsoft PowerPoint documents (.pptx)
            text/plain: Plain text files (.txt)
            application/rtf: Rich Text Format files (.rtf)
        */

        var sourceDocument = new MultipartFormFileData(Path.GetFileName(filePath), fileStream, "text/plain"); 
        DocumentTranslateContent content = new DocumentTranslateContent(sourceDocument);
        var response = client.DocumentTranslate(targetLanguage, content);

        string outputPath = @$"D:\CloudAcademy\AI-102\FosterMother\{targetLanguage}_FM_Chapter_1.txt";
        using (Stream outputStream = File.OpenWrite(outputPath))
        {
            outputStream.Write(Encoding.UTF8.GetBytes(response.Value.ToString()));
        }
        Console.WriteLine($"Response string after translation: {response.Value.ToString()}");
    }
    catch (Exception exception) 
    {
        Console.WriteLine($"Error Code: {exception.HResult}");
        Console.WriteLine($"Message: {exception.Message}");
    }
}