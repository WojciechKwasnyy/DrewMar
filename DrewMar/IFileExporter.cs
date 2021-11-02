using DrewMar.Domain;

namespace DrewMar
{
    internal interface IFileExporter
    {
        void Export(string targetPath, Transport transport);
    }
}
