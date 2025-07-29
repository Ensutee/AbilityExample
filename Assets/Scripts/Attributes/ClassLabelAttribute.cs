using System;


public class ClassLabelAttribute : Attribute
{
	public string Category { get; private set; }
	public string Label { get; private set; }
		
		
	public ClassLabelAttribute(string label)
	{
		if (label.Contains('/'))
		{
			Category = label[..(label.LastIndexOf('/') + 1)];
			Label = label[(Category.Length)..];
		}
		else
		{
			Category = null;
			Label = label;
		}
	}
}