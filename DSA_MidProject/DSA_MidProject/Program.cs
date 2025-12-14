namespace DSA_MidProject
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ApplicationConfiguration.Initialize();

            Main mainForm = new Main();

            NavigationManager.Initialize(mainForm);

            Application.Run(mainForm);
        }
        public static class AppData
        {
            public static DSA_MidProject.DL.UserCRUD userCrud = new DSA_MidProject.DL.UserCRUD();
            public static DSA_MidProject.DL.FriendCRUD friendCrud = new DSA_MidProject.DL.FriendCRUD();
            public static DSA_MidProject.DL.PostCRUD postCrud = new DSA_MidProject.DL.PostCRUD();
            public static DSA_MidProject.DL.CommentCRUD commentCRUD = new DSA_MidProject.DL.CommentCRUD();
            public static DSA_MidProject.DL.LikeCRUD likeCrud= new DSA_MidProject.DL.LikeCRUD();
        }
    }
}