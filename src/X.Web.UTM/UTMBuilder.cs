using System;
using System.Collections.Generic;
using System.Web;
using JetBrains.Annotations;

namespace X.Web.UTM;

/// <summary>
/// Provides methods to build and append Urchin Tracking Module (UTM) parameters to a given URI.
/// </summary>
[PublicAPI]
public class UTMBuilder
{
    
    /// <summary>
    /// Constructs a new URI with UTM parameters added to the query string.
    /// </summary>
    /// <param name="uri">The base URI to which UTM parameters will be added.</param>
    /// <param name="utm">The <see cref="UrchinTrackingModule"/> object containing UTM parameters.</param>
    /// <returns>A new <see cref="Uri"/> object with UTM parameters appended to the query string.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="uri"/> or <paramref name="utm"/> is null.
    /// </exception>
    public Uri Build(string uri, UrchinTrackingModule utm)
    {
        return Build(new Uri(uri), utm);
    }
    
    /// <summary>
    /// Constructs a new URI with UTM parameters added to the query string.
    /// </summary>
    /// <param name="uri">The base URI to which UTM parameters will be added.</param>
    /// <param name="utm">The <see cref="UrchinTrackingModule"/> object containing UTM parameters.</param>
    /// <returns>A new <see cref="Uri"/> object with UTM parameters appended to the query string.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="uri"/> or <paramref name="utm"/> is null.
    /// </exception>
    public Uri Build(Uri uri, UrchinTrackingModule utm)
    {
        if (uri == null || utm == null)
        {
            throw new ArgumentNullException(uri == null ? nameof(uri) : nameof(utm));
        }

        var uriBuilder = new UriBuilder(uri);
        var queryCollection = HttpUtility.ParseQueryString(uriBuilder.Query);

        // List of UTM parameters to be removed
        var utmParameters = new List<string>
        {
            UtmComponents.Term,
            UtmComponents.Content,
            UtmComponents.Medium,
            UtmComponents.Source,
            UtmComponents.Campaign
        };

        // Remove any existing UTM parameters
        foreach (var param in utmParameters)
        {
            queryCollection.Remove(param);
        }

        // Append the new UTM parameters
        var queryToAppend = utm.ToString();

        if (!string.IsNullOrEmpty(queryToAppend))
        {
            var newQuery = HttpUtility.ParseQueryString(queryToAppend);
            
            foreach (string key in newQuery)
            {
                queryCollection[key] = newQuery[key];
            }
        }

        uriBuilder.Query = queryCollection.ToString();
        
        return uriBuilder.Uri;
    }

    /// <summary>
    /// Constructs a new URI with UTM parameters added to the query string using individual parameter values.
    /// </summary>
    /// <param name="uri">The base URI to which UTM parameters will be added.</param>
    /// <param name="source">The source of the traffic, a required UTM parameter.</param>
    /// <param name="medium">The medium through which the traffic was generated (e.g., email, CPC).</param>
    /// <param name="campaign">The specific campaign that refers to the promotion of products.</param>
    /// <param name="term">Identifies paid search terms.</param>
    /// <param name="content">Used to differentiate similar content, or links within the same ad.</param>
    /// <returns>A new <see cref="Uri"/> object with UTM parameters appended to the query string.</returns>
    public Uri Build(Uri uri, string source, string medium, string campaign, string term, string content)
    {
        var utm = new UrchinTrackingModule
        {
            Campaign = campaign,
            Content = content,
            Medium = medium,
            Source = source,
            Term = term
        };
        
        return Build(uri, utm);
    }
}