
using GraphQLParser;
using GraphQLParser.AST;
using GraphQLParser.Visitors;
using System.Text;

namespace GraphQLSamples;
public class GraphQLOperationExtractor
{
    public static string PreProcessRequest(string originalRequest, string operationName)
    {
        var document = Parser.Parse(originalRequest);

        var operation = document.Definitions.OfType<GraphQLOperationDefinition>()
        .FirstOrDefault(op => op.Name == operationName);

        if (operation == null)
        {
            throw new ArgumentException($"Operation '{operationName}' not found in the document.");
        }

        var operationDefinition = document.Definitions.OfType<GraphQLOperationDefinition>()
            .First(op => op.Name == operationName);

        var fragments = CollectFragments(operationDefinition.SelectionSet, document);

        var sb = new StringBuilder();
        new SDLPrinter().Print(operationDefinition, sb);
        foreach( var fragment in fragments )
        {
            sb.AppendLine();
            new SDLPrinter().Print(fragment, sb);
        }


        return sb.ToString();
    }

    public static List<GraphQLFragmentDefinition> CollectFragments(GraphQLSelectionSet selectionSet, GraphQLDocument document)
    {
        var fragments = new List<GraphQLFragmentDefinition>();

        foreach (var selection in selectionSet.Selections)
        {
            if (selection is GraphQLFragmentSpread spread)
            {
                // Find the fragment definition by name
                var fragment = document.Definitions.OfType<GraphQLFragmentDefinition>()
                    .FirstOrDefault(f => f.FragmentName.Name == spread.FragmentName.Name);

                if (fragment != null)
                {
                    fragments.Add(fragment);
                    fragments.AddRange(CollectFragments(fragment.SelectionSet, document));
                }
            }
            else if (selection is GraphQLField fieldSelection)
            {
                if (fieldSelection.SelectionSet != null)
                {
                    fragments.AddRange(CollectFragments(fieldSelection.SelectionSet, document));
                }
            }
        }

        return fragments;
    }
}
