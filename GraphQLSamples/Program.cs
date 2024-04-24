// See https://aka.ms/new-console-template for more information


using GraphQLSamples;

string filePath = "query.graphql";
string fileContent = File.ReadAllText(filePath);

Console.WriteLine(
  "TEST A"
    );

Console.WriteLine(
    GraphQLOperationExtractor.PreProcessRequest(
    fileContent,
    "testA")
    );

Console.WriteLine(
  "TEST B"
    );

Console.WriteLine(
    GraphQLOperationExtractor.PreProcessRequest(
    fileContent,
    "testB")
    );

Console.ReadLine();