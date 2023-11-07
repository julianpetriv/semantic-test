using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;

namespace Semantic_Petriv;

public static class Task1
{
    public static void Execute()
    {
        IGraph g = new Graph();
        g.LoadFromFile("/Users/julianpetriv/Desktop/countrues_info.ttl");

        TripleStore store = new TripleStore();
        store.Add(g);
        InMemoryDataset ds = new InMemoryDataset(store);
        ISparqlQueryProcessor processor = new LeviathanQueryProcessor(ds);

        string queryStr = @"
            PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
            PREFIX : <http://example.com/demo/>
            SELECT ?countryName ?maxSquareCountryNeighbourName
            WHERE {
                ?country rdf:type :Country.
                ?country :part_of_continent :Continent\/EU.
                ?country :country_name ?countryName
                OPTIONAL
                {
                    {
                        SELECT ?country (MAX(?square) as ?maxSquare)
                        WHERE {
                            ?country :has_country_neighbour ?countryNeighbour.
                            ?countryNeighbour :country_neighbour_value ?neighbourValue.
                            ?neighbourValue :area_in_sq_km ?square.
                        }
                        GROUP BY ?country
                    }
                    OPTIONAL
                    {
                        ?country :has_country_neighbour ?maxSquareCountryNeighbour.
                        ?maxSquareCountryNeighbour :country_neighbour_value ?maxSquareCountryNeighbourValue.
                        ?maxSquareCountryNeighbourValue :area_in_sq_km ?maxSquare.
                    }
                    ?maxSquareCountryNeighbourValue :country_name ?maxSquareCountryNeighbourName
                }
            }
            ORDER BY ?countryName
        ";
        
        SparqlQueryParser parser = new SparqlQueryParser();
        SparqlQuery query = parser.ParseFromString(queryStr);
        var results = (SparqlResultSet)processor.ProcessQuery(query);

        foreach (var result in results)
        {
            var countryName = ((LiteralNode)result["countryName"]).Value;
            result.TryGetValue("maxSquareCountryNeighbourName", out var biggestNeighbourCountry);
            var biggestNeighbourCountryName = ((LiteralNode)biggestNeighbourCountry)?.Value;

            Console.WriteLine($"{countryName} - biggest country is {biggestNeighbourCountryName}");
        }
    }
}