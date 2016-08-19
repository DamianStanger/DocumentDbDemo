using System;
using Ingenie.BehaviourReviewService.Contracts.Events;
using Newtonsoft.Json;

namespace DocumentDbDemo.Service
{
    public class Aggregate
    {
        internal Aggregate(string policyNumber, DateTime aggregateStartDate)
        {
            //used by the factory when creating a default new aggregate
            PolicyNumber = policyNumber;
            AggregateStartDate = aggregateStartDate;
            id = policyNumber;
            CreatedDate = DateTime.UtcNow;
            Categories = new Categories {new Category(Category.CategoryB), new Category(Category.CategoryC), new Category(Category.CategoryS) };           
        }

        [JsonConstructor]
        internal Aggregate(string id, string policyNumber, double distance, int totalScore, DateTime createdDate, DateTime lastModifiedDate, DateTime lastJourneyDate, DateTime aggregateStartDate)
        {
            //Used by the repo when reading a document from the store
            this.id = id;
            PolicyNumber = policyNumber;
            Distance = distance;
            TotalScore = totalScore;
            CreatedDate = createdDate;
            LastModifiedDate = lastModifiedDate;
            LastJourneyDate = lastJourneyDate;
            AggregateStartDate = aggregateStartDate;
            Categories = new Categories();
        } 

        public string id { get; } 
        public string PolicyNumber { get; }
        public double Distance { get; private set; }
        public int TotalScore { get; private set; } 
        public DateTime CreatedDate { get; }
        public DateTime LastModifiedDate { get; private set; }
        public DateTime LastJourneyDate { get; private set; }
        public DateTime AggregateStartDate { get; }
        public Categories Categories { get; }


        public void  Add(JourneyAllocatedToPolicy journeyAllocatedToPolicy)
        {
            if (journeyAllocatedToPolicy.StartedAt < AggregateStartDate)
            {
                return;
            }

            Distance += journeyAllocatedToPolicy.Distance;
            LastJourneyDate = journeyAllocatedToPolicy.StartedAt;
            LastModifiedDate = DateTime.UtcNow;

            Categories.Add(journeyAllocatedToPolicy.Events);
        }        
    }
}