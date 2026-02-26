namespace eWatson.Abstractions.ValueObjects;

/// <summary>
/// Marker interface for value objects in the domain.
/// Value objects have no conceptual identity and are defined by their attributes.
/// They should be immutable and implement structural equality.
/// </summary>
/// <remarks>
/// Value Object Characteristics:
/// <list type="bullet">
/// <item>Immutability - once created, cannot be modified</item>
/// <item>Structural Equality - equal if all properties are equal</item>
/// <item>No Identity - identified by attributes, not ID</item>
/// <item>Self-Validating - validate invariants on construction</item>
/// </list>
/// </remarks>
public interface IValueObject { }
