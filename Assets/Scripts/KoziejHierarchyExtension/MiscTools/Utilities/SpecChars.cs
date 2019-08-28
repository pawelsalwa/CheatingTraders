namespace AWI {
	 public static class SpecChars {
		  public const string next = "▶";
		  public const string previous = "◀";
		  public const string up = "▲";
		  public const string down = "▼";
		  public const string ehDash = "–";
		  public const string spacedEhDash = " – ";
		  public const string lambda = "\u03BB";
		  public const string delta = "\u0394";
		  public const string phi = "\u03C8";
		  public const string omega = "\u03C9";
		  public const string inch = "″";
		  public const string degree = "°";

		  public static string IsUnfoldSpaced(bool value) {
				return value ? "▼ " : "► ";
		  }
	 }
}
