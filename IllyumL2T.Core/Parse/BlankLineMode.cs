namespace IllyumL2T.Core
{
  /// <summary>
  /// Possible values for the related FileParseBehaviorAttribute property.
  /// </summary>
  public enum BlankLineMode
  {
    Stop,  // The default for backward compatibility. The file parsing process stops at the first empty or whitespace line.
    Skip,  // The file parsing process does not stop at an empty or whitespace line, it does skip such a line, instead.
    Nulled // The file parsing process does not stop at an empty or whitespace line, it does return an null instance, instead.
  }
}