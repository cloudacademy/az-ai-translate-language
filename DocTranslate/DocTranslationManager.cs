using Azure.AI.Translation.Document;
using Azure;
public class DocTranslationManager
{
    private Uri endpoint;
    private Uri sourceContainer;
    private Uri targetContainer;
    private string targetLanguage;

    public DocTranslationManager(string endpointUrl, string sourceContainerUrl, string targetContainerUrl, string targetLang)
    {
        targetLanguage = targetLang;
        endpoint = new Uri(endpointUrl);
        sourceContainer = new Uri(sourceContainerUrl);
        targetContainer = new Uri(targetContainerUrl);
    }

    public async Task TranslateDocumentAsync(string key)
    {
        var client = new DocumentTranslationClient(endpoint, new AzureKeyCredential(key));

        var input = new DocumentTranslationInput(sourceContainer, targetContainer, targetLanguage);
        DocumentTranslationOperation operation = await client.StartTranslationAsync(input);

        await operation.WaitForCompletionAsync();

        await foreach(DocumentStatusResult document in operation.Value) 
        {
            Console.WriteLine($"Document with Id: {document.Id}");
            Console.WriteLine($"  Status:{document.Status}");
            if (document.Status == DocumentTranslationStatus.Succeeded) 
            {
                Console.WriteLine($"  Translated Document Uri: {document.TranslatedDocumentUri}");
                Console.WriteLine($"  Translated to language: {document.TranslatedToLanguageCode}.");
                Console.WriteLine($"  Document source Uri: {document.SourceDocumentUri}");
            } 
            else 
            {
                Console.WriteLine($"  Error Code: {document.Error.Code}");
                Console.WriteLine($"  Message: {document.Error.Message}");
            }
        }
    }
}