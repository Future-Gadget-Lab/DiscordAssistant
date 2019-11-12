using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Assistant.Modules.MicrosoftDocs
{
    public class Category
    {

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("from")]
        public object From { get; set; }

        [JsonProperty("to")]
        public object To { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class Facets
    {

        [JsonProperty("category")]
        public IList<Category> Category { get; set; }
    }

    public class HitHighlight
    {

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }
    }

    public class DisplayUrl
    {

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("hitHighlights")]
        public IList<HitHighlight> HitHighlights { get; set; }
    }

    public class Description
    {

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("hitHighlights")]
        public IList<HitHighlight> HitHighlights { get; set; }
    }

    public class Result
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("displayUrl")]
        public DisplayUrl DisplayUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("descriptions")]
        public IList<Description> Descriptions { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public DateTime LastUpdatedDate { get; set; }

        [JsonProperty("breadcrumbs")]
        public IList<object> Breadcrumbs { get; set; }
    }

    public class Docs
    {

        [JsonProperty("facets")]
        public Facets Facets { get; set; }

        [JsonProperty("results")]
        public IList<Result> Results { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("@nextLink")]
        public string NextLink { get; set; }
    }
}
