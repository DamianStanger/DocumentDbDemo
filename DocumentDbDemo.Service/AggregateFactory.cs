using System;

namespace DocumentDbDemo.Service
{
    public class AggregateFactory
    {
        public virtual Aggregate NewAggregate(string policyNumber, DateTime aggregateStartDate)
        {
            return new Aggregate(policyNumber, aggregateStartDate);
        }
    }
}