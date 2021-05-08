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

            //const string exampleFile = @"D:\Projects\Example Files\SixBricks.dae";
            const string exampleFile = @"D:\Projects\Example Files\castle.dae";
            Console.WriteLine($"Reading Collada file: {exampleFile}");

            var collada = new Collada(exampleFile);

            Console.WriteLine("Creating context: [{collada.Camera.Width}x{collada.Camera.Height}]");
            using (var renderer = new Renderer(collada.Camera.Width, collada.Camera.Height))
            {
                if (!renderer.InitialisedSuccessfully())
                {
                    Console.WriteLine("Context creation failed: check your OpenCL runtime and driver versions.");
                    return;
                }
                Console.WriteLine("Context successfully created.");

                // Camera
                renderer.SetCameraTransform(collada.Camera.Transform, 75f);

                // Lights
                var environmentLightImageId = renderer.CreateImage($"{executingPath}envLightImage.exr");
                _ = renderer.CreateLightFromImage(environmentLightImageId);

                // Materials

                var defaultMaterialId = renderer.CreateMaterial(MaterialNodeType.UBERV2);
                renderer.SetMaterialNode(defaultMaterialId, MaterialNodeInput.UBER_DIFFUSE_COLOR, 0.9f, 0.0f, 0.0f, 1f);
                renderer.SetMaterialNode(defaultMaterialId, MaterialNodeInput.UBER_DIFFUSE_WEIGHT, 1f, 1f, 1f, 1f);
                renderer.SetMaterialNode(defaultMaterialId, MaterialNodeInput.UBER_REFLECTION_COLOR, 0.5f, 0.5f, 0.5f,
                    1f);
                renderer.SetMaterialNode(defaultMaterialId, MaterialNodeInput.UBER_REFLECTION_WEIGHT, 0.5f, 0.5f, 0.5f,
                    1f);
                renderer.SetMaterialNode(defaultMaterialId, MaterialNodeInput.UBER_REFLECTION_ROUGHNESS, 0.1f, 0f, 0f,
                    0f);
                renderer.SetMaterialNode(defaultMaterialId, MaterialNodeInput.UBER_REFLECTION_MODE,
                    MaterialModeUber.METALNESS);
                renderer.SetMaterialNode(defaultMaterialId, MaterialNodeInput.UBER_REFLECTION_METALNESS, 0f, 0f, 0f, 1f);

                collada.ProcessGeometry(renderer, defaultMaterialId);

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
            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}