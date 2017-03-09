using System;

namespace IllyumL2T.Core
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class FileParseBehaviorAttribute : Attribute
  {
    public BlankLineMode BlankLineMode { get; set; }

    public FileParseBehaviorAttribute() : this(blankLineMode: BlankLineMode.Stop)
    {
    }

    public FileParseBehaviorAttribute(BlankLineMode blankLineMode)
    {
      BlankLineMode = blankLineMode;
    }
  }
}