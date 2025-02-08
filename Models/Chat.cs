namespace NetWork.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string SenderEmail { get; set; } 
        public string Message { get; set; }      
        public DateTime SentDate { get; set; }   
        public string MessageType { get; set; }  
        public string Status { get; set; }      
        public string? AdminResponse { get; set; } 
        public string? InstructorResponse { get; set; } 
    }
}