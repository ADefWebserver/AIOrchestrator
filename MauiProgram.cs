﻿using AIOrchestrator.Model;
using Microsoft.Extensions.Logging;
using Radzen;

namespace AIOrchestrator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<LogService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<OrchestratorMethods>();

            // Radzen
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();

            // Load Default files
            var folderPath = "";
            var filePath = "";

            // AIOrchestrator Directory
            folderPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/AIOrchestrator";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // AIOrchestrator Documents Directory
            var folderDocumentsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/AIOrchestrator/Documents";
            if (!Directory.Exists(folderDocumentsPath))
            {
                Directory.CreateDirectory(folderDocumentsPath);

                // Copy all files Assets to the AIOrchestrator Documents Directory
                List<string> files = new List<string>
                {
                    "A Room with a View.txt",
                    "A Tale of Two Cities.txt",
                    "The Great Gatsby.txt"
                };

                foreach (var file in files)
                {
                    using var stream = FileSystem.OpenAppPackageFileAsync(file);
                    using var reader = new StreamReader(stream.Result);
                    var text = reader.ReadToEnd();
                    File.WriteAllText(Path.Combine(folderDocumentsPath, file), text);
                }
            }

            // AIOrchestratorLog.csv
            filePath = Path.Combine(folderPath, "AIOrchestratorLog.csv");

            if (!File.Exists(filePath))
            {
                using (var streamWriter = new StreamWriter(filePath))
                {
                    streamWriter.WriteLine("Application started at " + DateTime.Now);
                }
            }
            else
            {
                // File already exists
                string[] AIOrchestratorLog;

                // Open the file to get existing content
                using (var file = new System.IO.StreamReader(filePath))
                {
                    AIOrchestratorLog = file.ReadToEnd().Split('\n');

                    if (AIOrchestratorLog[AIOrchestratorLog.Length - 1].Trim() == "")
                    {
                        AIOrchestratorLog = AIOrchestratorLog.Take(AIOrchestratorLog.Length - 1).ToArray();
                    }
                }

                // Append the text to csv file
                using (var streamWriter = new StreamWriter(filePath))
                {
                    streamWriter.WriteLine(string.Join("\n", "Application started at " + DateTime.Now));
                    streamWriter.WriteLine(string.Join("\n", AIOrchestratorLog));
                }
            }

            // AIOrchestratorMemory.csv
            filePath = Path.Combine(folderPath, "AIOrchestratorMemory.csv");

            if (!File.Exists(filePath))
            {
                using (var streamWriter = new StreamWriter(filePath))
                {
                    streamWriter.WriteLine("** AIOrchestratorMemory started at " + DateTime.Now + "|");
                }
            }

            // AIOrchestratorDatabase.json
            filePath = Path.Combine(folderPath, "AIOrchestratorDatabase.json");

            if (!File.Exists(filePath))
            {
                using (var streamWriter = new StreamWriter(filePath))
                {
                    streamWriter.WriteLine(
                        """
                        {                         
                        }
                        """);
                }
            }

            // AIOrchestratorSettings.config
            filePath = Path.Combine(folderPath, "AIOrchestratorSettings.config");

            if (!File.Exists(filePath))
            {
                using (var streamWriter = new StreamWriter(filePath))
                {
                    streamWriter.WriteLine(
                        """
                        {
                         "OpenAIServiceOptions": {
                         "Organization": "** Your OpenAI Organization **",
                         "ApiKey": "** Your OpenAI ApiKey **" } 
                        }
                        """);
                }
            }

            return builder.Build();
        }
    }
}