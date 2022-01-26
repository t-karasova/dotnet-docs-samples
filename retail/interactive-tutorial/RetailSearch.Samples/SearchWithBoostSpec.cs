// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START retail_search_product_with_boost_spec]
// Call Retail API to search for a products in a catalog, rerank the
// results boosting or burying the products that match defined condition.

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Search with boot spec sample class.
/// </summary>
public static class SearchWithBoostSpec
{
    /// <summary>The project number.</summary>
    private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");

    /// <summary>The default search placement.</summary>
    private static readonly string DefaultSearchPlacement = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/placements/default_search";

    /// <summary>Get search request.</summary>
    private static SearchRequest GetSearchRequest(string query, string condition, float boostStrength)
    {
        var conditionBoostSpec = new SearchRequest.Types.BoostSpec.Types.ConditionBoostSpec()
        {
            Condition = condition,
            Boost = boostStrength
        };

        var bootSpec = new SearchRequest.Types.BoostSpec();
        bootSpec.ConditionBoostSpecs.Add(conditionBoostSpec);

        var searchRequest = new SearchRequest()
        {
            Placement = DefaultSearchPlacement, // Placement is used to identify the Serving Config name
            Query = query,
            BoostSpec = bootSpec,
            VisitorId = "123456", // A unique identifier to track visitors
            PageSize = 10
        };

        var jsonSerializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        var searchRequestJson = JsonConvert.SerializeObject(searchRequest, jsonSerializeSettings);

        Console.WriteLine("\nSearch. request: \n\n" + searchRequestJson);
        return searchRequest;
    }

    /// <summary>Call the Retail Search.</summary>
    [RetailSearch.Samples.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        // Try different conditions here:
        string condition = "colorFamilies: ANY(\"Blue\")";
        float boost = 0.0f;
        string query = "Tee";

        var client = SearchServiceClient.Create();
        var searchRequest = GetSearchRequest(query, condition, boost);
        var searchResponse = client.Search(searchRequest).AsRawResponses();

        Console.WriteLine("\nSearch. response: \n");

        var jsonSerializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        var firstSearchResponse = searchResponse.FirstOrDefault();

        if (firstSearchResponse != null)
        {
            var objectToSerialize = new
            {
                results = firstSearchResponse.Results,
                totalSize = firstSearchResponse.TotalSize,
                attributionToken = firstSearchResponse.AttributionToken,
                nextPageToken = firstSearchResponse.NextPageToken,
                facets = firstSearchResponse.Facets,
                queryExpansionInfo = firstSearchResponse.QueryExpansionInfo
            };

            var serializedJson = JsonConvert.SerializeObject(objectToSerialize, jsonSerializeSettings);

            Console.WriteLine(serializedJson + "\n");
        }

        return searchResponse;
    }
}
// [END retail_search_product_with_boost_spec]