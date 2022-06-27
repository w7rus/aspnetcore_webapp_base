namespace Domain.Enums;

public enum AgeRating
{
    None,

    /// <summary>
    ///     Everyone (0+)
    /// </summary>
    E,

    /// <summary>
    ///     Teen (13+)
    /// </summary>
    T,

    /// <summary>
    ///     Mature (16+)
    /// </summary>
    M,

    /// <summary>
    ///     Rated (18+)
    /// </summary>
    R,

    /// <summary>
    ///     Rated (18+), Pornographic
    /// </summary>
    RX,

    /// <summary>
    ///     Rated (18+), Pornographic, Gore
    /// </summary>
    RG
}