# DocumentDbDemo
This is a demo to show the strange behaviours of DocumentDb on Azure. Its based on an internal project where i have stripped out practically all the business logic, removed all test projects bar the failing tests we are investigating here.

## Setup
You will need:
*  An Azure account
*  A DocumentDb account
*  A documentDb database
*  A collection

You then need to fill in the config inside `DocumentDbDemo.Data.Tests\app.config`
Run the tests. Im using the resharper runner, but ncrunch exhibits the same random failures

## Whats the matter?
Some times both tests pass, some times one or the other fails, sometimes both fail

### (Update)
Ive found that if i use a different database then the tests start to pass. The only difference between the two is that one database had the collections added and removed over and over by the tests, the other did not.
I have changed the tests found here so that they more closely resemble the tests we are running on our real project, in that they create and tear down the collections every time its run.
So what im saying is that you might have to run this thing many times to get the failure as it looks like its a problem with a database that has had its collections added and removed many times.


## Fail 1 [Resource with specified id or name already exists]
```
System.AggregateException : One or more errors occurred.
  ----> Microsoft.Azure.Documents.DocumentClientException : Message: {"Errors":["Resource with specified id or name already exists"]}
ActivityId: e273b9d6-b571-43d3-9802-c7d7c819a3f0, Request URI: /apps/c9c8f510-0ca7-4702-aa6c-9c596d797367/services/507e2a70-c787-437c-9587-0ff4341bc265/partitions/ae4ca317-e883-4419-84f9-c8d053ffc73d/replicas/131159218637566393p
   at System.Threading.Tasks.Task.ThrowIfExceptional(Boolean includeTaskCanceledExceptions)
   at System.Threading.Tasks.Task.Wait(Int32 millisecondsTimeout, CancellationToken cancellationToken)
   at System.Threading.Tasks.Task.Wait()
   at DocumentDbDemo.Data.AggregateRepository.CreateCollectionIfNotExists() in K:\_code\VisualStudio\DocumentDbPerfTests\DocumentDbDemo.Data\AggregateRepository.cs:line 32
   at DocumentDbDemo.Data.AggregateRepository..ctor(ConfigFactory configFactory) in K:\_code\VisualStudio\DocumentDbPerfTests\DocumentDbDemo.Data\AggregateRepository.cs:line 19
   at DocumentDbDemo.Data.Tests.AggregateRepositoryTests.ShouldReturnNullIfNotFound() in K:\_code\VisualStudio\DocumentDbPerfTests\DocumentDbDemo.Data.Tests\AggregateRepositoryTests.cs:line 24
--DocumentClientException
...
...
```

I do not understand this one bit. The exception is occuring in the code that first checks if the collection exists (it does) then if it does not creates it. Clearly the create will fail, the collection exists!!
```
try
{
    _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_config.DocumentDbDatabaseId, _config.DocumentDbCollectionId)).Wait();
}
catch (AggregateException e)
{
    if (((DocumentClientException)e.InnerException).StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        _client.CreateDocumentCollectionAsync(
            UriFactory.CreateDatabaseUri(_config.DocumentDbDatabaseId),
            new DocumentCollection { Id = _config.DocumentDbCollectionId },
            new RequestOptions { OfferThroughput = 1000 }).Wait();
    }
    else
    {
        throw;
    }
}
``` 

## Fail 2 [Owner resource does not exist]
```
System.AggregateException : One or more errors occurred.
  ----> Microsoft.Azure.Documents.DocumentClientException : Message: {"Errors":["Owner resource does not exist"]}
ActivityId: 9e25516a-25fe-4bf3-a88d-6234c76ac47d, Request URI: /apps/c9c8f510-0ca7-4702-aa6c-9c596d797367/services/507e2a70-c787-437c-9587-0ff4341bc265/partitions/ae4ca317-e883-4419-84f9-c8d053ffc73d/replicas/131159551041924002s
   at System.Threading.Tasks.Task.ThrowIfExceptional(Boolean includeTaskCanceledExceptions)
   at System.Threading.Tasks.Task.Wait(Int32 millisecondsTimeout, CancellationToken cancellationToken)
   at System.Threading.Tasks.Task.Wait()
   at DocumentDbDemo.Data.Tests.AggregateRepositoryTests.ShouldSaveNewAggregate(AggregateRepository aggregateRepository) in K:\_code\VisualStudio\DocumentDbPerfTests\DocumentDbDemo.Data.Tests\AggregateRepositoryTests.cs:line 48
   at DocumentDbDemo.Data.Tests.AggregateRepositoryTests.ShouldSaveAndReadTheDocument() in K:\_code\VisualStudio\DocumentDbPerfTests\DocumentDbDemo.Data.Tests\AggregateRepositoryTests.cs:line 42
--DocumentClientException
...
...
```

This is equally baffling, the collection again exists, but the document does not, we are creating it for the first time with a unique GUID, this is the code in the repo that is failing

```
public async Task SaveAsync(Aggregate aggregate)
{
    await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_config.DocumentDbDatabaseId, _config.DocumentDbCollectionId), aggregate);
}
```