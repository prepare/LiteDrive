# LiteDB - A .NET NoSQL Document Store in a single data file

Same LiteDB features:

- Serverless NoSQL Document Store
- Simple API similar to MongoDB
- 100% C# code for .NET 3.5 in a single DLL - install from NuGet: `Install-Package LiteDB`
- Transaction control - ACID
- Recovery in writing failure (journal mode)
- Use with POCO class or BsonDocument
- Store files and stream data (like GridFS in MongoDB)
- Single data file storage (like SQLite)
- Up to 8 indexes per collection
- Open source and free for everyone - including commercial use

## How install

LiteDB is a serverless database, so there is no install. Just copy LiteDB.dll to your Bin folder and add as Reference. If you prefer, you can use NuGet package: `Install-Package LiteDB`. If you are running in a web environment, be sure that IIS user has write permission on data folder.

## How to use

In LiteDB, document are storage in collections. Each collection have many documents with same type. Each document have an `Id` (like 'Primary Key' on relation databases). Id datatype can support any indexed datatype (see below)

```C#
// Open data file (or create if not exits)
using(var db = new LiteEngine(@"C:\Temp\MyData.db"))
{
    // Get a collection (or create, if not exits)
    var col = db.GetCollection<Customer>("customers");
    
    var customer = new Customer { Id = 1, Name = "John Doe" };

	// Insert new customer document
	col.Insert(customer);
    
    // Update a document inside a collection
    customer.Name = "Joana Doe";
    
    col.Update(customer);
    
	// Query documents using a simple api for query data using indexed fields
	col.EnsureIndex("Name");
	
	var result = col.Find(Query.StartsWith("Jo"));
}
```
## Documents

LiteDB works with documents to store and retrive data inside data file. Your document definition can be a POCO class  or BsonDocument class. In both case, LiteDB will convert your document in a Bson format to store inside disk.

Bson is a Binary Json, a serialization for store data objects as binary array. In Bson, we have more data types than Json. LiteDB supports `Null`, `Array`, `Object`, `Byte`, `Char`, `Boolean`, `String`, `Short`, `Int`, `Long`, `UShort`, `UInt`, `ULong`, `Float`, `Double`, `Decimal`, `DateTime`, `Guid`.

In LiteDB, documents are limited in 256Kb.

### Documents using POCO class

POCO class are simple C# classes using only `get/set` properties. It's the best way to create a strong typed documents. Your class must have a `Id` property to LiteDB identify your document. You can use `Id` named property, `<YouClassName>Id` name or decorate a property with `[BsonId]` attribute. Your id key must be non-null value and have a valid indexed data type. See Index section.

``` C#
public class Customer
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public List<string> Phones { get; set; }
}
``` 

### Documents using BsonDocument

BsonDocument is a special class that maps any document with a internal `Dictionary<string, object>`. Is very useful to read a unknown document type or use as a generic document

```C#
// Create a BsonDocument and populate
var doc = new BsonDocument();
doc.Id = Guid.NewGuid();
doc["Name"] = "John Doe";
doc["Phones"] = new BsonArray();
doc["Phones"].Add("55(51)9900-0000");    
```

With BsonDocument you can create any complex document. You can use a fluent api too.

```C#
var doc = new BsonDocument()
	.Add("Name", "John")
	.Add("Age", 37);
```

## Indexes
falar de como indexa. Skiplist, index por colecao, só prop diretas, que tipo de dados sao aceitos. PK = _id_

## Query

In LiteDB, queries use indexes to search documents. At this moment, LiteDB do not support Linq, only `Query` helper class to create indexed query results. But, the result it´s a `IEnumerable<T>`, so you can Linq after query execute.

```C#
var customers = db.GetCollection<Customer>("customers");

// Create a new index (if not exists)
customers.EnsureIndex("Name");

// Query documents using 'Name' index
var results = customers.Find(Query.StartsWith("Name", "John"));

// Return document by ID (PK index)
var customer = customers.FindById(1);

// Count only documents where ID >= 2
var count = customers.Count(Query.GTE("_id", 2));

// All query results returns an IEnumerable<T>, so you can use Linq after
var linq = customers.Find(Query.Between("Salary", 500, 1000)) 
    .Where(x => x.LastName.Length > 5 && x.Age > 22)
    .Select(x => new { x.Name, x.Salary })
    .OrderBy(x => x.Name);
```

`Query` class supports `All`, `Equals`, `Not`, `GreaterThan`, `LessThan`, `Between`, `In`, `StartsWtih`, `AND` and `OR`.
All operations need an index to be executed.

##Transactions

All write operations are created inside a transaction. If you do not use `BeginTrans` and `Commit`, transaction are implicit for each operation.

For simplicity, LiteDB do not support concurrency transactions. LiteDB locks your datafile to guarantee that 2 users are not changing data at same time.

If there is any error during write data file, journaling save a redo log file with database dirty pages, to recovery your datafile when datafile open again. 

```C#
using(var db = new LiteEngine(dbpath))
{
    db.BeginTrans();
    
    // do many write operations (insert, updates, deletes)...
    
    db.Commit(); // Persist dirty pages to disk (use journal redo log file)
}
```



## Storing Files

Sametimes we need store files in database. For this, LiteDB has a special `Files` collection to store files without document size limit (file limit is 2Gb per file). It's works like MongoDB `GridFS`.

```C#
// Storing a file stream inside database with NameValueCollection metadata related
db.Files.Upload("my_key.png", stream, metadata);

// Get file reference using key
var file = db.Files.FindByKey("my_key.png");

// Find all files using StartsWith
var files = db.Files.Find("my_");

// Get file stream
var stream = file.OpenRead(db);

// Write file stream in a external stream
db.Files.Download("my_key.png", stream);

```

## Data Structure
explicar como estao estruturados os dados: tipos de paginas

## Connection String

Connection string options to initialize LiteEngine class:

- **Filename**: Path for datafile. You can use only path as connection string (required)
- **Timeout**: timeout for wait for unlock datafile (default: 00:01:00)
- **Journal**: Enabled journal mode - recovery support (default: true)
- **MaxFileLength**: Max datafile length, in bytes (default: 4TB)

## Where to use?

- Desktop/local applications
- Small web applications
- One database **per account/user** data store
- Few concurrency write users operations

## Dependency

LiteDB has no external dependency, but use [fastBinaryJson](http://fastbinaryjson.codeplex.com/) as Bson converter 
from/to .NET objects. All source are included inside LiteDB source.

## Roadmap

Currently, LiteDB is in early development version. There are many tests to be done before ready for production. Please, be careful on use.

Same features/ideas for future

- More tests!!
- A repository pattern
- DBRef (as in MongoDB)
- Linq support OR string query engine
- Compound index: one index for multiple fields
- Multikey index: index for array values
- Full text search
- Simple admin GUI program