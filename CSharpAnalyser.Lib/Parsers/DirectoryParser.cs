using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Parsers
{
    public static class DirectoryParser
    {
        private const string CSharpFileExtension = "cs";
        private const string ProjectName = "CodeToBeAnalysed";

        public static Project Parse(string path)
        {
            if (!Directory.Exists(path)) throw new ArgumentException($"Cannot find directory {path}");

            var workspace = new AdhocWorkspace();
            var projectInfo = CreateProjectInfo();
            workspace.AddProject(projectInfo);

            string searchPattern = $"*.{CSharpFileExtension}";
            foreach(string filePath in Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(filePath);
                var fullPath = Path.GetFullPath(filePath);
                Console.WriteLine($"DEBUG: Adding {fullPath} to workspace");
                workspace.AddDocument(DocumentInfo.Create(
                    id: DocumentId.CreateNewId(projectInfo.Id),
                    name: fileInfo.FullName,
                    loader: new FileTextLoader(fullPath, null)
                ));
            }

            return workspace.CurrentSolution.Projects.Single();
        }

        private static ProjectInfo CreateProjectInfo(string projectName = ProjectName)
        {
            return ProjectInfo.Create(
                id: ProjectId.CreateNewId(),
                version: VersionStamp.Create(),
                name: projectName,
                assemblyName: projectName,
                language: LanguageNames.CSharp
            );
        }
    }
}
