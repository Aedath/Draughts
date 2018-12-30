using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Draughts.App.Infrastructure
{
    public static class PasswordBoxAssist
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(SecureString), typeof(PasswordBoxAssist), new PropertyMetadata(null, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
            "BindPassword", typeof(bool), typeof(PasswordBoxAssist), new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxAssist), new PropertyMetadata(false));

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = d as PasswordBox;

            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            box.PasswordChanged -= HandlePasswordChanged;

            SecureString newPassword = (SecureString)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword.ToUnsecuredString();
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (!(dp is PasswordBox box))
            {
                return;
            }

            bool wasBound = (bool)(e.OldValue);
            bool needToBind = (bool)(e.NewValue);

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        internal static string ToUnsecuredString(this SecureString secureString)
        {
            var bstrPtr = IntPtr.Zero;
            if (secureString == null)
            {
                return string.Empty;
            }

            if (secureString.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                bstrPtr = Marshal.SecureStringToBSTR(secureString);
                return Marshal.PtrToStringBSTR(bstrPtr);
            }
            finally
            {
                if (bstrPtr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstrPtr);
                }
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            SetUpdatingPassword(box, true);
            SetBoundPassword(box, box.SecurePassword);
            SetUpdatingPassword(box, false);
        }

        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPassword);
        }

        public static SecureString GetBoundPassword(DependencyObject dp)
        {
            return (SecureString)dp.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject dp, SecureString value)
        {
            dp.SetValue(BoundPassword, value);
        }

        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }
    }
}