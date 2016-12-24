using System;
using AppServiceHelpers.Models;

namespace Zen.Tracker.Client.Entities
{
    public class TodoItem : EntityData
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public bool Complete { get; set; }

        public DateTimeOffset? DueAt { get; set; }

        public DateTimeOffset? CompletedAt { get; set; }
    }
}
