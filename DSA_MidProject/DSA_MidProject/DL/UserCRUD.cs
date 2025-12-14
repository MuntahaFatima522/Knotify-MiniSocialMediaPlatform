using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using DSA_MidProject.BL;
using DSA_MidProject.DataStructures;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace DSA_MidProject.DL
{
    internal class UserCRUD
    {
        public UserList Users { get; private set; } = new UserList();

        public void LoadFromDB()
        {
            Users.Clear();
            string query = "SELECT * FROM users";
            using (MySqlDataReader reader = DatabaseHelper.Instance.getData(query))
            {
                while (reader.Read())
                {
                    User u = new User(
                        Convert.ToInt32(reader["UserID"]),
                        reader["UserName"].ToString(),
                        reader["Email"].ToString(),
                        reader["PasswordHash"].ToString(),
                        reader["Contact"].ToString(),
                        reader["ProfilePicture"].ToString(),
                        Convert.ToDateTime(reader["CreatedAt"])
                    );
                    Users.Add(u);
                }
            }
        }

        public void AddUser(User u)
        {

            string query = $"INSERT INTO users (UserName, Email, PasswordHash, Contact, ProfilePicture, CreatedAt) " +
                           $"VALUES ('{u.UserName}', '{u.Email}', '{u.PasswordHash}', '{u.Contact}', '{u.ProfilePicture}', '{u.CreatedAt:yyyy-MM-dd HH:mm:ss}')";
            DatabaseHelper.Instance.Update(query);

            string getLastId = "SELECT LAST_INSERT_ID()";
            int newUserId = Convert.ToInt32(DatabaseHelper.Instance.GetScalarValue(getLastId));

            u.UserID = newUserId;

            Users.Add(u);
        }

        public void UpdateUser(User u)
        {
            string query = $"UPDATE users SET " +
                           $"UserName = '{u.UserName}', " +
                           $"Email = '{u.Email}', " +
                           $"Contact = '{u.Contact}', " +
                           $"ProfilePicture = '{u.ProfilePicture}' " +
                           $"WHERE UserID = {u.UserID}";
            DatabaseHelper.Instance.Update(query);

            User existing = Users.SearchByID(u.UserID);
            if (existing != null)
            {
                existing.UserName = u.UserName;
                existing.Email = u.Email;
                existing.Contact = u.Contact;
                existing.ProfilePicture = u.ProfilePicture;
            }

            LoggedInUser.userName=u.UserName;
            LoggedInUser.email=u.Email;
            LoggedInUser.contact=u.Contact;
            LoggedInUser.profilePicture=u.ProfilePicture;
        }

        public (bool, int, string,string,string,DateTime) Login(string username, string password)
        {
            User u = Users.Search(username);
            if (u != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, u.PasswordHash))
                    return (true, u.UserID, u.Email,u.Contact,u.ProfilePicture,u.CreatedAt);
                else
                    MessageBox.Show("Invalid Password!", "Error");
            }
            else
            {
                MessageBox.Show("User Not Found!", "Error");
            }
            return (false, -1, "","","",DateTime.Now);
        }

        public bool IsEmailExists(string email)
        {
            foreach (var user in Users.GetAll())
            {
                if (user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public bool IsUserNameExists(string username)
        {
            return Users.Search(username) != null;
        }

        public static bool CheckPasswordStrength(string pass)
        {
            bool hasDigit = pass.Any(char.IsDigit);
            bool hasLetter = pass.Any(char.IsLetter);
            bool hasSpecial = pass.Any(ch => "!@#$%^&*()_+-=<>?/;:.".Contains(ch));
            return hasDigit && hasLetter && hasSpecial;
        }

        public static bool IsValidContact(string contact)
        {
            return Regex.IsMatch(contact, @"^(\+92|0)\d{10}$");
        }
        public bool ResetPassword(string email, string newPassword)
        {
            User u = Users.SearchByEmail(email);
            if (u == null)
            {
                MessageBox.Show("User not found!", "Error");
                return false;
            }

            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                u.PasswordHash = hashedPassword;

                string updateQuery = @"UPDATE users 
                               SET PasswordHash = @pass 
                               WHERE UserID = @id AND Email = @mail";

                using (MySqlConnection conn = DatabaseHelper.Instance.getConnection())
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@pass", hashedPassword);
                    cmd.Parameters.AddWithValue("@id", u.UserID);
                    cmd.Parameters.AddWithValue("@mail", email);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Failed to update password in database.", "Error");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting password: " + ex.Message, "Error");
                return false;
            }
        }

    }
}
