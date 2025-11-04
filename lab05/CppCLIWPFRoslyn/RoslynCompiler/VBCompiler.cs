using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic;

namespace RoslynCompiler;

public class VBCompiler
{
    public static CompilationResult CompileVBCode(string code, OutputKind outputKind)
    {
        var result = new CompilationResult();

        try
        {
            if (outputKind == OutputKind.ConsoleApplication || 
                outputKind == OutputKind.WindowsApplication ||
                outputKind == OutputKind.WindowsRuntimeApplication)
            {
                outputKind = OutputKind.DynamicallyLinkedLibrary;
            }

            var syntaxTree = VisualBasicSyntaxTree.ParseText(code);

            var diagnostics = syntaxTree.GetDiagnostics();
            if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                result.Success = false;
                result.Errors.Add("! Istniej¹ b³êdy sk³adni!");
                foreach (var diag in diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                {
                    result.Errors.Add(diag.ToString());
                }
                return result;
            }

            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                .Split(Path.PathSeparator);

            var references = trustedAssembliesPaths
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

            string assemblyName = Path.GetRandomFileName();

            var compilationOptions = new VisualBasicCompilationOptions(
                outputKind,
                optimizationLevel: OptimizationLevel.Debug,
                platform: Platform.AnyCpu);

            var compilation = VisualBasicCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                compilationOptions);

            using (var ms = new MemoryStream())
            {
                EmitResult emitResult = compilation.Emit(ms);

                if (!emitResult.Success)
                {
                    result.Success = false;
                    result.Errors = emitResult.Diagnostics
                        .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                        .Select(diagnostic => diagnostic.ToString())
                        .ToList();
                }
                else
                {
                    result.Success = true;
                    ms.Seek(0, SeekOrigin.Begin);
                    result.AssemblyBytes = ms.ToArray();
                    
                    result.Errors.Add($"Pomyœlnie skompilowano: assembly - {result.AssemblyBytes.Length}b");
                }
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Wyj¹tek! {ex.Message}");
            if (ex.InnerException != null)
            {
                result.Errors.Add($"Content: {ex.InnerException.Message}");
            }
            result.Errors.Add($"Stos (œlad): {ex.StackTrace}");
        }

        return result;
    }
}
