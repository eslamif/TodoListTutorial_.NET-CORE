using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoListTutorial.Models {
    public class Filters {
        public string FilterString { get; set; }
        public string CategoryId { get; set; }
        public string Due { get; set; }
        public string StatusId { get; set; }

        public bool HasCategory => CategoryId.ToLower() != "all";
        public bool HasDue => Due.ToLower() != "all";
        public bool HasStatus => StatusId.ToLower() != "all";

        public static Dictionary<string, string> DueFilterValues =>
            new Dictionary<string, string> {
                { "future", "Future" },
                { "past", "Past" },
                { "today", "Today" }
            };

        public bool IsPast => Due.ToLower() == "past";
        public bool IsFuture => Due.ToLower() == "future";
        public bool IsToday => Due.ToLower() == "today";

        public Filters(string filterString) {
            this.FilterString = filterString ?? "all-all-all";
            string[] filters = FilterString.Split('-');
            CategoryId = filters[0];
            Due = filters[1];
            StatusId = filters[2];
        }
    }
}
