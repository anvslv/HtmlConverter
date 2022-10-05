namespace HtmlConverter;

public class ConversionJob
{  
    public Guid? ID { get; set; }
    public ConversionStatus? Status { get; set; }
    public string? HtmlFileName { get; set; }
}


public enum ConversionStatus
{
    Done,
    InProgress,
    Failed
}