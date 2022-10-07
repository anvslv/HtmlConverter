using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HtmlConverter.Data.Entities
{ 
    public class ConversionJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public ConversionStatus? Status { get; set; } 

        [Required]
        public string HtmlFileName { get; set; } = default!;

        [Required] 
        public string HtmlContents { get; set; } = default!;
          
        public byte[]? PdfContents { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow; 

        public DateTime? Finished { get; set; }
    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversionStatus
    {
        [EnumMember(Value = "Received Input File")]
        ReceivedInputFile,
        [EnumMember(Value = "In Progress")]
        InProgress,
        [EnumMember(Value = "Done")]
        Done,
        [EnumMember(Value = "Failed: Conversion Service Unavailable")]
        Failed_ConversionServiceUnavailable,
        [EnumMember(Value = "Failed: Generic Error")]
        Failed_GenericError
    } 
}