using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Domain.Entities
{
    public class SmsLog
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string MessageText { get; set; }
        public SmsDirection Direction { get; set; }
        public DateTime Timestamp { get; set; }
        public string ProcessingStatus { get; set; }
        
        // Private constructor for EF Core
        private SmsLog() { }
        
        public SmsLog(string phoneNumber, string messageText, SmsDirection direction, string processingStatus = "Received")
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
                
            if (string.IsNullOrEmpty(messageText))
                throw new ArgumentException("Message text cannot be empty", nameof(messageText));
                
            PhoneNumber = phoneNumber;
            MessageText = messageText;
            Direction = direction;
            Timestamp = DateTime.UtcNow;
            ProcessingStatus = processingStatus;
        }
        
        public void UpdateStatus(string status)
        {
            ProcessingStatus = status;
        }
    }
    
    public enum SmsDirection
    {
        Inbound,
        Outbound
    }
}