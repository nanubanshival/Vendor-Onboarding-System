using System;
using System.Collections.Generic;
using System.Linq;
using VendorOnboardingSystem.Models;

namespace VendorOnboardingSystem.ViewModels
{
    public class VendorStatusVM
    {
        public Vendor Vendor { get; set; }
        public List<EditRequest> EditRequests { get; set; } = new List<EditRequest>();
        public List<TimelineEvent> Timeline { get; set; } = new List<TimelineEvent>();

        public int ChangesRequestedCount => EditRequests.Count;
        public int ChangesCompletedCount => EditRequests.Count(r => r.Status == "Completed");
        public EditRequest PendingEditRequest => EditRequests.FirstOrDefault(r => r.Status == "Pending");
        public bool HasPendingEditRequest => PendingEditRequest != null;

        public DateTime? LastActivityAt =>
            Timeline.Any() ? Timeline.Max(e => e.EventDate) : (DateTime?)null;
    }

    public class TimelineEvent
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Actor { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public List<string> AffectedSections { get; set; } = new List<string>();
        public Dictionary<string, string> SectionComments { get; set; } = new Dictionary<string, string>();
        public string Notes { get; set; }
        public int? EditRequestId { get; set; }
        public int RoundNumber { get; set; }
    }
}
