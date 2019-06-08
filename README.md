# CSharpAnalyser

## Background and Purpose

This project was created as part of an assignment. The application takes a text file containing SQL scripts and performs some basic parsing into an abstract syntax tree (AST).

## Assumptions/Limitations
- When specifying a directory, the solution assumes all the files belong to a singular project
- Logging was done using `Console.WriteLine`. This should rather be abstracted out into a logger.
- Variables are declared one per line/statement
- While some automated tests were created, they are definitely not comprehensive

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
