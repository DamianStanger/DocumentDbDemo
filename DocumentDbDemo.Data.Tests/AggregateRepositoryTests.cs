using System;
using System.Collections.Generic;
using DocumentDbDemo.Service;
using DocumentDbDemo.Service.Configuration;
using FluentAssertions;
using Ingenie.BehaviourReviewService.Contracts.Events;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;

namespace DocumentDbDemo.Data.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class AggregateRepositoryTests
    {        
        private readonly ConfigFactory _configFactory = new ConfigFactory();
        private Aggregate _aggregateToSave;


        [Test]
        [Category("Integration")]
        public void ShouldReturnNullIfNotFound()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                Clean();

                var aggregateRepository = new AggregateRepository(_configFactory);

                var retreivedAggregate = aggregateRepository.Read("MissingPolicyNumber");

                retreivedAggregate.Should().BeNull();
            }
        }

        [Test]
        [Category("Integration")]
        public void ShouldSaveAndReadTheDocument()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                Clean();

                _aggregateToSave = new AggregateFactory().NewAggregate(Guid.NewGuid().ToString(), DateTime.Parse("1-Jan-2016"));
                var journeyEvents = new List<JourneyEvent>() { new JourneyEvent() { Category = Category.CategoryS, Score = 99 } };
                var journeyAllocatedToPolicy = new JourneyAllocatedToPolicy() { Distance = 35d, PolicyNumber = "myFunkyPolicyNumber", StartedAt = DateTime.Parse("2-Jan-2016"), Events = journeyEvents };
                _aggregateToSave.Add(journeyAllocatedToPolicy);

                var aggregateRepository = new AggregateRepository(_configFactory);

                ShouldSaveNewAggregate(aggregateRepository);
                ShouldReadAggregate(aggregateRepository);
            }
        }

        private void ShouldSaveNewAggregate(AggregateRepository aggregateRepository)
        {
            aggregateRepository.SaveAsync(_aggregateToSave).Wait();
        }

        private void ShouldReadAggregate(AggregateRepository aggregateRepository)
        {
            var retreivedAggregate = aggregateRepository.Read(_aggregateToSave.PolicyNumber);


            retreivedAggregate.Should().NotBeNull();
            retreivedAggregate.PolicyNumber.Should().Be(_aggregateToSave.PolicyNumber);
            retreivedAggregate.id.Should().Be(_aggregateToSave.id);
            retreivedAggregate.Distance.Should().Be(_aggregateToSave.Distance);
            retreivedAggregate.CreatedDate.Should().Be(_aggregateToSave.CreatedDate);
            retreivedAggregate.LastModifiedDate.Should().Be(_aggregateToSave.LastModifiedDate);
            retreivedAggregate.LastJourneyDate.Should().Be(_aggregateToSave.LastJourneyDate);
            retreivedAggregate.AggregateStartDate.Should().Be(_aggregateToSave.AggregateStartDate);
            retreivedAggregate.TotalScore.Should().Be(_aggregateToSave.TotalScore);

            retreivedAggregate.Categories.Should().NotBeNull();
            retreivedAggregate.Categories.Count.Should().Be(3);
            retreivedAggregate.Categories[2].Name.Should().Be(Category.CategoryS);
            retreivedAggregate.Categories[2].Score.Should().Be(99);
            retreivedAggregate.Categories[2].Level.Should().Be(1);
            retreivedAggregate.Categories[2].MessageSequence.Should().Be(1);
        }

        private void Clean()
        {
            var authKeyOrResourceToken = _configFactory.GetConfig().DocumentDbAuthKey;
            var documentDbServiceEndpoint = _configFactory.GetConfig().DocumentDbServiceEndpoint;
            var documentDbDatabaseId = _configFactory.GetConfig().DocumentDbDatabaseId;
            var documentDbCollectionId = _configFactory.GetConfig().DocumentDbCollectionId;

            using (var client = new DocumentClient(documentDbServiceEndpoint, authKeyOrResourceToken))
            {                
                try
                {
                    client.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(documentDbDatabaseId, documentDbCollectionId)).Wait();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
