namespace RoslynCompiler;

public class CompilationResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public byte[]? AssemblyBytes { get; set; }
}
