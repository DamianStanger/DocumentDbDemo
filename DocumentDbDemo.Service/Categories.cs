using System.Collections.Generic;
using System.Linq;
using Ingenie.BehaviourReviewService.Contracts.Events;

namespace DocumentDbDemo.Service
{
    public class Categories : List<Category>
    {
        public void Add(List<JourneyEvent> events)
        {
            if (events == null) return;

            var groupedEvents = events.GroupBy(e => e.Category)
                                      .Select(grp => new { summedScores = grp.Sum(e => e.Score), category = grp.Key });

            foreach (var group in groupedEvents)
            {
                this.Single(c => c.Name == group.category).AddAdditionalScore(group.summedScores);
            }
        }
    }
}