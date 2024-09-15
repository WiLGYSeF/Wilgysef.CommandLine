using Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Instance handler options.
/// </summary>
public interface IInstanceHandlerOptions
{
    /// <summary>
    /// Deserialized argument instance value aggregators.
    /// </summary>
    ICollection<IArgumentValueAggregator> ValueAggregators { get; set; }

    /// <summary>
    /// Instance value handler.
    /// </summary>
    IInstanceValueHandler? InstanceValueHandler { get; set; }

    /// <summary>
    /// Whether to throw if a property to be set is missing.
    /// </summary>
    bool? ThrowOnMissingProperty { get; set; }

    /// <summary>
    /// Whether to throw if more values are given than expected.
    /// </summary>
    bool? ThrowOnTooManyValues { get; set; }
}
