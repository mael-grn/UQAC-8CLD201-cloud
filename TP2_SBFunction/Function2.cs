using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using static System.Net.Mime.MediaTypeNames;

namespace TP2_SBFunction
{
    public class Function2
    {
        [FunctionName("Function2")]
        public void Run([ServiceBusTrigger("messagequeue", Connection = "ServiceBusConnection")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            // 1. Récupérer la connection string pour le blob storage
            string blobStorageConnectionString = Environment.GetEnvironmentVariable("ConnectionString");
            string containerName = "blob"; // Nom du conteneur où se trouve le blob

            // 2. Créer un client BlobServiceClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(blobStorageConnectionString);

            // 3. Récupérer le client pour le conteneur
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // 4. Récupérer le blob (l'image) à partir de son nom (reçu dans le message)
            BlobClient blobClient = containerClient.GetBlobClient(myQueueItem);

            try
            {
                // 5. Télécharger l'image dans un MemoryStream
                using (MemoryStream originalImageStream = new MemoryStream())
                {
                    blobClient.DownloadTo(originalImageStream);
                    originalImageStream.Position = 0; // Important : Remettre le curseur au début du stream

                    // 6. Redimensionner l'image avec ImageSharp
                    using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(originalImageStream))
                    {
                        // Définir les nouvelles dimensions (exemple : 100x100)
                        int newWidth = 100;
                        int newHeight = 100;

                        // Redimensionner avec le mode ResizeMode.Max pour conserver les proportions
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(newWidth, newHeight),
                            Mode = ResizeMode.Max
                        }));

                        // 7. Enregistrer l'image redimensionnée dans un nouveau MemoryStream
                        using (MemoryStream resizedImageStream = new MemoryStream())
                        {
                            image.SaveAsJpeg(resizedImageStream); // Vous pouvez choisir un autre format si nécessaire
                            resizedImageStream.Position = 0; // Important : Remettre le curseur au début du stream

                            // 8. Remplacer le blob existant par l'image redimensionnée
                            blobClient.Upload(resizedImageStream, true); // Le 'true' ici permet d'écraser le blob existant

                            log.LogInformation($"Blob '{myQueueItem}' redimensionné et remplacé avec succès.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Erreur lors du traitement du blob '{myQueueItem}': {ex.Message}");
            }
        }
    }
}