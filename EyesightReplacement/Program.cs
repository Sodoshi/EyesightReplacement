using System;
using System.IO;
using System.Reflection;
using EyesightReplacement.Enums;
using EyesightReplacement.Processor;
using EyesightReplacement.Services;
namespace EyesightReplacement
{
    internal static class Program
    {
        private static string GetExecutingPath()
        {
            var location = new Uri(Assembly.GetEntryAssembly()?.GetName().CodeBase ?? string.Empty);
            var executingPath = new FileInfo(location.AbsolutePath).Directory?.FullName ?? string.Empty;
            return executingPath.EndsWith(@"\") ? executingPath : executingPath + @"\";
        }
        private static void Initialize(string[] args)
        {
            Console.WriteLine(string.Join("\n", args));

            var executingPath = GetExecutingPath();

            //const string exampleFile = @"D:\Projects\Example Files\plates.dae";
            //const string exampleFile = @"D:\Projects\Example Files\sixbricks.dae";
            //const string exampleFile = @"D:\Projects\Example Files\castle.dae";
            const string exampleFile = @"D:\Projects\Example Files\boat.dae";
            Console.WriteLine($"Reading Collada file: {exampleFile}");

            var collada = new Collada(exampleFile);

            Console.WriteLine($"Creating context: [{collada.CameraWidth}x{collada.CameraHeight}]");
            using (var renderer = new Renderer(collada.CameraWidth, collada.CameraHeight))
            {
                if (!renderer.InitialisedSuccessfully)
                {
                    Console.WriteLine("Context creation failed: check your OpenCL runtime and driver versions.");
                    return;
                }
                Console.WriteLine("Context successfully created.");

                // Additional Lights
                var environmentLightImageId = renderer.CreateImage($"{executingPath}envLightImage.exr");
                _ = renderer.CreateLightFromImage(environmentLightImageId);

                collada.RenderScene(renderer);

                Console.WriteLine("Rendering...");
                var fileName = $@"D:\{Guid.NewGuid().ToString("N").ToLower()}.png";
                renderer.RenderToImage(fileName, 64);
                Console.WriteLine($"File created: {fileName}");
            }
        }
        [STAThread]
        private static void Main(string[] args)
        {
            Initialize(args);
            //Console.WriteLine("Press a key to exit.");
            //Console.ReadKey();
            Environment.Exit(0);
        }
    }
}