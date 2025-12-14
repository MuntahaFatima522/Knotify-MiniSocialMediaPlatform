using System;
using System.Windows.Forms;
using DSA_MidProject.DataStructures;

namespace DSA_MidProject
{
    public static class NavigationManager
    {
        private static NavigationStack formStack = new NavigationStack();
        private static bool isInitialized = false;
        private static Form currentActiveForm = null;
        private static bool isNavigatingBack = false;

        public static void Initialize(Form initialForm)
        {
            if (!isInitialized)
            {
                formStack.Push(initialForm);
                currentActiveForm = initialForm;
                isInitialized = true;
            }
        }

        public static void NavigateTo(Form newForm, Form currentForm)
        {
            formStack.Push(currentForm);

            currentForm.Hide();

            newForm.Closed -= OnFormClosed;
            newForm.Closed += OnFormClosed;

            newForm.Show();
            currentActiveForm = newForm;
        }

        private static void OnFormClosed(object sender, EventArgs e)
        {
            if (sender is Form closedForm)
            {
                closedForm.Closed -= OnFormClosed;
            }

            Application.Exit();
        }

        public static bool GoBack(Form currentForm)
        {
            if (formStack.Count <= 1)
            {
                MessageBox.Show("You are at the main menu. Cannot go back further.", "Navigation",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            ManualNavigateBack(currentForm);
            return true;
        }

        private static void ManualNavigateBack(Form currentForm)
        {
            isNavigatingBack = true;

            currentForm.Closed -= OnFormClosed;

            currentForm.Hide();

            if (formStack.Count > 0 && formStack.Peek() == currentForm)
            {
                formStack.Pop();
            }

            if (formStack.Count > 0)
            {
                Form previousForm = formStack.Peek();
                if (previousForm != null && !previousForm.IsDisposed)
                {
                    previousForm.Show();
                    previousForm.BringToFront();
                    previousForm.Focus();
                    currentActiveForm = previousForm;
                }
                else
                {
                    formStack.Pop();
                    ManualNavigateBack(currentForm);
                }
            }

            isNavigatingBack = false;
        }

        private static void NavigateBackToPrevious()
        {
            if (formStack.Count > 0)
            {
                formStack.Pop();
            }

            if (formStack.Count > 0)
            {
                Form previousForm = formStack.Peek();
                if (previousForm != null && !previousForm.IsDisposed)
                {
                    previousForm.Show();
                    previousForm.BringToFront();
                    previousForm.Focus();
                    currentActiveForm = previousForm;
                }
                else
                {
                    formStack.Pop();
                    NavigateBackToPrevious();
                }
            }
            else
            {
                Application.Exit();
            }
        }

       
    }
}