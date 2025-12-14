using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSA_MidProject.BL;

namespace DSA_MidProject.DataStructures
{
    internal class FriendRequestList
    {
        private FriendRequestNode head;

        public void Add(FriendRequest request)
        {
            FriendRequestNode newNode = new FriendRequestNode(request);

            if (head == null)
            {
                head = newNode;
                return;
            }

            FriendRequestNode temp = head;
            while (temp.next != null)
                temp = temp.next;

            temp.next = newNode;
        }

        public FriendRequest SearchByRequestID(int requestID)
        {
            FriendRequestNode temp = head;
            while (temp != null)
            {
                if (temp.data.RequestID == requestID)
                    return temp.data;
                temp = temp.next;
            }
            return null;
        }

        public List<FriendRequest> GetPendingRequestsForUser(int userID)
        {
            List<FriendRequest> pendingRequests = new List<FriendRequest>();
            FriendRequestNode temp = head;
            while (temp != null)
            {
                if (temp.data.ReceiverID == userID && temp.data.Status == "pending")
                    pendingRequests.Add(temp.data);
                temp = temp.next;
            }
            return pendingRequests;
        }

        public void Clear()
        {
            head = null;
        }

        public bool HasPendingRequest(int senderID, int receiverID)
        {
            FriendRequestNode temp = head;
            while (temp != null)
            {
                if (temp.data.SenderID == senderID && temp.data.ReceiverID == receiverID && temp.data.Status == "pending")
                    return true;
                temp = temp.next;
            }
            return false;
        }

        public FriendRequest GetPendingRequest(int senderID, int receiverID)
        {
            FriendRequestNode temp = head;
            while (temp != null)
            {
                if (temp.data.SenderID == senderID && temp.data.ReceiverID == receiverID && temp.data.Status == "pending")
                    return temp.data;
                temp = temp.next;
            }
            return null;
        }

       
    }
}