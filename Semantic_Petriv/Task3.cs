using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;

namespace Semantic_Petriv;

public static class Task3
{
    public static void Execute()
    {
        SparqlRemoteEndpoint endpoint =
            new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");

        string q = @"
            PREFIX dbo: <http://dbpedia.org/ontology/>
            PREFIX dbp: <http://dbpedia.org/property/>

            SELECT ?actor1Alias ?actor2Alias ?movieName
            WHERE
            {
              ?actor1 a dbo:Actor.
              ?actor2 a dbo:Actor.
              ?actor1 dbp:name ?actor1Alias.
              ?actor2 dbp:name ?actor2Alias.
              ?actor1 dbo:wikiPageID ?actor1Id.
              ?actor2 dbo:wikiPageID ?actor2Id.
              ?movie dbo:starring ?actor1.
              ?movie dbo:starring ?actor2.
              ?movie dbp:name ?movieName.
              ?actor1 dbo:spouse ?actor2.
              FILTER (?actor1 != ?actor2)
            }
        ";

        SparqlResultSet results = endpoint.QueryWithResultSet(q);

        foreach (var result in results)
        {
            Console.WriteLine($"Actor 1: {result["actor1Alias"]} - Actor 2: {result["actor2Alias"]} - Movie: {result["movieName"]}");
        }
    }
}