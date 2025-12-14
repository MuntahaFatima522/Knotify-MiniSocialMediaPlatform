using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class FriendRequest
    {
        public int RequestID { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string Status {  get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string SenderName {  get; set; }
        public string SenderEmail {  get; set; }
        public string SenderProfilePicture {  get; set; }

        public FriendRequest() { }
        public FriendRequest(int requestID, int senderID, int receiverID, string status, DateTime sentAt, DateTime? respondedAt)
        {
            RequestID = requestID;
            SenderID = senderID;
            ReceiverID = receiverID;
            Status = status;
            SentAt = sentAt;
            RespondedAt = respondedAt;
        }
      
    }
}
