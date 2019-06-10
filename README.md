# CSharpAnalyser

## Background and Purpose

This project was created as part of an assignment. The application takes a directory of c-sharp files, and performs some static code analysis checks on them. Note that the analysis was only done using the Roslyn syntax API; use of the semantic API was not allowed.

## Assumptions/Limitations
- When specifying a directory, the solution assumes all the files belong to a singular project
- Logging was done using `Console.WriteLine`. This should rather be abstracted out into a logger.
- The analysers assume variables are declared one per line/statement
- While some automated tests were created, they are definitely not comprehensive
- Other assumptions/limitations mentioned in comments

## Setup
- Clone this repo
- Navigate to the `CSharpAnalyser` (root) directory
- Run: `dotnet restore`

## Running the console application
```bash
dotnet run --project CSharpAnalyser.App "ExampleCode"
```

## Running the tests
```bash
dotnet test
```

## Building and running with Docker
```
docker build -t csharp-analyser .
docker run --rm -v "$(pwd)/ExampleCode:/app/ToBeAnalysed" csharp-analyser ToBeAnalysed
```
