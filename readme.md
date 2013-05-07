# AzureGraphStorage

An extension to the .NET SDK for Windows Azure Storage which adds a triple store abstraction over Table Storage.

In simple terms, it makes it easy to use Table Storage as a graph database.

## Install

Install using the [nuget package](https://nuget.org/packages/AzureGraphStore/):

```
PM> Install-Package AzureGraphStore
```

## Example Usage

**Create a graph**

```
var account = CloudStorageAccount.DevelopmentStorageAccount;
var graphClient = account.CreateCloudGraphClient();
var graph = graphClient.GetGraphReference("example");
graph.CreateIfNotExists();
```

**Add triples to the graph**

```
// insert subject, property and value directly:
graph.Put("Richard", "Loves", "Cheese");

// insert a triple
var triple = new Triple("Richard", "Hates", "Marmite");
graph.Put(triple);
```

**Query the graph**

```
// query a single triple
var triple = graph.Get("Richard", "Loves", "Cheese").First();

// query using any combination of subject, property and value, i.e.
var triples = graph.Get(subject: "Richard");
var triples = graph.Get(property: "Loves");
var triples = graph.Get(values: "Cheese");
var triples = graph.Get(subject: "Richard", property: "Hates");
var triples = graph.Get(property: "Hates", value: "Marmite");
var triples = graph.Get(); // retrieving the entire graph is not recommended!
```

**Delete triples from the graph:**

```
graph.Delete("Richard", "Loves", "Cheese");
graph.Delete(triple);
```

## License

MIT
