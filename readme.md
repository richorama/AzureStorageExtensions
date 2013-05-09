# Azure Storage Extensions

A set of extensions to the Azure Storage SDK, to easily enable new types of storage, as abstractions over existing services.

## AzureGraphStore

An extension to the .NET SDK for Windows Azure Storage which adds a triple store abstraction over Table Storage.

In simple terms, it makes it easy to use Table Storage as a graph database.

### Installation

Install using the [nuget package](https://nuget.org/packages/AzureGraphStore/):

```
PM> Install-Package AzureGraphStore
```

### Example Usage

**Create a graph**

```c#
var account = CloudStorageAccount.DevelopmentStorageAccount;
var graphClient = account.CreateCloudGraphClient();
var graph = graphClient.GetGraphReference("example");
graph.CreateIfNotExists();
```

**Add triples to the graph**

```c#
// insert subject, property and value directly:
graph.Put("Richard", "Loves", "Cheese");

// insert a triple
var triple = new Triple("Richard", "Hates", "Marmite");
graph.Put(triple);
```

**Query the graph**

```c#
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

```c#
graph.Delete("Richard", "Loves", "Cheese");
graph.Delete(triple);
```

## AzureJsonStore

A strongly typed object store, using JSON to persist data in blob storage.

### Installation

Install using the [nuget package](https://nuget.org/packages/AzureJsonStore/):

```
PM> Install-Package AzureJsonStore
```

### Example Usage

We can use any POCO class, as long as it can be serialized to JSON. For this sample, we'll use this 'Foo' class:

```c#
class Foo
{
    public string Bar { get; set; }
    public int Baz { get; set; }
}
```

**Create a store**

```c#
var account = CloudStorageAccount.DevelopmentStorageAccount;
var jsonStoreClient = account.CreateCloudJsonStoreClient();
var store = jsonStoreClient.GetJsonStoreReference<Foo>("foostore");
store.CreateIfNotExists();
```

**Add objects to the store**

```c#
store.Put("foo1", new Foo{ Bar = "Hello", Baz = 3});
```

**Get objects from the store**

```c#
var foo = store.Get("foo1");
```

**Query the store for objects**

```c#
var foos = store.Query();
```

**Delete objects from the store**

```c#
store.Delete("foo1");
```

## License

MIT
