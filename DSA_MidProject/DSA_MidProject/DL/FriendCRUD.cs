using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSA_MidProject.BL;
using DSA_MidProject.DataStructures;
using MySql.Data.MySqlClient;

namespace DSA_MidProject.DL
{
    internal class FriendCRUD
    {
        public FriendList Friends { get; private set; } = new FriendList();
        public FriendRequestList FriendRequests { get; private set; } = new FriendRequestList();

        public void LoadFromDB()
        {
            LoadFriends();
            LoadFriendRequests();
        }

        private void LoadFriends()
        {
            Friends.Clear();
            string query = "SELECT * FROM friends";
            using (MySqlDataReader reader = DatabaseHelper.Instance.getData(query))
            {
                while (reader.Read())
                {
                    Friend friend = new Friend(
                        Convert.ToInt32(reader["FriendshipID"]),
                        Convert.ToInt32(reader["UserID1"]),
                        Convert.ToInt32(reader["UserID2"]),
                        Convert.ToDateTime(reader["FriendshipDate"])
                    );
                    Friends.Add(friend);
                }
            }
        }

        private void LoadFriendRequests()
        {
            FriendRequests.Clear();
            string query = "SELECT * FROM friend_requests";
            using (MySqlDataReader reader = DatabaseHelper.Instance.getData(query))
            {
                while (reader.Read())
                {
                    FriendRequest request = new FriendRequest(
                        Convert.ToInt32(reader["RequestID"]),
                        Convert.ToInt32(reader["SenderID"]),
                        Convert.ToInt32(reader["ReceiverID"]),
                        reader["Status"].ToString(),
                        Convert.ToDateTime(reader["SentAt"]),
                        reader["RespondedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["RespondedAt"])
                    );
                    FriendRequests.Add(request);
                }
            }
        }

        public List<FriendUser> SortFriendsByDate(List<FriendUser> friends, bool ascending = false)
        {
            if (friends == null || friends.Count <= 1)
                return friends;

            var sortedFriends = new List<FriendUser>(friends);
            QuickSortByDate(sortedFriends, 0, sortedFriends.Count - 1, ascending);
            return sortedFriends;
        }

        private void QuickSortByDate(List<FriendUser> friends, int low, int high, bool ascending)
        {
            if (low < high)
            {
                int pivotIndex = PartitionByDate(friends, low, high, ascending);
                QuickSortByDate(friends, low, pivotIndex - 1, ascending);
                QuickSortByDate(friends, pivotIndex + 1, high, ascending);
            }
        }

        private int PartitionByDate(List<FriendUser> friends, int low, int high, bool ascending)
        {
            DateTime pivot = friends[high].BecameFriendsAt;
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                bool shouldSwap = ascending ?
                    friends[j].BecameFriendsAt < pivot :
                    friends[j].BecameFriendsAt > pivot;

                if (shouldSwap)
                {
                    i++;
                    Swap(friends, i, j);
                }
            }

            Swap(friends, i + 1, high);
            return i + 1;
        }

        private void Swap(List<FriendUser> friends, int i, int j)
        {
            var temp = friends[i];
            friends[i] = friends[j];
            friends[j] = temp;
        }

        public List<FriendUser> SortFriendsByUsername(List<FriendUser> friends, bool ascending = true)
        {
            if (friends == null || friends.Count <= 1)
                return friends;

            return MergeSortByUsername(friends, ascending);
        }

        private List<FriendUser> MergeSortByUsername(List<FriendUser> friends, bool ascending)
        {
            if (friends.Count <= 1)
                return friends;

            int mid = friends.Count / 2;
            var left = new List<FriendUser>();
            var right = new List<FriendUser>();

            for (int i = 0; i < mid; i++)
                left.Add(friends[i]);
            for (int i = mid; i < friends.Count; i++)
                right.Add(friends[i]);

            left = MergeSortByUsername(left, ascending);
            right = MergeSortByUsername(right, ascending);

            return MergeByUsername(left, right, ascending);
        }

        private List<FriendUser> MergeByUsername(List<FriendUser> left, List<FriendUser> right, bool ascending)
        {
            var result = new List<FriendUser>();
            int leftIndex = 0, rightIndex = 0;

            while (leftIndex < left.Count && rightIndex < right.Count)
            {
                int comparison = string.Compare(left[leftIndex].Username, right[rightIndex].Username, StringComparison.OrdinalIgnoreCase);
                bool shouldTakeLeft = ascending ? comparison <= 0 : comparison >= 0;

                if (shouldTakeLeft)
                {
                    result.Add(left[leftIndex]);
                    leftIndex++;
                }
                else
                {
                    result.Add(right[rightIndex]);
                    rightIndex++;
                }
            }

            while (leftIndex < left.Count)
            {
                result.Add(left[leftIndex]);
                leftIndex++;
            }

            while (rightIndex < right.Count)
            {
                result.Add(right[rightIndex]);
                rightIndex++;
            }

            return result;
        }

     
        public List<FriendUser> SearchFriends(List<FriendUser> friends, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return friends;

            var results = new List<FriendUser>();
            searchTerm = searchTerm.ToLower();

            foreach (var friend in friends)
            {
                if (friend.Username.ToLower().Contains(searchTerm) ||
                    friend.Email.ToLower().Contains(searchTerm))
                {
                    results.Add(friend);
                }
            }

            return results;
        }
        public List<User> SearchNonFriendUsersBinary(List<User> nonFriendUsers, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || nonFriendUsers == null || nonFriendUsers.Count == 0)
                return nonFriendUsers;

            var results = new List<User>();
            searchTerm = searchTerm.ToLower();

            var sortedUsers = nonFriendUsers.OrderBy(u => u.UserName.ToLower()).ToList();

            int exactMatchIndex = BinarySearchExactUsername(sortedUsers, searchTerm);
            if (exactMatchIndex != -1)
            {
                results.Add(sortedUsers[exactMatchIndex]);

                int left = exactMatchIndex - 1;
                while (left >= 0 && sortedUsers[left].UserName.ToLower() == searchTerm)
                {
                    results.Add(sortedUsers[left]);
                    left--;
                }

                int right = exactMatchIndex + 1;
                while (right < sortedUsers.Count && sortedUsers[right].UserName.ToLower() == searchTerm)
                {
                    results.Add(sortedUsers[right]);
                    right++;
                }
            }

            foreach (var user in nonFriendUsers)
            {
                if (user.UserName.ToLower().Contains(searchTerm) &&
                    !results.Any(r => r.UserID == user.UserID))
                {
                    results.Add(user);
                }
            }

            foreach (var user in nonFriendUsers)
            {
                if (user.Email.ToLower().Contains(searchTerm) &&
                    !results.Any(r => r.UserID == user.UserID))
                {
                    results.Add(user);
                }
            }

            return results;
        }

        private int BinarySearchExactUsername(List<User> users, string searchTerm)
        {
            int left = 0;
            int right = users.Count - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                string midUsername = users[mid].UserName.ToLower();
                int comparison = string.Compare(midUsername, searchTerm);

                if (comparison == 0)
                    return mid;
                else if (comparison < 0)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            return -1;
        }

        public bool SendFriendRequest(int senderID, int receiverID)
        {
            if (Friends.Contains(senderID, receiverID))
            {
                MessageBox.Show("You are already friends with this user!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            bool hasPendingRequestFromMe = FriendRequests.HasPendingRequest(senderID, receiverID);
            bool hasPendingRequestFromThem = FriendRequests.HasPendingRequest(receiverID, senderID);

            if (hasPendingRequestFromMe)
            {
                MessageBox.Show("Friend request already sent!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (hasPendingRequestFromThem)
            {
                var theirRequest = FriendRequests.GetPendingRequest(receiverID, senderID);
                if (theirRequest != null)
                {
                    return AcceptFriendRequest(theirRequest.RequestID, senderID);
                }
            }

            string query = $"INSERT INTO friend_requests (SenderID, ReceiverID, Status, SentAt) " +
                          $"VALUES ({senderID}, {receiverID}, 'pending', '{DateTime.Now:yyyy-MM-dd HH:mm:ss}')";

            try
            {
                DatabaseHelper.Instance.Update(query);
                LoadFriendRequests();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending friend request: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool AcceptFriendRequest(int requestID, int currentUserID)
        {
            var request = FriendRequests.SearchByRequestID(requestID);
            if (request == null || request.ReceiverID != currentUserID || request.Status != "pending")
            {
                MessageBox.Show("Invalid friend request!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.Instance.getConnection())
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        string updateRequestQuery = $"UPDATE friend_requests SET Status = 'accepted', RespondedAt = NOW() WHERE RequestID = {requestID}";
                        DatabaseHelper.Instance.ExecuteNonQuery(updateRequestQuery, conn);

                        int user1 = Math.Min(request.SenderID, request.ReceiverID);
                        int user2 = Math.Max(request.SenderID, request.ReceiverID);

                        string insertFriendQuery = $"INSERT INTO friends (UserID1, UserID2, FriendshipDate) " +
                                                 $"VALUES ({user1}, {user2}, NOW())";
                        DatabaseHelper.Instance.ExecuteNonQuery(insertFriendQuery, conn);

                        transaction.Commit();
                    }
                }

                LoadFromDB();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accepting friend request: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool RejectFriendRequest(int requestID, int currentUserID)
        {
            var request = FriendRequests.SearchByRequestID(requestID);
            if (request == null || request.ReceiverID != currentUserID || request.Status != "pending")
            {
                MessageBox.Show("Invalid friend request!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string query = $"UPDATE friend_requests SET Status = 'rejected', RespondedAt = NOW() WHERE RequestID = {requestID}";

            try
            {
                DatabaseHelper.Instance.Update(query);
                LoadFriendRequests();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error rejecting friend request: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool RemoveFriend(int currentUserID, int friendID)
        {
            var friend = Friends.SearchByUsers(currentUserID, friendID);
            if (friend == null)
            {
                MessageBox.Show("Friend not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string query = $"DELETE FROM friends WHERE FriendshipID = {friend.FriendshipID}";

            try
            {
                DatabaseHelper.Instance.Update(query);
                Friends.Remove(friend.FriendshipID);

                MessageBox.Show("Friend removed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing friend: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<FriendUser> GetFriendsWithDetails(int currentUserID)
        {
            var friendsWithDetails = new List<FriendUser>();
            var userFriends = Friends.GetFriendsOfUser(currentUserID);

            foreach (var friend in userFriends)
            {
                int friendUserID = friend.UserID1 == currentUserID ? friend.UserID2 : friend.UserID1;

                var user = Program.AppData.userCrud.Users.SearchByID(friendUserID);
                if (user != null)
                {
                    friendsWithDetails.Add(new FriendUser
                    {
                        UserID = user.UserID,
                        Username = user.UserName,
                        Email = user.Email,
                        ProfilePicture = user.ProfilePicture,
                        CreatedAt = user.CreatedAt,
                        BecameFriendsAt = friend.FriendshipDate
                    });
                }
            }

            return friendsWithDetails;
        }

        public List<FriendRequest> GetPendingRequestsWithDetails(int currentUserID)
        {
            var pendingRequests = FriendRequests.GetPendingRequestsForUser(currentUserID);
            var requestsWithDetails = new List<FriendRequest>();

            foreach (var request in pendingRequests)
            {
                var sender = Program.AppData.userCrud.Users.SearchByID(request.SenderID);
                if (sender != null)
                {
                    requestsWithDetails.Add(new FriendRequest
                    {
                        RequestID = request.RequestID,
                        SenderID = request.SenderID,
                        ReceiverID = request.ReceiverID,
                        Status = request.Status,
                        SentAt = request.SentAt,
                        RespondedAt = request.RespondedAt,
                        SenderName = sender.UserName,
                        SenderEmail = sender.Email,
                        SenderProfilePicture = sender.ProfilePicture
                    });
                }
            }

            return requestsWithDetails;
        }

        public List<User> GetNonFriendUsers(int currentUserID)
        {
            var allUsers = Program.AppData.userCrud.Users.GetAll();
            var nonFriends = new List<User>();

            foreach (var user in allUsers)
            {
                if (user.UserID == currentUserID) continue;

                bool isFriend = Friends.Contains(currentUserID, user.UserID);
                if (isFriend) continue;

                bool hasPendingRequest = FriendRequests.HasPendingRequest(currentUserID, user.UserID) ||
                                       FriendRequests.HasPendingRequest(user.UserID, currentUserID);
                if (hasPendingRequest) continue;

                nonFriends.Add(user);
            }

            return nonFriends;
        }
    }
}