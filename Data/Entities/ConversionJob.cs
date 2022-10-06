using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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


    public enum ConversionStatus
    {
        ReceivedInputFile,
        InProgress,
        Done,
        Failed
    } 
}